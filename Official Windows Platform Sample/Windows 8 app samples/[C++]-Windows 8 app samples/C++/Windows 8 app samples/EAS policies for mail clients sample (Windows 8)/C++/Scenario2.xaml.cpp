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

using namespace SDKSample::EAS;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Security::ExchangeActiveSyncProvisioning;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;

Scenario2::Scenario2()
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
}



void SDKSample::EAS::Scenario2::Launch_Click2(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	try
	{
    
		EasClientSecurityPolicy^ RequestedPolicy = ref new EasClientSecurityPolicy;
		

		if (RequireEncryptionValue1 == nullptr)
		{
			RequestedPolicy->RequireEncryption=false;
		}
		else
		{
			if (RequireEncryptionValue1->IsChecked)
				RequestedPolicy->RequireEncryption=true;
			else
			{
				RequestedPolicy->RequireEncryption=false;
			}
			
		}

		if (MinPasswordLengthValue1==nullptr||MinPasswordLengthValue1->Text=="")
			RequestedPolicy->MinPasswordLength = 0;
		else
		{
			RequestedPolicy->MinPasswordLength = _wtoi(MinPasswordLengthValue1->Text->Data());
		}

		if (DisallowConvenienceLogonValue1 == nullptr)
		{
			RequestedPolicy->DisallowConvenienceLogon=false;
		}
		else
		{
			if (DisallowConvenienceLogonValue1->IsChecked)
				RequestedPolicy->DisallowConvenienceLogon=true;
			else
			{
				RequestedPolicy->DisallowConvenienceLogon=false;
			}
			
		}

		if (MinPasswordComplexCharactersValue1==nullptr||MinPasswordComplexCharactersValue1->Text=="")
			RequestedPolicy->MinPasswordComplexCharacters = 0;
		else
		{
			RequestedPolicy->MinPasswordComplexCharacters = _wtoi(MinPasswordComplexCharactersValue1->Text->Data());
		}

		TimeSpan ExpirationDays;
		if (PasswordExpirationValue1==nullptr||PasswordExpirationValue1->Text=="")
		{
			ExpirationDays.Duration = 0;
			RequestedPolicy->PasswordExpiration = ExpirationDays;
		}
		else
		{
			ExpirationDays.Duration = _wtoi(PasswordExpirationValue1->Text->Data())*86400000;
			RequestedPolicy->PasswordExpiration = ExpirationDays;
		}
		
		if (PasswordHistoryValue1==nullptr||PasswordHistoryValue1->Text=="")
			RequestedPolicy->PasswordHistory = 0;
		else
		{
			RequestedPolicy->PasswordHistory = _wtoi(PasswordHistoryValue1->Text->Data());
		}

		if (MaxPasswordFailedAttemptsValue1==nullptr||MaxPasswordFailedAttemptsValue1->Text=="")
			RequestedPolicy->MaxPasswordFailedAttempts = 0;
		else
		{
			RequestedPolicy->MaxPasswordFailedAttempts = _wtoi(MaxPasswordFailedAttemptsValue1->Text->Data());
		}

		TimeSpan Inactiveseconds;
		if (MaxInactivityTimeLockValue1==nullptr||MaxInactivityTimeLockValue1->Text=="")
		{
			Inactiveseconds.Duration=0;
			RequestedPolicy->MaxInactivityTimeLock = Inactiveseconds;
		}
		else
		{
			Inactiveseconds.Duration=_wtoi(MaxInactivityTimeLockValue1->Text->Data())*1000;
			RequestedPolicy->MaxInactivityTimeLock = Inactiveseconds;
		}
						
		EasComplianceResults^ CheckResult = RequestedPolicy->CheckCompliance();
			
		RequireEncryptionResult1->Text = CheckResult->RequireEncryptionResult.ToString();
		MinPasswordLengthResult1->Text = CheckResult->MinPasswordLengthResult.ToString();
		DisallowConvenienceLogonResult1->Text = CheckResult->DisallowConvenienceLogonResult.ToString();
		MinPasswordComplexCharactersResult1->Text = CheckResult->MinPasswordComplexCharactersResult.ToString();
		PasswordExpirationResult1->Text = CheckResult->PasswordExpirationResult.ToString();
		PasswordHistoryResult1->Text = CheckResult->PasswordHistoryResult.ToString();
		MaxPasswordFailedAttemptsResult1->Text = CheckResult->MaxPasswordFailedAttemptsResult.ToString();
		MaxInactivityTimeLockResult1->Text = CheckResult->MaxInactivityTimeLockResult.ToString();


	} 

	catch (Platform::Exception^ ex)
	{
		DebugArea->Text += "Error: " +ex->Message;
	}
    
}

void SDKSample::EAS::Scenario2::Reset_Click2(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	try
	{
		RequireEncryptionValue1->IsChecked=false;	
		MinPasswordLengthValue1->Text="";
		DisallowConvenienceLogonValue1->IsChecked=false;
		MinPasswordComplexCharactersValue1->Text="";
		PasswordExpirationValue1->Text="";
		PasswordHistoryValue1->Text="";
		MaxPasswordFailedAttemptsValue1->Text="";
		MaxInactivityTimeLockValue1->Text="";

		RequireEncryptionResult1->Text = "";
		MinPasswordLengthResult1->Text = "";
		DisallowConvenienceLogonResult1->Text = "";
		MinPasswordComplexCharactersResult1->Text = "";
		PasswordExpirationResult1->Text = "";
		PasswordHistoryResult1->Text = "";
		MaxPasswordFailedAttemptsResult1->Text = "";
		MaxInactivityTimeLockResult1->Text = "";
	}
	catch(Platform::Exception^ ex)
	{
		DebugArea->Text += "Error: " +ex->Message;
	}
}
