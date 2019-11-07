// Copyright (c) Microsoft Corporation. All rights reserved

//
// PdfViewContext.h
// Declaration of the PdfViewContext class
//

#pragma once
#include "Renderer.h"

namespace PdfViewModel
{
    // Enumeration for defined surface types used for rendering thumbnails and main view
    public enum class SurfaceType { SurfaceImageSource, VirtualSurfaceImageSource };

    // This class implements PdfViewContext. 
    ref class PdfViewContext
    {
    internal:
        PdfViewContext(_In_ Windows::Foundation::Size pgViewSize, _In_ Windows::Foundation::Size pgSize, _In_ PdfViewModel::SurfaceType surfaceType);
        Windows::Foundation::Size GetZoomedPageSize();

    public:
        // Rendering is defined as a property as it is referred from other classes
        property Renderer^ Renderer;
        property float ZoomFactor 
        { 
            float get() { return zoomFactor; }
            void set(_In_ float value) { zoomFactor = value; }
        }

        property PdfViewModel::SurfaceType SurfaceType
        {
            PdfViewModel::SurfaceType get() { return surfaceType; }
        }

        property Windows::Foundation::Size PageViewSize
        {
            Windows::Foundation::Size get() { return pageViewSize; }
        }

#pragma region State

    private:
        PdfViewModel::SurfaceType surfaceType;
        Windows::Foundation::Size pageViewSize;
        float zoomFactor;

#pragma endregion
    };
}
