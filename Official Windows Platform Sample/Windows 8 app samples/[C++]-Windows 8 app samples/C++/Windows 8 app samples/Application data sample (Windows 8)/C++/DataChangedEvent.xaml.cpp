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
// DataChangedEvent.xaml.cpp
// Implementation of the DataChangedEvent class
//

#include "pch.h"
#include "DataChangedEvent.xaml.h"

using namespace SDKSample::ApplicationDataSample;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::Core;
using namespace Windows::Storage;

#define settingName "userName"

DataChangedEvent::DataChangedEvent()
{
    InitializeComponent();

    applicationData = ApplicationData::Current;
    applicationData->DataChanged += ref new TypedEventHandler<ApplicationData^, Object^>(this, &DataChangedEvent::DataChangedHandler);

    DisplayOutput();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void DataChangedEvent::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void DataChangedEvent::SimulateRoaming_Click(Object^ sender, RoutedEventArgs^ e)
{
    applicationData->RoamingSettings->Values->Insert(settingName, UserName->Text);

    // Simulate roaming by intentionally signaling a data changed event.
    applicationData->SignalDataChanged();
}

void DataChangedEvent::DataChangedHandler(Windows::Storage::ApplicationData^ appData, Object^)
{
    // This event handler may be invoked on another thread, so use the Dispatcher to invoke the UI-related code on the UI thread.
    Dispatcher->RunAsync(CoreDispatcherPriority::Normal, ref new DispatchedHandler(
        [this]()
        {
            DisplayOutput();
        }));
}

void DataChangedEvent::DisplayOutput()
{
    String^ value = safe_cast<String^>(ApplicationData::Current->RoamingSettings->Values->Lookup(settingName));
    OutputTextBlock->Text = "Name: " + (value == nullptr ? "<empty>" : ("\"" + value + "\""));
}
