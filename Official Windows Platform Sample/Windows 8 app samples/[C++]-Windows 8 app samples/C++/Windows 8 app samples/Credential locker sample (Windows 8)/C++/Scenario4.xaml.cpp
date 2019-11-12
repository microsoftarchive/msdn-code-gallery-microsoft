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

using namespace SDKSample::PasswordVaultCPP;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Security::Credentials;
using namespace Platform;
using namespace Platform::Collections;



Scenario4::Scenario4()
{
    InitializeComponent();
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

void DebugPrintSc4(Page^ rootPage, Platform::String^ Trace)
{
	Page^ outputFrame = (Page^) rootPage;
    TextBox^ ErrorMessage = (TextBox^) outputFrame->FindName("ErrorMessage"); 
    ErrorMessage->Text += Trace + "\r\n";
 }

void CleanComboboxSc4(Page^ rootPage)
{
	Page^ inputFrame = (Page^) rootPage;
	ComboBox^ SelectResource = (ComboBox^) inputFrame->FindName("SelectResource");
	ComboBox^ SelectUser = (ComboBox^) inputFrame->FindName("SelectUser");
	try
    {
		SelectUser->SelectedIndex= -1;
		SelectUser->ItemsSource = ref new Vector<String ^>(); 

		SelectResource->SelectedIndex= -1;
		SelectResource->ItemsSource = ref new Vector<String ^>(); 
	}
    catch (Platform::Exception^ Error) 
    {
		DebugPrintSc4(rootPage, Error->ToString());
    }
}

void ResetSc4(Page^ rootPage)
{
	try
    {
		Page^ outputFrame = (Page^) rootPage;
        Page^ inputFrame = (Page^) rootPage;

		TextBox^ InputResourceValue = (TextBox^) outputFrame->FindName("InputResourceValue");
        InputResourceValue->Text = "";
        
        TextBox^ InputUserNameValue = (TextBox^) outputFrame->FindName("InputUserNameValue");
        InputUserNameValue->Text = "";
        PasswordBox^ InputPasswordValue = (PasswordBox^)outputFrame->FindName("InputPasswordValue");
        InputPasswordValue->Password= "";
        TextBox^ ErrorMessage = (TextBox^) outputFrame->FindName("ErrorMessage");
        ErrorMessage->Text = "";
        TextBox^ WelcomeMessage = (TextBox^) inputFrame->FindName("WelcomeMessage");
        CheckBox^ SaveCredCheck = (CheckBox^) outputFrame->FindName("SaveCredCheck");
        SaveCredCheck->IsChecked = false;
		
	}
    catch (Platform::Exception^ Error)
    {
		DebugPrintSc4(rootPage, Error->ToString());
	}
}


void SDKSample::PasswordVaultCPP::Scenario4::Launch_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	try
	{
		Page^ outputFrame = (Page^) rootPage;
        Page^ inputFrame = (Page^) rootPage;
		
		Vector<String^>^ itemSource1 = ref new Vector<String^>();
		Vector<String^>^ itemSource2 = ref new Vector<String^>();
		Vector <Object^>^ l = ref new Vector<Object^>();
		Vector <Object^>^ m = ref new Vector<Object^>();
		
		ResetSc4(rootPage);
		CleanComboboxSc4(rootPage);

		CheckBox^ AuthenticationFailCheck = (CheckBox ^) outputFrame->FindName("AuthenticationFailCheck");
		TextBox^ WelcomeMessage = (TextBox^) inputFrame->FindName("WelcomeMessage");
		ComboBox^ SelectUser = (ComboBox^) inputFrame->FindName("SelectUser");
		ComboBox^ SelectResource = (ComboBox^) inputFrame->FindName("SelectResource");
		
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
				Windows::Foundation::Collections::IVectorView<PasswordCredential^>^ creds = vault->RetrieveAll();
				for each (PasswordCredential^ cred in creds)
				{
					itemSource1->InsertAt(0, cred->UserName);
					itemSource2->InsertAt(0, cred->Resource);
				}
				
			}
			catch (Platform::COMException^ Error) // If there are no stored credentials, no list to populate
			{
				DebugPrintSc4(this->rootPage, Error->ToString());
			}
			itemSource1->InsertAt(0, "Add new user");
			itemSource1->InsertAt(0, "");
			SelectUser->ItemsSource = itemSource1;
			SelectUser->SelectedIndex = 0;


			itemSource2->InsertAt(0, "Add new resource");
			itemSource2->InsertAt(0, "");
			SelectResource->ItemsSource = itemSource2;
			SelectResource->SelectedIndex = 0;

			WelcomeMessage->Text = "Scenario is ready, please sign in";
			
		}

	}
	catch (Platform::COMException^ Error)
	{
		DebugPrintSc4(rootPage, Error->ToString());
	}

}


void SDKSample::PasswordVaultCPP::Scenario4::Signin_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	Page^ outputFrame = (Page^) rootPage;
	Page^ inputFrame = (Page^) rootPage;
	
	TextBox^ InputResourceNameValue = (TextBox^) outputFrame->FindName("InputResourceValue");
	TextBox^ InputUserNameValue = (TextBox^) outputFrame->FindName("InputUserNameValue");
	PasswordBox^ InputPasswordValue = (PasswordBox^) outputFrame->FindName("InputPasswordValue");
	TextBox^ WelcomeMessage = (TextBox^) inputFrame->FindName("WelcomeMessage");
	CheckBox^ SaveCredCheck = (CheckBox^) outputFrame->FindName("SaveCredCheck");

	try
	{
    
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
				PasswordCredential^ c = ref new PasswordCredential(InputResourceNameValue->Text, InputUserNameValue->Text, InputPasswordValue->Password);
				if (SaveCredCheck->IsChecked->Value)
				{
					vault->Add(c);
				}

				WelcomeMessage->Text = "Welcome to " + c->Resource + ", " + c->UserName;
			}
			catch (Platform::COMException^ Error) // No stored credentials, so none to delete
			{
				DebugPrintSc4(rootPage, Error->ToString());
			}
		}
	}
	catch (Exception^ Error) // No stored credentials, so none to delete
	{
		DebugPrintSc4(rootPage, Error->ToString());
	}
	
    ResetSc4(rootPage);

    CheckBox^ AuthenticationFailCheck = (CheckBox ^) outputFrame->FindName("AuthenticationFailCheck");
    AuthenticationFailCheck->IsChecked = false;
}


void SDKSample::PasswordVaultCPP::Scenario4::ChangeUser_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	Page^ outputFrame = (Page^) rootPage;
	Page^ inputFrame = (Page^) rootPage;
	
	try
    {
		Windows::Security::Credentials::PasswordVault^ vault = ref new Windows::Security::Credentials::PasswordVault();
		Windows::Foundation::Collections::IVectorView<PasswordCredential^>^ creds = vault->RetrieveAll();

		for each (PasswordCredential^ c in creds)
        {
			try
            {
				vault->Remove(c);
            }
			catch (Platform::COMException^ Error) // No stored credentials, so none to delete
			{
				DebugPrintSc4(rootPage, Error->ToString());
			}
		}
                
	}
	catch (Exception^ Error) // No stored credentials, so none to delete
	{
			DebugPrintSc4(rootPage, Error->ToString());
    }

	ResetSc4(rootPage);
	CleanComboboxSc4(rootPage);
    CheckBox^ AuthenticationFailCheck = (CheckBox ^) outputFrame->FindName("AuthenticationFailCheck");
	AuthenticationFailCheck->IsChecked = false;
    TextBox^ WelcomeMessage = (TextBox^) inputFrame->FindName("WelcomeMessage");
    WelcomeMessage->Text = "User has been changed, please resign in with new credentials, choose save and launch scenario again";

}


void SDKSample::PasswordVaultCPP::Scenario4::Reset_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	ResetSc4(rootPage);
	CleanComboboxSc4(this->rootPage);
}


void SDKSample::PasswordVaultCPP::Scenario4::SelectResource_Click(Platform::Object^ sender, Windows::UI::Xaml::Controls::SelectionChangedEventArgs^ e)
{
	Page^ outputFrame = (Page^) rootPage;
	Page^ inputFrame = (Page^) rootPage;
		
	TextBox^ InputUserNameValue = (TextBox^) outputFrame->FindName("InputUserNameValue");
	TextBox^ InputResourceValue = (TextBox^) outputFrame->FindName("InputResourceValue");
	PasswordBox^ InputPasswordValue = (PasswordBox^) outputFrame->FindName("InputPasswordValue");
	ComboBox^ SelectUser = (ComboBox^) inputFrame->FindName("SelectUser");
	ComboBox^ SelectResource = (ComboBox^) inputFrame->FindName("SelectResource");
	InputPasswordValue->Password = "";
	try
    {
		if ((SelectUser->SelectedIndex > 1) && SelectResource->SelectedIndex > 1)
		{
			InputUserNameValue->Text = SelectUser->SelectedItem->ToString();
			InputResourceValue->Text = SelectResource->SelectedItem->ToString();
			
			Windows::Security::Credentials::PasswordVault^ vault = ref new Windows::Security::Credentials::PasswordVault();
			PasswordCredential^ cred = vault->Retrieve((String^)SelectResource->SelectedItem, (String^)SelectUser->SelectedItem);
			if(cred->Password != "")
			{
				InputPasswordValue->Password = cred->Password;
			}
		}
	}
	catch (Platform::COMException^ Error) // No stored credentials, so none to delete
	{
		DebugPrintSc4(rootPage, Error->ToString());
    }

}

void SDKSample::PasswordVaultCPP::Scenario4::SelectUser_Click(Platform::Object^ sender, Windows::UI::Xaml::Controls::SelectionChangedEventArgs^ e)
{
	Page^ outputFrame = (Page^) rootPage;
	Page^ inputFrame = (Page^) rootPage;
		
	TextBox^ InputUserNameValue = (TextBox^) outputFrame->FindName("InputUserNameValue");
	PasswordBox^ InputPasswordValue = (PasswordBox^) outputFrame->FindName("InputPasswordValue");
	ComboBox^ SelectUser = (ComboBox^) inputFrame->FindName("SelectUser");
	ComboBox^ SelectResource = (ComboBox^) inputFrame->FindName("SelectResource");

	InputPasswordValue->Password = "";
		
	try
    {
		if ((SelectUser->SelectedIndex > 1) && SelectResource->SelectedIndex > 1)
		{
			InputUserNameValue->Text = SelectUser->SelectedItem->ToString();
			InputResourceValue->Text = SelectResource->SelectedItem->ToString();
			Windows::Security::Credentials::PasswordVault^ vault = ref new Windows::Security::Credentials::PasswordVault();
			PasswordCredential^ cred = vault->Retrieve((String^) SelectResource->SelectedItem, (String^)SelectUser->SelectedItem);
			if(cred->Password != "")
			{
				InputPasswordValue->Password = cred->Password;
			}
		}
	}
	catch (Platform::COMException^ Error) // No stored credentials, so none to delete
	{
		DebugPrintSc4(rootPage, Error->ToString());
    }
}
