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

using namespace SDKSample::PasswordVaultCPP;


using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Security::Credentials;

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


void SDKSample::PasswordVaultCPP::Scenario3::ScenarioButtonDelete_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	try {
		Windows::Security::Credentials::PasswordVault^ v = ref new Windows::Security::Credentials::PasswordVault();
		Windows::Foundation::Collections::IVectorView<PasswordCredential^>^ creds = v->RetrieveAll();
		DeleteSummary->Text = "Number of credentials deleted: " + creds->Size;
		for each (PasswordCredential^ cred in creds)
		{
			v->Remove(cred);
		}
        
		// GetAll is a snapshot in time, so to reflect the updated vault, get all credentials again
        creds = v->RetrieveAll();
        //The credentials should now be empty
	}
	catch (Platform::COMException^ Error)
	{
	}
}
