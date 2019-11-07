//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

ref class Renderer;
ref class Element;

//  Native host of VirtualSurfaceImageSource, which implements update callback.
//  The callback is called whenever the application needs to populate new content to
//  the image source which occurs when the image source hosting UI element repaints.
//
class ContentImageSource : public IVirtualSurfaceUpdatesCallbackNative
{
public:
    ContentImageSource(
        _In_ Element^ content,
        _In_ Document^ document,
        bool isOpaque
        );

    virtual HRESULT STDMETHODCALLTYPE UpdatesNeeded() override;

    virtual HRESULT STDMETHODCALLTYPE QueryInterface(
        REFIID uuid,
        _Outptr_ void** object
        ) override;

    virtual ULONG STDMETHODCALLTYPE AddRef() override;

    virtual ULONG STDMETHODCALLTYPE Release() override;

    Windows::UI::Xaml::Media::ImageSource^ GetImageSource();

    SIZE GetImageSize();

    void UpdateContent(_In_ Element^ content);

private:

    // This class is used to ensure proper pairing of IVirtualSurfaceImageSourceNative::BeginDraw
    // and EndDraw within an error condition. The XAML framework expects EndDraw to be called whenever
    // BeginDraw is called - even in failure cases
    class ImageSourceDrawHelper
    {
    public:
        ImageSourceDrawHelper(
            IVirtualSurfaceImageSourceNative *imageSourceNative,
            _In_ RECT updateRect,
            _Out_ IDXGISurface **pSurface,
            _Out_ POINT *offset,
            HRESULT *hr
            );
        ~ImageSourceDrawHelper();

    private:
        Microsoft::WRL::ComPtr<IVirtualSurfaceImageSourceNative> m_imageSourceNative;
        HRESULT *m_hr;
    };

    void Initialize();

    void Measure(_Out_ SIZE* contentSize);

    bool Draw(RECT const& drawingBounds);

    // Reference counter
    ULONG m_refCount;

    // The current document being rendered
    Document^ m_document;

    // Content to render
    Element^ m_content;

    // Size of the content in pixels
    SIZE m_contentSize;

    // Virtual surface image source
    Windows::UI::Xaml::Media::Imaging::VirtualSurfaceImageSource^ m_imageSource;

    // Native interface of the virtual surface image source
    Microsoft::WRL::ComPtr<IVirtualSurfaceImageSourceNative> m_imageSourceNative;

    // Flag indicating whether the image surface is opaque
    bool m_isOpaque;
};
