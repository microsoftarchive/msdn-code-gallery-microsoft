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

using namespace AppBarControl;

using namespace Platform;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario2::Scenario2()
{
    InitializeComponent();
	rootPage = MainPage::Current;
}


// Invoked when this page is about to be displayed in a Frame.
void Scenario2::OnNavigatedTo(NavigationEventArgs^ e)
{
     rootPage = MainPage::Current;
 }

// Handler for "Is Sticky" check box
void AppBarControl::Scenario2::IsSticky_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
		auto cb = (CheckBox^) sender;
		
		auto myAppBar = (AppBar^) rootPage->FindName(L"BottomAppBar1");
        if (myAppBar != nullptr)
        {
			myAppBar->IsSticky = cb->IsChecked->Value;
		}
}
