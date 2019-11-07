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
// HighPriority.xaml.cpp
// Implementation of the HighPriority class
//

#include "pch.h"
#include "HighPriority.xaml.h"

using namespace SDKSample::ApplicationDataSample;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::Core;
using namespace Windows::Storage;

HighPriority::HighPriority()
{
    InitializeComponent();

    ApplicationData^ applicationData = ApplicationData::Current;
    roamingSettings = applicationData->RoamingSettings;
    applicationData->DataChanged += ref new TypedEventHandler<ApplicationData^, Object^>(this, &HighPriority::DataChangedHandler);

    DisplayOutput(false);
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void HighPriority::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

// Guidance for using the HighPriority setting.
//
// Writing to the HighPriority setting enables a developer to store a small amount of
// data that will be roamed out to the cloud with higher priority than other roaming
// data, when possible.
//
// Applications should carefully consider which data should be stored in the 
// HighPriority setting.  "Context" data such as the user's location within
// media, or their current game-baord and high-score, can make the most sense to
// roam with high priority.  By using the HighPriority setting, this information has
// a higher likelihood of being available to the user when they begin to use another
// machine.
//
// Applications should update their HighPriority setting when the user makes
// a significant change to the data it represents.  Examples could include changing
// music tracks, turning the page in a book, or finishing a level in a game.

void HighPriority::IncrementHighPriority_Click(Object^ sender, RoutedEventArgs^ e)
{
    int counter = 0;

    IPropertyValue^ pv = safe_cast<IPropertyValue^>(roamingSettings->Values->Lookup("HighPriority"));
    if (pv != nullptr)
    {
        counter = pv->GetInt32();
    }

    roamingSettings->Values->Insert("HighPriority", PropertyValue::CreateInt32(counter + 1));

    DisplayOutput(false);
}

void HighPriority::DataChangedHandler(Windows::Storage::ApplicationData^ appData, Object^)
{
    // This event handler may be invoked on another thread, so use the Dispatcher to invoke the UI-related code on the UI thread.
    auto callback = ref new DispatchedHandler(
        [this]() { DisplayOutput(true); },
        CallbackContext::Any
        );
    this->Dispatcher->RunAsync(CoreDispatcherPriority::Normal, callback);
}

void HighPriority::DisplayOutput(bool remoteUpdate)
{
    int counter = 0;

    IPropertyValue^ pv = safe_cast<IPropertyValue^>(roamingSettings->Values->Lookup("HighPriority"));
    if (pv != nullptr)
    {
        counter = pv->GetInt32();
    }

    OutputTextBlock->Text = "Counter: " + counter.ToString() + (remoteUpdate ? " (updated remotely)" : "");
}
