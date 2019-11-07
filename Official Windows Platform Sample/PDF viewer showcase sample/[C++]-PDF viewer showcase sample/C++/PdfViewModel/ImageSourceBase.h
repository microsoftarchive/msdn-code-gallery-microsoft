// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

namespace PdfViewModel
{
    ref class PdfViewContext;

    template <typename T>
    _Ret_maybenull_ T^ AsObject(_In_ IUnknown *unk)
    {
        T^ obj = nullptr;
        ComPtr<IInspectable> insp;
        if (SUCCEEDED(unk->QueryInterface(IID_PPV_ARGS(&insp))))
        {
            obj = dynamic_cast<T^>(reinterpret_cast<Platform::Object^>(insp.Get()));
        }
        return obj;
    }

    ref class ImageSourceBase abstract
    {
    internal:
        ImageSourceBase(_In_ Windows::Foundation::Size pageViewSize, _In_ Windows::Data::Pdf::PdfPage^ page, _In_ PdfViewContext^ viewContext);

    public:
        void SetPageDimensions(_In_ float userWidth, _In_ float userHeight);
        Windows::UI::Xaml::Media::Imaging::SurfaceImageSource^ GetImageSource();
        virtual void CreateSurface() = 0;
        virtual void RenderPage() { }

    protected private:
        void RenderPageRect(_In_ RECT const& pageRect);

    protected private:
        Windows::Data::Pdf::PdfPage^ pdfPage;
        PdfViewContext^ pdfViewContext;
        float width;
        float height;
        Microsoft::WRL::ComPtr<ISurfaceImageSourceNative> sisNative;
    };

    struct SurfaceData
    {
        Microsoft::WRL::ComPtr<IDXGISurface> dxgiSurface;
        POINT offset;

        SurfaceData()
        {
            offset.x = offset.y = 0;
        }
    };
}