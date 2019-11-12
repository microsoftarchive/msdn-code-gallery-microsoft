// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "ImageSourceBase.h"
#include "PdfViewContext.h"

using namespace concurrency;
using namespace Microsoft::WRL;
using namespace Windows::Foundation;
using namespace Windows::Graphics::Display;
using namespace Windows::UI::Xaml::Media::Imaging;
using namespace Windows::Data::Pdf;

namespace PdfViewModel
{
    // This class is responsible for implementing base methods for creation of rendering surface for zoomed-in and zoomed-out view.
    ImageSourceBase::ImageSourceBase(_In_ Size pageViewSize, _In_ PdfPage^ page, _In_ PdfViewContext^ viewContext)
        : pdfPage(page), pdfViewContext(viewContext)
    {
        // Set page dimensions so that they can be used for creation of required surfaces
        SetPageDimensions(pageViewSize.Width, pageViewSize.Height);
    }

    void ImageSourceBase::SetPageDimensions(_In_ float userWidth, _In_ float userHeight)
    {
        float dpi = DisplayInformation::GetForCurrentView()->LogicalDpi;
        width = userWidth * dpi / 100;
        height = userHeight * dpi / 100;
    }

    /// <summary>
    /// Following function returns native surface as SurfaceImageSource
    /// </summary>
    /// <returns>SIS surface</returns>
    SurfaceImageSource^ ImageSourceBase::GetImageSource()
    {
        return AsObject<SurfaceImageSource>(sisNative.Get());
    }

    /// <summary>
    /// This function renders PDF page content on a SIS surface using pdfNativeRenderer
    /// </summary>
    /// <param name="pageRect">Page Rect to be rendered</param>
    void ImageSourceBase::RenderPageRect(_In_ RECT const& pageRect)
    {
        SurfaceData sisData;
        if (sisNative != nullptr)
        {
            // Begin Draw
            HRESULT hr = sisNative->BeginDraw(pageRect, &(sisData.dxgiSurface), &(sisData.offset));

            if (SUCCEEDED(hr))
            {
                // Draw to IDXGISurface (the surface paramater)
                ComPtr<IPdfRendererNative> pdfRendererNative;
                pdfViewContext->Renderer->GetPdfNativeRenderer(&pdfRendererNative);

                Size pageSize = pdfPage->Size;
                float scale = min(width / pageSize.Width, height / pageSize.Height);

                PDF_RENDER_PARAMS params;

                params.SourceRect = D2D1::RectF((pageRect.left / scale),
                    (pageRect.top / scale),
                    (pageRect.right / scale),
                    (pageRect.bottom / scale));
                params.DestinationHeight = pageRect.bottom - pageRect.top;
                params.DestinationWidth = pageRect.right - pageRect.left;

                params.BackgroundColor = D2D1::ColorF(D2D1::ColorF::White);

                // When this flag is set to FALSE high contrast mode will be honored by PDF API's
                params.IgnoreHighContrast = FALSE;

                // Call PDF API RenderPageToSurface to render the content of page on SIS surface 
                pdfRendererNative->RenderPageToSurface(reinterpret_cast<IUnknown*>(pdfPage), sisData.dxgiSurface.Get(), sisData.offset, &params);

                // End Draw
                sisNative->EndDraw();
            }
            else if ((hr == DXGI_ERROR_DEVICE_REMOVED) || (hr == DXGI_ERROR_DEVICE_RESET))
            {
                // Handle device lost
                pdfViewContext->Renderer->HandleDeviceLost();
            }
        }
    }
}