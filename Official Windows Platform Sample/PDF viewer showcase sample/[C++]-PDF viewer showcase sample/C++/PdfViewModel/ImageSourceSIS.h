// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once
#include "ImageSourceBase.h"

namespace PdfViewModel
{
    // ImageSourceSIS class
    ref class ImageSourceSIS sealed : public ImageSourceBase
    {
    public:
        ImageSourceSIS(_In_ Windows::Foundation::Size pageViewSize, _In_ Windows::Data::Pdf::PdfPage^ page, _In_ PdfViewContext^ viewContext)
            : ImageSourceBase(pageViewSize, page, viewContext)
        { }

        void CreateSurface() override;
        void RenderPage() override;
    };
}