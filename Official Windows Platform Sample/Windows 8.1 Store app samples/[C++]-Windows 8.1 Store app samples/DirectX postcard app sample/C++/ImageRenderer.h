//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once
ref class ImageRenderer
{
internal:
    void CreateDeviceIndependentResources(Microsoft::WRL::ComPtr<IWICImagingFactory2> wicFactory);
    void CreateDeviceResources(Microsoft::WRL::ComPtr<ID2D1DeviceContext> d2dContext);
    void CreateWindowSizeDependentResources(float dpi);

    void DrawImageAndEffects();

    void LoadImage(Windows::Storage::Streams::IRandomAccessStream^ randomAccessStream);
    void SetEffectIntensity(float value);
    void Reset();

private:
    void UpdateImageZoom();

    // Resources shared with PostcardRenderer.
    Microsoft::WRL::ComPtr<IWICImagingFactory2>             m_wicFactory;
    Microsoft::WRL::ComPtr<ID2D1DeviceContext>              m_d2dContext;
    float                                                   m_dpi;

    // Resources specific to this renderer.
    Microsoft::WRL::ComPtr<ID2D1Effect>                     m_bitmapSourceEffect;
    D2D1_SIZE_U                                             m_imagePixelSize;
    D2D1_SIZE_F                                             m_imageSizeDips;
    float                                                   m_imageZoomFactor;

    Microsoft::WRL::ComPtr<ID2D1Effect>                     m_saturationEffect;
};

