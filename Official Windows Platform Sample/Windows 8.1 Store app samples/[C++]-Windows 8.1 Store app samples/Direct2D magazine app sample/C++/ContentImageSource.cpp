//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"

using namespace Microsoft::WRL;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Media::Imaging;

ContentImageSource::ContentImageSource(
    _In_ Element^ content,
    _In_ Document^ document,
    bool isOpaque
    ) :
    m_refCount(),
    m_content(content),
    m_contentSize(),
    m_document(document),
    m_imageSource(nullptr),
    m_imageSourceNative(),
    m_isOpaque(isOpaque)
{
}

void ContentImageSource::Initialize()
{
    // Measure the content and store its size.
    Measure(&m_contentSize);

    // Create an image source at the initial pixel size.
    // The last parameter helps improve rendering performance when the image source is known to be opaque.
    m_imageSource = ref new VirtualSurfaceImageSource(m_contentSize.cx, m_contentSize.cy, m_isOpaque);

    ComPtr<IUnknown> unknown(reinterpret_cast<IUnknown*>(m_imageSource));
    unknown.As(&m_imageSourceNative);

    auto renderer = m_document->GetRenderer();

    // Set DXGI device to the image source
    ComPtr<IDXGIDevice> dxgiDevice;
    renderer->GetDXGIDevice(&dxgiDevice);
    m_imageSourceNative->SetDevice(dxgiDevice.Get());

    // Register image source's update callback so update can be made to it.
    m_imageSourceNative->RegisterForUpdatesNeeded(this);
}

ImageSource^ ContentImageSource::GetImageSource()
{
    if (m_imageSource == nullptr)
    {
        Initialize();
    }

    return m_imageSource;
}

SIZE ContentImageSource::GetImageSize()
{
    if (m_imageSource == nullptr)
    {
        Initialize();
    }

    return m_contentSize;
}

// This method is called when the framework needs to update region managed by
// the virtual surface image source.
HRESULT STDMETHODCALLTYPE ContentImageSource::UpdatesNeeded()
{
    HRESULT hr = S_OK;

    try
    {
        ULONG drawingBoundsCount = 0;

        DX::ThrowIfFailed(
            m_imageSourceNative->GetUpdateRectCount(&drawingBoundsCount)
            );

        std::unique_ptr<RECT[]> drawingBounds(new RECT[drawingBoundsCount]);

        DX::ThrowIfFailed(
            m_imageSourceNative->GetUpdateRects(drawingBounds.get(), drawingBoundsCount)
            );

        // This code doesn't try to coalesce multiple drawing bounds into one. Although that
        // extra process will reduce the number of draw calls, it requires the virtual surface
        // image source to manage non-uniform tile size, which requires it to make extra copy
        // operations to the compositor. By using the drawing bounds it directly returns, which are
        // of non-overlapping uniform tile size, the compositor is able to use these tiles directly,
        // which can greatly reduce the amount of memory needed by the virtual surface image source.
        // It will result in more draw calls though, but Direct2D will be able to accommodate that
        // without significant impact on presentation frame rate.
        for (ULONG i = 0; i < drawingBoundsCount; ++i)
        {
            if (Draw(drawingBounds[i]))
            {
                // Drawing isn't complete. This can happen when the content is still being
                // asynchronously loaded. Inform the image source to invalidate the drawing
                // bounds so that it calls back to redraw.
                DX::ThrowIfFailed(
                    m_imageSourceNative->Invalidate(drawingBounds[i])
                    );
            }
        }
    }
    catch (Platform::Exception^ exception)
    {
        hr = exception->HResult;

        if (hr == D2DERR_RECREATE_TARGET ||
            hr == DXGI_ERROR_DEVICE_REMOVED)
        {
            // If a device lost error is encountered, notify the renderer. The renderer will then
            // recreate the device and fire the DeviceLost event so that all listeners can take
            // an appropriate action.
            m_document->GetRenderer()->HandleDeviceLost();

            // Invalidate the image source so that it calls back to redraw
            RECT bounds = { 0, 0, m_contentSize.cx, m_contentSize.cy };
            m_imageSourceNative->Invalidate(bounds);
        }
    }

    return hr;
}

void ContentImageSource::Measure(_Out_ SIZE* contentSize)
{
    D2D1_RECT_F contentBounds = {0};

    // Measure the drawing content
    m_content->Measure(
        m_document,
        m_document->GetPageSize(),
        &contentBounds
        );

    contentSize->cx = static_cast<long>(m_document->DesignToDisplayWidth(contentBounds.right - contentBounds.left));
    contentSize->cy = static_cast<long>(m_document->DesignToDisplayHeight(contentBounds.bottom - contentBounds.top));
}

bool ContentImageSource::Draw(RECT const& drawingBounds)
{
    ComPtr<IDXGISurface> dxgiSurface;
    POINT surfaceOffset = {0};
    bool needRedraw;

    HRESULT hr = S_OK;
    {
        // If the underlying device is lost, the virtual surface image source may return a
        // device lost error from BeginDraw. In this case, the error will be thrown here
        // and caught in UpdatesNeeded
        ImageSourceDrawHelper imageSourceDrawHelper(
            m_imageSourceNative.Get(),
            drawingBounds,
            &dxgiSurface,
            &surfaceOffset,
            &hr
            );

        auto renderer = m_document->GetRenderer();

        ComPtr<ID2D1DeviceContext> d2dDeviceContext;
        renderer->GetD2DDeviceContext(&d2dDeviceContext);

        ComPtr<ID2D1Bitmap1> bitmap;
        DX::ThrowIfFailed(
            d2dDeviceContext->CreateBitmapFromDxgiSurface(
                dxgiSurface.Get(),
                nullptr,
                &bitmap
                )
            );

        // Begin the drawing batch
        d2dDeviceContext->BeginDraw();

        // Scale content design coordinate to the display coordinate,
        // then translate the drawing to the designated place on the surface.
        D2D1::Matrix3x2F transform =
            D2D1::Matrix3x2F::Scale(
                m_document->DesignToDisplayWidth(1.0f),
                m_document->DesignToDisplayHeight(1.0f)
                ) *
            D2D1::Matrix3x2F::Translation(
                static_cast<float>(surfaceOffset.x - drawingBounds.left),
                static_cast<float>(surfaceOffset.y - drawingBounds.top)
                );

        // Prepare to draw content. This is the appropriate time for content element
        // to draw to an intermediate if there is any. It is important for performance
        // reason that SetTarget isn't called too frequently. Preparing the intermediates
        // upfront reduces the number of times the render target switches back and forth.
        needRedraw = m_content->PrepareToDraw(
            m_document,
            transform
            );

        if (!needRedraw)
        {
            // Set the render target to surface given by the framework
            d2dDeviceContext->SetTarget(bitmap.Get());

            d2dDeviceContext->SetTransform(D2D1::IdentityMatrix());

            // Constrain the drawing only to the designated portion of the surface
            d2dDeviceContext->PushAxisAlignedClip(
                D2D1::RectF(
                    static_cast<float>(surfaceOffset.x),
                    static_cast<float>(surfaceOffset.y),
                    static_cast<float>(surfaceOffset.x + (drawingBounds.right - drawingBounds.left)),
                    static_cast<float>(surfaceOffset.y + (drawingBounds.bottom - drawingBounds.top))
                    ),
                D2D1_ANTIALIAS_MODE_ALIASED
                );

            // The Clear call must follow and not precede the PushAxisAlignedClip call.
            // Placing the Clear call before the clip is set violates the contract of the
            // virtual surface image source in that the application draws outside the
            // designated portion of the surface the image source hands over to it. This
            // violation won't actually cause the content to spill outside the designated
            // area because the image source will safeguard it. But this extra protection
            // has a runtime cost associated with it, and in some drivers this cost can be
            // very expensive. So the best performance strategy here is to never create a
            // situation where this protection is required. Not drawing outside the appropriate
            // clip does that the right way.
            d2dDeviceContext->Clear(D2D1::ColorF(0, 0));

            // Draw the content
            needRedraw = m_content->Draw(
                m_document,
                transform
                );

            d2dDeviceContext->PopAxisAlignedClip();

            d2dDeviceContext->SetTarget(nullptr);
        }

        // End the drawing
        // UpdatesNeeded handles D2DERR_RECREATETARGET, so it is okay to throw this error here.
        DX::ThrowIfFailed(
            d2dDeviceContext->EndDraw()
            );
    }

    DX::ThrowIfFailed(hr);

    return needRedraw;
}

void ContentImageSource::UpdateContent(_In_ Element^ content)
{
    if (m_imageSourceNative != nullptr)
    {
        SIZE oldContentSize = m_contentSize;

        m_content = content;

        Measure(&m_contentSize);

        DX::ThrowIfFailed(
            m_imageSourceNative->Resize(m_contentSize.cx, m_contentSize.cy)
            );

        RECT bounds = {0};
        bounds.right =  m_contentSize.cx;
        bounds.bottom = m_contentSize.cy;

        DX::ThrowIfFailed(
            m_imageSourceNative->Invalidate(bounds)
            );
    }
}

HRESULT STDMETHODCALLTYPE ContentImageSource::QueryInterface(
    REFIID uuid,
    _Outptr_ void** object
    )
{
    if (    uuid == IID_IUnknown
        ||  uuid == __uuidof(IVirtualSurfaceUpdatesCallbackNative)
        )
    {
        *object = this;
        AddRef();
        return S_OK;
    }
    else
    {
        *object = nullptr;
        return E_NOINTERFACE;
    }
}

ULONG STDMETHODCALLTYPE ContentImageSource::AddRef()
{
    return static_cast<ULONG>(InterlockedIncrement(&m_refCount));
}

ULONG STDMETHODCALLTYPE ContentImageSource::Release()
{
    ULONG newCount = static_cast<ULONG>(InterlockedDecrement(&m_refCount));

    if (newCount == 0)
        delete this;

    return newCount;
}

ContentImageSource::ImageSourceDrawHelper::ImageSourceDrawHelper(
    IVirtualSurfaceImageSourceNative *imageSourceNative,
    _In_ RECT updateRect,
    _Out_ IDXGISurface **pSurface,
    _Out_ POINT *offset,
    _Inout_ HRESULT *hr
    ) :
    m_imageSourceNative(imageSourceNative),
    m_hr(hr)
{
    DX::ThrowIfFailed(
        imageSourceNative->BeginDraw(
            updateRect,
            pSurface,
            offset
            )
        );
}

ContentImageSource::ImageSourceDrawHelper::~ImageSourceDrawHelper()
{
    // It is not safe to throw from a destructor. Instead we pass the error back to the caller
    // and let the caller throw the error.
    *m_hr = m_imageSourceNative->EndDraw();
}