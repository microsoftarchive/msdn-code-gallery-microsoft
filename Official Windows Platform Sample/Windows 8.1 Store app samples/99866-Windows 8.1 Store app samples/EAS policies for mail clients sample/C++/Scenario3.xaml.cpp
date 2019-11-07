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

using namespace SDKSample::EAS;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Graphics::Display;
using namespace Windows::UI::ViewManagement;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Security::ExchangeActiveSyncProvisioning;
using namespace concurrency;
using namespace Windows::UI::Core;

Scenario3::Scenario3()
{
    InitializeComponent();
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
}



void SDKSample::EAS::Scenario3::Launch_Click3(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	try
	{
    
		EasClientSecurityPolicy^ RequestedPolicy = ref new EasClientSecurityPolicy;
		

		if (RequireEncryptionValue2 == nullptr)
		{
			RequestedPolicy->RequireEncryption=false;
		}
		else
		{
			if (RequireEncryptionValue2->IsChecked)
				RequestedPolicy->RequireEncryption=true;
			else
			{
				RequestedPolicy->RequireEncryption=false;
			}
			
		}

		if (MinPasswordLengthValue2==nullptr||MinPasswordLengthValue2->Text=="")
			RequestedPolicy->MinPasswordLength = 0;
		else
		{
			RequestedPolicy->MinPasswordLength = _wtoi(MinPasswordLengthValue2->Text->Data());
		}

		if (DisallowConvenienceLogonValue2 == nullptr)
		{
			RequestedPolicy->DisallowConvenienceLogon=false;
		}
		else
		{
			if (DisallowConvenienceLogonValue2->IsChecked)
				RequestedPolicy->DisallowConvenienceLogon=true;
			else
			{
				RequestedPolicy->DisallowConvenienceLogon=false;
			}
			
		}

		if (MinPasswordComplexCharactersValue2==nullptr||MinPasswordComplexCharactersValue2->Text=="")
			RequestedPolicy->MinPasswordComplexCharacters = 0;
		else
		{
			RequestedPolicy->MinPasswordComplexCharacters = _wtoi(MinPasswordComplexCharactersValue2->Text->Data());
		}

		TimeSpan ExpirationDays;
		if (PasswordExpirationValue2==nullptr||PasswordExpirationValue2->Text=="")
		{
			ExpirationDays.Duration = 0;
			RequestedPolicy->PasswordExpiration = ExpirationDays;
		}
		else
		{
			ExpirationDays.Duration = _wtoi(PasswordExpirationValue2->Text->Data())*86400000;
			RequestedPolicy->PasswordExpiration = ExpirationDays;
		}
		
		if (PasswordHistoryValue2==nullptr||PasswordHistoryValue2->Text=="")
			RequestedPolicy->PasswordHistory = 0;
		else
		{
			RequestedPolicy->PasswordHistory = _wtoi(PasswordHistoryValue2->Text->Data());
		}

		if (MaxPasswordFailedAttemptsValue2==nullptr||MaxPasswordFailedAttemptsValue2->Text=="")
			RequestedPolicy->MaxPasswordFailedAttempts = 0;
		else
		{
			RequestedPolicy->MaxPasswordFailedAttempts = _wtoi(MaxPasswordFailedAttemptsValue2->Text->Data());
		}

		TimeSpan Inactiveseconds;
		if (MaxInactivityTimeLockValue2==nullptr||MaxInactivityTimeLockValue2->Text=="")
		{
			Inactiveseconds.Duration=0;
			RequestedPolicy->MaxInactivityTimeLock = Inactiveseconds;
		}
		else
		{
			Inactiveseconds.Duration=_wtoi(MaxInactivityTimeLockValue2->Text->Data())*1000;
			RequestedPolicy->MaxInactivityTimeLock = Inactiveseconds;
		}
						
	task<EasComplianceResults^> ApplyOp(RequestedPolicy->ApplyAsync());
        ApplyOp.then([=](task<EasComplianceResults^> resultTask)
        {
            try
            {
			auto results = resultTask.get();

			RequireEncryptionResult2->Text = results->RequireEncryptionResult.ToString();
			MinPasswordLengthResult2->Text = results->MinPasswordLengthResult.ToString();
			DisallowConvenienceLogonResult2->Text = results->DisallowConvenienceLogonResult.ToString();
			MinPasswordComplexCharactersResult2->Text = results->MinPasswordComplexCharactersResult.ToString();
			PasswordExpirationResult2->Text = results->PasswordExpirationResult.ToString();
			PasswordHistoryResult2->Text = results->PasswordHistoryResult.ToString();
			MaxPasswordFailedAttemptsResult2->Text = results->MaxPasswordFailedAttemptsResult.ToString();
			MaxInactivityTimeLockResult2->Text = results->MaxInactivityTimeLockResult.ToString();
			}
			catch(Platform::Exception^ ex)
			{
				// ignore that 0x800704C7 exception  
				if(ex->HResult!=0x800704C7)
					DebugArea->Text += "Error: " +ex->Message;
			}
		});

	} 

	catch (Platform::Exception^ ex)
	{

		DebugArea->Text += "Error: " +ex->Message;
	}
    
}

void SDKSample::EAS::Scenario3::Reset_Click3(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	try
	{
		RequireEncryptionValue2->IsChecked=false;	
		MinPasswordLengthValue2->Text="";
		DisallowConvenienceLogonValue2->IsChecked=false;
		MinPasswordComplexCharactersValue2->Text="";
		PasswordExpirationValue2->Text="";
		PasswordHistoryValue2->Text="";
		MaxPasswordFailedAttemptsValue2->Text="";
		MaxInactivityTimeLockValue2->Text="";

		RequireEncryptionResult2->Text = "";
		MinPasswordLengthResult2->Text = "";
		DisallowConvenienceLogonResult2->Text = "";
		MinPasswordComplexCharactersResult2->Text = "";
		PasswordExpirationResult2->Text = "";
		PasswordHistoryResult2->Text = "";
		MaxPasswordFailedAttemptsResult2->Text = "";
		MaxInactivityTimeLockResult2->Text = "";

		DebugArea->Text = "";
	}
	catch(Platform::Exception^ ex)
	{
		DebugArea->Text += "Error: " +ex->Message;
	}
}

