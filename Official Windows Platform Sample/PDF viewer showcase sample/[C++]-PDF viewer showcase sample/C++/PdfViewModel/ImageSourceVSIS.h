// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once
#include "ImageSourceBase.h"

namespace PdfViewModel
{
    // ImageSourceVSIS class
    ref class ImageSourceVSIS sealed : public ImageSourceBase
    {
    public:
        ImageSourceVSIS(_In_ Windows::Foundation::Size pageViewSize, _In_ Windows::Data::Pdf::PdfPage^ page, _In_ PdfViewContext^ viewContext)
            : ImageSourceBase(pageViewSize, page, viewContext)
        { }

        void CreateSurface() override;

        Windows::UI::Xaml::Media::Imaging::VirtualSurfaceImageSource^ GetImageSourceVsisBackground();
        void UpdatesNeeded();
        void SwapVsis(_In_ float width, _In_ float height);

    private:
        Windows::UI::Xaml::Media::Imaging::VirtualSurfaceImageSource^ vsisBackground;
    };
}