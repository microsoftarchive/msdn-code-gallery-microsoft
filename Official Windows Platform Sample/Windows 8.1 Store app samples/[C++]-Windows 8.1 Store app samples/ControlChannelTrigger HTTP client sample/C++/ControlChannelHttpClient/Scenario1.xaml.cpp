// Copyright (c) Microsoft Corporation. All rights reserved.

//
// Scenario1.xaml.cpp
// Implementation of the Scenario1 class
//

#include "pch.h"
#include "Scenario1.xaml.h"
#include "MainPage.xaml.h"

using namespace concurrency;
using namespace HttpClientTransportHelper;
using namespace HttpClientTransportHelper::DiagnosticsHelper;
using namespace Platform;
using namespace SDKSample;
using namespace SDKSample::ControlChannelHttpClient;
using namespace Windows::ApplicationModel::Background;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

ConnectionStates Scenario1::connectionState = ConnectionStates::NotConnected;

Scenario1::Scenario1():lockScreenAdded(false)
{
    InitializeComponent();
    Diag::SetCoreDispatcher(CoreWindow::GetForCurrentThread()->Dispatcher);
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
void Scenario1::OnNavigatedTo(_In_ NavigationEventArgs^ /*e*/)
{
    Diag::SetDebugTextBlock(DebugTextBlock);

    // Initialize the client.
    InitializeClient();

    // Register for network state change notifications.
    RegisterNetworkChangeTask();
}

void Scenario1::InitializeClient()
{
    communicationModule = ref new CommunicationModule();

    // In the client role, we require the application to be on lock screen.
    // Lock screen is required to allow in-process RealTimeCommunication related
    // background code to execute.
    if (lockScreenAdded == false)
    {
        try 
        {
            create_task(BackgroundExecutionManager::RequestAccessAsync()).then([this](BackgroundAccessStatus status)
            {
                switch (status)
                {
                case BackgroundAccessStatus::AllowedWithAlwaysOnRealTimeConnectivity:
                    // App is allowed to use ControlChannelTrigger
                    // functionality even in low power mode.
                    lockScreenAdded = true;
                    Diag::DebugPrint("Lock screen status: AllowedWithAlwaysOnRealTimeConnectivity");
                    break;
                case BackgroundAccessStatus::AllowedMayUseActiveRealTimeConnectivity:
                    // App is allowed to use ControlChannelTrigger
                    // functionality but not in low power mode.
                    lockScreenAdded = true;
                    Diag::DebugPrint("Lock screen status: AllowedMayUseActiveRealTimeConnectivity");
                    break;
                case BackgroundAccessStatus::Denied:
                    lockScreenAdded = false;
                    Diag::DebugPrint("Lock screen status was denied.");
                    break;
                }
            }).then([this](task<void> previousTask)
            {
                try 
                {
                    previousTask.get();
                    Diag::DebugPrint("Lock screen add " + NotifyType::StatusMessage.ToString());
                } 
                catch (Exception^ ex)
                {
                    Diag::DebugPrint("Lock screen add failed" + ex->Message + NotifyType::ErrorMessage.ToString());
                }           
            });

        } 
        catch (Exception^ ex)
        {
            // Need to catch any navigation exception when lock screen access page is shown.
            Diag::DebugPrint("Lock screen request failed: " + ex->Message);
        }
    }
}

void Scenario1::RegisterNetworkChangeTask()
{
    try
    {
        // Delete previously registered network status change tasks as
        // the background triggers are persistent by nature across process
        // lifetimes.
        IIterator<IKeyValuePair<Guid, IBackgroundTaskRegistration^>^>^ iterator = BackgroundTaskRegistration::AllTasks->First();
        while (iterator->HasCurrent)
        {
            Diag::DebugPrint("Deleting background task " + iterator->Current->Value->Name);
            iterator->Current->Value->Unregister(true);
            iterator->MoveNext();
        }

        auto myTaskBuilder = ref new BackgroundTaskBuilder();
        auto myTrigger = ref new SystemTrigger(SystemTriggerType::NetworkStateChange, false);
        myTaskBuilder->SetTrigger(myTrigger);
        myTaskBuilder->TaskEntryPoint = "BackgroundTaskHelper.NetworkChangeTask";
        myTaskBuilder->Name = "Network change task";
        BackgroundTaskRegistration^ myTask = myTaskBuilder->Register();
    }
    catch (Exception^ ex)
    {
        Diag::DebugPrint("Exception caught while setting up system event" + ex->ToString());
    }
}

void Scenario1::ConnectButton_Click(_In_ Object^ sender, _In_ RoutedEventArgs ^ /*e*/)
{
    if (connectionState == ConnectionStates::NotConnected)
    {
        Uri^ serverUri;
        try
        {
            if (ServerUri->Text->IsEmpty())
            {
                Diag::DebugPrint("URI must not be empty.");
                return;
            }
            serverUri = ref new Uri(ServerUri->Text);
        }
        catch (InvalidArgumentException^ ex)
        {
            Diag::DebugPrint("Please provide a valid URI input.");
            return;
        }

        ConnectButton->Content = "Connecting...";
        connectionState = ConnectionStates::Connecting;

        // Finally, initiate the connection and set up transport
        // to be CCT capable. But do this heavy lifting outside of the UI thread.
        create_task([this, serverUri]()
        {
            communicationModule->ServerUri = serverUri;
            return communicationModule->SetUpTransport("Scenario1");
        }).then([this](bool result)
        {
            Diag::DebugPrint("CommunicationModule setup result: " + result);
            if (result == true)
            {
                ConnectButton->Content = "Disconnect";
                connectionState = ConnectionStates::Connected;
            }
            else
            {
                ConnectButton->Content = "Failed to connect, click to retry.";
                connectionState = ConnectionStates::NotConnected;
            }
        }, task_continuation_context::use_current());
    }
    else if (connectionState == ConnectionStates::Connected)
    {
        create_task([this]()
        {
            communicationModule->Reset();
        }).then([this]()
        {        
            connectionState = ConnectionStates::NotConnected;
            ConnectButton->Content = "Connect";
        }, cancellation_token::none(), task_continuation_context::use_current());
    }
}

void SDKSample::ControlChannelHttpClient::Scenario1::ClearButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    DebugTextBlock->Text = "";
}
