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
// Scenario4.xaml.cpp
// Implementation of the Scenario4 class
//

#include "pch.h"
#include "Scenario4.xaml.h"
#include "SampleDataSource.h"

using namespace SDKSample::ListViewInteraction;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Graphics::Display;
using namespace Windows::UI::ViewManagement;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;

using namespace SDKSample::ListViewInteractionSampleDataSource;

Scenario4::Scenario4()
{
    InitializeComponent();
	toppingsData = ref new ToppingsData();
    storeData = ref new StoreData();
    FlavorGrid->ItemsSource = storeData->Items;
    FixinsGrid->ItemsSource = toppingsData->Items;
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Scenario4::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void SDKSample::ListViewInteraction::Scenario4::CreateCustomCarton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	rootPage->NotifyUser("", NotifyType::StatusMessage);
    
	if(FlavorGrid->SelectedItems->Size >0)
	{
		CustomCarton->Text = "Custom Carton: ";
		CustomCarton->Text += ((Item^)FlavorGrid->SelectedItem)->Title;
		if(FixinsGrid->SelectedItems->Size >0)
		{
			UINT i=0;
			CustomCarton->Text += " with ";
			while(i<FixinsGrid->SelectedItems->Size)
			{
				Item^ item = dynamic_cast<Item^>(FixinsGrid->SelectedItems->GetAt(i));
				CustomCarton->Text += item->Title;
				if(i<FixinsGrid->SelectedItems->Size-1)
					CustomCarton->Text += ", ";
				i++;
			}
			CustomCarton->Text += " toppings";
		}
	}
	else
	{
		rootPage->NotifyUser("Please select at least a flavor...", NotifyType::StatusMessage);
	}
}


