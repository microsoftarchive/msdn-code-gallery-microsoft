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
// Scenario6.xaml.cpp
// Implementation of the Scenario6 class
//

#include "pch.h"
#include "Scenario6.xaml.h"

using namespace SDKSample::WebViewControl;

using namespace Windows::Foundation;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;

Scenario6::Scenario6()
{
    InitializeComponent();
}

// Invoked when this page is about to be displayed in a Frame.
void Scenario6::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
	
	// Select on of the items in the ComboBox
	ComboBox1->SelectedIndex = 0;
	
	// Ensure that our Rectangle used to simulate the WebView is not shown initially
	Rect1->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
	
	WebView6->Navigate(ref new Uri("http://www.bing.com"));
}


// Click handler for the 'Solution' button.
void SDKSample::WebViewControl::Scenario6::Solution_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	Rect1->Visibility = Windows::UI::Xaml::Visibility::Visible;
}


/// <summary>
/// When the ComboBox opens we have an airspace conflict and the ComboBox cannot render its content over
/// the WebView.  Therefore, we create a WebViewBrush and set the WebView as its source and call the Redraw() method
/// which will take a visual snapshot of the WebView.  We then use that brush to fill our Rectangle.
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
void SDKSample::WebViewControl::Scenario6::ComboBox1_DropDownOpened(Platform::Object^ sender, Platform::Object^ e)
{
	if (Rect1->Visibility == Windows::UI::Xaml::Visibility::Visible)
            {
                WebViewBrush^ b = ref new WebViewBrush();
                b->SourceName = "WebView6";
                b->Redraw();
                Rect1->Fill = b;
                WebView6->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
            }
}

/// <summary>
/// When the ComboBox is closed we no longer need the simulated WebView so we put things back the way they were
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
void SDKSample::WebViewControl::Scenario6::ComboBox1_DropDownClosed(Platform::Object^ sender, Platform::Object^ e)
{
	
	WebView6->Visibility = Windows::UI::Xaml::Visibility::Visible;
    Rect1->Fill = ref new SolidColorBrush(Windows::UI::Colors::Transparent);
}

