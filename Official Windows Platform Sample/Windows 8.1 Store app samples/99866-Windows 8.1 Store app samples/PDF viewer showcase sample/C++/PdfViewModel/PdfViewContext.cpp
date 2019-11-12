// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "PdfViewContext.h"

using namespace Platform;
using namespace Windows::UI::Xaml;
using namespace Windows::Graphics::Display;
using namespace Windows::Foundation;

namespace PdfViewModel
{
    /// <summary>
    /// Constructor for PdfViewContext
    /// </summary>
    /// <param name="pgViewSize">Size of rendered page in view</param>
    /// <param name="pgSize">Size of pdf page in document</param>
    /// <param name="surfaceType">Surface Type SIS/VSIS</param>
    PdfViewContext::PdfViewContext(_In_ Size pgViewSize, _In_ Size pgSize, _In_ PdfViewModel::SurfaceType surfaceType)
        : surfaceType(surfaceType), zoomFactor(1)
    {
        // Create rendering resources
        // Creating new renderer which in turn creates all required reesources needed to render a page
        Renderer = ref new PdfViewModel::Renderer(Window::Current->Bounds, DisplayInformation::GetForCurrentView()->LogicalDpi);

        //Setting height and width of rendered item based on view and actual size
        float scale = min(pgViewSize.Width / pgSize.Width, pgViewSize.Height / pgSize.Height);
        scale = static_cast<int>(scale * 100) > 0 ? scale : 1.0f;
        pageViewSize.Width = pgSize.Width * scale;
        pageViewSize.Height = pgSize.Height * scale;
    }

    /// <summary>
    /// Returns the zoomed-in view size for the document
    /// </summary>
    Size PdfViewContext::GetZoomedPageSize()
    {
        Size zoomedPageSize = pageViewSize;
        zoomedPageSize.Height *= zoomFactor;
        zoomedPageSize.Width *= zoomFactor;
        return zoomedPageSize;
    }
};