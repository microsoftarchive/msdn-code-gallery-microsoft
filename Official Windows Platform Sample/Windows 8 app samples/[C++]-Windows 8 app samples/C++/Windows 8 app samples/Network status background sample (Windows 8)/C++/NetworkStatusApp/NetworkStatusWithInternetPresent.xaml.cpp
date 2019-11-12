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
// NetworkStatusWithInternetPresent.xaml.cpp
// Implementation of the NetworkStatusWithInternetPresent class
//

#include "pch.h"
#include "NetworkStatusWithInternetPresent.xaml.h"
#include "Constants.h"

using namespace SDKSample::NetworkStatusApp;

using namespace Windows::ApplicationModel::Background;
using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Networking::Connectivity;
using namespace Platform;

NetworkStatusWithInternetPresent::NetworkStatusWithInternetPresent()
{
    InitializeComponent();
    SampleBackgroundTaskWithConditionDispatcher = CoreWindow::GetForCurrentThread()->Dispatcher;
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void NetworkStatusWithInternetPresent::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;

    //
    // Attach completed handler to any existing tasks.
    //
    auto iter = BackgroundTaskRegistration::AllTasks->First();
    auto hascur = iter->HasCurrent;
    while (hascur)
    {
        auto cur = iter->Current->Value;

        if (cur->Name == SampleBackgroundTaskWithConditionName)
        {
            AttachCompletedHandler(cur);
            BackgroundTaskSample::UpdateBackgroundTaskStatus(cur->Name, true);
        }

        hascur = iter->MoveNext();
    }

    UpdateUI();
}

/// <summary>
/// Attach completed hander to a background task.
/// </summary>
/// <param name="task">The task to attach completed handler to.</param>
void NetworkStatusWithInternetPresent::AttachCompletedHandler(IBackgroundTaskRegistration^ task)
{
    task->Completed += ref new BackgroundTaskCompletedEventHandler(this, &NetworkStatusWithInternetPresent::OnCompleted);
}

void NetworkStatusWithInternetPresent::RegisterBackgroundTask(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    //Save current internet profile and network adapter ID globally
    try
    {
        ConnectionProfile^ profile = NetworkInformation::GetInternetConnectionProfile();
        if (profile != nullptr)
        {
            internetProfile = profile->ProfileName;
            if (profile->NetworkAdapter != nullptr)
            {
                networkAdapter = profile->NetworkAdapter->NetworkAdapterId.ToString();
            }
            else
            {
                networkAdapter = "Not connected to Internet";
            }
        }
        else
        {
            internetProfile = "Not connected to Internet";
            networkAdapter = "Not connected to Internet";
        }
    }
    
    catch (Exception^ ex)
    {
        rootPage->NotifyUser("An unexpected exception occured: " + ex->Message, NotifyType::ErrorMessage);
    }

    auto task = BackgroundTaskSample::RegisterBackgroundTask(SampleBackgroundTaskEntryPoint,
                                                             SampleBackgroundTaskWithConditionName,
                                                             ref new SystemTrigger(SystemTriggerType::NetworkStateChange, false),
                                                             ref new SystemCondition(SystemConditionType::InternetAvailable));
    AttachCompletedHandler(task);
    UpdateUI();
    rootPage->NotifyUser("Registered", NotifyType::StatusMessage);
}

void NetworkStatusWithInternetPresent::UnregisterBackgroundTask(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    BackgroundTaskSample::UnregisterBackgroundTasks(SampleBackgroundTaskWithConditionName);
    UpdateUI();
    rootPage->NotifyUser("Unregistered", NotifyType::StatusMessage);
}

/// <summary>
/// Handle background task completion.
/// </summary>
/// <param name="task">The task that is reporting completion.</param>
/// <param name="e">Arguments of the completion report.</param>
void NetworkStatusWithInternetPresent::OnCompleted(BackgroundTaskRegistration^ task, BackgroundTaskCompletedEventArgs^ args)
{
    auto uiDelegate = [this]()
    {
        ApplicationDataContainer^ localSettings = ApplicationData::Current->LocalSettings;
        String^ profile = safe_cast<String^>(localSettings->Values->Lookup("InternetProfile"));
        String^ adapter = safe_cast<String^>(localSettings->Values->Lookup("NetworkAdapterId"));

        //If internet profile has changed, display the new internet profile
        if (!(profile->Equals(internetProfile)) || 
            !(adapter->Equals(networkAdapter)))
        {
            internetProfile = profile;
            networkAdapter = adapter;
            rootPage->NotifyUser("Internet Profile changed\n" + "=================\n" + "Current Internet Profile : " + internetProfile, NotifyType::StatusMessage);
        }
    };

    auto handler = ref new Windows::UI::Core::DispatchedHandler(uiDelegate, Platform::CallbackContext::Any);
    SampleBackgroundTaskWithConditionDispatcher->RunAsync(CoreDispatcherPriority::Normal, handler);

    UpdateUI();
}

/// <summary>
/// Update the scenario UI.
/// </summary>
void NetworkStatusWithInternetPresent::UpdateUI()
{
    auto uiDelegate = [this]()
    {
        RegisterButton->IsEnabled = !BackgroundTaskSample::SampleBackgroundTaskWithConditionRegistered;
        UnregisterButton->IsEnabled = BackgroundTaskSample::SampleBackgroundTaskWithConditionRegistered;
    };
    auto handler = ref new Windows::UI::Core::DispatchedHandler(uiDelegate, Platform::CallbackContext::Any);

    SampleBackgroundTaskWithConditionDispatcher->RunAsync(CoreDispatcherPriority::Normal, handler);
}
