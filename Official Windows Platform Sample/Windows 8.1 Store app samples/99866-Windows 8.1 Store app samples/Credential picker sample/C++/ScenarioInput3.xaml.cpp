// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// Scenario3Input.xaml.cpp
// Implementation of the Scenario3Input class.
//

#include "pch.h"
#include "ScenarioInput3.xaml.h"
#include "MainPage.xaml.h"

using namespace CredentialPickerCPP;

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
using namespace Windows::Security::Credentials::UI;
using namespace concurrency;
using namespace Windows::UI::Core;

ScenarioInput3::ScenarioInput3()
{
    InitializeComponent();
}

ScenarioInput3::~ScenarioInput3()
{
}

#pragma region Template-Related Code - Do not remove
void ScenarioInput3::OnNavigatedTo(NavigationEventArgs^ e)
{
    // Get a pointer to our main page
    rootPage = dynamic_cast<MainPage^>(e->Parameter);

    // We want to be notified with the OutputFrame is loaded so we can get to the content.
    _frameLoadedToken = rootPage->OutputFrameLoaded += ref new Windows::Foundation::EventHandler<Platform::Object^>(this, &ScenarioInput3::rootPage_OutputFrameLoaded);
}

void ScenarioInput3::OnNavigatedFrom(NavigationEventArgs^ e)
{
    rootPage->OutputFrameLoaded -= _frameLoadedToken;
}

#pragma endregion

void ScenarioInput3::SetError(Platform::String^ errorText)
{
    this->rootPage->NotifyUser(errorText,NotifyType::ErrorMessage);
}

void ScenarioInput3::SetResult(CredentialPickerResults^ result)
{
    auto domainName = result->CredentialDomainName;
	auto userName = result->CredentialUserName;
	auto password = result->CredentialPassword;
	auto savedByApi = result->CredentialSaved;
	auto saveOption = result->CredentialSaveOption;
	Page^ outputFrame = (Page^)this->rootPage->OutputFrame->Content;
	TextBox^ status = (TextBox^)outputFrame->FindName("Status");
	status->Text = "OK";
	TextBox^ domain = (TextBox^)outputFrame->FindName("Domain");
	domain->Text = domainName;
	TextBox^ username = (TextBox^)outputFrame->FindName("Username");
	username->Text = userName;
	TextBox^ passwordBox = (TextBox^)outputFrame->FindName("Password");
	passwordBox->Text = password;
	TextBox^ credsaved = (TextBox^)outputFrame->FindName("CredentialSaved");
	credsaved->Text = (savedByApi ? "true" : "false");
	TextBox^ checkboxState = (TextBox^)outputFrame->FindName("CheckboxState");
	switch (result->CredentialSaveOption)
	{
		case Windows::Security::Credentials::UI::CredentialSaveOption::Hidden:
			checkboxState->Text = "Hidden";
			break;
		case Windows::Security::Credentials::UI::CredentialSaveOption::Selected:
			checkboxState->Text = "Selected";
			break;
		case Windows::Security::Credentials::UI::CredentialSaveOption::Unselected:
			checkboxState->Text = "Unselected";
			break;
	}       
    
}

void ScenarioInput3::SetPasswordExplainVisibility(bool isShown)
{
    Page^ outputFrame = (Page^)this->rootPage->OutputFrame->Content;
    TextBlock^ text1 = (TextBlock^)outputFrame->FindName("PasswordExplain1");
    TextBlock^ text2 = (TextBlock^)outputFrame->FindName("PasswordExplain2");
    if (isShown)
    {
        text1->Visibility = Windows::UI::Xaml::Visibility::Visible;
        text2->Visibility = Windows::UI::Xaml::Visibility::Visible;
    }
    else
    {
        text1->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
        text2->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
    }
}

        

#pragma region Use this code if you need access to elements in the output frame - otherwise delete
void ScenarioInput3::rootPage_OutputFrameLoaded(Object^ sender, Object^ e)
{
    // Get a pointer to the content within the OutputFrame.
    Page^ outputFrame = dynamic_cast<Page^>(rootPage->OutputFrame->Content);

    // Go find the elements that we need for this scenario
    // ex: flipView1 = dynamic_cast<FlipView^>(outputFrame->FindName("FlipView1"));
}

#pragma endregion

#pragma region Sample click handlers - modify if you need them, otherwise delete
void ScenarioInput3::Launch_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    if ((Message->Text == nullptr) || (Caption->Text == nullptr) || (Target->Text == nullptr))
    {
        return;
    }

    CredentialPickerOptions^ credPickerOptions = ref new CredentialPickerOptions();
    credPickerOptions->AlwaysDisplayDialog = AlwaysShowDialog->IsChecked->Value;
    credPickerOptions->Message = Message->Text;
    credPickerOptions->Caption = Caption->Text;
    credPickerOptions->TargetName = Target->Text;
    if (Protocol->SelectedItem == nullptr)
    {
        credPickerOptions->AuthenticationProtocol = Windows::Security::Credentials::UI::AuthenticationProtocol::Negotiate;
    }
    else
    {
        String^ protocolName = ((ComboBoxItem^)Protocol->SelectedItem)->Content->ToString();
        if (protocolName->Equals("Negotiate"))
        {
            credPickerOptions->AuthenticationProtocol = Windows::Security::Credentials::UI::AuthenticationProtocol::Negotiate;
        }
        else if (protocolName->Equals("NTLM"))
        {
            credPickerOptions->AuthenticationProtocol = Windows::Security::Credentials::UI::AuthenticationProtocol::Ntlm;
        }
        else if (protocolName->Equals("Kerberos"))
        {
            credPickerOptions->AuthenticationProtocol = Windows::Security::Credentials::UI::AuthenticationProtocol::Kerberos;
        }
        else if (protocolName->Equals("CredSsp"))
        {
            credPickerOptions->AuthenticationProtocol = Windows::Security::Credentials::UI::AuthenticationProtocol::CredSsp;
        }
        else if (protocolName->Equals("Basic"))
        {
            credPickerOptions->AuthenticationProtocol = Windows::Security::Credentials::UI::AuthenticationProtocol::Basic;
        }
        else if (protocolName->Equals("Digest"))
        {
            credPickerOptions->AuthenticationProtocol = Windows::Security::Credentials::UI::AuthenticationProtocol::Digest;
        }
        else if (protocolName->Equals("Custom"))
        {
            credPickerOptions->AuthenticationProtocol = Windows::Security::Credentials::UI::AuthenticationProtocol::Custom;
            credPickerOptions->CustomAuthenticationProtocol = CustomProtocol->Text;
        }
        else
        {
            rootPage->NotifyUser("Unknown Protocol",NotifyType::ErrorMessage);
        }
    }
    if (CheckboxState->SelectedItem != nullptr)
    {
        String^ checkboxState = ((ComboBoxItem^)CheckboxState->SelectedItem)->Content->ToString();
        if (checkboxState->Equals("Hidden"))
        {
            credPickerOptions->CredentialSaveOption = Windows::Security::Credentials::UI::CredentialSaveOption::Hidden;
        }
        else if (checkboxState->Equals("Selected"))
        {
            credPickerOptions->CredentialSaveOption = Windows::Security::Credentials::UI::CredentialSaveOption::Selected;
        }
        else if (checkboxState->Equals("Unselected"))
        {
            credPickerOptions->CredentialSaveOption = Windows::Security::Credentials::UI::CredentialSaveOption::Unselected;
        }
        else
        {
            rootPage->NotifyUser("Unknown Checkbox state",NotifyType::ErrorMessage);
        }
    }
    create_task(CredentialPicker::PickAsync(credPickerOptions))
        .then([this](CredentialPickerResults^ credPickerResult)
    {
        SetResult(credPickerResult);
    });
	

}

void ScenarioInput3::Protocol_SelectionChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::SelectionChangedEventArgs^ e)
{
    ComboBox^ box = (ComboBox^)sender;
    if (Protocol == nullptr || Protocol->SelectedItem == nullptr)
    {
        //return if this was triggered before all components are initialized
        return;
    }
            
    String^ selectedProtocol = ((ComboBoxItem^)Protocol->SelectedItem)->Content->ToString();
            
    if (selectedProtocol->Equals("Custom"))
    {
        CustomProtcolStackPanel->Visibility = Windows::UI::Xaml::Visibility::Visible;
    }
    else
    {
        CustomProtcolStackPanel->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
        //Basic and Digest return plaintext passwords
        if (selectedProtocol->Equals("Basic"))
        {
            SetPasswordExplainVisibility(false);
        }
        else if (selectedProtocol->Equals("Digest"))
        {
            SetPasswordExplainVisibility(false);
        }
        else
        {
            SetPasswordExplainVisibility(true);
        }
    }
}

#pragma endregion

