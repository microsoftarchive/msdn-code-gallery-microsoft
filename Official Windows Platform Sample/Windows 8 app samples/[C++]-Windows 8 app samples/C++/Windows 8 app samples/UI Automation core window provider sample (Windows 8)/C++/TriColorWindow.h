//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved.

// Main window for the TriColor sample provider
#pragma once

#include "DirectXBase.h"

#using "UiaCoreWindowProvider.winmd"

// The main window for the target provider
// Implemented on top of ICoreWindow interface

// The C4691 warning can ignored given that the UiaCoreWindowProvider::Framework::Rect and Point data types referenced below 
// are contained in the InternalInterfaces_w.winmd file being regenerated during the build from the InternalInterfaces.idl,
// (which has the Rect and Point data types in the UiaCoreWindowProvider.Framework namespace.)
#pragma warning(push)
#pragma warning(disable: 4691)

ref class CTriColorWindow sealed:
        public DirectXBase, 
        public Windows::ApplicationModel::Core::IFrameworkView,
        public UiaCoreWindowProvider::ITriColorControlInternal
{
public:
    CTriColorWindow();

    // DirectXBase Methods
    virtual void CreateDeviceIndependentResources() override;
    virtual void CreateDeviceResources() override;
    virtual void CreateWindowSizeDependentResources() override;
    virtual void Render() override;

    // IFrameworkView Methods
    virtual void Initialize(_In_ Windows::ApplicationModel::Core::CoreApplicationView^ applicationView);
    virtual void SetWindow(_In_ Windows::UI::Core::CoreWindow^ window);
    virtual void Load(_In_ Platform::String^ entryPoint);
    virtual void Run();
    virtual void Uninitialize();

    // ITriColorControlInternal
    virtual property UiaCoreWindowProvider::TriColorValue ControlValue
    {
        UiaCoreWindowProvider::TriColorValue get();
        void set(_In_ UiaCoreWindowProvider::TriColorValue value);
    }
    virtual property short HasFocus
    {
        short get();
    }
    virtual property Windows::Foundation::Rect ControlRect
    {
        Windows::Foundation::Rect get();
    }
    virtual Windows::Foundation::Rect RectFromValue(_In_ UiaCoreWindowProvider::TriColorValue value);
    virtual UiaCoreWindowProvider::TriColorValue ValueFromPoint(_In_ Windows::Foundation::Point pt);
    virtual Platform::Object ^GetAppWindowProvider();
    virtual Platform::Object ^GetTriColorControlProvider();
    virtual Platform::Object ^GetTriColorFragmentProvider(_In_ UiaCoreWindowProvider::TriColorValue value);

private:

    ~CTriColorWindow();

    void OnWindowSizeChanged(
        _In_ Windows::UI::Core::CoreWindow^ sender,
        _In_ Windows::UI::Core::WindowSizeChangedEventArgs^ args
        );
        
    void OnLogicalDpiChanged(
        _In_ Platform::Object^ sender
        );

    void OnDisplayContentsInvalidated(
        _In_ Platform::Object^ sender
        );

    void OnActivated(
        _In_ Windows::ApplicationModel::Core::CoreApplicationView^ applicationView,
        _In_ Windows::ApplicationModel::Activation::IActivatedEventArgs^ args
        );

    void OnSuspending(
        _In_ Platform::Object^ sender,
        _In_ Windows::ApplicationModel::SuspendingEventArgs^ args
        );

    void OnResuming(
        _In_ Platform::Object^ sender,
        _In_ Platform::Object^ args
        );

    void OnKeyDown(
        _In_ Windows::UI::Core::CoreWindow^ sender, 
        _In_ Windows::UI::Core::KeyEventArgs^ e
        );

    void OnPointerReleased(
        _In_ Windows::UI::Core::CoreWindow^ sender, 
        _In_ Windows::UI::Core::PointerEventArgs^ e
        );

    void OnWindowActivated(
        _In_ Windows::UI::Core::CoreWindow^ sender, 
        _In_ Windows::UI::Core::WindowActivatedEventArgs^ e
        );

    void OnWindowClosed(
        _In_ Windows::UI::Core::CoreWindow^ sender, 
        _In_ Windows::UI::Core::CoreWindowEventArgs^ e
        );

    void OnAutomationProviderRequested(
        _In_ Windows::UI::Core::CoreWindow^ sender, 
        _In_ Windows::UI::Core::AutomationProviderRequestedEventArgs ^e
        );

    void OnHighContrastChanged(
        _In_ Windows::UI::ViewManagement::AccessibilitySettings^ sender, 
        _In_ Platform::Object^ e
        );

    void CreateThemeSpecificResources();
    void DiscardThemeSpecificResources();

    void EnsureAppWindowProviderCreated();
    void EnsureControlProviderCreated();
    void EnsureFragmentProviderCreated(_In_ UiaCoreWindowProvider::TriColorValue value);

    Windows::Foundation::Rect GetRedRect();
    Windows::Foundation::Rect GetYellowRect();
    Windows::Foundation::Rect GetGreenRect();
    HRESULT ValueFromPoint(_In_ Windows::Foundation::Point pt, _Out_ UiaCoreWindowProvider::TriColorValue * value);
    void SetValue(_In_ UiaCoreWindowProvider::TriColorValue newValue);
    static bool PointInRect(_In_ Windows::Foundation::Rect rect, _In_ Windows::Foundation::Point pt);

private:

    Windows::UI::ViewManagement::AccessibilitySettings^ m_accSettings;
    Windows::UI::ViewManagement::UISettings^ m_uiSettings;

    Platform::Object^ m_appWindowProvider;
    Platform::Object^ m_controlProvider;
    Platform::Object^ m_redProvider;
    Platform::Object^ m_yellowProvider;
    Platform::Object^ m_greenProvider;

    Microsoft::WRL::ComPtr<ID2D1SolidColorBrush> m_windowTextBrush;
    Microsoft::WRL::ComPtr<ID2D1SolidColorBrush> m_blackBrush;
    Microsoft::WRL::ComPtr<ID2D1SolidColorBrush> m_whiteBrush;
    Microsoft::WRL::ComPtr<ID2D1SolidColorBrush> m_redBrush;
    Microsoft::WRL::ComPtr<ID2D1SolidColorBrush> m_yellowBrush;
    Microsoft::WRL::ComPtr<ID2D1SolidColorBrush> m_greenBrush;
    Microsoft::WRL::ComPtr<IDWriteTextFormat> m_titleFormat;
    Microsoft::WRL::ComPtr<IDWriteTextFormat> m_labelFormat;

    Windows::Foundation::Rect m_controlRect;
    UiaCoreWindowProvider::TriColorValue m_value;
    bool m_activated;
};

#pragma warning(pop)

// A factory to create TriColorWindow objects.
ref class CTriColorWindowFactory : public Windows::ApplicationModel::Core::IFrameworkViewSource
{
public:
    virtual Windows::ApplicationModel::Core::IFrameworkView^ CreateView();
};
