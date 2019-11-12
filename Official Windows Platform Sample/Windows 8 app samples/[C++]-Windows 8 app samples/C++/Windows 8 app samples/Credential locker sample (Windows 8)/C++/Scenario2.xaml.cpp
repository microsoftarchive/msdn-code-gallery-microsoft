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

using namespace SDKSample::PasswordVaultCPP;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Security::Credentials;
using namespace Platform;
using namespace Platform::Collections;

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


void DebugPrint2(Page^ rootPage, Platform::String^ Trace)
{
    Page^ outputFrame = (Page^) rootPage;
    TextBox^ ErrorMessage = (TextBox^) outputFrame->FindName("ErrorMessage"); 
    ErrorMessage->Text += Trace + "\r\n";
 }

void CleanCombobox(Page^ rootPage)
{
	Page^ inputFrame = (Page^) rootPage;
	ComboBox^ SelectUser = (ComboBox^) inputFrame->FindName("SelectUser");
	try
    {
		SelectUser->SelectedIndex= -1;
		SelectUser->ItemsSource = ref new Vector<String ^>(); 
	}
    catch (Platform::Exception^ Error) 
    {
		DebugPrint2(rootPage, Error->ToString());
    }
}

void Reset2(Page^ rootPage)
{
	try
    {
		Page^ outputFrame = (Page^) rootPage;
        Page^ inputFrame = (Page^) rootPage;
        TextBox^ InputUserNameValue = (TextBox^) outputFrame->FindName("InputUserNameValue");
        InputUserNameValue->Text = "";
        PasswordBox^ InputPasswordValue = (PasswordBox^)outputFrame->FindName("InputPasswordValue");
        InputPasswordValue->Password= "";
        TextBox^ ErrorMessage = (TextBox^) outputFrame->FindName("ErrorMessage");
        ErrorMessage->Text = "";
        TextBox^ WelcomeMessage = (TextBox^) inputFrame->FindName("WelcomeMessage");
        CheckBox^ SaveCredCheck = (CheckBox^) outputFrame->FindName("SaveCredCheck");
        SaveCredCheck->IsChecked = false;
		CleanCombobox(rootPage);
	}
    catch (Platform::Exception^ Error)
    {
		DebugPrint2(rootPage, Error->ToString());
	}
}



void SDKSample::PasswordVaultCPP::Scenario2::Launch_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	try
	{
		Page^ outputFrame = (Page^) rootPage;
        Page^ inputFrame = (Page^) rootPage;
		
		Vector<String^>^ itemSource = ref new Vector<String^>();
		Vector <Object^>^ l = ref new Vector<Object^>();
		
		Reset2(rootPage);
		CheckBox^ AuthenticationFailCheck = (CheckBox ^) outputFrame->FindName("AuthenticationFailCheck");
		TextBox^ WelcomeMessage = (TextBox^) inputFrame->FindName("WelcomeMessage");
		ComboBox^ SelectUser = (ComboBox^) inputFrame->FindName("SelectUser");
		if (AuthenticationFailCheck->IsChecked->Value == true)
		{
			WelcomeMessage->Text = "Blocked";
		}
		else
		{
			try
			{
				PasswordCredential^ cred = ref new PasswordCredential();
				Windows::Security::Credentials::PasswordVault^ vault = ref new Windows::Security::Credentials::PasswordVault();
				Windows::Foundation::Collections::IVectorView<PasswordCredential^>^ creds = vault->FindAllByResource("Scenario 2");
				for each (PasswordCredential^ cred in creds)
				{
					itemSource->InsertAt(0, cred->UserName);
				}
			}
			catch (Platform::COMException^ Error)
			{
				DebugPrint2(this->rootPage, Error->ToString());
			}
			itemSource->InsertAt(0, "Add new user");
			itemSource->InsertAt(0, "");
			SelectUser->ItemsSource = itemSource;
			SelectUser->SelectedIndex = 0;

			WelcomeMessage->Text = "Scenario is ready, please sign in";
			
		}

	}
	catch (Platform::Exception^ Error)
	{
		DebugPrint2(rootPage, Error->ToString());
	}
}


void SDKSample::PasswordVaultCPP::Scenario2::Signin_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	Page^ outputFrame = (Page^) rootPage;
	Page^ inputFrame = (Page^) rootPage;
		
	TextBox^ InputUserNameValue = (TextBox^) outputFrame->FindName("InputUserNameValue");
	PasswordBox^ InputPasswordValue = (PasswordBox^) outputFrame->FindName("InputPasswordValue");
	TextBox^ WelcomeMessage = (TextBox^) inputFrame->FindName("WelcomeMessage");
	CheckBox^ SaveCredCheck = (CheckBox^) outputFrame->FindName("SaveCredCheck");
    if (InputUserNameValue->Text == "" || InputPasswordValue->Password == "")
    {
		TextBox^ ErrorMessage = (TextBox ^) outputFrame->FindName("ErrorMessage");
        ErrorMessage->Text = "User name and password are not allowed to be empty, Please input user name and password";
    }
    else
    {
		try
        {
			Windows::Security::Credentials::PasswordVault^ vault = ref new Windows::Security::Credentials::PasswordVault();
			PasswordCredential^ c = ref new PasswordCredential("Scenario 2", InputUserNameValue->Text, InputPasswordValue->Password);
            if (SaveCredCheck->IsChecked->Value)
			{
				vault->Add(c);
			}

			WelcomeMessage->Text = "Welcome to " + c->Resource + ", " + c->UserName;
        }
		catch (Platform::COMException^ Error)// No stored credentials, so none to delete
		{
			DebugPrint2(rootPage, Error->ToString());
        }
    }
    Reset2(rootPage);

    CheckBox^ AuthenticationFailCheck = (CheckBox ^) outputFrame->FindName("AuthenticationFailCheck");
    AuthenticationFailCheck->IsChecked = false;
}


void SDKSample::PasswordVaultCPP::Scenario2::ChangeUser_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	Page^ outputFrame = (Page^) rootPage;
	Page^ inputFrame = (Page^) rootPage;
	
	try
    {
		Windows::Security::Credentials::PasswordVault^ vault = ref new Windows::Security::Credentials::PasswordVault();
		Windows::Foundation::Collections::IVectorView<PasswordCredential^>^ creds = vault->FindAllByResource("Scenario 2");

		for each (PasswordCredential^ c in creds)
        {
			try
            {
				vault->Remove(c);
            }
			catch (Platform::COMException^ Error) // No stored credentials, so none to delete
			{
				DebugPrint2(rootPage, Error->ToString());
			}
		}
                
	}
	catch (Platform::COMException^ Error) // No stored credentials, so none to delete
	{
			DebugPrint2(rootPage, Error->ToString());
    }
	Reset2(rootPage);
    CheckBox^ AuthenticationFailCheck = (CheckBox ^) outputFrame->FindName("AuthenticationFailCheck");
	AuthenticationFailCheck->IsChecked = false;
    TextBox^ WelcomeMessage = (TextBox^) inputFrame->FindName("WelcomeMessage");
    WelcomeMessage->Text = "User has been changed, please resign in with new credentials, choose save and launch scenario again";
}


void SDKSample::PasswordVaultCPP::Scenario2::Reset_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	Reset2(rootPage);
}


void SDKSample::PasswordVaultCPP::Scenario2::SelectUser_Click(Platform::Object^ sender, Windows::UI::Xaml::Controls::SelectionChangedEventArgs^ e)
{
	Page^ outputFrame = (Page^) rootPage;
	Page^ inputFrame = (Page^) rootPage;
		
	TextBox^ InputUserNameValue = (TextBox^) outputFrame->FindName("InputUserNameValue");
	PasswordBox^ InputPasswordValue = (PasswordBox^) outputFrame->FindName("InputPasswordValue");
	ComboBox^ SelectUser = (ComboBox^) inputFrame->FindName("SelectUser");
		
	try
    {
		if (SelectUser->SelectedIndex > 1)
		{
			InputUserNameValue->Text = SelectUser->SelectedItem->ToString();
			Windows::Security::Credentials::PasswordVault^ vault = ref new Windows::Security::Credentials::PasswordVault();
			PasswordCredential^ cred = vault->Retrieve("Scenario 2", (String^)SelectUser->SelectedItem);
			if(cred->Password != "")
			{
				InputPasswordValue->Password = cred->Password;
			}
		}
	}
	catch (Platform::COMException^ Error) // No stored credentials, so none to delete
	{
		DebugPrint2(rootPage, Error->ToString());
    }
}
