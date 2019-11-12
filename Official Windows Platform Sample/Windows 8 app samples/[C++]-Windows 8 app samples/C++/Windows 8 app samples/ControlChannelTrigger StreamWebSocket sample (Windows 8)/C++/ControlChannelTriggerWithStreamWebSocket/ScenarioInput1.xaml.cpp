//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

//
// ScenarioInput1.xaml.cpp
// Implementation of the ScenarioInput1 class
//

#include "pch.h"
#include "ScenarioInput1.xaml.h"

using namespace concurrency;
using namespace ControlChannelTrigger;
using namespace StreamWebSocketTransportHelper;
using namespace StreamWebSocketTransportHelper::DiagnosticsHelper;
using namespace Windows::ApplicationModel::Background;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

ScenarioInput1::ScenarioInput1() : lockScreenAdded(false)
{
    InitializeComponent();
    Diag::SetCoreDispatcher(CoreWindow::GetForCurrentThread()->Dispatcher);
    ConnectButton->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
}

connectionStates ScenarioInput1::connectionState=connectionStates::notConnected;

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void ScenarioInput1::OnNavigatedTo(NavigationEventArgs^ e)
{
    // Get a pointer to our main page
    rootPage = dynamic_cast<MainPage^>(e->Parameter);

    // We want to be notified with the OutputFrame is loaded so we can get to the content.
    outputFrameERToken=rootPage->OutputFrameLoaded += ref new EventHandler<Platform::Object^>(this,&ScenarioInput1::rootPage_OutputFrameLoaded);
}

void ScenarioInput1::rootPage_OutputFrameLoaded(Object^ sender, Object^ e)
{
    // At this point, we know that the Output Frame has been loaded and we can go ahead
    // and reference elements in the page contained in the Output Frame.

    // Get a pointer to the content within the OutputFrame.
    Page^ outputFrame = dynamic_cast<Page^>(rootPage->OutputFrame->Content);

    // Go find the elements that we need for this scenario.
    TextBlock^ debugblock = dynamic_cast<TextBlock^>(outputFrame->FindName("DebugTextBlock"));
    Diag::SetDebugTextBlock(debugblock);
}

void ScenarioInput1::OnNavigatedFrom(NavigationEventArgs^ e)
{
    rootPage->OutputFrameLoaded -= outputFrameERToken;
}

void ScenarioInput1::ClientRole_Click(Object^ sender, RoutedEventArgs^ e)
{
    // In order to simplify the sample and focus on the core ControlChannelTrigger
    // related concepts, once a role is selected, the app has to be restarted to change the role.
    Diag::DebugPrint("Client role selected");
    ClientRoleButton->IsEnabled = false;
    ClientInit();
}

void ScenarioInput1::ClientInit()
{
    // In the client role, we require the application to be on lock screen.
    // Lock screen is required to let in-process RealTimeCommunication related
    // background code to execute.
    if (lockScreenAdded)
    {
        ClientSettings->Visibility=Windows::UI::Xaml::Visibility::Visible;
        ConnectButton->Visibility = Windows::UI::Xaml::Visibility::Visible;
        return;
    }
    else
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
                
                // App should switch to polling mode (example: poll for email based on time triggers)
                lockScreenAdded = false;
                Diag::DebugPrint("As Lockscreen status was Denied, App should switch to polling mode such as email based on time triggers.");
                break;
            }
            
            // Now, enable the client settings if the role hasn't changed.
            ClientSettings->Visibility=Windows::UI::Xaml::Visibility::Visible;
            ConnectButton->Visibility = Windows::UI::Xaml::Visibility::Visible;
            return;
        }).then([this](task<void> previousTask)
        {
            try 
            {
                previousTask.get();
                Diag::DebugPrint("Lockscreen add " + NotifyType::StatusMessage.ToString());
            } 
            catch (Exception^ e)
            {
                Diag::DebugPrint("Lockscreen add failed" + e->Message + NotifyType::ErrorMessage.ToString());
            }           
        });

    }
}

String^ ScenarioInput1::GetServerUri()
{
    return ServerUri->Text;
}

void ScenarioInput1::RegisterNetworkChangeTask()
{
    try
    {
        IIterator<IKeyValuePair<Guid,IBackgroundTaskRegistration^>^>^ iterator = BackgroundTaskRegistration::AllTasks->First();
        while(iterator->HasCurrent)
        {
            iterator->Current->Value->Unregister(true);
            iterator->MoveNext();
        }
        auto myTaskBuilder = ref new BackgroundTaskBuilder();
        auto myTrigger = ref new SystemTrigger(SystemTriggerType::NetworkStateChange, false);
        myTaskBuilder->SetTrigger(myTrigger);
        myTaskBuilder->TaskEntryPoint = "Background.NetworkChangeTask";
        myTaskBuilder->Name = "Network change task";
        BackgroundTaskRegistration^ myTask = myTaskBuilder->Register();
    }
    catch (Exception^ exp)
    {
        Diag::DebugPrint("Exception caught while setting up system event" + exp->ToString());
    }
}

void ScenarioInput1::ConnectButton_Click(Object^ sender, RoutedEventArgs^ e)
{
    if (connectionState == connectionStates::notConnected)
    {
        ConnectButton->Content = "Connecting...";
        connectionState = connectionStates::connecting;
        String^ serverUri = GetServerUri();
        
        // Init the commModule.
        if (commModule == nullptr) 
        {
            commModule = ref new CommModule(AppRole::ClientRole);
        }
        
        // Register for network state change notifications.
        RegisterNetworkChangeTask();
        
        // Finally, initiate the connection and set up transport
        // to be CCT capable. But do this heavy lifting outside of the UI thread.
        create_task([this, serverUri]() 
        {
            return commModule->SetupTransport(serverUri);
        }).then([this](bool result)
        {
            Diag::DebugPrint("CommModule setup result: " + result);
            if (result)
            {
                Dispatcher->RunAsync(CoreDispatcherPriority::Normal,ref new DispatchedHandler([this]()
                {
                    ConnectButton->Content = "Disconnect";
                }));
                connectionState = connectionStates::connected;
            }
            else
            {
                Dispatcher->RunAsync(CoreDispatcherPriority::Normal,ref new DispatchedHandler([this]()
                {
                    ConnectButton->Content = "failed to connect. click to retry";
                }));
                connectionState = connectionStates::notConnected;
            }
        });
    }
    else if (connectionState == connectionStates::connected)
    {
        create_task([this]() 
        {
            if (commModule) 
            {
                commModule->Reset();
            }            
        }).then([this]()
        {
            connectionState = connectionStates::notConnected;
            Dispatcher->RunAsync(CoreDispatcherPriority::Normal,ref new DispatchedHandler([this]()
            {
                ConnectButton->Content = "Connect";
            }));
        });
    }
}

void ScenarioInput1::SendButton_Click(Object^ sender, RoutedEventArgs^ e)
{
    String^ message = MessageBox->Text;
    create_task([this, message]() 
    {
        if (message!= "" && commModule != nullptr)
        {
            commModule->SendMessage(message);
        }
    });
}

