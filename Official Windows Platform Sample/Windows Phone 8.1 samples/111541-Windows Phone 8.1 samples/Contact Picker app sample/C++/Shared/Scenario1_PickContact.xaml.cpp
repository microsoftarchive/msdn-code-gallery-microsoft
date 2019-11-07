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
// Scenario1_PickContact.xaml.cpp
// Implementation of the Scenario1_PickContact class
//

#include "pch.h"
#include "Scenario1_PickContact.xaml.h"

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
/// Initializes a new instance of the Scenario1_PickContact class. 
/// </summary>
Scenario1_PickContact::Scenario1_PickContact()
{
    InitializeComponent();
	PickAContactButton->Click += ref new RoutedEventHandler(this, &Scenario1_PickContact::PickAContactButton_Click);
}

/// <summary>
/// Pick a single contact. 
/// </summary>
/// <param name="sender">Click sender</param>
/// <param name="e">Routed event args</param>
void Scenario1_PickContact::PickAContactButton_Click(Object^ sender, RoutedEventArgs^ e)
{
    auto contactPicker = ref new Contacts::ContactPicker();
	contactPicker->DesiredFieldsWithContactFieldType->Append(ContactFieldType::Email);

    create_task(contactPicker->PickContactAsync()).then([this](Contacts::Contact^ contact)
    {
        if (contact != nullptr)
        {
            OutputFields->Visibility = Windows::UI::Xaml::Visibility::Visible;
            OutputEmpty->Visibility = Windows::UI::Xaml::Visibility::Collapsed;

            OutputName->Text = contact->DisplayName;
            
            String^ output = "";
            // Append emails
            if (contact->Emails->Size > 0)
            {
				for (ContactEmail^ email : contact->Emails)
                {
					output += "Email Address: " + email->Address + " (" + email->Kind.ToString() + ")\n";
                }

                OutputEmailHeader->Visibility = Windows::UI::Xaml::Visibility::Visible;
                OutputEmails->Visibility = Windows::UI::Xaml::Visibility::Visible;
                OutputEmails->Text = output;
            }
            else
            {
                OutputEmailHeader->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
                OutputEmails->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
            }
           
            // Append phones
            output = "";
            if (contact->Phones->Size > 0)
            {
				for (ContactPhone^ phone : contact->Phones)
                {
					output += "Phone: " + phone->Number + " (" + phone->Kind.ToString() + ")\n";
                }

                OutputPhoneNumberHeader->Visibility = Windows::UI::Xaml::Visibility::Visible;
                OutputPhoneNumbers->Visibility = Windows::UI::Xaml::Visibility::Visible;
                OutputPhoneNumbers->Text = output;
            }
            else
            {
                OutputPhoneNumberHeader->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
                OutputPhoneNumbers->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
            }

            if (contact->Thumbnail != nullptr)
            {
                create_task(contact->Thumbnail->OpenReadAsync()).then([this](IRandomAccessStreamWithContentType^ stream)
                {
                    if (stream != nullptr && stream->Size > 0)
                    {
                        BitmapImage^ bitmap = ref new BitmapImage();
                        bitmap->SetSource(stream);
                        OutputThumbnail->Source = bitmap;
                    }
                    else
                    {
                        OutputThumbnail->Source = nullptr;
                    }
                });
            }
        }
        else
        {
            OutputEmpty->Visibility = Windows::UI::Xaml::Visibility::Visible;
            OutputFields->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
            OutputThumbnail->Source = nullptr;
        }
    });
}