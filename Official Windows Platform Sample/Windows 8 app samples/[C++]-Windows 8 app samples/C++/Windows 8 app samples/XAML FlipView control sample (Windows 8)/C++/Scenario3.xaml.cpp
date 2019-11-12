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
// Scenario3.xaml.cpp
// Implementation of the Scenario3 class
//

#include "pch.h"
#include "Scenario3.xaml.h"
#include "SampleDataSource.h"

using namespace SDKSample::FlipViewCPP;
using namespace SDKSample::FlipViewData;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario3^ Scenario3::Current;

Scenario3::Scenario3()
{
    InitializeComponent();
	Current = this;
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Scenario3::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;

	// Construct the table of contents used for navigating the FlipView
    // Create a StackPanel to host the TOC
    StackPanel^ sp = ref new StackPanel();
    sp->Orientation = Orientation::Vertical;
	sp->HorizontalAlignment = Windows::UI::Xaml::HorizontalAlignment::Center;

	// Add the TOC title
    TextBlock^ tb = ref new TextBlock();
    tb->Text = "Table of Contents";
	tb->Style = dynamic_cast<Windows::UI::Xaml::Style^>(Scenario3::Current->Resources->Lookup("TOCTitle"));
    sp->Children->Append(tb);

	// Get our sample data
	auto sampleData = ref new FlipViewData::SampleDataSource();

	// Create the TOC from the data
    // Use buttons for each TOC entry using the Tag property
    // to contain the index of the target
    UINT i = 0;
	UINT count = sampleData->Items->Size;
	while(i<count)
	{
		SampleDataItem^ sItem = dynamic_cast<SampleDataItem^>(sampleData->Items->GetAt(i));
		if(sItem)
		{
			Button^ b = ref new Button();
			b->Style = dynamic_cast<Windows::UI::Xaml::Style^>(Scenario3::Current->Resources->Lookup("ButtonStyle1"));			
			b->Content = sItem->Title;
			b->Click += ref new RoutedEventHandler(this, & Scenario3::TOCButton_Click);
			b->Tag = i.ToString();
			sp->Children->Append(b);
			b->TabIndex = i + 1;
			i++;
		}
		else
			break;
	}


	// Add the TOC to our data set
    sampleData->Items->InsertAt(0, sp); 

    // Use a template selector to style the TOC entry differently from the other data entries
    FlipView3->ItemTemplateSelector = ref new ItemSelector();
	FlipView3->ItemsSource = sampleData->Items;
}


void SDKSample::FlipViewCPP::Scenario3::TOCButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	Button^ b = safe_cast<Button^>(sender);
    if (b != nullptr)
    {
		FlipView3->SelectedIndex = b->TabIndex;
    }
}
