//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "DirectXApp.h"

using namespace Windows::UI::Core;
using namespace Windows::Foundation;
using namespace Windows::Graphics::Display;
using namespace Windows::Graphics::Printing;
using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::Core;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::UI::ViewManagement;

DirectXApp::DirectXApp() :
    m_windowClosed(false),
    m_windowVisible(true)
{
}

void DirectXApp::Initialize(
    _In_ CoreApplicationView^ applicationView
    )
{
    applicationView->Activated +=
        ref new TypedEventHandler<CoreApplicationView^, IActivatedEventArgs^>(this, &DirectXApp::OnActivated);

    CoreApplication::Suspending +=
        ref new EventHandler<SuspendingEventArgs^>(this, &DirectXApp::OnSuspending);

    CoreApplication::Resuming +=
        ref new EventHandler<Platform::Object^>(this, &DirectXApp::OnResuming);

    m_renderer = ref new PageRenderer();
}

void DirectXApp::SetWindow(
    _In_ CoreWindow^ window
    )
{
    window->PointerCursor = ref new CoreCursor(CoreCursorType::Arrow, 0);

    window->SizeChanged +=
        ref new TypedEventHandler<CoreWindow^, WindowSizeChangedEventArgs^>(this, &DirectXApp::OnWindowSizeChanged);

    window->VisibilityChanged +=
        ref new TypedEventHandler<CoreWindow^, VisibilityChangedEventArgs^>(this, &DirectXApp::OnVisibilityChanged);

    window->Closed +=
        ref new TypedEventHandler<CoreWindow^, CoreWindowEventArgs^>(this, &DirectXApp::OnWindowClosed);

    DisplayProperties::LogicalDpiChanged +=
        ref new DisplayPropertiesEventHandler(this, &DirectXApp::OnLogicalDpiChanged);

    m_renderer->Initialize(window, DisplayProperties::LogicalDpi);
    InitPrintManager();
}

void DirectXApp::Load(
    _In_ Platform::String^ entryPoint
    )
{
}

void DirectXApp::Run()
{
    while (!m_windowClosed)
    {
        if (m_windowVisible)
        {
            CoreWindow::GetForCurrentThread()->Dispatcher->ProcessEvents(Windows::UI::Core::CoreProcessEventsOption::ProcessAllIfPresent);
            m_renderer->Render();
        }
        else
        {
            CoreWindow::GetForCurrentThread()->Dispatcher->ProcessEvents(Windows::UI::Core::CoreProcessEventsOption::ProcessOneAndAllPending);
        }
    }
}

void DirectXApp::Uninitialize()
{
}

void DirectXApp::OnWindowSizeChanged(
    _In_ Windows::UI::Core::CoreWindow^ sender,
    _In_ Windows::UI::Core::WindowSizeChangedEventArgs^ args
    )
{
    m_renderer->SetSnappedMode(ApplicationView::Value == ApplicationViewState::Snapped);
    m_renderer->UpdateForWindowSizeChange();
}

void DirectXApp::OnVisibilityChanged(
    _In_ CoreWindow^ sender,
    _In_ VisibilityChangedEventArgs^ args
    )
{
    m_windowVisible = args->Visible;
}

void DirectXApp::OnWindowClosed(
    _In_ CoreWindow^ sender,
    _In_ CoreWindowEventArgs^ args
    )
{
    m_windowClosed = true;
}

void DirectXApp::OnLogicalDpiChanged(_In_ Platform::Object^ sender)
{
    m_renderer->SetDpi(DisplayProperties::LogicalDpi);
}

void DirectXApp::OnActivated(
    _In_ CoreApplicationView^ applicationView,
    _In_ IActivatedEventArgs^ args
    )
{
    CoreWindow::GetForCurrentThread()->Activate();
}

void DirectXApp::OnSuspending(
    _In_ Platform::Object^ sender,
    _In_ SuspendingEventArgs^ args
    )
{
}

void DirectXApp::OnResuming(
    _In_ Platform::Object^ sender,
    _In_ Platform::Object^ args
    )
{
}

void DirectXApp::InitPrintManager()
{
    // Call static method to get PrintManager object.
    m_printManager = Windows::Graphics::Printing::PrintManager::GetForCurrentView();

    // Start print task event.
    m_printManager->PrintTaskRequested +=
        ref new TypedEventHandler<PrintManager^, PrintTaskRequestedEventArgs^>(this, &DirectXApp::SetPrintTask);
}

void DirectXApp::SetPrintTask(
    _In_ PrintManager^ /*sender*/,
    _In_ PrintTaskRequestedEventArgs^ args
    )
{
    PrintTaskSourceRequestedHandler^ sourceHandler =
        ref new PrintTaskSourceRequestedHandler([this](PrintTaskSourceRequestedArgs^ args)-> void{
            Microsoft::WRL::ComPtr<CDocumentSource> documentSource;
            DX::ThrowIfFailed(
                Microsoft::WRL::MakeAndInitialize<CDocumentSource>(&documentSource, reinterpret_cast<IUnknown*>(m_renderer))
                );

            // Here CDocumentSource is inherited from IPrintDocumentSource, cast it back to an object.
            IPrintDocumentSource^ objSource(
                reinterpret_cast<IPrintDocumentSource^>(documentSource.Get())
                );

            args->SetSource(objSource);
        });

    // Request initializing print task.
    PrintTask^ printTask = args->Request->CreatePrintTask(
        L"Direct2D Windows Store app printing sample",
        sourceHandler
        );
}

IFrameworkView^ DirectXAppSource::CreateView()
{
    return ref new DirectXApp();
}

[Platform::MTAThread]
int main(Platform::Array<Platform::String^>^)
{
    auto directXAppSource = ref new DirectXAppSource();
    CoreApplication::Run(directXAppSource);
    return 0;
}
