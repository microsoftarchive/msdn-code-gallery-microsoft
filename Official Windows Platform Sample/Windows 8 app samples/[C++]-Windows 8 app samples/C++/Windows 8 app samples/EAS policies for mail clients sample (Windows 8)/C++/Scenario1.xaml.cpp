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

using namespace SDKSample::EAS;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Security::ExchangeActiveSyncProvisioning;

Scenario1::Scenario1()
{
    InitializeComponent();
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

void SDKSample::EAS::Scenario1::Launch_Click1(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	DeviceID ->Text= "";	
	FriendlyName -> Text= "";
	SystemManufacturer ->Text= "";
	SystemProductName ->Text= "";
	SystemSku ->Text= "";
	OperatingSystem ->Text= "";
	try
	{

		EasClientDeviceInformation^ CurrentDeviceInfor = ref new EasClientDeviceInformation();
		
		auto id = CurrentDeviceInfor->Id;
		OperatingSystem->Text = CurrentDeviceInfor->OperatingSystem;
		FriendlyName->Text = CurrentDeviceInfor ->FriendlyName;
		SystemManufacturer->Text = CurrentDeviceInfor->SystemManufacturer;
		SystemProductName->Text = CurrentDeviceInfor->SystemProductName;
		SystemSku->Text = CurrentDeviceInfor->SystemSku;

	} 
	catch (Platform::Exception^ ex)
	{
		DebugArea->Text += "Error: " +ex->Message;
	}
    
}

void SDKSample::EAS::Scenario1::Reset_Click1(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	try
	{
		DeviceID ->Text= "";	
		FriendlyName -> Text= "";
		SystemManufacturer ->Text= "";
		SystemProductName ->Text= "";
		SystemSku ->Text= "";
		OperatingSystem ->Text= "";
	}
	catch(Platform::Exception^ ex)
	{
		DebugArea->Text += "Error: " +ex->Message;
	}
}
