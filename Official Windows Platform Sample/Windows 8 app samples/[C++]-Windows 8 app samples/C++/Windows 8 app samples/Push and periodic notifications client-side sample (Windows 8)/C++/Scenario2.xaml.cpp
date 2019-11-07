//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

#include "pch.h"
#include "Scenario2.xaml.h"

using namespace SDKSample::SDKTemplate;

using namespace Concurrency;
using namespace Platform;
using namespace PushNotificationsHelper;
using namespace Windows::ApplicationModel::Background;
using namespace Windows::Networking::PushNotifications;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

const wchar_t* PUSH_NOTIFICATIONS_TASK_NAME = L"UpdateChannels";
const wchar_t* PUSH_NOTIFICATIONS_TASK_ENTRY_POINT = L"PushNotificationsHelper.MaintenanceTask";
const int MAINTENANCE_INTERVAL = 10 * 24 * 60; // Check for channels that need to be updated every 10 days

Scenario2::Scenario2()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Scenario2::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;

	// The Notifier object allows using the same code in the maintenance task and this foreground application
	if (rootPage->Notifier == nullptr)
	{
		rootPage->Notifier = ref new Notifier();
	}
}

void SDKSample::SDKTemplate::Scenario2::RenewChannelsButton_Click(Object^ sender, RoutedEventArgs^ e)
{
	create_task(rootPage->Notifier->RenewAllAsync(true)).then([this] (task<void> renewalTask)
	{
		try
		{
			// Check if we had any errors renewing channels
			renewalTask.get();
			rootPage->NotifyUser("Channels renewed successfully", NotifyType::StatusMessage);
		}
		catch (Exception^ ex)
        {
            rootPage->NotifyUser("Channels renewal failed: " + ex->Message, NotifyType::ErrorMessage);
        }   
	});
}

void SDKSample::SDKTemplate::Scenario2::RegisterTaskButton_Click(Object^ sender, RoutedEventArgs^ e)
{
	if (GetRegisteredTask() == nullptr)
    {
        BackgroundTaskBuilder^ taskBuilder = ref new BackgroundTaskBuilder();
        MaintenanceTrigger^ trigger = ref new MaintenanceTrigger(MAINTENANCE_INTERVAL, false);
        taskBuilder->SetTrigger(trigger);
        taskBuilder->TaskEntryPoint = ref new String(PUSH_NOTIFICATIONS_TASK_ENTRY_POINT);
        taskBuilder->Name = ref new String(PUSH_NOTIFICATIONS_TASK_NAME);

        SystemCondition^ internetCondition = ref new SystemCondition(SystemConditionType::InternetAvailable);
        taskBuilder->AddCondition(internetCondition);

        try
        {
            taskBuilder->Register();
            rootPage->NotifyUser("Task registered", NotifyType::StatusMessage);
        }
        catch (Exception^ ex)
        {
            rootPage->NotifyUser("Error registering task: " + ex->Message, NotifyType::ErrorMessage);
        }
    }
    else
    {
        rootPage->NotifyUser("Task already registered", NotifyType::ErrorMessage);
    }
}

void SDKSample::SDKTemplate::Scenario2::UnregisterTaskButton_Click(Object^ sender, RoutedEventArgs^ e)
{
	auto task = GetRegisteredTask();
    if (task != nullptr)
    {
        task->Unregister(true);
        rootPage->NotifyUser("Task unregistered", NotifyType::StatusMessage);
    }
    else
    {
        rootPage->NotifyUser("Task not registered", NotifyType::ErrorMessage);
    }
}

IBackgroundTaskRegistration^ SDKSample::SDKTemplate::Scenario2::GetRegisteredTask()
{
	auto iter = BackgroundTaskRegistration::AllTasks->First();
	while (iter->HasCurrent)
	{
		auto task = iter->Current->Value;
		if (task->Name == ref new String(PUSH_NOTIFICATIONS_TASK_NAME))
		{
			return task;
		}
		iter->MoveNext();
	}
	return nullptr;
}
