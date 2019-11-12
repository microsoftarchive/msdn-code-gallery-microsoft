//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include <math.h>
#include "D2DPageRenderer.h"
#include "D2DPageRendererContext.h"

using namespace Microsoft::WRL;
using namespace Microsoft::WRL::Wrappers;
using namespace Windows::Foundation;
using namespace Windows::UI::Core;
using namespace Windows::Graphics::Display;

PageRendererContext::PageRendererContext(
    _In_ D2D1_RECT_F targetBox,
    _In_ ID2D1DeviceContext* d2dContext,
    _In_ DrawTypes type,
    _In_ PageRenderer^ pageRenderer
    )
{
    m_margin = 96.0f;

    m_d2dContext = d2dContext;

    m_type = type;

    UpdateTargetBox(targetBox);

    DX::ThrowIfFailed(
        d2dContext->CreateSolidColorBrush(
            D2D1::ColorF(D2D1::ColorF::Black),
            &m_blackBrush
            )
        );

    // Pull out all the immutable resources from the PageRenderer that we will need.
    m_textFormat = pageRenderer->GetTextFormatNoRef();
    m_messageFormat = pageRenderer->GetMessageFormatNoRef();
}

void PageRendererContext::UpdateTargetBox(_In_ D2D1_RECT_F& targetBox)
{
    m_targetBox = targetBox;
}

// Draws the scene to a rendering device context or a printing device context.
void PageRendererContext::Draw(_In_ float scale)
{
    // Clear rendering background with CornflowerBlue and clear preview
    // background with white color. For the printing case (command list), it
    // is recommended not to clear because the surface is clean when created.
    if (m_type == DrawTypes::Rendering)
    {
        m_d2dContext->Clear(D2D1::ColorF(D2D1::ColorF::CornflowerBlue));
    }
    else if (m_type == DrawTypes::Preview)
    {
        m_d2dContext->Clear(D2D1::ColorF(D2D1::ColorF::White));
    }

    // We use scale matrix to shrink the text size and scale is only available
    // for preview.  For on-screen rendering or printing, scale is 1.f, that
    // is, the Identity is the transform matrix.
    m_d2dContext->SetTransform(D2D1::Matrix3x2F(1/scale, 0, 0, 1/scale, 0, 0));

    D2D1_RECT_F textBox =
        D2D1::RectF(
            m_targetBox.left + m_margin,
            m_targetBox.top + m_margin,
            m_targetBox.right - m_margin,
            m_targetBox.bottom - m_margin
            );

    const char16 textString[] = L"\
        Lorem ipsum dolor sit amet, consectetur adipisicing elit, \
sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. \
Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris \
nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in \
reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla\
pariatur. Excepteur sint occaecat cupidatat non proident, sunt in \
culpa qui officia deserunt mollit anim id est laborum.";

    m_d2dContext->DrawText(
        textString,
        ARRAYSIZE(textString) - 1,
        m_textFormat.Get(),
        textBox,
        m_blackBrush.Get()
        );
}

// Draws a string to a rendering device context or a printing device context.
void PageRendererContext::DrawMessage(_In_ Platform::String^ string)
{
    // Clear rendering background with CornflowerBlue.
    m_d2dContext->Clear(D2D1::ColorF(D2D1::ColorF::CornflowerBlue));

    m_d2dContext->DrawText(
        string->Data(),
        string->Length(),
        m_messageFormat.Get(),
        m_targetBox,
        m_blackBrush.Get()
        );
}
