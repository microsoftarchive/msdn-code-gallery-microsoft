//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario1.xaml.cpp
// Implementation of the Scenario1 class
//

#include "pch.h"
#include "Scenario1.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::PasswordVaultCPP;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Security::Credentials;
using namespace concurrency;
using namespace Windows::System;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario1::Scenario1()
{
    InitializeComponent();
	// Initialize the password vault, this may take less than a second
    // An optimistic initialization at this stage improves the UI performance
    // when any other call to the password vault may be made later on
    auto task = InitializeVaultAsync();
}

/*
void Scenario1::OnNavigatedFrom(NavigationEventArgs^ e)
{
	
}
*/
Windows::Foundation::IAsyncAction^ Scenario1::InitializeVaultAsync()
{
    // any call to the password vault will load the vault
    return create_async([](){
        Windows::Security::Credentials::PasswordVault^ vault = ref new Windows::Security::Credentials::PasswordVault();
        vault->RetrieveAll();       
    });
}

void PasswordVaultCPP::Scenario1::Save_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	try{
		auto resource = InputResourceValue->Text;
		auto username = InputUserNameValue->Text;
		auto password = InputPasswordValue->Password;

		if(!resource||!username||!password)
		{
			DebugPrint("Resource, User name and password are not allowed to be empty, Please input resource, user name and password" );
			return;
		}

		else 
		{
			//
			//Adding a credential to PasswordVault.
			//
			Windows::Security::Credentials::PasswordVault^ vault = ref new Windows::Security::Credentials::PasswordVault();
            PasswordCredential^ c = ref new PasswordCredential(resource, username, password);
			vault->Add(c);
			DebugPrint("Credential saved successfully. Resource: " + resource + " Username: " + username + " Password:" + password +".");
			return;
		}
	}
	catch(Platform::COMException^ e)
	{
		DebugPrint(e->Message);
		return;
	}	
}

void PasswordVaultCPP::Scenario1::Read_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	try{
		auto resource = InputResourceValue->Text;
		auto username = InputUserNameValue->Text;
		auto password = InputPasswordValue->Password;
		Windows::Foundation::Collections::IVectorView<PasswordCredential^>^ creds;
		PasswordCredential^ cred;		

		//
		//Reading a credential from PasswordVault.
		//
		Windows::Security::Credentials::PasswordVault^ vault = ref new Windows::Security::Credentials::PasswordVault();
			
		//
		//If both resource and username are empty, you can use RetrieveAll() to enumerate all credentials
		//
		if (!resource&&!username)
		{
			DebugPrint("Retrieving all credentials since resource or username are not specified.");
			creds = vault->RetrieveAll();
		}
		//
		//If there is only resouce, you can use FindAllByResource() to enumerate by resource.
		//
		else if(!username)
		{
			DebugPrint("Retrieving credentials in PasswordVault by resource: " + resource);
			creds = vault->FindAllByResource(resource);		
		}
		//
		//If there is only username, you can use findbyusername() to enumerate by resource.
		//
		else if(!resource)
		{	
			DebugPrint("Retrieving credentials in PasswordVault by username: " + username);		
			creds = vault->FindAllByUserName(username);
		}
		//
		//If both resource and username are provided, you can use Retrieve to search the credential
		//
		else
		{
			DebugPrint("Retrieving credentials in PasswordVault by resource and username: " + resource +"/" + username);
			cred = vault->Retrieve(resource,username);		
		}

		//
		//print out search result in debug output
		//
		if(cred)
		{
			DebugPrint("Read credential successfully. Resource: " + cred->Resource + ", Username: " + cred->UserName + " Password: " + cred->Password +".");	

		}
		else if (creds->Size>0)
		{
			DebugPrint("There are(is) "+ creds->Size  + " credential(s) found in PasswordVault");

			for each (PasswordCredential^ c in creds)
			{
				DebugPrint("Read credential successfully. Resource:" + c->Resource + ", Username: " + c->UserName + ".");
			}
		}   
		else
			DebugPrint("Credential not found.");
		return;		
	}
	catch(Platform::COMException^ e)
	{
		if (e->HResult==0x80070490)
			DebugPrint("Credential not found.");
		else
			DebugPrint(e->Message);
		return;
	}	
}

void PasswordVaultCPP::Scenario1::Delete_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	try{
		auto resource = InputResourceValue->Text;
		auto username = InputUserNameValue->Text;
		PasswordCredential^ cred;		
		//
		//Deleting a credential from PasswordVault.
		//
		Windows::Security::Credentials::PasswordVault^ vault = ref new Windows::Security::Credentials::PasswordVault();			
		//
		//If both resource and username are empty, you can use RetrieveAll() to enumerate all credentials
		//
		if (!resource||!username)
		{
			DebugPrint("Please provide both resource and username to delete a credential");
		}
		else
		{
			cred = vault->Retrieve(resource,username);
			vault->Remove(cred);
			DebugPrint("Credential removed successfully. Resource: " + resource + " Username: " + username + ".");
		}
		return;		
	}
	catch(Platform::COMException^ e)
	{
		if (e->HResult==0x80070490)
			DebugPrint("Credential not found.");
		else
			DebugPrint(e->Message);
		return;
	}	
}

void PasswordVaultCPP::Scenario1::Reset_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
		try{
		auto resource = InputResourceValue->Text;
		auto username = InputUserNameValue->Text;
		Windows::Foundation::Collections::IVectorView<PasswordCredential^>^ creds;
		//
		//Enumerate and delete all credentials from PasswordVault.
		//
		Windows::Security::Credentials::PasswordVault^ vault = ref new Windows::Security::Credentials::PasswordVault();			
		creds = vault->RetrieveAll();
		if(creds)
		{
			if(creds->Size>0)
			{
				for each (PasswordCredential^ c in creds)
				{
					vault->Remove(c);
				}
			}
			DebugPrint("Removing "+ creds->Size + "credentials from PasswordVault in total");
		}
		//
		//resetting all input field
		//
		InputResourceValue->Text = "";
		InputUserNameValue->Text = "";
		InputPasswordValue->Password = "";
		ErrorMessage->Text = "";
		DebugPrint("Scenario 1 is reset");
		return;		
	}
	catch(Platform::COMException^ e)
	{
		DebugPrint(e->Message);
		return;
	}	
}


void PasswordVaultCPP::Scenario1::DebugPrint(Platform::String^ Message)
{
	ErrorMessage->Text = Message  + "\n" +  ErrorMessage->Text;
}

