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
// Scenario5.xaml.cpp
// Implementation of the Scenario5 class
//

#include "pch.h"
#include "Scenario5.xaml.h"
#include "SampleDataSource.h"

using namespace SDKSample::FlipViewCPP;
using namespace SDKSample::FlipViewData;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario5::Scenario5():bHorizontal(true)
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Scenario5::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;

	auto sampleData = ref new FlipViewData::SampleDataSource();
	FlipView5Horizontal->ItemsSource = sampleData->Items;
	FlipView5Vertical->ItemsSource = sampleData->Items;
}

void SDKSample::FlipViewCPP::Scenario5::Orientation_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    bHorizontal = !bHorizontal;
    if (bHorizontal)
    {
        Orientation->Content = "Vertical";
        FlipView5Horizontal->SelectedIndex = FlipView5Vertical->SelectedIndex;
        FlipView5Horizontal->Visibility = Windows::UI::Xaml::Visibility::Visible;
        FlipView5Vertical->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
    }
    else
    {
        Orientation->Content = "Horizontal";
        FlipView5Vertical->SelectedIndex = FlipView5Horizontal->SelectedIndex;
        FlipView5Vertical->Visibility = Windows::UI::Xaml::Visibility::Visible;
        FlipView5Horizontal->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
    }
}