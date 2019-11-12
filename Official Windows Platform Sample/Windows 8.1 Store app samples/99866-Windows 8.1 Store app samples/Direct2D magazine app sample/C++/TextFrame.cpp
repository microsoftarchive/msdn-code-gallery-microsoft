//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"

using namespace Microsoft::WRL;
using namespace Windows::Data::Xml::Dom;

TextFrame::TextFrame() :
    Element(),
    m_sourceName(nullptr),
    m_sourceText(nullptr),
    m_brush(),
    m_contentBitmap(),
    m_displaySize(),
    m_glowEffect(),
    m_compositeEffect(),
    m_topMargin(0),
    m_bottomMargin(0),
    m_leading(0),
    m_baseline(0),
    m_textAlign(DWRITE_TEXT_ALIGNMENT_LEADING),
    m_color(0),
    m_opacity(1.0f),
    m_fontSize()
{
}

bool TextFrame::Initialize(
    _In_ Document^ document,
    _In_ XmlElement^ xmlElement
    )
{
    if (!Element::Initialize(document, xmlElement))
    {
        return false;
    }

    Platform::String^ value = xmlElement->GetAttribute("source");

    if (value != nullptr)
    {
        m_sourceName = ParseNameIdentifier(value);
    }

    value = xmlElement->GetAttribute("font-size");

    if (value != nullptr)
    {
        m_fontSize = static_cast<float>(_wtof(value->Data()));
    }

    value = xmlElement->GetAttribute("color");

    if (value != nullptr)
    {
        m_color = ParseColor(value);
    }

    value = xmlElement->GetAttribute("opacity");

    if (value != nullptr)
    {
        m_opacity = static_cast<float>(_wtof(value->Data()));
    }

    value = xmlElement->GetAttribute("top-margin");

    if (value != nullptr)
    {
        m_topMargin = static_cast<float>(_wtof(value->Data()));
    }

    value = xmlElement->GetAttribute("bottom-margin");

    if (value != nullptr)
    {
        m_bottomMargin = static_cast<float>(_wtof(value->Data()));
    }

    value = xmlElement->GetAttribute("leading");

    if (value != nullptr)
    {
        m_leading = static_cast<float>(_wtof(value->Data()));
    }

    value = xmlElement->GetAttribute("baseline");

    if (value != nullptr)
    {
        m_baseline = static_cast<float>(_wtof(value->Data()));
    }

    value = xmlElement->GetAttribute("text-align");

    if (value != nullptr)
    {
        if (value == "leading")
        {
            m_textAlign = DWRITE_TEXT_ALIGNMENT_LEADING;
        }
        else if (value == "trailing")
        {
            m_textAlign = DWRITE_TEXT_ALIGNMENT_TRAILING;
        }
        else if (value == "center")
        {
            m_textAlign = DWRITE_TEXT_ALIGNMENT_CENTER;
        }
    }

    value = xmlElement->GetAttribute("glow");

    if (value != nullptr)
    {
        auto renderer = document->GetRenderer();

        ComPtr<ID2D1DeviceContext> d2dDeviceContext;
        renderer->GetD2DDeviceContext(&d2dDeviceContext);

        uint32 glowColor = 0;

        DX::ThrowIfFailed(
            d2dDeviceContext->CreateEffect(
                CLSID_D2D1Shadow,
                &m_glowEffect
                )
            );

        DX::ThrowIfFailed(
            m_glowEffect->SetValue(
                D2D1_SHADOW_PROP_BLUR_STANDARD_DEVIATION,
                static_cast<float>(_wtof(value->Data()))
                )
            );

        value = xmlElement->GetAttribute("glow-color");

        if (value != nullptr)
        {
            glowColor = ParseColor(value);
        }

        DX::ThrowIfFailed(
            m_glowEffect->SetValue(
                D2D1_SHADOW_PROP_COLOR,
                D2D1::ColorF(glowColor)
                )
            );

        DX::ThrowIfFailed(
            d2dDeviceContext->CreateEffect(
                CLSID_D2D1Composite,
                &m_compositeEffect
                )
            );

        DX::ThrowIfFailed(
            m_compositeEffect->SetValue(
                D2D1_COMPOSITE_PROP_MODE,
                D2D1_COMPOSITE_MODE_SOURCE_OVER
                )
            );

        m_compositeEffect->SetInputEffect(
            0,
            m_glowEffect.Get()
            );
    }

    return true;
}

bool TextFrame::BindResource(_In_ Element^ rootElement)
{
    if (rootElement != nullptr)
    {
        TreeIterator<Element> it(rootElement);

        // Traverse the tree looking for the source image
        do
        {
            ::Text^ text(dynamic_cast<::Text^>(it.GetCurrentNode()));

            if (text != nullptr && text->GetName() == m_sourceName)
            {
                m_sourceText = text;
                return true;
            }

        } while (++it);
    }

    return false;
}

void TextFrame::Measure(
    _In_ Document^ document,
    D2D1_SIZE_F const& parentSize,
    _Out_ D2D1_RECT_F* bounds
    )
{
    ComPtr<IDWriteTextLayout> textLayout;
    m_sourceText->GetTextLayout(&textLayout);

    if (m_fontSize != 0)
    {
        DWRITE_TEXT_RANGE range = {0};
        range.length = static_cast<uint32>(m_sourceText->GetLength());

        DX::ThrowIfFailed(
            textLayout->SetFontSize(m_fontSize, range)
            );
    }

    if (m_size.width <= 0 && m_size.height <= 0)
    {
        // When no size is specified, assume its parent width and height
        m_size = D2D1::SizeF(parentSize.width, parentSize.height);
    }

    D2D1_SIZE_F size = D2D1::SizeF(m_size.width, FLT_MAX);

    if (m_topMargin + m_bottomMargin < size.height)
    {
        size = D2D1::SizeF(size.width, size.height - m_topMargin - m_bottomMargin);
    }
    else
    {
        m_topMargin = m_bottomMargin = 0;
    }

    // Set layout max width and height for text
    DX::ThrowIfFailed(
        textLayout->SetMaxWidth(size.width)
        );

    DX::ThrowIfFailed(
        textLayout->SetMaxHeight(size.height)
        );

    // Set proper horizontal alignment and line spacing
    DX::ThrowIfFailed(
        textLayout->SetTextAlignment(m_textAlign)
        );

    if (m_leading != 0)
    {
        DX::ThrowIfFailed(
            textLayout->SetLineSpacing(
                DWRITE_LINE_SPACING_METHOD_UNIFORM,
                m_leading,
                m_baseline
                )
            );
    }
    else
    {
        DX::ThrowIfFailed(
            textLayout->SetLineSpacing(
                DWRITE_LINE_SPACING_METHOD_DEFAULT,
                m_leading,
                m_baseline
                )
            );
    }

    // Calculate the text layout metrics
    DWRITE_TEXT_METRICS textMetrics = {0};

    DX::ThrowIfFailed(
        textLayout->GetMetrics(&textMetrics)
        );

    // The final height is the text layout height plus top and bottom margin.
    m_size = D2D1::SizeF(textMetrics.width, textMetrics.height + m_topMargin + m_bottomMargin);

    *bounds = D2D1::RectF(
        m_offset.x,
        m_offset.y,
        m_offset.x + m_size.width,
        m_offset.y + m_size.height
        );
}

bool TextFrame::PrepareToDraw(
    _In_ Document^ document,
    D2D1::Matrix3x2F const& transform
    )
{
    auto renderer = document->GetRenderer();

    ComPtr<ID2D1DeviceContext> d2dDeviceContext;
    renderer->GetD2DDeviceContext(&d2dDeviceContext);

    if (m_brush == nullptr)
    {
        ComPtr<ID2D1SolidColorBrush> solidColorBrush;

        DX::ThrowIfFailed(
            d2dDeviceContext->CreateSolidColorBrush(
                D2D1::ColorF(m_color, m_opacity),
                D2D1::BrushProperties(),
                &solidColorBrush
                )
            );

        m_brush = solidColorBrush;
    }

    if (    m_glowEffect != nullptr
        &&  m_compositeEffect != nullptr
        &&  (   m_contentBitmap == nullptr
            ||  m_displaySize.width != renderer->GetDisplayWidth()
            ||  m_displaySize.height != renderer->GetDisplayHeight()
            )
        )
    {
        ComPtr<IDWriteTextLayout> textLayout;
        m_sourceText->GetTextLayout(&textLayout);

        D2D1_SIZE_U bitmapPixelSize = D2D1::SizeU(
            static_cast<uint32>(transform._11 * m_size.width),
            static_cast<uint32>(transform._22 * m_size.height)
            );

        // Create intermediate bitmap as input to the effects.
        // The bitmap size is specified in pixels.
        DX::ThrowIfFailed(
            d2dDeviceContext->CreateBitmap(
                bitmapPixelSize,
                nullptr,
                0,
                D2D1::BitmapProperties1(
                    D2D1_BITMAP_OPTIONS_TARGET,
                    D2D1::PixelFormat(DXGI_FORMAT_B8G8R8A8_UNORM, D2D1_ALPHA_MODE_PREMULTIPLIED),
                    96.0,
                    96.0
                    ),
                &m_contentBitmap
                )
            );

        // Keep track of what display size the cached content is for.
        m_displaySize = renderer->GetDisplaySize();

        // Preserve pre-existing target
        ComPtr<ID2D1Image> oldTarget;

        d2dDeviceContext->GetTarget(&oldTarget);

        // Set the view target to an intermediate bitmap
        d2dDeviceContext->SetTarget(m_contentBitmap.Get());

        d2dDeviceContext->Clear(D2D1::ColorF(0, 0));

        // Only set the scale transform and leave the translation
        // as we're drawing only this element to an intermediate.
        d2dDeviceContext->SetTransform(
            D2D1::Matrix3x2F::Scale(transform._11, transform._22)
            );

        // Draw text onto an intermediate bitmap
        d2dDeviceContext->DrawTextLayout(
            D2D1::Point2F(0, m_topMargin),
            textLayout.Get(),
            m_brush.Get(),
            D2D1_DRAW_TEXT_OPTIONS_NONE
            );

        // Restore the previous target
        d2dDeviceContext->SetTarget(oldTarget.Get());
    }

    return false;
}

bool TextFrame::Draw(
    _In_ Document^ document,
    D2D1::Matrix3x2F const& transform
    )
{
    auto renderer = document->GetRenderer();

    ComPtr<ID2D1DeviceContext> d2dDeviceContext;
    renderer->GetD2DDeviceContext(&d2dDeviceContext);

    ComPtr<IDWriteTextLayout> textLayout;
    m_sourceText->GetTextLayout(&textLayout);

    if (m_glowEffect == nullptr || m_compositeEffect == nullptr)
    {
        d2dDeviceContext->SetTransform(transform);

        d2dDeviceContext->DrawTextLayout(
            D2D1::Point2F(m_offset.x, m_offset.y + m_topMargin),
            textLayout.Get(),
            m_brush.Get(),
            D2D1_DRAW_TEXT_OPTIONS_NONE
            );
    }
    else if (m_contentBitmap != nullptr)
    {
        // Set input bitmap for the glow effect, then set the second
        // input to the composite effect as the input bitmap to create
        // the final result where the original text is composited over
        // the glow.
        m_glowEffect->SetInput(
            0,
            m_contentBitmap.Get()
            );

        m_compositeEffect->SetInput(
            1,
            m_contentBitmap.Get()
            );

        ComPtr<ID2D1Image> image;
        m_compositeEffect->GetOutput(&image);

        // Remove the scaling factor as the local bitmap is drawn to the render target.
        d2dDeviceContext->SetTransform(
            D2D1::Matrix3x2F::Translation(
                transform._31 + transform._11 * m_offset.x,
                transform._32 + transform._22 * m_offset.y
                )
            );

        // Execute the effect graph and draw the final result onto the
        // original view target.
        d2dDeviceContext->DrawImage(
            image.Get(),
            &m_offset,
            nullptr,
            D2D1_INTERPOLATION_MODE_HIGH_QUALITY_CUBIC,
            D2D1_COMPOSITE_MODE_SOURCE_OVER
            );
    }

    return false;
}
