//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario2.xaml.cpp
// Implementation of the Scenario2 class
//

#include "pch.h"
#include "Scenario2.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::PasswordVaultCPP;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::ApplicationSettings;
using namespace Windows::UI::Popups;
using namespace Windows::Security::Credentials;
using namespace Windows::UI::Xaml::Navigation;


Scenario2::Scenario2()
{
	try
	{
		InitializeComponent();

		SettingsPane::GetForCurrentView()->CommandsRequested += ref new TypedEventHandler<SettingsPane ^, SettingsPaneCommandsRequestedEventArgs ^>(this, &Scenario2::OnCommandsRequested);
		AccountsSettingsPane::GetForCurrentView()->AccountCommandsRequested += ref new TypedEventHandler<AccountsSettingsPane ^, AccountsSettingsPaneCommandsRequestedEventArgs ^>(this, &Scenario2::OnAccountCommandsRequested);
	}
	catch(Platform::COMException^ e)
	{
		DebugPrint(e->Message);
		return;
	}
}

/*
void Scenario2::OnNavigatedFrom(NavigationEventArgs^ e)
{}
*/
void PasswordVaultCPP::Scenario2::OnCommandsRequested(Windows::UI::ApplicationSettings::SettingsPane ^sender, Windows::UI::ApplicationSettings::SettingsPaneCommandsRequestedEventArgs ^args)
{
	try
	{
		args->Request->ApplicationCommands->Append(SettingsCommand::AccountsCommand);
	}
	catch(Platform::COMException^ e)
	{
		DebugPrint(e->Message);
		return;
	}
}

void PasswordVaultCPP::Scenario2::OnAccountCommandsRequested(Windows::UI::ApplicationSettings::AccountsSettingsPane ^sender, Windows::UI::ApplicationSettings::AccountsSettingsPaneCommandsRequestedEventArgs ^args)
{
	try
	{
		auto  deferral = args->GetDeferral();
	
		// add credential commands 
		Windows::Security::Credentials::PasswordVault^ vault = ref new Windows::Security::Credentials::PasswordVault();
		auto creds= vault->RetrieveAll();
		if (creds)
		{
			for each (PasswordCredential^ c in creds)
			{
				CredentialCommandCredentialDeletedHandler^ credHandler = ref new CredentialCommandCredentialDeletedHandler(this, &Scenario2::CredentialDeleted); 
				CredentialCommand^ credCmd = ref new CredentialCommand(c, credHandler); 
				args->CredentialCommands->Append(credCmd); 
			}
		}
		// add app specific commands which will appear as links below all the accounts in the 
	
		UICommandInvokedHandler^ handler = ref new UICommandInvokedHandler( this, &Scenario2::HandleAppSpecificCmd); 
		SettingsCommand^ appCmd = ref new SettingsCommand( 1, // id 
														"App Specific Command Label…", // Label 
														handler); 
		args->Commands->Append(appCmd); 	 
		deferral->Complete();
	}
	catch(Platform::COMException^ e)
	{
		DebugPrint(e->Message);
		return;
	}
}


void PasswordVaultCPP::Scenario2::CredentialDeleted(CredentialCommand^ credCmd) 
{ 
	DebugPrint("Credential removed successfully from PasswordVault");
	AccountsSettingsPane::Show();
} 


void PasswordVaultCPP::Scenario2::HandleAppSpecificCmd(IUICommand^ command) 
{ 
	DebugPrint("Please add specific cmd");
	AccountsSettingsPane::Show(); 
} 

void PasswordVaultCPP::Scenario2::Manage_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	try
	{
		AccountsSettingsPane::Show();
	}
	catch(Platform::COMException^ e)
	{
		DebugPrint(e->Message);
		return;
	}
}

void PasswordVaultCPP::Scenario2::DebugPrint(String^ Message)
{
	ErrorMessage->Text = Message;
}