//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "DeviceResources.h"
#include "StepTimer.h"

namespace BlockCompressedImages
{
    // Information about the DDS image resource that is
    // generated using the BlockCompressedAssets C++ project.
    struct DdsImageResourceParameters
    {
        PWSTR filename;
        DXGI_FORMAT format;
        D2D1_ALPHA_MODE d2dAlpha;
        // The DDS format defines a slightly different set of alpha modes than Direct2D.
        WICDdsAlphaMode ddsAlpha;
    };

    // This sample renderer instantiates a basic rendering pipeline.
    class BlockCompressedImagesRenderer
    {
    public:
        BlockCompressedImagesRenderer(const std::shared_ptr<DX::DeviceResources>& deviceResources);
        void CreateDeviceIndependentResources();
        void CreateDeviceDependentResources();
        void CreateWindowSizeDependentResources();
        void ReleaseDeviceDependentResources();
        void Update(DX::StepTimer const& timer);
        void Render();

    private:
        _Success_(return) bool GetDdsBitmapSource(
            DdsImageResourceParameters parameters,
            _Outptr_result_nullonfailure_ IWICBitmapSource **ppSource
            );

        // Cached pointer to device resources.
        std::shared_ptr<DX::DeviceResources> m_deviceResources;
        DdsImageResourceParameters m_guitarImageInfo;
        DdsImageResourceParameters m_woodImageInfo;

        // If resource validation fails at any point, suppress further operations.
        bool m_hasValidResources;

        // Wood transform state
        float m_woodWidth;
        float m_woodHeight;
        float m_woodTranslateX;
        float m_woodTranslateY;

        // Guitar transform state
        float m_guitarRotation;
        float m_guitarTranslateX;
        float m_guitarTranslateY;
        float m_guitarWidth;
        float m_guitarHeight;

        Microsoft::WRL::ComPtr<IWICBitmapSource> m_wicSourceGuitar;
        Microsoft::WRL::ComPtr<IWICBitmapSource> m_wicSourceWood;
        Microsoft::WRL::ComPtr<ID2D1Bitmap1> m_d2dBitmapGuitar;
        Microsoft::WRL::ComPtr<ID2D1Bitmap1> m_d2dBitmapWood;

        // Defines the error message in case the DDS assets are invalid.
        Microsoft::WRL::ComPtr<IDWriteTextFormat> m_errorTextFormat;
        Platform::String^ m_errorText;
    };
}
