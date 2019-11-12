//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once
#include <DirectXBase.h>
#include <printpreview.h>
#include "SampleOverlay.h"

enum class DrawTypes
{
    Rendering,
    Preview,
    Printing
};

// RAII (Resource Acquisition Is Initialization) class for manually
// acquiring/releasing the D2D lock.
class D2DFactoryLock
{
public:
    D2DFactoryLock(_In_ ID2D1Factory* d2dFactory)
    {
        DX::ThrowIfFailed(
            d2dFactory->QueryInterface(IID_PPV_ARGS(&m_d2dMultithread))
            );

        m_d2dMultithread->Enter();
    }

    ~D2DFactoryLock()
    {
        m_d2dMultithread->Leave();
    }

private:
    Microsoft::WRL::ComPtr<ID2D1Multithread> m_d2dMultithread;
};

// Struct for each image section.
struct ImageSection
{
    D2D1_RECT_F         box;            // Bounding box for the whole image section.
    D2D1_RECT_F         titleBox;       // The bounding box for the image title.
    Platform::String^   titleString;    // Image title string.
    D2D1_RECT_F         imageBox;       // Bounding box for the image.
};

// This class is in charge of rendering the contents of the page to an
// arbitrary device context (which may be backed by a D2D bitmap in the case of
// display or print-preview, or a D2D command list in the case of print).
//
// Note: unlike the PageRender class, this class is thread-specific, and hence
// can hold on to both mutable and immutable resources.
//
// To conserve memory, it is preferable to store heavy-weight resources in the
// PageRenderer, and take only a reference to those resources inside
// PageRendererContext.
ref class PageRendererContext
{
internal:
    PageRendererContext(
        _In_ D2D1_RECT_F targetBox,
        _In_ ID2D1DeviceContext* d2dContext,
        _In_ DrawTypes type,
        _In_ PageRenderer^ pageRenderer
        );

    void UpdateTargetBox(_In_ D2D1_RECT_F& targetBox);

    void Draw();

    void DrawMessage(_In_ Platform::String^ string);

private:
    void InitializeSections();

    void CreateImageResources(
        _In_ ID2D1DeviceContext* d2dContext,
        _In_ DrawTypes type,
        _In_ IWICBitmapSource* originalWicBitmapSource,
        _In_ IWICBitmapSource* wicBitmapSourceWithColorContext,
        _In_ IWICColorContext* wicColorContext
        );

    void SetTitleAndImageBoxes(
        _In_ D2D1_RECT_F sectionBox,
        _Out_ D2D1_RECT_F* titleBox,
        _Out_ D2D1_RECT_F* imageBox
        );

    void DrawImages();

    Microsoft::WRL::ComPtr<ID2D1DeviceContext>      m_d2dContext;
    Microsoft::WRL::ComPtr<IDWriteTextFormat>       m_textFormat;
    Microsoft::WRL::ComPtr<IDWriteTextFormat>       m_messageFormat;
    Microsoft::WRL::ComPtr<ID2D1SolidColorBrush>    m_blackBrush;

    // Effects.
    Microsoft::WRL::ComPtr<ID2D1Effect>             m_blurEffect;
    Microsoft::WRL::ComPtr<ID2D1Effect>             m_colorManagementEffect;
    Microsoft::WRL::ComPtr<ID2D1Effect>             m_originalBitmapSourceEffect;
    Microsoft::WRL::ComPtr<ID2D1Effect>             m_bitmapSourceEffectForColorManagement;

    // Image sections.
    ImageSection                                    m_sectionOriginal;      // Section for the original image.
    ImageSection                                    m_sectionBlur;          // Section for a blurred image.
    ImageSection                                    m_sectionColorManaged;  // Seciton for a color-managed image.

    bool                                            m_sectionsValid;        // Whether m_targetBox is large enough to draw all image sections.
    D2D1_RECT_F                                     m_targetBox;            // The region that the page contents should be formatted for.

    UINT                                            m_bitmapWidth;
    UINT                                            m_bitmapHeight;
    float                                           m_bitmapDpiX;
    float                                           m_bitmapDpiY;
    DrawTypes                                       m_type;
};

