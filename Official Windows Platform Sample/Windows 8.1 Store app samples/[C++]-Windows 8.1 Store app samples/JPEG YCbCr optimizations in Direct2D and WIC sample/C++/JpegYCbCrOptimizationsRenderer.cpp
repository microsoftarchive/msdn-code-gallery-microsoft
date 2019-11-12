//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "JpegYCbCrOptimizationsRenderer.h"

#include "DirectXHelper.h"

using namespace JpegYCbCrOptimizations;

using namespace DirectX;
using namespace Microsoft::WRL;
using namespace Windows::Foundation;
using namespace Windows::System::Threading;
using namespace Windows::UI::Core;

using namespace concurrency;

/// <summary>
/// Sets up the renderer and associated resources.
/// Can be instructed to force usage of BGRA resources.
/// </summary>
JpegYCbCrOptimizationsRenderer::JpegYCbCrOptimizationsRenderer(
    const std::shared_ptr<DX::DeviceResources>& deviceResources,
    _In_ ResourcesLoadedHandler^ handler,
    bool forceBgra
    ) : m_deviceResources(deviceResources),
        m_sampleMode(YCbCrSupportMode::Unknown),
        m_deviceResourceTaskMode(Running),
        m_resourcesLoadedHandler(handler)
{
    // Create resources on a background thread. This requires both Direct2D and Direct3D to be
    // configured for multithread safety (see DeviceResources.cpp).
    m_backgroundTask = create_task([this, forceBgra]()
    {
        CreateDeviceIndependentResources();

        // Checking for task cancellation allows the worker thread to terminate work early.
        // To preserve system responsiveness, background work should be finely grained enough
        // so that the worker can quickly respond.
        interruption_point();

        CreateDeviceDependentResources(forceBgra);
        interruption_point();
    }, m_cancellationTokenSource.get_token()).then([](task<void> t)
    {
        // This get() call will throw task_canceled if the task was cancelled. We don't catch the
        // task_canceled so control falls through to the next exception handler.
        t.get();
    }).then([this]()
    {
        m_deviceResourceTaskMode = Completed;
        CreateWindowSizeDependentResources();
        DX::ThrowIfFailed(m_resourcesLoadedHandler == nullptr ? E_FAIL : S_OK);

        // m_resourcesLoadedHandler must be called on the UI thread (associated with a CoreWindow).
        m_resourcesLoadedHandler(m_sampleMode);
    }, task_continuation_context::use_current()).then([](task<void> t)
    {
        try
        {
            t.get();
        }
        catch (Platform::Exception^ e)
        {
            // D2DERR_RECREATE_TARGET indicates that the device was lost.
            // This is handled by the DisplayInformation::DisplayContentsInvalidated event.
            if (e->HResult != D2DERR_RECREATE_TARGET)
            {
                throw;
            }
        }
        catch (task_canceled e)
        {
            // No need to do anything; the canceller is responsible for recreating us.
        }
    });
}

/// <summary>
/// Marks the renderer as invalid and asynchronously cancels the background resource creation task
/// if it is still running.
/// This method must be called before destroying/recreating the renderer class.
/// </summary>
task<void> JpegYCbCrOptimizationsRenderer::InvalidateAsync()
{
    if (m_deviceResourceTaskMode == Running)
    {
        m_deviceResourceTaskMode = Cancelling;
        m_cancellationTokenSource.cancel();
    }

    return create_task([this]()
    {
        // wait() returns when the task is completed or cancelled.
        m_backgroundTask.wait();
    });
}

/// <summary>
/// Determines if WIC supports the requested YCbCr configuration (WicYCbCrFormats).
/// Some possible reasons the configuration is unsupported:
/// 1. The decoder which backs the IWICBitmapSource does not support YCbCr access.
/// 2. The IWICBitmapSource is incapable of performing the requested transforms.
/// 3. The particular image does not contain a supported YCbCr configuration.
/// </summary>
bool JpegYCbCrOptimizationsRenderer::DoesWicSupportRequestedYCbCr()
{
    ComPtr<IWICPlanarBitmapSourceTransform> wicPlanarSource;
    HRESULT hr = m_wicScaler.As(&wicPlanarSource);
    if (SUCCEEDED(hr))
    {
        BOOL isTransformSupported;
        uint32 supportedWidth = m_cachedBitmapPixelWidth;
        uint32 supportedHeight = m_cachedBitmapPixelHeight;
        DX::ThrowIfFailed(
            wicPlanarSource->DoesSupportTransform(
                &supportedWidth,
                &supportedHeight,
                WICBitmapTransformRotate0,
                WICPlanarOptionsDefault,
                SampleConstants::WicYCbCrFormats,
                m_planeDescriptions,
                SampleConstants::NumPlanes,
                &isTransformSupported
                )
            );

        // The returned width and height may be larger if IWICPlanarBitmapSourceTransform does not
        // exactly support what is requested.
        if ((isTransformSupported == TRUE) &&
            (supportedWidth == m_cachedBitmapPixelWidth) &&
            (supportedHeight == m_cachedBitmapPixelHeight))
        {
            return true;
        }
    }

    return false;
}

/// <summary>
/// Determines if the graphics driver supports YCbCr in Direct2D. Note that this does not require support
/// for video YUV formats like DXGI_FORMAT_NV12. Feature level 10 and above support YCbCr, as well as
/// feature level 9 hardware with WDDM 1.3 and above drivers.
/// </summary>
bool JpegYCbCrOptimizationsRenderer::DoesDriverSupportYCbCr()
{
    auto d2dContext = m_deviceResources->GetD2DDeviceContext();

    if ((d2dContext->IsDxgiFormatSupported(DXGI_FORMAT_R8_UNORM)) &&
        (d2dContext->IsDxgiFormatSupported(DXGI_FORMAT_R8G8_UNORM)))
    {
        return true;
    }

    return false;
}

/// <summary>
/// Creates a Direct2D YCbCr effect backed by YCbCr data at the requested resolution.
/// This method assumes that DoesWicSupportRequestedYCbCr() and DoesDriverSupportYCbCr()
/// have already been called to confirm the requested YCbCr configuration is supported.
/// </summary>
void JpegYCbCrOptimizationsRenderer::CreateYCbCrDeviceResources()
{
    auto wicFactory = m_deviceResources->GetWicImagingFactory();
    auto d2dContext = m_deviceResources->GetD2DDeviceContext();

    ComPtr<IWICPlanarBitmapSourceTransform> wicPlanarSource;
    DX::ThrowIfFailed(
        m_wicScaler.As(&wicPlanarSource)
        );

    ComPtr<IWICBitmap> bitmaps[SampleConstants::NumPlanes];
    ComPtr<IWICBitmapLock> locks[SampleConstants::NumPlanes];
    WICBitmapPlane planes[SampleConstants::NumPlanes];

    for (uint32 i = 0; i < SampleConstants::NumPlanes; i++)
    {
        DX::ThrowIfFailed(
            wicFactory->CreateBitmap(
                m_planeDescriptions[i].Width,
                m_planeDescriptions[i].Height,
                m_planeDescriptions[i].Format,
                WICBitmapCacheOnLoad,
                &bitmaps[i]
                )
            );

        LockBitmap(bitmaps[i].Get(), WICBitmapLockWrite, nullptr, &locks[i], &planes[i]);
    }

    DX::ThrowIfFailed(
        wicPlanarSource->CopyPixels(
            nullptr, // Copy the entire source region.
            m_cachedBitmapPixelWidth,
            m_cachedBitmapPixelHeight,
            WICBitmapTransformRotate0,
            WICPlanarOptionsDefault,
            planes,
            SampleConstants::NumPlanes
            )
        );

    DX::ThrowIfFailed(d2dContext->CreateEffect(CLSID_D2D1YCbCr, &m_d2dYCbCrEffect));

    ComPtr<ID2D1Bitmap1> d2dBitmaps[SampleConstants::NumPlanes];
    for (uint32 i = 0; i < SampleConstants::NumPlanes; i++)
    {
        // IWICBitmapLock must be released before using the IWICBitmap.
        locks[i] = nullptr;

        // First ID2D1Bitmap1 is DXGI_FORMAT_R8 (Y), second is DXGI_FORMAT_R8G8 (CbCr).
        DX::ThrowIfFailed(d2dContext->CreateBitmapFromWicBitmap(bitmaps[i].Get(), &d2dBitmaps[i]));
        m_d2dYCbCrEffect->SetInput(i, d2dBitmaps[i].Get());
    }
}

/// <summary>
/// Creates resources that are not hardware dependent.
/// </summary>
void JpegYCbCrOptimizationsRenderer::CreateDeviceIndependentResources()
{
    auto wicFactory = m_deviceResources->GetWicImagingFactory();

    ComPtr<IWICBitmapDecoder> decoder;
    DX::ThrowIfFailed(
        wicFactory->CreateDecoderFromFilename(
            L"mammoth.jpg",
            nullptr,
            GENERIC_READ,
            WICDecodeMetadataCacheOnDemand,
            &decoder
            )
        );

    ComPtr<IWICBitmapFrameDecode> frameDecode;
    DX::ThrowIfFailed(
        decoder->GetFrame(0, &frameDecode)
        );

    uint32 imageWidth, imageHeight;
    DX::ThrowIfFailed(
        frameDecode->GetSize(&imageWidth, &imageHeight)
        );

    // Restrict the decoded image size to the pixel resolution of the display. We do not use
    // the render target size as the window may be snapped (less than full display resolution).
    GetDisplayResolution();
    float horzScale = static_cast<float>(m_displayResolutionX) / static_cast<float>(imageWidth);
    float vertScale = static_cast<float>(m_displayResolutionY) / static_cast<float>(imageHeight);
    float scale = min(horzScale, min(vertScale, 1.0f));
    m_cachedBitmapPixelWidth = static_cast<uint32>(imageWidth * scale);
    m_cachedBitmapPixelHeight = static_cast<uint32>(imageHeight * scale);

    DX::ThrowIfFailed(
        wicFactory->CreateBitmapScaler(&m_wicScaler)
        );

    // The JPEG frame decode supports power-of-two downscales when using IWICBitmapSourceTransform
    // or IWICPlanarBitmapSourceTransform. Appending a scaler after the decoder provides support
    // for arbitrary downscales.
    DX::ThrowIfFailed(
        m_wicScaler->Initialize(
            frameDecode.Get(),
            m_cachedBitmapPixelWidth,             // Note that the requested size and interpolation mode
            m_cachedBitmapPixelHeight,            // are not used if YCbCr data is requested from
            WICBitmapInterpolationModeLinear // IWICPlanarBitmapSource transform.
            )
        );
}

/// <summary>
/// Locks an IWICBitmap as a WICBitmapPlane.
/// </summary>
void JpegYCbCrOptimizationsRenderer::LockBitmap(
    _In_ IWICBitmap *pBitmap,
    DWORD bitmapLockFlags,
    _In_opt_ const WICRect *prcSource,
    _Outptr_ IWICBitmapLock **ppBitmapLock,
    _Out_ WICBitmapPlane *pPlane
    )
{
    // ComPtr guarantees the IWICBitmapLock is released if an exception is thrown.
    ComPtr<IWICBitmapLock> lock;
    DX::ThrowIfFailed(pBitmap->Lock(prcSource, bitmapLockFlags, &lock));
    DX::ThrowIfFailed(lock->GetStride(&pPlane->cbStride));
    DX::ThrowIfFailed(lock->GetDataPointer(&pPlane->cbBufferSize, &pPlane->pbBuffer));
    DX::ThrowIfFailed(lock->GetPixelFormat(&pPlane->Format));
    *ppBitmapLock = lock.Detach();
}

/// <summary>
/// Creates a Direct2D transform effect backed by BGRA pixel data. This fallback codepath is what
/// a "traditional" non YCbCr-aware app would follow. Unlike its YCbCr counterpart,
/// this method assumes the IWICBitmapSource has already been scaled to the correct size.
/// </summary>
void JpegYCbCrOptimizationsRenderer::CreateBgraDeviceResources()
{
    auto wicFactory = m_deviceResources->GetWicImagingFactory();
    auto d2dContext = m_deviceResources->GetD2DDeviceContext();

    // Ensure the WIC image is in 32bppPBGRA for Direct2D compatibility.
    ComPtr<IWICFormatConverter> formatConverter;
    DX::ThrowIfFailed(wicFactory->CreateFormatConverter(&formatConverter));

    DX::ThrowIfFailed(
        formatConverter->Initialize(
            m_wicScaler.Get(),
            GUID_WICPixelFormat32bppPBGRA,
            WICBitmapDitherTypeNone,
            nullptr,
            0.0L,
            WICBitmapPaletteTypeCustom
            )
        );

    // Create a Direct2D affine transform effect backed by a BGRA Direct2D bitmap.
    ComPtr<ID2D1Bitmap1> bgraBitmap;
    DX::ThrowIfFailed(d2dContext->CreateBitmapFromWicBitmap(formatConverter.Get(), &bgraBitmap));
    DX::ThrowIfFailed(d2dContext->CreateEffect(CLSID_D2D12DAffineTransform, &m_d2dTransformEffect));
    m_d2dTransformEffect->SetInput(0, bgraBitmap.Get());
}

/// <summary>
/// Checks system support for the requested YCbCr configuration and creates the appropriate
/// resources. Can be instructed to force usage of BGRA resources.
/// </summary>
void JpegYCbCrOptimizationsRenderer::CreateDeviceDependentResources(bool forceBgra)
{
    if (forceBgra == false)
    {
        if (DoesWicSupportRequestedYCbCr() && DoesDriverSupportYCbCr())
        {
            CreateYCbCrDeviceResources();
            m_sampleMode = YCbCrSupportMode::Enabled;
        }
        else
        {
            CreateBgraDeviceResources();
            m_sampleMode = YCbCrSupportMode::DisabledFallback;
        }
    }
    else
    {
        CreateBgraDeviceResources();
        m_sampleMode = YCbCrSupportMode::DisabledForced;
    }
}

/// <summary>
/// Sets parameters that depend on window size. Does nothing if the renderer is invalid.
/// </summary>
void JpegYCbCrOptimizationsRenderer::CreateWindowSizeDependentResources()
{
    if (m_deviceResourceTaskMode == Completed)
    {
        auto size = m_deviceResources->GetLogicalSize();
        float dpi = m_deviceResources->GetDpi();

        // Calculate the scale and offset to center the image in the window.
        float horzScale = size.Width / DX::ConvertPixelsToDips(static_cast<float>(m_cachedBitmapPixelWidth), dpi);
        float vertScale = size.Height / DX::ConvertPixelsToDips(static_cast<float>(m_cachedBitmapPixelHeight), dpi);
        float scale = min(horzScale, min(vertScale, 1.0f));

        D2D1_SIZE_F offset;
        offset.width = (size.Width - DX::ConvertPixelsToDips(static_cast<float>(m_cachedBitmapPixelWidth), dpi) * scale) / 2;
        offset.height = (size.Height - DX::ConvertPixelsToDips(static_cast<float>(m_cachedBitmapPixelHeight), dpi) * scale) / 2;

        D2D1::Matrix3x2F transform =
            D2D1::Matrix3x2F::Scale(scale, scale) *
            D2D1::Matrix3x2F::Translation(offset);

        // The rendered output does not change unless the window size changes, so set
        // the effect transform values here and not in Render().
        switch (m_sampleMode)
        {
        case YCbCrSupportMode::DisabledFallback:
        case YCbCrSupportMode::DisabledForced:
            DX::ThrowIfFailed(
                m_d2dTransformEffect->SetValue(
                    D2D1_2DAFFINETRANSFORM_PROP_TRANSFORM_MATRIX,
                    transform
                    )
                );

            break;

        case YCbCrSupportMode::Enabled:
            DX::ThrowIfFailed(
                m_d2dYCbCrEffect->SetValue(
                    D2D1_YCBCR_PROP_TRANSFORM_MATRIX,
                    transform
                    )
                );

            break;

        case YCbCrSupportMode::Unknown:
        default:
            DX::ThrowIfFailed(E_FAIL);
        }
    }
}

/// <summary>
/// Gets the pixel resolution of the display.
/// </summary>
void JpegYCbCrOptimizationsRenderer::GetDisplayResolution()
{
    ComPtr<IDXGIDevice> dxgiDevice;
    DX::ThrowIfFailed(
        m_deviceResources->GetD3DDevice()->QueryInterface(IID_PPV_ARGS(&dxgiDevice))
        );

    ComPtr<IDXGIAdapter> dxgiAdapter;
    DX::ThrowIfFailed(
        dxgiDevice->GetAdapter(&dxgiAdapter)
        );

    ComPtr<IDXGIOutput> dxgiOutput;
    DX::ThrowIfFailed(
        dxgiAdapter->EnumOutputs(0, &dxgiOutput)
        );

    DXGI_OUTPUT_DESC desc;
    DX::ThrowIfFailed(
        dxgiOutput->GetDesc(&desc)
        );

    m_displayResolutionX = desc.DesktopCoordinates.right - desc.DesktopCoordinates.left;
    m_displayResolutionY = desc.DesktopCoordinates.bottom - desc.DesktopCoordinates.top;
}

/// <summary>
/// Renders one frame. Does not draw the image unless the renderer is valid.
/// </summary>
void JpegYCbCrOptimizationsRenderer::Render()
{
    if (m_deviceResourceTaskMode == Completed)
    {
        auto d2dContext = m_deviceResources->GetD2DDeviceContext();

        d2dContext->SetTransform(
            m_deviceResources->GetOrientationTransform2D()
            );

        d2dContext->BeginDraw();

        switch (m_sampleMode)
        {
        case YCbCrSupportMode::DisabledFallback:
        case YCbCrSupportMode::DisabledForced:
            d2dContext->DrawImage(m_d2dTransformEffect.Get());
            break;

        case YCbCrSupportMode::Enabled:
            d2dContext->DrawImage(m_d2dYCbCrEffect.Get());
            break;

        case YCbCrSupportMode::Unknown:
        default:
            DX::ThrowIfFailed(E_FAIL);
        }

        // We ignore D2DERR_RECREATE_TARGET here. This error indicates that the device
        // is lost. It will be handled during the next call to Present.
        HRESULT hr = d2dContext->EndDraw();
        if (hr != D2DERR_RECREATE_TARGET)
        {
            DX::ThrowIfFailed(hr);
        }
    }
}