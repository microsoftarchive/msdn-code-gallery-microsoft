//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"

using namespace Microsoft::WRL;
using namespace Platform;
using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::Core;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::Foundation;
using namespace Windows::Graphics::Display;
using namespace Windows::UI::ApplicationSettings;
using namespace Windows::UI::Core;
using namespace Windows::UI::ViewManagement;

// Constants that control UI layout.
static const float PropertyOffsetLeft = -555.0f;
static const float PropertyOffsetTop = -150.0f;
static const float PropertyWidth = 160.0f;
static const float PropertyHeight = 100.0f;
static const float PropertyMargin = 10.0f;
static const float CaptionOffsetTop = -175.0f;
static const float MatrixOffsetLeft = -550.0f;
static const float MatrixOffsetTop = -150.0f;
static const float MatrixMargin = 25.0f;
static const float MatrixWidth = 150.0f;
static const float MatrixHeight = 100.0f;

D2D3DTransformsRenderer::D2D3DTransformsRenderer()
{
}

void D2D3DTransformsRenderer::CreateDeviceIndependentResources()
{
    DirectXBase::CreateDeviceIndependentResources();

    // Text formats used for displaying the effect properties are created in
    // this method, since they are independent of the Direct2D device itself.
    // In the event the device needs to be reinstantiated (due to a resolution
    // change for example), these font resources will not need to be recreated.

    String^ fontName = "Segoe UI";
    String^ locale = "en-US";

    DX::ThrowIfFailed(
        m_dwriteFactory->CreateTextFormat(
            fontName->Data(),
            nullptr,
            DWRITE_FONT_WEIGHT_NORMAL,
            DWRITE_FONT_STYLE_NORMAL,
            DWRITE_FONT_STRETCH_NORMAL,
            11,
            locale->Data(),
            &m_captionFormat
            )
        );

    // Center the text horizontally.
    DX::ThrowIfFailed(
        m_captionFormat->SetTextAlignment(DWRITE_TEXT_ALIGNMENT_CENTER)
        );

    // Center the text vertically.
    DX::ThrowIfFailed(
        m_captionFormat->SetParagraphAlignment(DWRITE_PARAGRAPH_ALIGNMENT_CENTER)
        );

    DX::ThrowIfFailed(
        m_dwriteFactory->CreateTextFormat(
            fontName->Data(),
            nullptr,
            DWRITE_FONT_WEIGHT_LIGHT,
            DWRITE_FONT_STYLE_NORMAL,
            DWRITE_FONT_STRETCH_NORMAL,
            12,
            locale->Data(),
            &m_matrixValueFormat
            )
        );

    // Center the text horizontally.
    DX::ThrowIfFailed(
        m_matrixValueFormat->SetTextAlignment(DWRITE_TEXT_ALIGNMENT_CENTER)
        );

    // Center the text vertically.
    DX::ThrowIfFailed(
        m_matrixValueFormat->SetParagraphAlignment(DWRITE_PARAGRAPH_ALIGNMENT_CENTER)
        );

    DX::ThrowIfFailed(
        m_dwriteFactory->CreateTextFormat(
            fontName->Data(),
            nullptr,
            DWRITE_FONT_WEIGHT_NORMAL,
            DWRITE_FONT_STYLE_NORMAL,
            DWRITE_FONT_STRETCH_NORMAL,
            18,
            locale->Data(),
            &m_propertyValueFormat
            )
        );

    // Center the text horizontally.
    DX::ThrowIfFailed(
        m_propertyValueFormat->SetTextAlignment(DWRITE_TEXT_ALIGNMENT_CENTER)
        );

    DX::ThrowIfFailed(
        m_dwriteFactory->CreateTextFormat(
            fontName->Data(),
            nullptr,
            DWRITE_FONT_WEIGHT_LIGHT,
            DWRITE_FONT_STYLE_NORMAL,
            DWRITE_FONT_STRETCH_NORMAL,
            18,
            locale->Data(),
            &m_symbolFormat
            )
        );

    // Center the text horizontally.
    DX::ThrowIfFailed(
        m_symbolFormat->SetTextAlignment(DWRITE_TEXT_ALIGNMENT_CENTER)
        );

    // Center the text vertically.
    DX::ThrowIfFailed(
        m_symbolFormat->SetParagraphAlignment(DWRITE_PARAGRAPH_ALIGNMENT_CENTER)
        );

    DX::ThrowIfFailed(
        m_dwriteFactory->CreateTextFormat(
            fontName->Data(),
            nullptr,
            DWRITE_FONT_WEIGHT_LIGHT,
            DWRITE_FONT_STYLE_NORMAL,
            DWRITE_FONT_STRETCH_NORMAL,
            20,
            locale->Data(),
            &m_snappedViewFormat
            )
        );

    // Center the text horizontally.
    DX::ThrowIfFailed(
        m_snappedViewFormat->SetTextAlignment(DWRITE_TEXT_ALIGNMENT_CENTER)
        );

    // Center the text vertically.
    DX::ThrowIfFailed(
        m_snappedViewFormat->SetParagraphAlignment(DWRITE_PARAGRAPH_ALIGNMENT_CENTER)
        );

    // Load an image from a Windows Imaging Component
    // decoder, which will be passed to the BitmapSource effect
    // in CreateDeviceResources.
    ComPtr<IWICBitmapDecoder> decoder;
    DX::ThrowIfFailed(
        m_wicFactory->CreateDecoderFromFilename(
            L"mammoth_small.jpg",
            nullptr,
            GENERIC_READ,
            WICDecodeMetadataCacheOnDemand,
            &decoder
            )
        );

    ComPtr<IWICBitmapFrameDecode> frame;
    DX::ThrowIfFailed(
        decoder->GetFrame(0, &frame)
        );

    DX::ThrowIfFailed(
        m_wicFactory->CreateFormatConverter(&m_wicFormatConverter)
        );

    DX::ThrowIfFailed(
        m_wicFormatConverter->Initialize(
            frame.Get(),
            GUID_WICPixelFormat32bppPBGRA,
            WICBitmapDitherTypeNone,
            nullptr,
            0.0f,
            WICBitmapPaletteTypeCustom
            )
        );

    // Get the size of the image.
    unsigned int width, height;
    DX::ThrowIfFailed(m_wicFormatConverter->GetSize(&width, &height));
    m_imageSize = D2D1::SizeU(width, height);
}

void D2D3DTransformsRenderer::CreateDeviceResources()
{
    DirectXBase::CreateDeviceResources();

    DX::ThrowIfFailed(
        m_d2dContext->CreateSolidColorBrush(
            D2D1::ColorF(D2D1::ColorF::White),
            &m_whiteBrush
            )
        );

    // Create a bitmap source effect and bind the WIC format converter to it.
    m_d2dContext->CreateEffect(CLSID_D2D1BitmapSource, &m_bitmapSourceEffect);
    DX::ThrowIfFailed(
        m_bitmapSourceEffect->SetValue(
            D2D1_BITMAPSOURCE_PROP_WIC_BITMAP_SOURCE,
            m_wicFormatConverter.Get()
            )
        );

    // Because the image will not be changing, the BitmapSource effect should be cached for better performance.

    // The property system expects TRUE and FALSE rather than true or false to ensure that
    // the size of the value is consistent regardless of architecture.
    DX::ThrowIfFailed(
        m_bitmapSourceEffect->SetValue(
            D2D1_PROPERTY_CACHED,
            TRUE
            )
        );

    // Create the matrix-based transform effect.
    DX::ThrowIfFailed(
        m_d2dContext->CreateEffect(CLSID_D2D13DTransform, &m_3DTransformEffect)
        );

    m_3DTransformEffect->SetInputEffect(0, m_bitmapSourceEffect.Get());

    // Create the property-based transform effect.
    DX::ThrowIfFailed(
        m_d2dContext->CreateEffect(CLSID_D2D13DPerspectiveTransform, &m_3DPerspectiveTransformEffect)
        );

    m_3DPerspectiveTransformEffect->SetInputEffect(0, m_bitmapSourceEffect.Get());
}

void D2D3DTransformsRenderer::CreateWindowSizeDependentResources()
{
    DirectXBase::CreateWindowSizeDependentResources();

    D2D1_SIZE_F size = m_d2dContext->GetSize();

    // Render effect properties centered at the bottom of the screen.
    m_captionTop = size.height + CaptionOffsetTop;
    m_matrixTop = size.height + MatrixOffsetTop;
    m_propertyTop = size.height + PropertyOffsetTop;

    // Scale image based on DPI.
    D2D1_POINT_2F scale = D2D1::Point2F(m_dpi / 96.0f, m_dpi / 96.0f);
    m_bitmapSourceEffect->SetValue(D2D1_BITMAPSOURCE_PROP_SCALE, scale);

    Render();
}

void D2D3DTransformsRenderer::Render()
{
    m_d2dContext->BeginDraw();

    m_d2dContext->Clear(D2D1::ColorF(D2D1::ColorF::CornflowerBlue));

    if (m_windowState != ApplicationViewState::Snapped)
    {
        // Only render the effect that is currently being manipuated by the XAML code.
        if (m_currentEffect == TransformEffect::D2D13DTransform)
        {
            Render3DTransformEffect();
        }
        else if (m_currentEffect == TransformEffect::D2D13DPerspectiveTransform)
        {
            Render3DPerspectiveTransformEffect();
        }
        else
        {
            throw ref new Platform::FailureException();
        }
    }
    else
    {
        // Display message that app is not designed to be run in snapped mode.

        Platform::String^ snapMessage = "This sample does not support snapped view.";

        D2D1_SIZE_F size = m_d2dContext->GetSize();

        m_d2dContext->DrawText(
            snapMessage->Data(),
            snapMessage->Length(),
            m_snappedViewFormat.Get(),
            D2D1::RectF(0.0f, 0.0f, size.width, size.height),
            m_whiteBrush.Get()
            );
    }

    // We ignore D2DERR_RECREATE_TARGET here. This error indicates that the device
    // is lost. It will be handled during the next call to Present.
    HRESULT hr = m_d2dContext->EndDraw();
    if (hr != D2DERR_RECREATE_TARGET)
    {
        DX::ThrowIfFailed(hr);
    }

    Present();
}

void D2D3DTransformsRenderer::SetTransformEffect(TransformEffect transformEffect)
{
    m_currentEffect = transformEffect;

    if (m_swapChainPanel != nullptr)
    {
        Render();
    }
}

void D2D3DTransformsRenderer::SetTransformProperty(TransformProperty transformProperty, float value)
{
    switch (transformProperty)
    {
    case TransformProperty::ScaleX:
        m_scaleX = value;
        break;
    case TransformProperty::ScaleY:
        m_scaleY = value;
        break;
    case TransformProperty::LocalOffsetX:
        m_localOffsetX = value;
        break;
    case TransformProperty::LocalOffsetY:
        m_localOffsetY = value;
        break;
    case TransformProperty::LocalOffsetZ:
        m_localOffsetZ = value;
        break;
    case TransformProperty::RotationX:
        m_rotationX = value;
        break;
    case TransformProperty::RotationY:
        m_rotationY = value;
        break;
    case TransformProperty::RotationZ:
        m_rotationZ = value;
        break;
    case TransformProperty::GlobalOffsetX:
        m_globalOffsetX = value;
        break;
    case TransformProperty::GlobalOffsetY:
        m_globalOffsetY = value;
        break;
    case TransformProperty::GlobalOffsetZ:
        m_globalOffsetZ = value;
        break;
    case TransformProperty::Perspective:
        m_perspective = value;
        break;
    default:
        throw ref new Platform::FailureException();
        break;
    }

    if (m_swapChainPanel != nullptr)
    {
        Render();
    }
}

void D2D3DTransformsRenderer::Render3DTransformEffect()
{
    // Generate Transform matrix for 3DTransform effect.
    D2D1::Matrix4x4F transformMatrix =                   // Parentheses below denote equivalent property in 3DPerspectiveTransform.
        D2D1::Matrix4x4F::Scale(m_scaleX, m_scaleY, 1) * // Scales the image. (No equivalent in 3DPerspectiveTransform)
        D2D1::Matrix4x4F::Translation(                   // Translate the image before rotation. (LOCAL_OFFSET property)
            m_localOffsetX,
            m_localOffsetY,
            m_localOffsetZ
            ) *
        D2D1::Matrix4x4F::RotationX(m_rotationX) *       // Rotate the image on the X, Y and Z axes. (ROTATION property)
        D2D1::Matrix4x4F::RotationY(m_rotationY) *
        D2D1::Matrix4x4F::RotationZ(m_rotationZ) *
        D2D1::Matrix4x4F::Translation(                   // Translate the image after rotation. (GLOBAL_OFFSET property)
            m_globalOffsetX,
            m_globalOffsetY,
            m_globalOffsetZ
            ) *
        D2D1::Matrix4x4F::PerspectiveProjection(         // Apply perspective to the image. (DEPTH property)
            m_perspective
            );

    DX::ThrowIfFailed(m_3DTransformEffect->SetValue(D2D1_3DTRANSFORM_PROP_TRANSFORM_MATRIX, transformMatrix));

    D2D1_SIZE_F size = m_d2dContext->GetSize();

    // Base matrix position off of the screen center.
    D2D1_POINT_2F screenCenter = D2D1::Point2F(
        size.width / 2.0f,
        size.height / 2.0f
        );

    // Draw Scale matrix.
    DrawMatrix(
        "Matrix4x4F::Scale(" + m_scaleX.ToString() + ", " + m_scaleY.ToString() + ", 1)",
        D2D1::Matrix4x4F::Scale(m_scaleX, m_scaleY, 1),
        screenCenter.x + MatrixOffsetLeft
        );

    // Draw 'x' between matrices.
    DrawSymbol("x", screenCenter.x + MatrixOffsetLeft + MatrixWidth);

    // Draw the Local Offset matrix.
    DrawMatrix(
        "Matrix4x4F::Translation(" +
            m_localOffsetX.ToString() + ", " +
            m_localOffsetY.ToString() + ", " +
            m_localOffsetZ.ToString() + ")",
        D2D1::Matrix4x4F::Translation(m_localOffsetX, m_localOffsetY, m_localOffsetZ),
        screenCenter.x + MatrixOffsetLeft + MatrixWidth + MatrixMargin
        );

    // Draw 'x' between matrices.
    DrawSymbol("x", screenCenter.x + MatrixOffsetLeft + MatrixWidth * 2 + MatrixMargin);

    // Draw Rotation matrix. Two separate helper matrices are multipled together to create one rotation matrix.
    DrawMatrix(
        "Matrix4x4F::RotationX(" +m_rotationX.ToString() +
            ") * Matrix4x4F::RotationY(" + m_rotationY.ToString() +
            ") * Matrix4x4F::RotationZ(" + m_rotationZ.ToString() + ")",
        D2D1::Matrix4x4F::RotationX(m_rotationX) *
        D2D1::Matrix4x4F::RotationY(m_rotationY) *
        D2D1::Matrix4x4F::RotationZ(m_rotationZ),
        screenCenter.x + MatrixOffsetLeft + MatrixWidth * 2 + MatrixMargin * 2
        );

    // Draw 'x' between matrices.
    DrawSymbol("x", screenCenter.x + MatrixOffsetLeft + MatrixWidth * 3 + MatrixMargin * 2);

    // Draw the Global Offset matrix.
    DrawMatrix(
        "Matrix4x4F::Translation(" + m_globalOffsetX.ToString() + ", " + m_globalOffsetY.ToString() + ", " + m_globalOffsetZ.ToString() + ")",
        D2D1::Matrix4x4F::Translation(m_globalOffsetX, m_globalOffsetY, m_globalOffsetZ),
        screenCenter.x + MatrixOffsetLeft + MatrixWidth * 3 + MatrixMargin * 3
        );

    // Draw 'x' between matrices.
    DrawSymbol("x", screenCenter.x + MatrixOffsetLeft + MatrixWidth * 4 + MatrixMargin * 3);

    // Draw the Perspective matrix.
    DrawMatrix(
        "Matrix4x4F::PerspectiveProjection(\n" + m_perspective.ToString() + ")",
        D2D1::Matrix4x4F::PerspectiveProjection(m_perspective),
        screenCenter.x + MatrixOffsetLeft + MatrixWidth * 4 + MatrixMargin * 4
        );

    // Draw '=' between matrices.
    DrawSymbol("=", screenCenter.x + MatrixOffsetLeft + MatrixWidth * 5 + MatrixMargin * 4);

    // Result resultant transform matrix that is applied to m_3DTransformEffect.
    DrawMatrix(
        "",
        transformMatrix,
        screenCenter.x + MatrixOffsetLeft + MatrixWidth * 5 + MatrixMargin * 5
        );

    // Render 3DTransform effect.
    m_d2dContext->DrawImage(
        m_3DTransformEffect.Get(),
        D2D1::Point2F(
            screenCenter.x - (m_imageSize.width / 2.0f),
            screenCenter.y - (m_imageSize.height / 2.0f)
            )
        );
}

void D2D3DTransformsRenderer::Render3DPerspectiveTransformEffect()
{
    D2D1_SIZE_F size = m_d2dContext->GetSize();

    // Base property position off of the screen center.
    D2D1_POINT_2F screenCenter = D2D1::Point2F(
        size.width / 2.0f,
        size.height / 2.0f
        );

    // Assign properties to 3DPerspectiveProperties effect.
    D2D1_VECTOR_3F localOffset = D2D1::Vector3F(m_localOffsetX, m_localOffsetY, m_localOffsetZ);

    DX::ThrowIfFailed(
        m_3DPerspectiveTransformEffect->SetValue(
            D2D1_3DPERSPECTIVETRANSFORM_PROP_LOCAL_OFFSET,
            localOffset
            )
        );

    D2D1_VECTOR_3F rotation = D2D1::Vector3F(m_rotationX, m_rotationY, m_rotationZ);

    DX::ThrowIfFailed(
        m_3DPerspectiveTransformEffect->SetValue(
            D2D1_3DPERSPECTIVETRANSFORM_PROP_ROTATION,
            rotation
            )
        );

    DX::ThrowIfFailed(
        m_3DPerspectiveTransformEffect->SetValue(
            D2D1_3DPERSPECTIVETRANSFORM_PROP_DEPTH,
            m_perspective
            )
        );

    D2D1_VECTOR_3F globalOffset = D2D1::Vector3F(m_globalOffsetX, m_globalOffsetY, m_globalOffsetZ);

    DX::ThrowIfFailed(
        m_3DPerspectiveTransformEffect->SetValue(
            D2D1_3DPERSPECTIVETRANSFORM_PROP_GLOBAL_OFFSET,
            globalOffset
            )
        );

    Draw3DPerspectiveProperty("Scale Property", "Not available in 3DPerspective Transform", screenCenter.x + PropertyOffsetLeft);

    String^ localOffsetValue = "(" + m_localOffsetX.ToString() + ", " + m_localOffsetY.ToString() + ", " + m_localOffsetZ.ToString() + ")";
    Draw3DPerspectiveProperty("LocalOffset Property", localOffsetValue, screenCenter.x + PropertyOffsetLeft + (PropertyWidth + PropertyMargin));

    String^ rotationValue = "(" + m_rotationX.ToString() + ", " + m_rotationY.ToString() + ", " + m_rotationZ.ToString() + ")";
    Draw3DPerspectiveProperty("Rotation Property", rotationValue, screenCenter.x + PropertyOffsetLeft + (PropertyWidth + PropertyMargin) * 2.0f);

    String^ globalOffsetValue = "(" + m_globalOffsetX.ToString() + ", " + m_globalOffsetY.ToString() + ", " + m_globalOffsetZ.ToString() + ")";
    Draw3DPerspectiveProperty("GlobalOffset Property", globalOffsetValue, screenCenter.x + PropertyOffsetLeft + (PropertyWidth + PropertyMargin) * 3.0f);

    String^ depthValue = m_perspective.ToString();
    Draw3DPerspectiveProperty("Depth Property", depthValue, screenCenter.x + PropertyOffsetLeft + (PropertyWidth + PropertyMargin) * 4.0f);

    // Render 3DPerspectiveTransform effect.
    m_d2dContext->DrawImage(
        m_3DPerspectiveTransformEffect.Get(),
        D2D1::Point2F(
            screenCenter.x - (m_imageSize.width / 2.0f),
            screenCenter.y - (m_imageSize.height / 2.0f)
            )
        );
}

void D2D3DTransformsRenderer::Draw3DPerspectiveProperty(String^ caption, String^ value, float horizontalOffset)
{
    // Helper method that draws the names and values of 3DPerspectiveTransform properties.
    m_d2dContext->DrawText(
        caption->Data(),
        caption->Length(),
        m_captionFormat.Get(),
        D2D1::RectF(
            horizontalOffset,
            m_captionTop,
            horizontalOffset + PropertyWidth,
            m_captionTop + m_captionHeight
            ),
        m_whiteBrush.Get()
        );

    m_d2dContext->DrawText(
        value->Data(),
        value->Length(),
        m_propertyValueFormat.Get(),
        D2D1::RectF(
            horizontalOffset,
            m_propertyTop,
            horizontalOffset + PropertyWidth,
            m_propertyTop + PropertyHeight
            ),
        m_whiteBrush.Get()
        );
}

void D2D3DTransformsRenderer::DrawMatrix(String^ caption, D2D1_MATRIX_4X4_F matrix, float horizontalOffset)
{
    // Render Matrix Caption.
    m_d2dContext->DrawText(
        caption->Data(),
        caption->Length(),
        m_captionFormat.Get(),
        D2D1::RectF(
            horizontalOffset - MatrixMargin,
            m_captionTop,
            horizontalOffset + MatrixWidth + MatrixMargin,
            m_captionTop + m_captionHeight
            ),
        m_whiteBrush.Get()
        );

    // Draw left bracket.
    m_d2dContext->DrawLine(
        D2D1::Point2F(horizontalOffset, m_matrixTop),
        D2D1::Point2F(horizontalOffset, m_matrixTop + MatrixHeight),
        m_whiteBrush.Get()
        );

    m_d2dContext->DrawLine(
        D2D1::Point2F(horizontalOffset, m_matrixTop),
        D2D1::Point2F(horizontalOffset + MatrixWidth * 0.1f, m_matrixTop),
        m_whiteBrush.Get()
        );

    m_d2dContext->DrawLine(
        D2D1::Point2F(horizontalOffset, m_matrixTop + MatrixHeight),
        D2D1::Point2F(horizontalOffset + MatrixWidth * 0.1f, m_matrixTop + MatrixHeight),
        m_whiteBrush.Get()
        );

    // Draw right bracket.
    m_d2dContext->DrawLine(
        D2D1::Point2F(horizontalOffset + MatrixWidth, m_matrixTop),
        D2D1::Point2F(horizontalOffset + MatrixWidth, m_matrixTop + MatrixHeight),
        m_whiteBrush.Get()
        );

    m_d2dContext->DrawLine(
        D2D1::Point2F(horizontalOffset + MatrixWidth * 0.9f, m_matrixTop),
        D2D1::Point2F(horizontalOffset + MatrixWidth, m_matrixTop),
        m_whiteBrush.Get()
        );

    m_d2dContext->DrawLine(
        D2D1::Point2F(horizontalOffset + MatrixWidth * 0.9f, m_matrixTop + MatrixHeight),
        D2D1::Point2F(horizontalOffset + MatrixWidth, m_matrixTop + MatrixHeight),
        m_whiteBrush.Get()
        );

    // Render contents of the passed-in matrix.
    for (int x = 0; x < 4; x++)
    {
        for (int y = 0; y < 4; y++)
        {
            // Use the floorf function to truncate the number to two decimal places.
            String^ matrixValue = (floorf(matrix.m[x][y] * 100.0f) / 100.0f).ToString();

            m_d2dContext->DrawText(
                matrixValue->Data(),
                matrixValue->Length(),
                m_matrixValueFormat.Get(),
                D2D1::RectF(
                    horizontalOffset + MatrixWidth * (0.25f * x),
                    m_matrixTop + MatrixHeight * (0.25f * y),
                    horizontalOffset + MatrixWidth * (0.25f * (x + 1)),
                    m_matrixTop + MatrixHeight * (0.25f * (y + 1))
                    ),
                m_whiteBrush.Get()
                );
        }
    }
}

void D2D3DTransformsRenderer::DrawSymbol(Platform::String^ symbol, float horizontalOffset)
{
    m_d2dContext->DrawText(
        symbol->Data(),
        symbol->Length(),
        m_symbolFormat.Get(),
        D2D1::RectF(
            horizontalOffset,
            m_matrixTop,
            horizontalOffset + MatrixMargin,
            m_matrixTop + MatrixHeight
            ),
        m_whiteBrush.Get()
        );
}

void D2D3DTransformsRenderer::UpdateForViewStateChanged(ApplicationViewState state)
{
    m_windowState = state;
}