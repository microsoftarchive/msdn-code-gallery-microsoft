//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "DirectXApp.h"
#include "BasicTimer.h"

using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::Core;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::UI::Core;
using namespace Windows::System;
using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Graphics::Display;
using namespace Windows::Graphics::Printing;
using namespace Windows::Graphics::Printing::OptionDetails;
using namespace Microsoft::WRL;
using namespace Windows::Storage::Streams;

DirectXApp::DirectXApp() :
    m_windowClosed(false),
    m_windowVisible(true),
    m_ctrlKeyDown(false)
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

    m_renderer = ref new PageRenderer3D();
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

    DisplayInformation::GetForCurrentView()->DpiChanged +=
        ref new TypedEventHandler<DisplayInformation^, Platform::Object^>(this, &DirectXApp::OnDpiChanged);

    m_renderer->Initialize(window, DisplayInformation::GetForCurrentView()->LogicalDpi);

    // Initialize the print infrastructure
    InitializePrint();

    window->KeyDown +=
        ref new TypedEventHandler<CoreWindow^, KeyEventArgs^>(this, &DirectXApp::OnKeyDown);

    window->KeyUp +=
        ref new TypedEventHandler<CoreWindow^, KeyEventArgs^>(this, &DirectXApp::OnKeyUp);
}

void DirectXApp::Load(
    _In_ Platform::String^ entryPoint
    )
{
}

void DirectXApp::Run()
{
    BasicTimer^ timer = ref new BasicTimer();

    while (!m_windowClosed)
    {
        if (m_windowVisible)
        {
            timer->Update();
            CoreWindow::GetForCurrentThread()->Dispatcher->ProcessEvents(CoreProcessEventsOption::ProcessAllIfPresent);
            m_renderer->Update(timer->Total, timer->Delta);
            m_renderer->Render();
        }
        else
        {
            CoreWindow::GetForCurrentThread()->Dispatcher->ProcessEvents(CoreProcessEventsOption::ProcessOneAndAllPending);
        }
    }
}

void DirectXApp::Uninitialize()
{
}

void DirectXApp::OnWindowSizeChanged(
    _In_ CoreWindow^ sender,
    _In_ WindowSizeChangedEventArgs^ args
    )
{
    m_renderer->UpdateForWindowSizeChange();
}

void DirectXApp::OnSuspending(
    _In_ Platform::Object^ sender,
    _In_ SuspendingEventArgs^ args
    )
{
    // Hint to the driver that the app is entering an idle state and that its memory
    // can be temporarily used for other apps.
    m_renderer->Trim();
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

void DirectXApp::OnDpiChanged(
    _In_ DisplayInformation^ sender, 
    _In_ Platform::Object^ args
    )
{
    m_renderer->SetDpi(sender->LogicalDpi);
}

void DirectXApp::OnActivated(
    _In_ CoreApplicationView^ applicationView,
    _In_ IActivatedEventArgs^ args
    )
{
    CoreWindow::GetForCurrentThread()->Activate();
}

// Print-specific functions
void DirectXApp::InitializePrint()
{
    // Call static method to get PrintManager object.
    m_printManager = Windows::Graphics::Printing::PrintManager::GetForCurrentView();

    // Start print task event.
    m_printManager->PrintTaskRequested +=
        ref new TypedEventHandler<PrintManager^, PrintTaskRequestedEventArgs^>(this, &DirectXApp::SetPrintTask);
}


void DirectXApp::SetPrintTask(
    PrintManager^ /*sender*/,
    PrintTaskRequestedEventArgs^ args
    )
{
    auto printTaskRef = std::make_shared<PrintTask^>();

    PrintTaskSourceRequestedHandler^ sourceHandler =
        ref new PrintTaskSourceRequestedHandler([this,printTaskRef](PrintTaskSourceRequestedArgs^ args)-> void{
            Microsoft::WRL::ComPtr<CDocumentSource> documentSource;
            DX::ThrowIfFailed(
                Microsoft::WRL::MakeAndInitialize<CDocumentSource>(&documentSource, reinterpret_cast<IUnknown*>(m_renderer))
                );
            PrintTask^ printTask = *printTaskRef;

            //  Retrieve the advanced Print Task Options
            PrintTaskOptionDetails^ printDetailedOptions = PrintTaskOptionDetails::GetFromPrintTaskOptions(printTask->Options);

            // Create lists of options
            PrintCustomItemListOptionDetails^ listQuality = printDetailedOptions->CreateItemListOption(L"Job3DQuality", L"Quality");
            listQuality->AddItem(L"Job3DQualityHigh", L"High");
            listQuality->AddItem(L"Job3DQualityMedium", L"Medium");
            listQuality->AddItem(L"Job3DQualityDraft", L"Draft");

            PrintCustomItemListOptionDetails^ listDensity = printDetailedOptions->CreateItemListOption(L"Job3DDensity", L"Density");
            listDensity->AddItem(L"Job3DDensityHollow", L"Hollow");
            listDensity->AddItem(L"Job3DDensityLow", L"Low");
            listDensity->AddItem(L"Job3DDensityMedium", L"Medium");
            listDensity->AddItem(L"Job3DDensityHigh", L"High");
            listDensity->AddItem(L"Job3DDensitySolid", L"Solid");

            PrintCustomItemListOptionDetails^ listSupports = printDetailedOptions->CreateItemListOption(L"Job3DAddSupports", L"Add breakaway structures to support model overhangs");
            listSupports->AddItem(L"Job3DSupportsExcluded", L"No");
            listSupports->AddItem(L"Job3DSupportsIncluded", L"Yes");

            PrintCustomItemListOptionDetails^ listRaft = printDetailedOptions->CreateItemListOption(L"Job3DAddRaft", L"Add breakaway foundation for improved model adhesion");
            listRaft->AddItem(L"Job3DRaftExcluded", L"No");
            listRaft->AddItem(L"Job3DRaftIncluded", L"Yes");

            IVector<String^>^ displayedOptions = printTask->Options->DisplayedOptions;

            // Figure out if any of our desired settings are already present. If they are not, add the custom option 
            // to the option list. It is necessary to do this for forward compatibility; in the future these options
            // may be pre-populated by Windows if declared by the driver.
            bool qualityFound = false;
            bool densityFound = false;
            bool supportsFound = false;
            bool raftFound = false;
            for(unsigned int i = 0; i < displayedOptions->Size; i++)
            {
                String ^ option = displayedOptions->GetAt(i);
                if (option == "Job3DQuality")
                {
                    qualityFound = true;
                }
                else if (option == "Job3DDensity")
                {
                    densityFound = true;
                }
                else if (option == "Job3DAddSupports")
                {
                    supportsFound = true;
                }
                else if (option == "Job3DAddRaft")
                {
                    raftFound = true;
                }
            }

            // Add the custom option to the option list
            if (!qualityFound)
            {
                displayedOptions->Append(L"Job3DQuality");
            }
            if (!densityFound)
            {
                displayedOptions->Append(L"Job3DDensity");
            }
            if (!supportsFound)
            {
                displayedOptions->Append(L"Job3DAddSupports");
            }
            if (!raftFound)
            {
                displayedOptions->Append(L"Job3DAddRaft");
            }

            printDetailedOptions->OptionChanged += 
                ref new TypedEventHandler<PrintTaskOptionDetails^, PrintTaskOptionChangedEventArgs^>(
                    this, &DirectXApp::PrintDetailedOptions_OptionChanged);

            // Print Task event handler is invoked when the print job is completed.
            printTask->Completed += ref new TypedEventHandler<PrintTask^, PrintTaskCompletedEventArgs^>(
            [=](PrintTask^ sender, PrintTaskCompletedEventArgs^ e)
            {
                if (e->Completion == Windows::Graphics::Printing::PrintTaskCompletion::Failed)
                {
                    // Notify the user when the print operation fails.
                }
            });

            // Here DocSource is inherited from IPrintDocumentSource, cast it back to an object.
            IPrintDocumentSource^ objSource(
                reinterpret_cast<IPrintDocumentSource^>(documentSource.Get())
                );

            args->SetSource(objSource);
        });

    // Request initializing print task.
    *printTaskRef = args->Request->CreatePrintTask(
        L"3D print", //job name
        sourceHandler
        );

    // Flag printTask as 3D print job
    (*printTaskRef)->Is3DManufacturingTargetEnabled = true;
    (*printTaskRef)->IsPrinterTargetEnabled = false;
}

void DirectXApp::PrintDetailedOptions_OptionChanged(
    PrintTaskOptionDetails^ sender, 
    PrintTaskOptionChangedEventArgs^ args
    )
{
    // Listen for option changes
    String^ optionId = safe_cast<String^>(args->OptionId);

    // Example of how to test if a particular setting, e.g. Job3dQuality, has changed and how to respond to it
    if (optionId != nullptr && optionId == L"Job3DQuality")
    {
        // Respond to option change, then update and invalidate the preview if needed
        // m_documentSource->InvalidatePreview();
    }
}

void DirectXApp::OnKeyDown(
    _In_ Windows::UI::Core::CoreWindow^ sender,
    _In_ Windows::UI::Core::KeyEventArgs^ args
    )
{
    VirtualKey key = args->VirtualKey;

    if (VirtualKey::Control == key)
    {
        m_ctrlKeyDown = true;
    }
    else if (m_ctrlKeyDown)
    {
        if (VirtualKey::P == key)
        {
            Windows::Graphics::Printing::PrintManager::ShowPrintUIAsync();
            m_ctrlKeyDown = false;
        }
    }
}

void DirectXApp::OnKeyUp(
    _In_ Windows::UI::Core::CoreWindow^ sender,
    _In_ Windows::UI::Core::KeyEventArgs^ args
    )
{
    if (VirtualKey::Control == args->VirtualKey)
    {
        m_ctrlKeyDown = false;
    }
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
