//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved
#pragma once

#include "pch.h"

namespace InkingPanelDX
{
    // Base class for a SwapChainPanel-based DirectX rendering surface to be used in XAML apps.
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class DirectXPanelBase : public Windows::UI::Xaml::Controls::SwapChainPanel
    {
    protected private:
        DirectXPanelBase();

        virtual void CreateDeviceIndependentResources();
        virtual void CreateDeviceResources();
        virtual void CreateSizeDependentResources();
        virtual void HandleDeviceLost();
        virtual void Present();
        virtual void ReleaseSizeDependentResources();
        virtual void Render() { };
        virtual void SetSwapChain();

        virtual void OnSizeChanged(
            Platform::Object^ sender, 
            Windows::UI::Xaml::SizeChangedEventArgs^ e);
        virtual void OnCompositionScaleChanged(
            Windows::UI::Xaml::Controls::SwapChainPanel ^sender, 
            Platform::Object ^args);
        virtual void OnSuspending(
            Platform::Object^ sender, 
            Windows::ApplicationModel::SuspendingEventArgs^ e);

    protected private:
        Microsoft::WRL::ComPtr<ID3D11Device1>           m_d3dDevice;
        Microsoft::WRL::ComPtr<ID3D11DeviceContext1>    m_d3dContext;
        Microsoft::WRL::ComPtr<IDXGISwapChain2>         m_swapChain;

        Microsoft::WRL::ComPtr<ID2D1Factory2>           m_d2dFactory;
        Microsoft::WRL::ComPtr<ID2D1Device>             m_d2dDevice;
        Microsoft::WRL::ComPtr<ID2D1DeviceContext>      m_d2dContext;
        Microsoft::WRL::ComPtr<ID2D1Bitmap1>            m_d2dTargetBitmap;

        D2D1_COLOR_F                                    m_backgroundColor;
        DXGI_ALPHA_MODE                                 m_alphaMode;

        // SwapChainPanel panel is typically used to handle input off the UI thread.
        // Since XAML properties can only be accessed from the UI thread cache here
        // those that are likely to be accessed from background thread.
        float                                           m_width;
        float                                           m_height;
        float                                           m_compositionScaleX;
        float                                           m_compositionScaleY;
    };
}