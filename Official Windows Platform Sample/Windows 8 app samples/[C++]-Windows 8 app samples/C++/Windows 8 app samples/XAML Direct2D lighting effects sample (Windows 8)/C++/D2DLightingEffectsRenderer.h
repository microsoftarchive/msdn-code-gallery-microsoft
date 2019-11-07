//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "DirectXBase.h"

enum class LightingEffect
{
    PointSpecular,
    SpotSpecular,
    DistantSpecular,
    PointDiffuse,
    SpotDiffuse,
    DistantDiffuse
};

// Each lighting effect has its own property enumerations. This list standardizes
// the enumerations across all the different effects.
enum class LightingProperty
{
    LightPositionZ,
    SpecularExponent,
    SpecularConstant,
    DiffuseConstant,
    Focus,
    LimitingConeAngle,
    Elevation,
    Azimuth,
    SurfaceScale
};

ref class D2DLightingEffectsRenderer : public DirectXBase
{
internal:
    D2DLightingEffectsRenderer();

    virtual void HandleDeviceLost() override;
    virtual void CreateDeviceIndependentResources() override;
    virtual void CreateDeviceResources() override;
    virtual void CreateWindowSizeDependentResources() override;
    virtual void Render() override;

    void OnPointerMoved(float x, float y);

    void SetLightingEffect(LightingEffect lightingEffect);
    void SetLightingProperty(LightingProperty lightingProperty, float value);

    void UpdateForViewStateChanged(Windows::UI::ViewManagement::ApplicationViewState state);

    bool NeedsResourceUpdate()
    {
        return m_needsResourceUpdate;
    }

private:
    Windows::UI::ViewManagement::ApplicationViewState       m_windowState;
    bool                                                    m_needsResourceUpdate;

    Microsoft::WRL::ComPtr<IWICFormatConverter>             m_wicConverter;
    D2D1_SIZE_U                                             m_imageSize;

    Microsoft::WRL::ComPtr<ID2D1Effect>                     m_bitmapSourceEffect;

    Microsoft::WRL::ComPtr<ID2D1Effect>                     m_currentEffect;
    Microsoft::WRL::ComPtr<ID2D1Effect>                     m_pointSpecularEffect;
    Microsoft::WRL::ComPtr<ID2D1Effect>                     m_spotSpecularEffect;
    Microsoft::WRL::ComPtr<ID2D1Effect>                     m_distantSpecularEffect;
    Microsoft::WRL::ComPtr<ID2D1Effect>                     m_pointDiffuseEffect;
    Microsoft::WRL::ComPtr<ID2D1Effect>                     m_spotDiffuseEffect;
    Microsoft::WRL::ComPtr<ID2D1Effect>                     m_distantDiffuseEffect;

    float                                                   m_lightPositionZ;
};
