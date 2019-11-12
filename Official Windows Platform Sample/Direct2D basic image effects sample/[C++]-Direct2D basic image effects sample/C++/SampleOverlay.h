//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "DirectXSample.h"

// This class is responsible for initializing and rendering the title overlay visible
// at the top of the DirectX SDK samples.
ref class SampleOverlay
{
internal:
    SampleOverlay();

    void Initialize(
        _In_ ID2D1Device*         d2dDevice,
        _In_ ID2D1DeviceContext*  d2dContext,
        _In_ IWICImagingFactory*  wicFactory,
        _In_ IDWriteFactory*      dwriteFactory,
        _In_ Platform::String^    caption
        );

    void ResetDirectXResources();

    void UpdateForWindowSizeChange();

    void Render();

    float GetTitleHeightInDips();

private:

    Microsoft::WRL::ComPtr<ID2D1Factory1>           m_d2dFactory;
    Microsoft::WRL::ComPtr<ID2D1Device>             m_d2dDevice;
    Microsoft::WRL::ComPtr<ID2D1DeviceContext>      m_d2dContext;
    Microsoft::WRL::ComPtr<IDWriteFactory>          m_dwriteFactory;
    Microsoft::WRL::ComPtr<ID2D1SolidColorBrush>    m_whiteBrush;
    Microsoft::WRL::ComPtr<ID2D1DrawingStateBlock>  m_stateBlock;

    Microsoft::WRL::ComPtr<IWICImagingFactory>      m_wicFactory;
    Microsoft::WRL::ComPtr<ID2D1Bitmap>             m_logoBitmap;
    Microsoft::WRL::ComPtr<IDWriteTextLayout>       m_textLayout;

    UINT                                            m_idIncrement;
    bool                                            m_drawOverlay;
    Platform::String^                               m_sampleName;
    float                                           m_padding;
    float                                           m_textVerticalOffset;
    D2D1_SIZE_F                                     m_logoSize;
    float                                           m_overlayWidth;
};
