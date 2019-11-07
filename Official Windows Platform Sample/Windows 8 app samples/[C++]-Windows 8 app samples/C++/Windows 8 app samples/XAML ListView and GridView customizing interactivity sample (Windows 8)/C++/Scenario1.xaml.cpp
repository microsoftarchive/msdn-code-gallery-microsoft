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
#include "SampleDataSource.h"

using namespace SDKSample::ListViewInteraction;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::Xaml::Media;
using namespace SDKSample::ListViewInteractionSampleDataSource;


Scenario1::Scenario1()
{
    InitializeComponent();
	storeData = ref new StoreData();
	GridView1->ItemsSource = storeData->Items;
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


void SDKSample::ListViewInteraction::Scenario1::AddToCart_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	rootPage->NotifyUser("", NotifyType::StatusMessage);
    ShoppingCart->Text = "Cart Contents: ";

	if(GridView1->SelectedItems->Size >0)
	{
		UINT i=0;
		while(i<GridView1->SelectedItems->Size)
		{
			Item^ item = dynamic_cast<Item^>(GridView1->SelectedItems->GetAt(i));
			ShoppingCart->Text += item->Title;
			if(i<GridView1->SelectedItems->Size-1)
				ShoppingCart->Text += ", ";
			i++;
		}
		ShoppingCart->Text += " added to cart";
	}
	else
	{
		rootPage->NotifyUser("Please select items to place in the cart", NotifyType::StatusMessage);
	}
}

