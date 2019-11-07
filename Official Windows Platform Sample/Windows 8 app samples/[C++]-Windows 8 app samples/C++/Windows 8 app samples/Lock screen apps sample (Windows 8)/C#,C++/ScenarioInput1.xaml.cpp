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

using namespace LockScreenAppsCPP;

using namespace concurrency;
using namespace Platform;
using namespace Windows::ApplicationModel::Background;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

ScenarioInput1::ScenarioInput1()
{
    InitializeComponent();
    RequestLockScreenAccess->Click += ref new RoutedEventHandler(this, &ScenarioInput1::RequestLockScreenAccess_Click);
    RemoveLockScreenAccess->Click += ref new RoutedEventHandler(this, &ScenarioInput1::RemoveLockScreenAccess_Click);
    QueryLockScreenAccess->Click += ref new RoutedEventHandler(this, &ScenarioInput1::QueryLockScreenAccess_Click);
}

ScenarioInput1::~ScenarioInput1()
{
}

void ScenarioInput1::RequestLockScreenAccess_Click(Object^ sender, RoutedEventArgs^ e)
{    
    task<BackgroundAccessStatus> statusTask(BackgroundExecutionManager::RequestAccessAsync());
    statusTask.then([this](BackgroundAccessStatus backgroundStatus)
    {
        switch (backgroundStatus)
        {
        case BackgroundAccessStatus::AllowedWithAlwaysOnRealTimeConnectivity:
            rootPage->NotifyUser("This app is on the lock screen and has access to Always-On Real Time Connectivity.", NotifyType::StatusMessage);
            break;
        case BackgroundAccessStatus::AllowedMayUseActiveRealTimeConnectivity:
            rootPage->NotifyUser("This app is on the lock screen and has access to Active Real Time Connectivity.", NotifyType::StatusMessage);
            break;
        case BackgroundAccessStatus::Denied:
            rootPage->NotifyUser("This app is not on the lock screen.", NotifyType::StatusMessage);
            break;
        case BackgroundAccessStatus::Unspecified:
            rootPage->NotifyUser("The user has not yet taken any action. This is the default setting and the app is not on the lock screen.", NotifyType::StatusMessage);
        }
    });
}

void ScenarioInput1::RemoveLockScreenAccess_Click(Object^ sender, RoutedEventArgs^ e)
{
    BackgroundExecutionManager::RemoveAccess(); 
    rootPage->NotifyUser("This app has been removed from the lock screen.", NotifyType::StatusMessage);
}

void ScenarioInput1::QueryLockScreenAccess_Click(Object^ sender, RoutedEventArgs^ e)
{
    switch (BackgroundExecutionManager::GetAccessStatus())
    {
    case BackgroundAccessStatus::AllowedWithAlwaysOnRealTimeConnectivity:
        rootPage->NotifyUser("This app is on the lock screen and has access to Always-On Real Time Connectivity.", NotifyType::StatusMessage);
        break;
    case BackgroundAccessStatus::AllowedMayUseActiveRealTimeConnectivity:
        rootPage->NotifyUser("This app is on the lock screen and has access to Active Real Time Connectivity.", NotifyType::StatusMessage);
        break;
    case BackgroundAccessStatus::Denied:
        rootPage->NotifyUser("This app is not on the lock screen.", NotifyType::StatusMessage);
        break;
    case BackgroundAccessStatus::Unspecified:
        rootPage->NotifyUser("The user has not yet taken any action. This is the default setting and the app is not on the lock screen.", NotifyType::StatusMessage);
        break;
    default:
        break;
    }
}

void ScenarioInput1::OnNavigatedTo(NavigationEventArgs^ e)
{
    // Get a pointer to our main page.
    rootPage = dynamic_cast<MainPage^>(e->Parameter);
}
