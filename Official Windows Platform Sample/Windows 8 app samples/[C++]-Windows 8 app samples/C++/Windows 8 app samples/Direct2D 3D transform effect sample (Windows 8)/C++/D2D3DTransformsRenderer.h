//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "DirectXBase.h"

// Because commands to change effect properties travel from MainPage.cpp to App.cpp to
// D2D3DTransformsRenderer.cpp, we use these enums to specify which properties we are changing.
enum class TransformEffect
{
    D2D13DTransform,
    D2D13DPerspectiveTransform
};

enum class TransformProperty
{
    ScaleX,
    ScaleY,
    LocalOffsetX,
    LocalOffsetY,
    LocalOffsetZ,
    RotationX,
    RotationY,
    RotationZ,
    GlobalOffsetX,
    GlobalOffsetY,
    GlobalOffsetZ,
    Perspective
};

ref class D2D3DTransformsRenderer : public DirectXBase
{
internal:
    D2D3DTransformsRenderer();

    virtual void CreateDeviceIndependentResources() override;
    virtual void CreateDeviceResources() override;
    virtual void CreateWindowSizeDependentResources() override;
    virtual void Render() override;

    void SetTransformEffect(TransformEffect transformEffect);
    void SetTransformProperty(TransformProperty transformProperty, float value);

    void Render3DTransformEffect();
    void Render3DPerspectiveTransformEffect();

    void UpdateForViewStateChanged(Windows::UI::ViewManagement::ApplicationViewState state);

private:
    // Helper methods to draw current transform values / properties to the screen.
    void Draw3DPerspectiveProperty(
        Platform::String^ caption,
        Platform::String^ value,
        float horizontalOffset
        );

    void DrawMatrix(
        Platform::String^ caption,
        D2D1_MATRIX_4X4_F matrix,
        float horizontalOffset
        );

    void DrawSymbol(
        Platform::String^ symbol,
        float horizontalOffset
        );

    Windows::UI::ViewManagement::ApplicationViewState       m_windowState;

    Microsoft::WRL::ComPtr<IWICFormatConverter>             m_wicFormatConverter;
    Microsoft::WRL::ComPtr<ID2D1Effect>                     m_bitmapSourceEffect;
    Microsoft::WRL::ComPtr<ID2D1Effect>                     m_3DTransformEffect;
    Microsoft::WRL::ComPtr<ID2D1Effect>                     m_3DPerspectiveTransformEffect;

    TransformEffect                                         m_currentEffect;
    D2D1_SIZE_U                                             m_imageSize;

    // Variables used to define matrices / set properties for the
    // 3DTransform and 3DPerspectiveTransform effects, respectively.
    float                                                   m_scaleX;
    float                                                   m_scaleY;
    float                                                   m_localOffsetX;
    float                                                   m_localOffsetY;
    float                                                   m_localOffsetZ;
    float                                                   m_rotationX;
    float                                                   m_rotationY;
    float                                                   m_rotationZ;
    float                                                   m_globalOffsetX;
    float                                                   m_globalOffsetY;
    float                                                   m_globalOffsetZ;
    float                                                   m_perspective;

    // Variables used to render current matrices / properties of effects
    // to the screen.
    Microsoft::WRL::ComPtr<IDWriteTextFormat>               m_captionFormat;
    Microsoft::WRL::ComPtr<IDWriteTextFormat>               m_matrixValueFormat;
    Microsoft::WRL::ComPtr<IDWriteTextFormat>               m_propertyValueFormat;
    Microsoft::WRL::ComPtr<IDWriteTextFormat>               m_symbolFormat;
    Microsoft::WRL::ComPtr<IDWriteTextFormat>               m_snappedViewFormat;
    Microsoft::WRL::ComPtr<ID2D1SolidColorBrush>            m_whiteBrush;
    float                                                   m_captionTop;
    float                                                   m_captionHeight;
    float                                                   m_matrixTop;
    float                                                   m_matrixHeight;
    float                                                   m_matrixWidth;
    float                                                   m_propertyTop;
    float                                                   m_propertyHeight;
    float                                                   m_propertyWidth;
};
