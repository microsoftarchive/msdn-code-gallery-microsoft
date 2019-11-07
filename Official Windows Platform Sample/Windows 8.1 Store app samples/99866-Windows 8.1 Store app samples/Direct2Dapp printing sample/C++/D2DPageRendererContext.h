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

    void Draw(_In_ float scale);

    void DrawMessage(_In_ Platform::String^ string);

private:
    Microsoft::WRL::ComPtr<ID2D1DeviceContext>      m_d2dContext;
    Microsoft::WRL::ComPtr<IDWriteTextFormat>       m_textFormat;
    Microsoft::WRL::ComPtr<IDWriteTextFormat>       m_messageFormat;
    Microsoft::WRL::ComPtr<ID2D1SolidColorBrush>    m_blackBrush;

    float                                           m_margin;        // The margin size is in DIPs.

    D2D1_RECT_F                                     m_targetBox;     // The region that the page contents should be formatted for.

    DrawTypes                                       m_type;
};
