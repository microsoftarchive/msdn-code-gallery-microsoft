//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "DirectXPage.g.h"

#include "DeviceResources.h"
#include "StepTimer.h"

namespace ShaderCompiler
{
    namespace SampleSettings
    {
        namespace Benchmark
        {
            static const int Iterations = 10;
        }
    }

    enum class CodeRegion
    {
        None,
        Header,
        Source,
        Hidden
    };

    class PageNotifier : public DX::IDeviceNotify
    {
    public:
        PageNotifier(DirectXPage^ page) : m_page(page) { }
        virtual void OnDeviceLost();
        virtual void OnDeviceRestored();

    private:
        DirectXPage^ m_page;
    };

    /// <summary>
    /// A page that hosts a DirectX SwapChainBackgroundPanel.
    /// This page must be the root of the Window content (it cannot be hosted on a Frame).
    /// </summary>
    public ref class DirectXPage sealed
    {
    public:
        DirectXPage();

        void SaveInternalState(Windows::Foundation::Collections::IPropertySet^ state);
        void LoadInternalState(Windows::Foundation::Collections::IPropertySet^ state);

        void CreateDeviceDependentResources();
        void CreatePanelSizeDependentResources();

    private:
        // XAML low-level rendering event handler.
        void OnRendering(Platform::Object^ sender, Platform::Object^ args);

        // Window event handlers.
        void OnVisibilityChanged(Windows::UI::Core::CoreWindow^ sender, Windows::UI::Core::VisibilityChangedEventArgs^ args);

        void OnCompositionScaleChanged(Windows::UI::Xaml::Controls::SwapChainPanel^ sender, Object^ args);

        // Resource used to keep track of the rendering event registration.
        Windows::Foundation::EventRegistrationToken m_eventToken;

        std::shared_ptr<DX::DeviceResources> m_deviceResources;

        // Timer class.
        DX::StepTimer m_timer;

        bool m_windowVisible;
        bool m_swapChainPanelDeviceResourcesInitialized;
        std::string m_shaderModelSuffix;
        bool m_compiledOnce;

        void LinkingThrowIfFailed(HRESULT hr, ID3D11FunctionLinkingGraph* graph);

        Platform::String^ m_headerCode;
        Platform::String^ m_hiddenCode;
        Platform::String^ m_lastGoodSourceCode;
        std::string ConvertString(Platform::String^ platformString);
        Platform::String^ ConvertString(const std::string& stdString);
        std::string CleanOutputMessage(const std::string& message);
        void Recompile();
        void PreviewImageSizeChanged(Platform::Object^ sender, Windows::UI::Xaml::SizeChangedEventArgs^ e);
        void SourceCodeTextChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::TextChangedEventArgs^ e);
        void LightingToggleButtonClick(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void FogToggleButtonClick(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void ToggleButtonClick(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void PreviewPanelSizeChanged(Platform::Object^ sender, Windows::UI::Xaml::SizeChangedEventArgs^ e);

        Microsoft::WRL::ComPtr<ID3D11Buffer> m_cameraConstantBuffer;
        Microsoft::WRL::ComPtr<ID3D11Buffer> m_signalConstantBuffer;
        Microsoft::WRL::ComPtr<ID3D11Buffer> m_linkingConstantBuffer;

        std::unique_ptr<PageNotifier> m_pageNotifier;
    };
}
