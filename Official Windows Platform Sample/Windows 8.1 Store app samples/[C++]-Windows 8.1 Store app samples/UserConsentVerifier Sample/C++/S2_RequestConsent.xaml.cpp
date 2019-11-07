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
#include "S2_RequestConsent.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::UserConsentVerifierCPP;

using namespace concurrency; 
using namespace Platform;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::Security::Credentials::UI;

Scenario2::Scenario2()
{
    InitializeComponent();
}

// Requests fingerprint consent from the current user.
void Scenario2::RequestConsent_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	Button^ requestConsentButton = dynamic_cast<Button^>(sender);
    requestConsentButton->IsEnabled = false;

	// Read the message that has to be displayed in the consent request prompt
	TextBox^ promptText = (TextBox^)Input->FindName("Message");
	if(promptText->Text != nullptr)
	{
		try
		{
			// Request the currently logged on user's consent via fingerprint swipe
			create_task(UserConsentVerifier::RequestVerificationAsync(promptText->Text))
				.then([requestConsentButton](UserConsentVerificationResult consentResult) 
			{ 
				switch (consentResult) 
				{ 
					case UserConsentVerificationResult::Verified:
					{
						MainPage::Current->NotifyUser("User's presence verified.", NotifyType::StatusMessage);
						break; 
					}

					case UserConsentVerificationResult::DeviceBusy:
					{
						MainPage::Current->NotifyUser("Biometric device is busy.", NotifyType::ErrorMessage);
						break;
					}

					case UserConsentVerificationResult::DeviceNotPresent: 
					{
						MainPage::Current->NotifyUser("No biometric device found.", NotifyType::ErrorMessage);
						break;
					}

					case UserConsentVerificationResult::DisabledByPolicy:
					{
						MainPage::Current->NotifyUser("Biometrics is disabled by policy.", NotifyType::ErrorMessage);
						break;
					}

					case UserConsentVerificationResult::NotConfiguredForUser:
					{
						MainPage::Current->NotifyUser("User has no fingeprints registered.", NotifyType::ErrorMessage);
						break;
					}

					case UserConsentVerificationResult::RetriesExhausted:
					{
						MainPage::Current->NotifyUser("Too many failed attempts.", NotifyType::ErrorMessage);
						break;
					}

					case UserConsentVerificationResult::Canceled:
					{
						MainPage::Current->NotifyUser("Consent request prompt was canceled.", NotifyType::ErrorMessage);
						break;
					}

					default:
					{
						MainPage::Current->NotifyUser("Consent verification with fingerprints is currently unavailable.", NotifyType::ErrorMessage);
						break;
					}
				}
				requestConsentButton->IsEnabled = true;
			});
		}
		catch(Exception ^ex)
        {
			MainPage::Current->NotifyUser("Request current user's consent failed with exception. Operation: RequestVerificationAsync, Exception: " + ex->ToString() , NotifyType::ErrorMessage);
			requestConsentButton->IsEnabled = true;
        }
	}
	else
	{
		MainPage::Current->NotifyUser("Empty Message String. Enter prompt string in the Message text field.", NotifyType::ErrorMessage);
		requestConsentButton->IsEnabled = true;
	}
}
