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

static const float sc_sectionMargin = 96.0f * 0.5f;     // Gaps between neighboring sections, in DIPs.
static const float sc_sectionWidth = 96.0f * 3.0f;      // Section width, in DIPs.
static const float sc_sectionHeight = 96.0f * 3.0f;     // Section height, in DIPs.
static const float sc_blur_deviation = 5.0f;            // Deviation value for the D2D Gaussian Blur effect, in DIPs.

PageRendererContext::PageRendererContext(
    _In_ D2D1_RECT_F targetBox,
    _In_ ID2D1DeviceContext* d2dContext,
    _In_ DrawTypes type,
    _In_ PageRenderer^ pageRenderer
    )
{
    m_d2dContext = d2dContext;

    m_type = type;

    // Initialize image sections for display/print.
    InitializeSections();

    CreateImageResources(
        d2dContext,
        type,
        pageRenderer->GetOriginalWicBitmapSourceNoRef(),
        pageRenderer->GetWicBitmapSourceWithEmbeddedColorContextNoRef(),
        pageRenderer->GetWicColorContextNoRef()
        );

    // Get bitmap size.
    DX::ThrowIfFailed(
        pageRenderer->GetOriginalWicBitmapSourceNoRef()->GetSize(&m_bitmapWidth, &m_bitmapHeight)
        );

    // Get bitmap resolution.
    double bitmapDpiX;
    double bitmapDpiY;
    DX::ThrowIfFailed(
        pageRenderer->GetOriginalWicBitmapSourceNoRef()->GetResolution(&bitmapDpiX, &bitmapDpiY)
        );

    m_bitmapDpiX = static_cast<float>(bitmapDpiX);
    m_bitmapDpiY = static_cast<float>(bitmapDpiY);

    UpdateTargetBox(targetBox);

    DX::ThrowIfFailed(
        d2dContext->CreateSolidColorBrush(
            D2D1::ColorF(D2D1::ColorF::Black),
            &m_blackBrush
            )
        );

    m_textFormat = pageRenderer->GetTextFormatNoRef();
    m_messageFormat = pageRenderer->GetMessageFormatNoRef();
}

void PageRendererContext::UpdateTargetBox(_In_ D2D1_RECT_F& targetBox)
{
    m_targetBox = targetBox;

    FLOAT left = targetBox.left;
    FLOAT top  = targetBox.top;
    FLOAT right = targetBox.right;
    FLOAT bottom = targetBox.bottom;

    // Gap between the left edge of the imageable area and the first section.
    FLOAT marginLeft = (right - left - sc_sectionMargin) / 2 - sc_sectionWidth;

    // Gap between the top edge of the imageable area and the first section.
    FLOAT marginTop = (bottom - top - sc_sectionMargin) / 2 - sc_sectionHeight;

    // Check if we have sufficient area to draw all images.
    if (marginLeft < 0.0f || marginTop < 0.0f)
    {
        m_sectionsValid = false;
    }
    else
    {
        // The original image is placed at the top-left quadrant.
        m_sectionOriginal.box = D2D1::RectF(
            left + marginLeft,
            top + marginTop,
            left + marginLeft + sc_sectionWidth,
            top + marginTop + sc_sectionHeight
            );
        SetTitleAndImageBoxes(
            m_sectionOriginal.box,
            &m_sectionOriginal.titleBox,
            &m_sectionOriginal.imageBox
            );

        // The blurred image is placed at the top-right quadrant.
        m_sectionBlur.box = D2D1::RectF(
            right - marginLeft - sc_sectionWidth,
            top + marginTop,
            right - marginLeft,
            top + marginTop + sc_sectionHeight
            );
        SetTitleAndImageBoxes(
            m_sectionBlur.box,
            &m_sectionBlur.titleBox,
            &m_sectionBlur.imageBox
            );

        // The color managed image is placed at the bottom-left quadrant.
        m_sectionColorManaged.box = D2D1::RectF(
            left + marginLeft,
            bottom - marginTop - sc_sectionHeight,
            left + marginLeft + sc_sectionWidth,
            bottom - marginTop
            );
        SetTitleAndImageBoxes(
            m_sectionColorManaged.box,
            &m_sectionColorManaged.titleBox,
            &m_sectionColorManaged.imageBox
            );

        // Resize all images to fit the target.
        // Note that both the target size and the bitmap size are adjusted to
        // DIPs before calculating the scale ratios.
        float zoomX = (m_sectionOriginal.imageBox.right - m_sectionOriginal.imageBox.left)
                        / (m_bitmapWidth / m_bitmapDpiX * 96.0f);
        float zoomY = (m_sectionOriginal.imageBox.bottom - m_sectionOriginal.imageBox.top)
                        / (m_bitmapHeight / m_bitmapDpiY * 96.0f);
        D2D1_VECTOR_2F zoomVector = D2D1::Vector2F(zoomX, zoomY);

        // Scale the BitmapSource effect that corresponds to the original bitmap.
        DX::ThrowIfFailed(
            m_originalBitmapSourceEffect->SetValue(D2D1_BITMAPSOURCE_PROP_SCALE, zoomVector)
            );

        DX::ThrowIfFailed(
            m_bitmapSourceEffectForColorManagement->SetValue(D2D1_BITMAPSOURCE_PROP_SCALE, zoomVector)
            );

        m_sectionsValid = true;
    }
}

//
// Draws the scene to a rendering device context or a printing device context.
//
void PageRendererContext::Draw()
{
    // Clear rendering background with CornflowerBlue and clear preview
    // background with white color.
    // For the printing case (command list), it is recommended not to clear
    // because the surface is clean when created.
    if (m_type == DrawTypes::Rendering)
    {
        m_d2dContext->Clear(D2D1::ColorF(D2D1::ColorF::CornflowerBlue));
    }
    else if (m_type == DrawTypes::Preview)
    {
        m_d2dContext->Clear(D2D1::ColorF(D2D1::ColorF::White));
    }

    DrawImages();
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

// Initialize image sections.
void PageRendererContext::InitializeSections()
{
    m_sectionOriginal.titleString = L"Original";
    m_sectionBlur.titleString = L"Blurred";
    m_sectionColorManaged.titleString = L"Color Managed";
}

// Create device-dependent image resources.
void PageRendererContext::CreateImageResources(
    _In_ ID2D1DeviceContext* d2dContext,
    _In_ DrawTypes type,
    _In_ IWICBitmapSource* originalWicBitmapSource,
    _In_ IWICBitmapSource* wicBitmapSourceWithColorContext,
    _In_ IWICColorContext* wicColorContext
    )
{
    // Create a BitmapSource effect based on the input bitmap.
    // The output of this effect will be used for both display and printing.
    //
    // There are two ways to display a bitmap on the screen:
    // 1. To create a D2D bitmap from the WIC bitmap source, then draw with
    //    ID2D1DeviceContext::DrawBitmap();
    //
    // 2. To create a BitmapSource Effect from the WIC bitmap source, then draw
    //    with ID2D1DeviceContext::DrawImage();
    //
    // The second approach is recommended in cases where the bitmap can be
    // arbitrarily large, as the GPU may not support the particular image size.
    DX::ThrowIfFailed(
        d2dContext->CreateEffect(CLSID_D2D1BitmapSource, &m_originalBitmapSourceEffect)
        );

    DX::ThrowIfFailed(
        m_originalBitmapSourceEffect->SetValue(D2D1_BITMAPSOURCE_PROP_WIC_BITMAP_SOURCE, originalWicBitmapSource)
        );

    // When DPI correction is enabled, the BitmapSource effect will convert
    // from the IWICBitmapSource's DPI to the D2D context's DPI. By doing so,
    // we keep a constant size when displaying vs. printing.
    DX::ThrowIfFailed(
        m_originalBitmapSourceEffect->SetValue(D2D1_BITMAPSOURCE_PROP_ENABLE_DPI_CORRECTION, TRUE)
        );

    // Cache the effect. This ensures that in the display case we aren't repeatedly
    // uploading the bitmap to the GPU.
    DX::ThrowIfFailed(
        m_originalBitmapSourceEffect->SetValue(D2D1_PROPERTY_CACHED, TRUE)
        );

    // Create a Gaussian Blur effect.
    // The output of this effect will be used for both display and printing.
    DX::ThrowIfFailed(
        d2dContext->CreateEffect(CLSID_D2D1GaussianBlur, &m_blurEffect)
        );

    DX::ThrowIfFailed(
        m_blurEffect->SetValue(D2D1_GAUSSIANBLUR_PROP_STANDARD_DEVIATION, sc_blur_deviation)
        );

    m_blurEffect->SetInputEffect(0, m_originalBitmapSourceEffect.Get());

    // When drawing to screen (during Rendering or Preview), we explicitly
    // apply a color management effect, then pass the resulting image with
    // converted pixels to the screen for fast rendering.
    //
    DX::ThrowIfFailed(
        d2dContext->CreateEffect(CLSID_D2D1BitmapSource, &m_bitmapSourceEffectForColorManagement)
        );

    // When DPI correction is enabled, the BitmapSource effect will convert
    // from the IWICBitmapSource's DPI to the D2D context's DPI. By doing so,
    // we keep a constant size when displaying vs. printing.
    DX::ThrowIfFailed(
        m_bitmapSourceEffectForColorManagement->SetValue(D2D1_BITMAPSOURCE_PROP_ENABLE_DPI_CORRECTION, TRUE)
        );

    if (DrawTypes::Printing == m_type)
    {
        DX::ThrowIfFailed(
            m_bitmapSourceEffectForColorManagement->SetValue(
                D2D1_BITMAPSOURCE_PROP_WIC_BITMAP_SOURCE,
                wicBitmapSourceWithColorContext
                )
            );
    }
    else
    {
        DX::ThrowIfFailed(
            m_bitmapSourceEffectForColorManagement->SetValue(
                D2D1_BITMAPSOURCE_PROP_WIC_BITMAP_SOURCE,
                originalWicBitmapSource
                )
            );

        DX::ThrowIfFailed(
            m_bitmapSourceEffectForColorManagement->SetValue(D2D1_PROPERTY_CACHED, TRUE)
            );

        // This ColorManagement effect converts the source image pixels from
        // the input color space to the sRGB space.
        DX::ThrowIfFailed(
            d2dContext->CreateEffect(CLSID_D2D1ColorManagement, &m_colorManagementEffect)
            );

        m_colorManagementEffect->SetInputEffect(0, m_bitmapSourceEffectForColorManagement.Get());

        ComPtr<ID2D1ColorContext> d2dSourceColorContext;
        DX::ThrowIfFailed(
            d2dContext->CreateColorContextFromWicColorContext(wicColorContext, &d2dSourceColorContext)
            );

        DX::ThrowIfFailed(
            m_colorManagementEffect->SetValue(
                D2D1_COLORMANAGEMENT_PROP_SOURCE_COLOR_CONTEXT,
                d2dSourceColorContext.Get()
                )
            );

        // Use the default sRGB space as the target color space.
        ComPtr<ID2D1ColorContext> d2dDestinationColorContext;
        DX::ThrowIfFailed(
            d2dContext->CreateColorContext(D2D1_COLOR_SPACE_SRGB, nullptr, 0, &d2dDestinationColorContext)
            );

        DX::ThrowIfFailed(
            m_colorManagementEffect->SetValue(
                D2D1_COLORMANAGEMENT_PROP_DESTINATION_COLOR_CONTEXT,
                d2dDestinationColorContext.Get()
                )
            );
    }
}

// Set title box and image box based on the section box.
void PageRendererContext::SetTitleAndImageBoxes(
    _In_ D2D1_RECT_F sectionBox,
    _Out_ D2D1_RECT_F* titleBox,
    _Out_ D2D1_RECT_F* imageBox
    )
{
    FLOAT sectionHeight = sectionBox.bottom - sectionBox.top;

    *titleBox = sectionBox;
    titleBox->bottom = sectionBox.top + sectionHeight * 0.25f;

    *imageBox = sectionBox;
    imageBox->top = titleBox->bottom;
}

// Draw images and titles for all sections.
void PageRendererContext::DrawImages()
{
    if (!m_sectionsValid)
    {
        // Draw a warning message if there is insufficient area on the screen.
        DrawMessage("Insufficient area to draw all images.");
        return;
    }

    // Draw the original image, for both display and printing.
    m_d2dContext->DrawImage(
        m_originalBitmapSourceEffect.Get(),
        D2D1::Point2F(m_sectionOriginal.imageBox.left, m_sectionOriginal.imageBox.top)
        );
    m_d2dContext->DrawText(
        m_sectionOriginal.titleString->Data(),
        m_sectionOriginal.titleString->Length(),
        m_textFormat.Get(),
        m_sectionOriginal.titleBox,
        m_blackBrush.Get()
        );

    // Draw the blurred image, for both display and printing.
    m_d2dContext->DrawImage(
        m_blurEffect.Get(),
        D2D1::Point2F(m_sectionBlur.imageBox.left, m_sectionBlur.imageBox.top)
        );
    m_d2dContext->DrawText(
        m_sectionBlur.titleString->Data(),
        m_sectionBlur.titleString->Length(),
        m_textFormat.Get(),
        m_sectionBlur.titleBox,
        m_blackBrush.Get()
        );

    // Draw the color managed image.
    if (m_type == DrawTypes::Rendering || m_type == DrawTypes::Preview)
    {
        // Draw the output of the color management effect.
        m_d2dContext->DrawImage(
            m_colorManagementEffect.Get(),
            D2D1::Point2F(m_sectionColorManaged.imageBox.left, m_sectionColorManaged.imageBox.top)
            );
    }
    else
    {
        // Draw the output of the BitmapSource effect from a profiled bitmap.
        m_d2dContext->DrawImage(
            m_bitmapSourceEffectForColorManagement.Get(),
            D2D1::Point2F(m_sectionColorManaged.imageBox.left, m_sectionColorManaged.imageBox.top)
            );
    }
    m_d2dContext->DrawText(
        m_sectionColorManaged.titleString->Data(),
        m_sectionColorManaged.titleString->Length(),
        m_textFormat.Get(),
        m_sectionColorManaged.titleBox,
        m_blackBrush.Get()
        );
}
