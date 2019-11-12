// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// Scenario2Input.xaml.cpp
// Implementation of the Scenario2Input class.
//

#include "pch.h"
#include "ScenarioInput2.xaml.h"
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

ScenarioInput2::ScenarioInput2()
{
    InitializeComponent();
}

ScenarioInput2::~ScenarioInput2()
{
}

void ScenarioInput2::SetError(Platform::String^ errorText)
{
    this->rootPage->NotifyUser(errorText,NotifyType::ErrorMessage);
}

void ScenarioInput2::SetResult(CredentialPickerResults^ result)
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


#pragma region Template-Related Code - Do not remove
void ScenarioInput2::OnNavigatedTo(NavigationEventArgs^ e)
{
    // Get a pointer to our main page.
    rootPage = dynamic_cast<MainPage^>(e->Parameter);

    // We want to be notified with the OutputFrame is loaded so we can get to the content.
    _frameLoadedToken = rootPage->OutputFrameLoaded += ref new Windows::Foundation::EventHandler<Platform::Object^>(this, &ScenarioInput2::rootPage_OutputFrameLoaded);
}

void ScenarioInput2::OnNavigatedFrom(NavigationEventArgs^ e)
{
    rootPage->OutputFrameLoaded -= _frameLoadedToken;
}

#pragma endregion

#pragma region Use this code if you need access to elements in the output frame - otherwise delete
void ScenarioInput2::rootPage_OutputFrameLoaded(Object^ sender, Object^ e)
{
    // Get a pointer to the content within the OutputFrame.
    Page^ outputFrame = dynamic_cast<Page^>(rootPage->OutputFrame->Content);

    // Go find the elements that we need for this scenario
    // ex: flipView1 = dynamic_cast<FlipView^>(outputFrame->FindName("FlipView1"));
}

#pragma endregion

#pragma region Sample click handlers - modify if you need them, otherwise delete
void ScenarioInput2::Launch_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	if ((Message->Text != nullptr) && (Caption->Text != nullptr) && (Target->Text != nullptr))
	{
		create_task(CredentialPicker::PickAsync(Message->Text, Caption->Text, Target->Text))
			.then([this](CredentialPickerResults^ credPickerResult)
		{
			SetResult(credPickerResult);
		});
	}

}

#pragma endregion
