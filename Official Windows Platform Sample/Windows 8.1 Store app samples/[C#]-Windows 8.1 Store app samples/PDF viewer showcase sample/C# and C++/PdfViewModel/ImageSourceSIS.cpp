// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "ImageSourceSIS.h"
#include "PdfViewContext.h"

using namespace Microsoft::WRL;
using namespace Windows::UI::Xaml::Media::Imaging;

namespace PdfViewModel
{
    /// <summary>
    /// Creates required resources SIS
    /// </summary>
    void ImageSourceSIS::CreateSurface()
    {
        auto sis = ref new SurfaceImageSource(static_cast<int>(width), static_cast<int>(height), true /*isOpaque*/);
        reinterpret_cast<IInspectable*>(sis)->QueryInterface(IID_PPV_ARGS(&sisNative));
        ComPtr<IDXGIDevice> dxgiDevice;
        pdfViewContext->Renderer->GetDXGIDevice(&dxgiDevice);
        DX::ThrowIfFailed(sisNative->SetDevice(dxgiDevice.Get()));
    }

    /// <summary>
    /// Following functions triggers rendering of page on the binded surface
    /// </summary>
    void ImageSourceSIS::RenderPage()
    {
        RECT updateRect = { 0, 0, static_cast<long>(width), static_cast<long>(height) };
        RenderPageRect(updateRect);
    }
}