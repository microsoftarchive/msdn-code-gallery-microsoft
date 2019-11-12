// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// Scenario1Input.xaml.cpp
// Implementation of the Scenario1Input class
//

#include "pch.h"
#include "ScenarioInput1.xaml.h"

using namespace RawNotificationsSampleCPP;

using namespace concurrency;
using namespace Platform;
using namespace Windows::ApplicationModel::Background;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Graphics::Display;
using namespace Windows::Networking::PushNotifications;
using namespace Windows::Storage;
using namespace Windows::UI::Core;
using namespace Windows::UI::ViewManagement;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;

const wchar_t* SAMPLE_TASK_NAME = L"SampleBackgroundTask";
const wchar_t* SAMPLE_TASK_ENTRY_POINT = L"BackgroundTasks.SampleBackgroundTask";

ScenarioInput1::ScenarioInput1()
{
	InitializeComponent();
    _dispatcher = Window::Current->Dispatcher;
}

ScenarioInput1::~ScenarioInput1()
{
}

void ScenarioInput1::OnNavigatedTo(NavigationEventArgs^ e)
{
	// Get a pointer to our main page.
	rootPage = dynamic_cast<MainPage^>(e->Parameter);
	_frameLoadedToken = rootPage->OutputFrameLoaded += ref new EventHandler<Object^>(this, &ScenarioInput1::OutputFrameLoaded);
}

void ScenarioInput1::OutputFrameLoaded(Object^ sender, Object^ e)
{
	if (rootPage->Channel != nullptr)
	{
		OutputToTextBox(rootPage->Channel->Uri);
	}
}

void ScenarioInput1::OnNavigatedFrom(NavigationEventArgs^ e)
{
	rootPage->OutputFrameLoaded -= _frameLoadedToken;
}

void ScenarioInput1::OutputToTextBox(String^ text)
{
    // Find the output text box on the page
    auto outputFrame = dynamic_cast<Page^>(rootPage->OutputFrame->Content);
    auto textBox = dynamic_cast<TextBox^>(outputFrame->FindName("Scenario1ChannelOutput"));

    // Write text
    textBox->Text = text;
}

void ScenarioInput1::Scenario1Open_Click(Object^ sender, RoutedEventArgs^ e)
{
    // Applications must have lock screen privileges in order to receive raw notifications
    create_task(BackgroundExecutionManager::RequestAccessAsync()).then([this] (BackgroundAccessStatus backgroundStatus)
	{
        // Make sure the user allowed privileges
        if (backgroundStatus != BackgroundAccessStatus::Denied && backgroundStatus != BackgroundAccessStatus::Unspecified)
        {
            OpenChannelAndRegisterTask();
        }
        else
        {
            // This event comes back in a background thread, so we need to move to the UI thread to access any UI elements
            rootPage->NotifyUser("Lock screen access is denied.", NotifyType::ErrorMessage);
        }
    });
}

void ScenarioInput1::Scenario1Unregister_Click(Object^ sender, RoutedEventArgs^ e)
{
	if (UnregisterBackgroundTask())
    {
        rootPage->NotifyUser("Task unregistered", NotifyType::StatusMessage);
    }
    else
    {
        rootPage->NotifyUser("No task is registered", NotifyType::ErrorMessage);
    }
}

void ScenarioInput1::OpenChannelAndRegisterTask()
{
	// Clear out the task just for the purpose of this sample
	UnregisterBackgroundTask();

    // Open the channel. See the "Push and Polling Notifications" sample for more detail
	
	if (rootPage->Channel == nullptr) {
		create_task(PushNotificationChannelManager::CreatePushNotificationChannelForApplicationAsync()).then([this] (task<PushNotificationChannel^> channelTask)
		{
			PushNotificationChannel^ newChannel;
			try
			{
				newChannel = channelTask.get();
			}
			catch (Exception^ ex)
			{
				rootPage->NotifyUser("Could not create a channel. Error: " + ex->Message, NotifyType::ErrorMessage);
			}

			if (newChannel != nullptr)
			{
				// This event comes back in a background thread, so we need to move to the UI thread to access any UI elements
				OutputToTextBox(newChannel->Uri);
				rootPage->NotifyUser("Channel request succeeded!", NotifyType::StatusMessage);
            
				rootPage->Channel = newChannel;

				RegisterBackgroundTask();
				rootPage->NotifyUser("Task registered", NotifyType::StatusMessage);
			}
		});
	} else {
		RegisterBackgroundTask();
		rootPage->NotifyUser("Task registered", NotifyType::StatusMessage);
	}
}

void ScenarioInput1::RegisterBackgroundTask()
{
    auto taskBuilder = ref new BackgroundTaskBuilder();
    auto trigger = ref new PushNotificationTrigger();
    taskBuilder->SetTrigger(trigger);

    // Background tasks must live in separate DLL, and be included in the package manifest
    // Also, make sure that your main application project includes a reference to this DLL
    taskBuilder->TaskEntryPoint = ref new String(SAMPLE_TASK_ENTRY_POINT);
    taskBuilder->Name = ref new String(SAMPLE_TASK_NAME);
    
	BackgroundTaskRegistration^ task;
	try
	{
		task = taskBuilder->Register();
	}
	catch (Exception^ ex)
	{
		rootPage->NotifyUser("Registration error: " + ex->Message, NotifyType::ErrorMessage);
		UnregisterBackgroundTask();
	}

	if (task != nullptr)
	{
		task->Completed += ref new BackgroundTaskCompletedEventHandler(this, &ScenarioInput1::BackgroundTaskCompleted);
	}
}

bool ScenarioInput1::UnregisterBackgroundTask()
{
	auto iter = BackgroundTaskRegistration::AllTasks->First();
	while (iter->HasCurrent)
	{
		auto task = iter->Current->Value;
		if (task->Name == ref new String(SAMPLE_TASK_NAME))
		{
			task->Unregister(true);
			return true;
		}
		iter->MoveNext();
	}

	return false;
}

void ScenarioInput1::BackgroundTaskCompleted(BackgroundTaskRegistration^ sender, BackgroundTaskCompletedEventArgs^ args)
{
    try
    {
        // This sample assumes the payload is a string, but it can be of any type.
        auto payload = dynamic_cast<String^>(ApplicationData::Current->LocalSettings->Values->Lookup(ref new String(SAMPLE_TASK_NAME)));
        _dispatcher->RunAsync(CoreDispatcherPriority::Normal, ref new DispatchedHandler([this, payload] () {
            rootPage->NotifyUser("Background work item triggered by raw notification with payload = " + payload + " has completed!", NotifyType::StatusMessage);
        }, CallbackContext::Any));
    } 
    catch (Exception^ ex)
    {
        _dispatcher->RunAsync(CoreDispatcherPriority::Normal, ref new DispatchedHandler([this] () {
            rootPage->NotifyUser("Failed to retrieve background payload", NotifyType::ErrorMessage);
        }, CallbackContext::Any));
    }
}