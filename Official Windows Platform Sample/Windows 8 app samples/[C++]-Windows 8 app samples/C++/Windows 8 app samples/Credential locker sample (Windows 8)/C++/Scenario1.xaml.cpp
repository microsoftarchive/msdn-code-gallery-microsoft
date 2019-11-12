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

using namespace SDKSample::PasswordVaultCPP;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Security::Credentials;
using namespace concurrency;

Scenario1::Scenario1()
{
    InitializeComponent();

    // Initialize the password vault, this may take less than a second
    // An optimistic initialization at this stage improves the UI performance
    // when any other call to the password vault may be made later on
    auto task = InitializeVaultAsync();
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

Windows::Foundation::IAsyncAction^ Scenario1::InitializeVaultAsync()
{
    // any call to the password vault will load the vault
    return create_async([](){
        Windows::Security::Credentials::PasswordVault^ vault = ref new Windows::Security::Credentials::PasswordVault();
        vault->RetrieveAll();       
    });
}

void DebugPrint(Page^ rootPage, Platform::String^ Trace)
{
    Page^ outputFrame = (Page^) rootPage;
    TextBox^ ErrorMessage = (TextBox^) outputFrame->FindName("ErrorMessage"); 
    ErrorMessage->Text += Trace + "\r\n";
}

void Reset1(Page^ rootPage)
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
    }
    catch (Platform::Exception^ Error)
    {
        DebugPrint(rootPage, Error->ToString());
    }
}


void SDKSample::PasswordVaultCPP::Scenario1::Launch_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    try
    {
        Page^ outputFrame = (Page^) rootPage->Current;
        Page^ inputFrame = (Page^) rootPage->Current;
        Reset1(rootPage);
        CheckBox^ AuthenticationFailCheck = (CheckBox^) outputFrame->FindName("AuthenticationFailCheck");
        TextBox^ WelcomeMessage = (TextBox^) inputFrame->FindName("WelcomeMessage");
        if (AuthenticationFailCheck->IsChecked->Value == true)
        {
            WelcomeMessage->Text = "Blocked";
        }
        else
        {
            PasswordCredential^ cred = ref new PasswordCredential();
            Windows::Security::Credentials::PasswordVault^ vault = ref new Windows::Security::Credentials::PasswordVault();
            Windows::Foundation::Collections::IVectorView<PasswordCredential^>^ Creds = vault->FindAllByResource("Scenario 1");
            for each (PasswordCredential^ cred in Creds)
            {
                try
                {
                    vault->Remove(cred);
                }
                catch (Platform::COMException^ Error)
                {
                    DebugPrint(rootPage, Error->ToString());
                }
             }
             WelcomeMessage->Text = "Scenario is ready, please sign in";
        }
    }
    catch (Platform::COMException^ Error)
    {
          DebugPrint(rootPage, Error->ToString());        

    }
}



void SDKSample::PasswordVaultCPP::Scenario1::Signin_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Page^ outputFrame = (Page^) rootPage->Current;
    Page^ inputFrame = (Page^) rootPage->Current;
    TextBox^ InputUserNameValue = (TextBox^) outputFrame->FindName("InputUserNameValue");
    PasswordBox^ InputPasswordValue = (PasswordBox^) outputFrame->FindName("InputPasswordValue");
    TextBox^ WelcomeMessage = (TextBox^) inputFrame->FindName("WelcomeMessage");
    CheckBox^ SaveCredCheck = (CheckBox ^) outputFrame->FindName("SaveCredCheck");
    if (InputUserNameValue->Text == "" || InputPasswordValue->Password == "")
    {
        TextBox^ ErrorMessage = (TextBox^) outputFrame->FindName("ErrorMessage");
        ErrorMessage->Text = "User name and password are not allowed to be empty, Please input user name and password";
    }
    else
    {
        try
        {
            Windows::Security::Credentials::PasswordVault^ vault = ref new Windows::Security::Credentials::PasswordVault();
            PasswordCredential^ c = ref new PasswordCredential("Scenario 1", InputUserNameValue->Text, InputPasswordValue->Password);
            if (SaveCredCheck->IsChecked)
            {
                vault->Add(c);
            }
            Reset1(rootPage);
            WelcomeMessage->Text = "Welcome to " + c->Resource + ", " + c->UserName;
        }
        catch (Platform::COMException^ Error)
        {
            DebugPrint(rootPage, Error->ToString());
        }
    }
    Reset1(rootPage);
    CheckBox^ AuthenticationFailCheck = (CheckBox^) outputFrame->FindName("AuthenticationFailCheck");
    AuthenticationFailCheck->IsChecked= false;
}


void SDKSample::PasswordVaultCPP::Scenario1::ChangeUser_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    try
    {
        Windows::Security::Credentials::PasswordVault^ vault = ref new Windows::Security::Credentials::PasswordVault();
        Windows::Foundation::Collections::IVectorView<PasswordCredential^>^ creds = vault->FindAllByResource("Scenario 1");
        for each (PasswordCredential^ c in creds)
        {
            try
            {
                vault->Remove(c);
            }
            catch (Platform::COMException^ Error)
            {
                DebugPrint(rootPage, Error->ToString());
            }
        }
    }
    catch (Platform::COMException^ Error)
    {
        DebugPrint(rootPage, Error->ToString());
    }

    Reset1(rootPage);
    Page^ outputFrame = (Page^) rootPage->Current;
    Page^ inputFrame = (Page^) rootPage->Current;
    CheckBox^ AuthenticationFailCheck = (CheckBox^) outputFrame->FindName("AuthenticationFailCheck");
    AuthenticationFailCheck->IsChecked = false;
    TextBox^ WelcomeMessage = (TextBox^) inputFrame->FindName("WelcomeMessage");
    WelcomeMessage->Text = "User has been changed, please resign in with new credentials, choose save and launch scenario again";
}


void SDKSample::PasswordVaultCPP::Scenario1::Reset_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Reset1(rootPage);
}
