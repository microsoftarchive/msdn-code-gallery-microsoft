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
// Scenario2_PickContacts.xaml.cpp
// Implementation of the Scenario2_PickContacts class
//
#include "pch.h"
#include "Scenario2_PickContacts.xaml.h"

using namespace SDKSample;
using namespace concurrency;
using namespace Platform;
using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::Contacts;
using namespace Windows::Foundation::Collections;
using namespace Windows::Storage::Streams;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Media::Imaging;

/// <summary>
/// Initializes a new instance of the Scenario2_PickContacts class. 
/// </summary>
Scenario2_PickContacts::Scenario2_PickContacts()
{
    InitializeComponent();
	PickContactsButton->Click += ref new RoutedEventHandler(this, &Scenario2_PickContacts::PickContactsButton_Click);
}

/// <summary>
/// Pick multiple contacts. 
/// </summary>
/// <param name="sender">Click sender</param>
/// <param name="e">Routed event args</param>
void Scenario2_PickContacts::PickContactsButton_Click(Object^ sender, RoutedEventArgs^ e)
{
    MainPage^ page = MainPage::Current;

    auto contactPicker = ref new Contacts::ContactPicker();
	contactPicker->DesiredFieldsWithContactFieldType->Append(ContactFieldType::Email);

    create_task(contactPicker->PickContactsAsync()).then([this](IVector<Contacts::Contact^>^ contacts)
    {
		if (contacts != nullptr)
		{
			if (contacts->Size > 0)
			{
				String^ output = "Selected contacts:\n";
				for (Contact^ contact : contacts)
				{
					output += contact->DisplayName + "\n";
				}
				OutputText->Text = output;
			}
			else
			{
				OutputText->Text = "No contacts were selected";
			}
		}
    });
}

