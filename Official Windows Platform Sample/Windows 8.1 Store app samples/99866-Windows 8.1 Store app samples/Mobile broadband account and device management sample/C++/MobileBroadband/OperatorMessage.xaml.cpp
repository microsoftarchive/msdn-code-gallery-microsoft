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
// OperatorMessage.xaml.cpp
// Implementation of the OperatorMessage class
//

#include "pch.h"
#include "OperatorMessage.xaml.h"

using namespace SDKSample::MobileBroadband;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::ApplicationModel::Background;
using namespace Windows::Networking::NetworkOperators;
using namespace Windows::UI::Core;
using namespace Windows::Storage;

OperatorMessage::OperatorMessage():
    operatorNotificationTaskEntryPoint("OperatorNotificationTask.OperatorNotification"),
    operatorNotificationTaskName("OperatorNotificationTask")
{
    InitializeComponent();

    sampleDispatcher = Window::Current->Dispatcher;
}

//
// Initialize variables and controls for the scenario
// This method is called just before the scenario page is displayed
//
void OperatorMessage::OnNavigatedTo(NavigationEventArgs^ e)
{
    //
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    //
    rootPage = MainPage::Current;

    try
    {
        RegisterOperatorNotificationTask();
    }
    catch (Platform::Exception^ ex)
    {
        rootPage->NotifyUser("Error: " + ex->ToString(), NotifyType::ErrorMessage);
    }
}

//
// Registers a background task completion event handler for the operator notification system event
// This event occurs when the application is updated
//
void OperatorMessage::RegisterOperatorNotificationTask()
{
    //
    // Register completion handler if the operator notification background task is registered
    //
    auto iter = BackgroundTaskRegistration::AllTasks->First();
    auto hasCur = iter->HasCurrent;
    while (hasCur)
    {
        auto cur = iter->Current;
        if (cur->Value->Name == "MobileOperatorNotificationHandler")
        {
            cur->Value->Completed += ref new BackgroundTaskCompletedEventHandler(this, &OperatorMessage::OnCompleted);
            OperatorNotificationStatus->Text = "Completion handler registered";
        }
        hasCur = iter->MoveNext();
    }

    //
    // Get all active Mobilebroadband accounts
    //
    auto allAccounts = MobileBroadbandAccount::AvailableNetworkAccountIds;

    if (allAccounts->Size > 0)
    {
        rootPage->NotifyUser("Mobile broadband account found", NotifyType::StatusMessage);
    }
    else
    {
        rootPage->NotifyUser("No Mobilebroadband accounts found", NotifyType::StatusMessage);
    }
}

//
// Handle background task completion event
//
void OperatorMessage::OnCompleted(BackgroundTaskRegistration^ sender, BackgroundTaskCompletedEventArgs^ e)
{
    //
    // Update the UI with progress reported by the background task
    //
    sampleDispatcher->RunAsync(CoreDispatcherPriority::Normal, ref new DispatchedHandler
        ([this, sender, e] () {
            try
            {
                if ((sender != nullptr) && (e != nullptr))
                {
                    //
                    // This method throws if the event is reporting an error
                    //
                    e->CheckResult();

                    //
                    // Update the UI with the completion status of the background task
                    // The Run method of the background task sets this status
                    //
                    auto key = sender->TaskId.ToString();
                    auto settings = ApplicationData::Current->LocalSettings;

                    if (sender->Name == operatorNotificationTaskName)
                    {
                        OperatorNotificationStatus->Text = settings->Values->Lookup(key)->ToString();
                        rootPage->NotifyUser("Operator Notification background task completed", NotifyType::StatusMessage);
                    }
                }
            }
            catch (Platform::Exception^ ex)
            {
                rootPage->NotifyUser("Error: " + ex->ToString(), NotifyType::ErrorMessage);
                OperatorNotificationStatus->Text = "Background task error";
            }
    }));
}
