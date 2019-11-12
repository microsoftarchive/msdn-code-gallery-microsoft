//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// ScenarioScannerEnumeration.xaml.cpp
// Implementation of the ScenarioScannerEnumeration class
//

#include "pch.h"
#include "ScenarioScannerEnumeration.xaml.h"
#include "MainPage.xaml.h"
#include "Utils.h"

using namespace SDKSample;
using namespace SDKSample::ScanRuntimeAPI;

using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::Foundation;
using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml;
using namespace Windows::Devices::Scanners;
using namespace concurrency;
using namespace Windows::Storage::Streams;
using namespace Windows::Storage;
using namespace Windows::UI::Xaml::Controls;

/// <summary>
/// Constructor
/// </summary>
ScenarioScannerEnumeration::ScenarioScannerEnumeration()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e"></param>
void ScenarioScannerEnumeration::OnNavigatedTo(_In_ Windows::UI::Xaml::Navigation::NavigationEventArgs^  e)
{
    UpdateStartStopButtons();
}

/// <summary>
/// Even Handler for click on Start Scanner Enumeration button 
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
void ScenarioScannerEnumeration::Start_Enumeration_Watcher_Click(_In_ Platform::Object^ sender, _In_ Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ b = dynamic_cast<Button^>(sender);
    
    if (b != nullptr)
    {
        MainPage::Current->NotifyUser("Starting Enumeration of scanners.", NotifyType::StatusMessage);	
        ModelDataContext->ScannerDataContext->StartScannerWatcher();
        UpdateStartStopButtons();
    }
}

/// <summary>
/// Stops the enumeration of scanner devices
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
void ScenarioScannerEnumeration::Stop_Enumeration_Watcher_Click(_In_ Platform::Object^ sender, _In_ Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ b = dynamic_cast<Button^>(sender);	
    if (b != nullptr)
    {
        MainPage::Current->NotifyUser("Stoping device watcher for scanners.", NotifyType::StatusMessage);	
        ModelDataContext->ScannerDataContext->StopScannerWatcher();
        UpdateStartStopButtons();
    }
}

/// <summary>
/// Updates Start and Stop Enumeration buttons
/// </summary>
void ScenarioScannerEnumeration::UpdateStartStopButtons()
{
    StartEnumerationWatcher->IsEnabled = !ModelDataContext->ScannerDataContext->WatcherStarted;
    StopEnumerationWatcher->IsEnabled = ModelDataContext->ScannerDataContext->WatcherStarted;
}