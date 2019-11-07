//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "DocSource.h"
#include "PageRenderer3D.h"

ref class DirectXApp : public Windows::ApplicationModel::Core::IFrameworkView
{
internal:
    DirectXApp();

public:
    // IFrameworkView Methods
    virtual void Initialize(_In_ Windows::ApplicationModel::Core::CoreApplicationView^ applicationView);
    virtual void SetWindow(_In_ Windows::UI::Core::CoreWindow^ window);
    virtual void Load(_In_ Platform::String^ entryPoint);
    virtual void Run();
    virtual void Uninitialize();

private:
    // Event Handlers
    void OnWindowSizeChanged(
        _In_ Windows::UI::Core::CoreWindow^ sender,
        _In_ Windows::UI::Core::WindowSizeChangedEventArgs^ args
        );

    void OnSuspending(
        _In_ Platform::Object^ sender,
        _In_ Windows::ApplicationModel::SuspendingEventArgs^ args
        );

    void OnVisibilityChanged(
        _In_ Windows::UI::Core::CoreWindow^ sender,
        _In_ Windows::UI::Core::VisibilityChangedEventArgs^ args
        );

    void OnDpiChanged(
        _In_ Windows::Graphics::Display::DisplayInformation^ sender, 
        _In_ Platform::Object^ args
        );

    void OnActivated(
        _In_ Windows::ApplicationModel::Core::CoreApplicationView^ applicationView,
        _In_ Windows::ApplicationModel::Activation::IActivatedEventArgs^ args
        );

    void OnWindowClosed(
        _In_ Windows::UI::Core::CoreWindow^ sender,
        _In_ Windows::UI::Core::CoreWindowEventArgs^ args
        );

    // To support printing with Ctrl+P
    void OnKeyDown(
        _In_ Windows::UI::Core::CoreWindow^ sender,
        _In_ Windows::UI::Core::KeyEventArgs^ args
        );

    void OnKeyUp(
        _In_ Windows::UI::Core::CoreWindow^ sender,
        _In_ Windows::UI::Core::KeyEventArgs^ args
        );

    // Print support
    void InitializePrint();

    void SetPrintTask(
        _In_ Windows::Graphics::Printing::PrintManager^ sender,
        _In_ Windows::Graphics::Printing::PrintTaskRequestedEventArgs^ args
        );

    void PrintDetailedOptions_OptionChanged(
        _In_ Windows::Graphics::Printing::OptionDetails::PrintTaskOptionDetails^ sender, 
        _In_ Windows::Graphics::Printing::OptionDetails::PrintTaskOptionChangedEventArgs^ args
        );

    PageRenderer3D^                                     m_renderer;
    Windows::Graphics::Printing::PrintManager^          m_printManager;
    bool                                                m_windowClosed;
    bool                                                m_windowVisible;
    bool                                                m_ctrlKeyDown;
};

ref class DirectXAppSource : Windows::ApplicationModel::Core::IFrameworkViewSource
{
public:
    virtual Windows::ApplicationModel::Core::IFrameworkView^ CreateView();
};
