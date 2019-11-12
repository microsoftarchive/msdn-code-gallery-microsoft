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
// Scenario1.xaml.cpp
// Implementation of the Scenario1 class
//

#include "pch.h"
#include "Scenario1.xaml.h"

using namespace SDKSample::PersonalityAnimations;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::Xaml::Media::Animation;

Scenario1::Scenario1()
{
    InitializeComponent();
    PopIn->IsEnabled = false;
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Scenario1::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void Scenario1::PopInClick(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	PopInStoryboard->Begin();
        PopIn->IsEnabled = false;
        PopOut->IsEnabled = true;
}

void Scenario1::PopOutClick(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	PopOutStoryboard->Begin();
        PopIn->IsEnabled = true;
        PopOut->IsEnabled = false;
}

