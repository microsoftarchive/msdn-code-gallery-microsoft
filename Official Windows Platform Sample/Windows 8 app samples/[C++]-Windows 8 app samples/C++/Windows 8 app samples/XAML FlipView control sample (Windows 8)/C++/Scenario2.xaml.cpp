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
// Scenario2.xaml.cpp
// Implementation of the Scenario2 class
//

#include "pch.h"
#include "Scenario2.xaml.h"
#include "SampleDataSource.h"

using namespace SDKSample::FlipViewCPP;
using namespace SDKSample::FlipViewData;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario2::Scenario2(): bHorizontal(true)
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

	SampleDataSource^ sampleData = ref new FlipViewData::SampleDataSource();
	FlipView2Horizontal->ItemsSource = sampleData->Items;
	FlipView2Vertical->ItemsSource = sampleData->Items;
}

void Scenario2::Orientation_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	bHorizontal = !bHorizontal;
    if (bHorizontal)
    {
        Orientation->Content = "Vertical";
        FlipView2Horizontal->SelectedIndex = FlipView2Vertical->SelectedIndex;
        FlipView2Horizontal->Visibility = Windows::UI::Xaml::Visibility::Visible;
        FlipView2Vertical->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
    }
    else
    {
        Orientation->Content = "Horizontal";
        FlipView2Vertical->SelectedIndex = FlipView2Horizontal->SelectedIndex;
        FlipView2Vertical->Visibility =  Windows::UI::Xaml::Visibility::Visible;
        FlipView2Horizontal->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
    }
}

