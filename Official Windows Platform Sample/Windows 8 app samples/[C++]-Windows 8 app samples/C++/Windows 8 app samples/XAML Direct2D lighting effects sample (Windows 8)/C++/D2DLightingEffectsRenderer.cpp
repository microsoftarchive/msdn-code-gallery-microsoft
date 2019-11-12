//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "D2DLightingEffectsRenderer.h"
#include "DirectXSample.h"

using namespace Microsoft::WRL;
using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::Core;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::Foundation;
using namespace Windows::Graphics::Display;
using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml::Controls;

D2DLightingEffectsRenderer::D2DLightingEffectsRenderer()
{
}

void D2DLightingEffectsRenderer::HandleDeviceLost()
{
    m_needsResourceUpdate = true;

    DirectXBase::HandleDeviceLost();
}

void D2DLightingEffectsRenderer::CreateDeviceIndependentResources()
{
    DirectXBase::CreateDeviceIndependentResources();

    // Load an image from a Windows Imaging Component decoder.
    ComPtr<IWICBitmapDecoder> decoder;
    DX::ThrowIfFailed(
        m_wicFactory->CreateDecoderFromFilename(
            L"heightmap.png",
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
        m_wicFactory->CreateFormatConverter(&m_wicConverter)
        );

    DX::ThrowIfFailed(
        m_wicConverter->Initialize(
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
    DX::ThrowIfFailed(m_wicConverter->GetSize(&width, &height));
    m_imageSize = D2D1::SizeU(width, height);
}

void D2DLightingEffectsRenderer::CreateDeviceResources()
{
    DirectXBase::CreateDeviceResources();

    // Create a bitmap source effect and bind the WIC format converter to it.
    m_d2dContext->CreateEffect(CLSID_D2D1BitmapSource, &m_bitmapSourceEffect);
    DX::ThrowIfFailed(m_bitmapSourceEffect->SetValue(D2D1_BITMAPSOURCE_PROP_WIC_BITMAP_SOURCE, m_wicConverter.Get()));

    // Because the image will not be changing, we should cache the effect for performance reasons.
    DX::ThrowIfFailed(m_bitmapSourceEffect->SetValue(D2D1_PROPERTY_CACHED, TRUE));

    // Create effects and set their input to the BitmapSource effect. These lighting effects use the alpha channel
    // of the inputed image as a heightmap - higher values represent higher elevations off the surface of the image.
    DX::ThrowIfFailed(m_d2dContext->CreateEffect(CLSID_D2D1PointSpecular, &m_pointSpecularEffect));
    m_pointSpecularEffect->SetInputEffect(0, m_bitmapSourceEffect.Get());

    DX::ThrowIfFailed(m_d2dContext->CreateEffect(CLSID_D2D1SpotSpecular, &m_spotSpecularEffect));
    m_spotSpecularEffect->SetInputEffect(0, m_bitmapSourceEffect.Get());

    DX::ThrowIfFailed(m_d2dContext->CreateEffect(CLSID_D2D1DistantSpecular, &m_distantSpecularEffect));
    m_distantSpecularEffect->SetInputEffect(0, m_bitmapSourceEffect.Get());

    DX::ThrowIfFailed(m_d2dContext->CreateEffect(CLSID_D2D1PointDiffuse, &m_pointDiffuseEffect));
    m_pointDiffuseEffect->SetInputEffect(0, m_bitmapSourceEffect.Get());

    DX::ThrowIfFailed(m_d2dContext->CreateEffect(CLSID_D2D1SpotDiffuse, &m_spotDiffuseEffect));
    m_spotDiffuseEffect->SetInputEffect(0, m_bitmapSourceEffect.Get());

    DX::ThrowIfFailed(m_d2dContext->CreateEffect(CLSID_D2D1DistantDiffuse, &m_distantDiffuseEffect));
    m_distantDiffuseEffect->SetInputEffect(0, m_bitmapSourceEffect.Get());

    // m_currentEffect represents the current effect being rendered in the Render() method. At startup,
    // we default to m_pointSpecularEffect.
    m_currentEffect = m_pointSpecularEffect;
}

void D2DLightingEffectsRenderer::CreateWindowSizeDependentResources()
{
    DirectXBase::CreateWindowSizeDependentResources();

    // Resize the image depending on the view state.
    if (m_windowState == Windows::UI::ViewManagement::ApplicationViewState::Snapped)
    {
        // Scale the image to fit vertically.
        D2D1_VECTOR_2F scale = D2D1::Vector2F(
            1.0f,
            m_renderTargetSize.Height / m_imageSize.height
            );

        DX::ThrowIfFailed(
            m_bitmapSourceEffect->SetValue(D2D1_BITMAPSOURCE_PROP_SCALE, scale)
            );
    }
    else
    {
        // Scale the image to fit horizontally and vertically.
        D2D1_VECTOR_2F scale = D2D1::Vector2F(
            m_renderTargetSize.Width / m_imageSize.width,
            m_renderTargetSize.Height / m_imageSize.height
            );

        DX::ThrowIfFailed(
            m_bitmapSourceEffect->SetValue(D2D1_BITMAPSOURCE_PROP_SCALE, scale)
            );
    }

    D2D1_SIZE_F size = m_d2dContext->GetSize();

    // Set the pointsAt property for the Spot effects to the center of the screen.
    D2D1_VECTOR_3F pointsAt = D2D1::Vector3F(
        size.width / 2.0F,
        size.height / 2.0F,
        0.0f
        );

    DX::ThrowIfFailed(m_spotSpecularEffect->SetValue(D2D1_SPOTSPECULAR_PROP_POINTS_AT, pointsAt));
    DX::ThrowIfFailed(m_spotDiffuseEffect->SetValue(D2D1_SPOTDIFFUSE_PROP_POINTS_AT, pointsAt));
}

void D2DLightingEffectsRenderer::Render()
{
    m_needsResourceUpdate = false;
    m_d2dContext->BeginDraw();

    // Draw the currently selected effect. By using SOURCE_COPY, the screen does
    // not need to be cleared prior to each render since the pixels are completely
    // overwritten, not blended as with SOURCE_OVER.
    m_d2dContext->DrawImage(
        m_currentEffect.Get(),
        D2D1_INTERPOLATION_MODE_LINEAR,
        D2D1_COMPOSITE_MODE_SOURCE_COPY
        );

    // We ignore D2DERR_RECREATE_TARGET here. This error indicates that the device
    // is lost. It will be handled during the next call to Present.
    HRESULT hr = m_d2dContext->EndDraw();
    if (hr != D2DERR_RECREATE_TARGET)
    {
        DX::ThrowIfFailed(hr);
    }

    Present();
}

// Set the X and Y coordinates of the LightPosition property to the current pointer position.
void D2DLightingEffectsRenderer::OnPointerMoved(float x, float y)
{
    // The Z position is manually set by the user.
    D2D1_VECTOR_3F lightPosition = D2D1::Vector3F(x, y, m_lightPositionZ);

    // The DistantSpecular and DistantDiffuse effects do not have a LightPosition property.
    DX::ThrowIfFailed(m_pointSpecularEffect->SetValue(D2D1_POINTSPECULAR_PROP_LIGHT_POSITION, lightPosition));
    DX::ThrowIfFailed(m_spotSpecularEffect->SetValue(D2D1_SPOTSPECULAR_PROP_LIGHT_POSITION, lightPosition));
    DX::ThrowIfFailed(m_pointDiffuseEffect->SetValue(D2D1_POINTDIFFUSE_PROP_LIGHT_POSITION, lightPosition));
    DX::ThrowIfFailed(m_spotDiffuseEffect->SetValue(D2D1_SPOTDIFFUSE_PROP_LIGHT_POSITION, lightPosition));
}

void D2DLightingEffectsRenderer::SetLightingEffect(LightingEffect lightingEffect)
{
    if (m_swapChainPanel == nullptr)
    {
        return; // Wait until renderer / swap chain have been initialized.
    }

    switch (lightingEffect)
    {
        case LightingEffect::PointSpecular:
            m_currentEffect = m_pointSpecularEffect;
            break;
        case LightingEffect::SpotSpecular:
            m_currentEffect = m_spotSpecularEffect;
            break;
        case LightingEffect::DistantSpecular:
            m_currentEffect = m_distantSpecularEffect;
            break;
        case LightingEffect::PointDiffuse:
            m_currentEffect = m_pointDiffuseEffect;
            break;
        case LightingEffect::SpotDiffuse:
            m_currentEffect = m_spotDiffuseEffect;
            break;
        case LightingEffect::DistantDiffuse:
            m_currentEffect = m_distantDiffuseEffect;
            break;
        default:
            throw ref new Platform::FailureException();
            break;
    }
}

void D2DLightingEffectsRenderer::SetLightingProperty(LightingProperty lightingProperty, float value)
{
    if (m_swapChainPanel == nullptr)
    {
        return; // Wait until renderer / swap chain have been initialized.
    }

    // Not all effects have all properties. For example, the Specular effects do not have a DiffuseConstant property.
    switch (lightingProperty)
    {
        case LightingProperty::LightPositionZ:
            m_lightPositionZ = value;
            break;
        case LightingProperty::SpecularConstant:
            DX::ThrowIfFailed(m_pointSpecularEffect->SetValue(D2D1_POINTSPECULAR_PROP_SPECULAR_CONSTANT, value));
            DX::ThrowIfFailed(m_spotSpecularEffect->SetValue(D2D1_SPOTSPECULAR_PROP_SPECULAR_CONSTANT, value));
            DX::ThrowIfFailed(m_distantSpecularEffect->SetValue(D2D1_DISTANTSPECULAR_PROP_SPECULAR_CONSTANT, value));
            break;
        case LightingProperty::SpecularExponent:
            DX::ThrowIfFailed(m_pointSpecularEffect->SetValue(D2D1_POINTSPECULAR_PROP_SPECULAR_EXPONENT, value));
            DX::ThrowIfFailed(m_spotSpecularEffect->SetValue(D2D1_SPOTSPECULAR_PROP_SPECULAR_EXPONENT, value));
            DX::ThrowIfFailed(m_distantSpecularEffect->SetValue(D2D1_DISTANTSPECULAR_PROP_SPECULAR_EXPONENT, value));
            break;
        case LightingProperty::DiffuseConstant:
            DX::ThrowIfFailed(m_pointDiffuseEffect->SetValue(D2D1_POINTDIFFUSE_PROP_DIFFUSE_CONSTANT, value));
            DX::ThrowIfFailed(m_spotDiffuseEffect->SetValue(D2D1_SPOTDIFFUSE_PROP_DIFFUSE_CONSTANT, value));
            DX::ThrowIfFailed(m_distantDiffuseEffect->SetValue(D2D1_DISTANTDIFFUSE_PROP_DIFFUSE_CONSTANT, value));
            break;
        case LightingProperty::Focus:
            DX::ThrowIfFailed(m_spotSpecularEffect->SetValue(D2D1_SPOTSPECULAR_PROP_FOCUS, value));
            DX::ThrowIfFailed(m_spotDiffuseEffect->SetValue(D2D1_SPOTDIFFUSE_PROP_FOCUS, value));
            break;
        case LightingProperty::LimitingConeAngle:
            DX::ThrowIfFailed(m_spotSpecularEffect->SetValue(D2D1_SPOTSPECULAR_PROP_LIMITING_CONE_ANGLE, value));
            DX::ThrowIfFailed(m_spotDiffuseEffect->SetValue(D2D1_SPOTDIFFUSE_PROP_LIMITING_CONE_ANGLE, value));
            break;
        case LightingProperty::Azimuth:
            DX::ThrowIfFailed(m_distantSpecularEffect->SetValue(D2D1_DISTANTSPECULAR_PROP_AZIMUTH, value));
            DX::ThrowIfFailed(m_distantDiffuseEffect->SetValue(D2D1_DISTANTDIFFUSE_PROP_AZIMUTH, value));
            break;
        case LightingProperty::Elevation:
            DX::ThrowIfFailed(m_distantSpecularEffect->SetValue(D2D1_DISTANTSPECULAR_PROP_ELEVATION, value));
            DX::ThrowIfFailed(m_distantDiffuseEffect->SetValue(D2D1_DISTANTDIFFUSE_PROP_ELEVATION, value));
            break;
        case LightingProperty::SurfaceScale:
            DX::ThrowIfFailed(m_pointSpecularEffect->SetValue(D2D1_POINTSPECULAR_PROP_SURFACE_SCALE, value));
            DX::ThrowIfFailed(m_spotSpecularEffect->SetValue(D2D1_SPOTSPECULAR_PROP_SURFACE_SCALE, value));
            DX::ThrowIfFailed(m_distantSpecularEffect->SetValue(D2D1_DISTANTSPECULAR_PROP_SURFACE_SCALE, value));
            DX::ThrowIfFailed(m_pointDiffuseEffect->SetValue(D2D1_POINTDIFFUSE_PROP_SURFACE_SCALE, value));
            DX::ThrowIfFailed(m_spotDiffuseEffect->SetValue(D2D1_SPOTDIFFUSE_PROP_SURFACE_SCALE, value));
            DX::ThrowIfFailed(m_distantDiffuseEffect->SetValue(D2D1_DISTANTDIFFUSE_PROP_SURFACE_SCALE, value));
            break;
        default:
            throw ref new Platform::FailureException();
            break;
    }
}

void D2DLightingEffectsRenderer::UpdateForViewStateChanged(Windows::UI::ViewManagement::ApplicationViewState state)
{
    m_windowState = state;
}