//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "BlockCompressedImagesRenderer.h"

#include "DirectXHelper.h"

using namespace BlockCompressedImages;

using namespace D2D1;
using namespace DirectX;
using namespace Microsoft::WRL;
using namespace Windows::Foundation;

// Initialization.
BlockCompressedImagesRenderer::BlockCompressedImagesRenderer(const std::shared_ptr<DX::DeviceResources>& deviceResources) :
    m_deviceResources(deviceResources),
    m_hasValidResources(true) // Assume that resources are valid until proven otherwise.
{
    // Define the expected parameters for the DDS image resources generated
    // by the BlockCompressedAssets C++ project.
    m_guitarImageInfo =
    {
        L"BlockCompressedAssets\\guitar-transparent.dds",
        DXGI_FORMAT_BC3_UNORM,
        D2D1_ALPHA_MODE_PREMULTIPLIED, // For use in Direct2D, always specify premultiplied.
        WICDdsAlphaModePremultiplied
    };

    m_woodImageInfo =
    {
        L"BlockCompressedAssets\\oldWood4_nt.dds",
        DXGI_FORMAT_BC1_UNORM,
        D2D1_ALPHA_MODE_PREMULTIPLIED, // For use in Direct2D, always specify premultiplied.
        WICDdsAlphaModePremultiplied
    };

    CreateDeviceIndependentResources();
    CreateDeviceDependentResources();
    CreateWindowSizeDependentResources();
}

// Initialize resources that do not need to be recreated on device lost.
void BlockCompressedImagesRenderer::CreateDeviceIndependentResources()
{
    // Error resources are needed in case the DDS assets are invalid.
    DX::ThrowIfFailed(
            m_deviceResources->GetDWriteFactory()->CreateTextFormat(
            L"Segoe UI",
            nullptr,
            DWRITE_FONT_WEIGHT_LIGHT,
            DWRITE_FONT_STYLE_NORMAL,
            DWRITE_FONT_STRETCH_NORMAL,
            36.0f,
            L"en-US",
            &m_errorTextFormat
            )
        );

    m_errorTextFormat->SetTextAlignment(DWRITE_TEXT_ALIGNMENT_CENTER);
    m_errorTextFormat->SetParagraphAlignment(DWRITE_PARAGRAPH_ALIGNMENT_CENTER);

    m_errorText =
        "A DDS image asset either is missing or does not match the expected parameters. "
        "Follow the instructions in readme.txt in the root of the solution.";

    // Exit early if the DDS resource doesn't exist or doesn't match expected parameters.
    if (!GetDdsBitmapSource(m_guitarImageInfo, &m_wicSourceGuitar) ||
        !GetDdsBitmapSource(m_woodImageInfo, &m_wicSourceWood))
    {
        m_hasValidResources = false;
        return;
    }

    uint32 width = 0;
    uint32 height = 0;
    DX::ThrowIfFailed(m_wicSourceGuitar->GetSize(&width, &height));
    m_guitarWidth = static_cast<float>(width);
    m_guitarHeight = static_cast<float>(height);

    DX::ThrowIfFailed(m_wicSourceWood->GetSize(&width, &height));
    m_woodWidth = static_cast<float>(width);
    m_woodHeight = static_cast<float>(height);
}

// Creates an IWICBitmapSource that is backed by block compressed data.
// The returned IWICBitmapSource can provide decoded pixel data (e.g. 32bppPBGRA),
// but it can also directly provide the block compressed data when used with an API
// such as ID2D1DeviceContext::CreateBitmapFromWicBitmap.
// Returns true if resource creation succeeded, false if it failed.
_Success_(return) bool BlockCompressedImagesRenderer::GetDdsBitmapSource(
    DdsImageResourceParameters parameters,
    _Outptr_result_nullonfailure_ IWICBitmapSource **ppSource
    )
{
    // Exit early if the DDS resource doesn't exist or doesn't match expected parameters.
    *ppSource = nullptr;
    if (!m_hasValidResources)
    {
        return false;
    }

    auto wicFactory = m_deviceResources->GetWicImagingFactory();
    ComPtr<IWICBitmapDecoder> decoder;
    HRESULT hr = wicFactory->CreateDecoderFromFilename(
        parameters.filename,
        nullptr,
        GENERIC_READ,
        WICDecodeMetadataCacheOnDemand,
        &decoder
        );

    // This check is for developer education only.
    // Ensure that you have generated the block compressed image assets and included them in the project.
    if (hr == HRESULT_FROM_WIN32(ERROR_PATH_NOT_FOUND) ||
        hr == HRESULT_FROM_WIN32(ERROR_FILE_NOT_FOUND))
    {
        return false;
    }
    else
    {
        DX::ThrowIfFailed(hr);
    }

    // This check is for developer education only.
    // Use new DDS-specific WIC APIs to confirm that the DDS file matches the expected parameters.
    ComPtr<IWICDdsDecoder> ddsDecoder;
    WICDdsParameters ddsParameters;
    DX::ThrowIfFailed(decoder.As(&ddsDecoder));
    DX::ThrowIfFailed(ddsDecoder->GetParameters(&ddsParameters));

    if (ddsParameters.AlphaMode != parameters.ddsAlpha ||
        ddsParameters.DxgiFormat != parameters.format)
    {
        return false;
    }

    ComPtr<IWICBitmapFrameDecode> frameDecode;
    DX::ThrowIfFailed(decoder->GetFrame(0, &frameDecode));
    *ppSource = frameDecode.Detach();

    return true;
}

// Initialize resources that need to be recreated on device lost but do not depend
// on the window size.
void BlockCompressedImagesRenderer::CreateDeviceDependentResources()
{
    // Exit early if the DDS resource doesn't exist or doesn't match expected parameters.
    if (!m_hasValidResources)
    {
        return;
    }

    auto d2dContext = m_deviceResources->GetD2DDeviceContext();

    // Typically, CreateBitmapFromWicBitmap creates a D2D bitmap whose pixel format is derived from
    // the WIC bitmap source's pixel format. However, when it is passed a WIC bitmap source that is
    // backed by a DDS frame decode, it will create a D2D bitmap directly from the block compressed data.
    DX::ThrowIfFailed(d2dContext->CreateBitmapFromWicBitmap(m_wicSourceGuitar.Get(), &m_d2dBitmapGuitar));
    DX::ThrowIfFailed(d2dContext->CreateBitmapFromWicBitmap(m_wicSourceWood.Get(), &m_d2dBitmapWood));

    // This check is for developer education only.
    // Confirm that the D2D bitmaps match the expected parameters.
    auto guitarFormat = m_d2dBitmapGuitar->GetPixelFormat();
    auto woodFormat = m_d2dBitmapWood->GetPixelFormat();
    if (guitarFormat.alphaMode != m_guitarImageInfo.d2dAlpha ||
        guitarFormat.format != m_guitarImageInfo.format ||
        woodFormat.alphaMode != m_woodImageInfo.d2dAlpha ||
        woodFormat.format != m_woodImageInfo.format)
    {
        m_hasValidResources = false;
        return;
    }
}

// Initialize resources that need to be recreated on device lost and depend
// on the window size.
void BlockCompressedImagesRenderer::CreateWindowSizeDependentResources()
{
    // Exit early if the DDS resource doesn't exist or doesn't match expected parameters.
    if (!m_hasValidResources)
    {
        return;
    }

    auto size = m_deviceResources->GetLogicalSize();
    m_woodTranslateX = (size.Width - m_woodWidth) / 2;
    m_woodTranslateY = (size.Height - m_woodHeight) / 2;
    m_guitarTranslateX = (size.Width - m_guitarWidth) / 2;
    m_guitarTranslateY = (size.Height - m_guitarHeight) / 2;
}

// Release all references to resources that depend on the graphics device.
// This method is invoked when the device is lost and resources are no longer usable.
void BlockCompressedImagesRenderer::ReleaseDeviceDependentResources()
{
    m_d2dBitmapGuitar.Reset();
    m_d2dBitmapWood.Reset();
}

// Called once per frame.
void BlockCompressedImagesRenderer::Update(DX::StepTimer const& timer)
{
    m_guitarRotation = static_cast<float>(timer.GetTotalSeconds() * 60.0f);
}

// Renders one frame.
void BlockCompressedImagesRenderer::Render()
{
    auto d2dContext = m_deviceResources->GetD2DDeviceContext();
    d2dContext->BeginDraw();

    // All Direct2D draw calls must respect the 2D transform that accounts for device orientation.
    Matrix3x2F orientation = m_deviceResources->GetOrientationTransform2D();

    // For developer education purposes only.
    // In the case that the expected DDS image assets do not exist, show an error message.
    if (!m_hasValidResources)
    {
        auto size = m_deviceResources->GetLogicalSize();
        ComPtr<ID2D1SolidColorBrush> brush;
        DX::ThrowIfFailed(d2dContext->CreateSolidColorBrush(ColorF(ColorF::White), &brush));
        d2dContext->SetTransform(orientation);
        d2dContext->DrawText(
            m_errorText->Data(),
            m_errorText->Length(),
            m_errorTextFormat.Get(),
            D2D1::RectF(0, 0, size.Width, size.Height),
            brush.Get()
            );
    }
    else
    {
        Matrix3x2F woodTransform = Matrix3x2F::Translation(m_woodTranslateX, m_woodTranslateY) * orientation;
        Matrix3x2F guitarTransform =
            Matrix3x2F::Rotation(m_guitarRotation, Point2F(m_guitarWidth / 2, m_guitarHeight / 2)) *
            Matrix3x2F::Translation(m_guitarTranslateX, m_guitarTranslateY) *
            orientation;

        d2dContext->SetTransform(woodTransform);
        d2dContext->DrawBitmap(m_d2dBitmapWood.Get());
        d2dContext->SetTransform(guitarTransform);
        d2dContext->DrawBitmap(m_d2dBitmapGuitar.Get());
    }

    // We ignore D2DERR_RECREATE_TARGET here. This error indicates that the device
    // is lost. It will be handled during the next call to Present.
    HRESULT hr = d2dContext->EndDraw();
    if (hr != D2DERR_RECREATE_TARGET)
    {
        DX::ThrowIfFailed(hr);
    }
}
