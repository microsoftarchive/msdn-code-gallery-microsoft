//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario2.xaml.cpp
// Implementation of the Scenario2 class
//

#include "pch.h"
#include "Scenario2.xaml.h"
#include "Constants.h"

using namespace SDKSample;
using namespace SDKSample::MultipleViews;

using namespace Platform;
using namespace Windows::Storage;
using namespace Windows::UI::ViewManagement;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario2::Scenario2()
{
    InitializeComponent();
}

void Scenario2::OnNavigatedTo(NavigationEventArgs^ e)
{
    // Normally, you would hard code calling DisableShowingMainViewOnActivation
    // in the activation/launch handlers of App.xaml.cs. The sample allows 
    // the user to change the preference for demonstration purposes

    // Check the data stored from last run. Restart if you change this. See
    // App.xaml.cpp for calling DisableShowingMainViewOnActivation
    auto shouldDisable = ApplicationData::Current->LocalSettings->Values->Lookup(DISABLE_MAIN_VIEW_KEY);
    if (shouldDisable != nullptr && safe_cast<bool>(shouldDisable))
    {
        DisableMainBox->IsChecked = true;
    }
}

void Scenario2::DisableMainBox_Checked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    // Normally, you would hard code calling DisableShowingMainViewOnActivation
    // in the activation/launch handlers of App.xaml.cs. The sample allows 
    // the user to change the preference for demonstration purposes
    ApplicationData::Current->LocalSettings->Values->Insert(DISABLE_MAIN_VIEW_KEY, true);
}

void Scenario2::DisableMainBox_Unchecked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    // Normally, you would hard code calling DisableShowingMainViewOnActivation
    // in the activation/launch handlers of App.xaml.cs. The sample allows 
    // the user to change the preference for demonstration purposes
    ApplicationData::Current->LocalSettings->Values->Remove(DISABLE_MAIN_VIEW_KEY);
}