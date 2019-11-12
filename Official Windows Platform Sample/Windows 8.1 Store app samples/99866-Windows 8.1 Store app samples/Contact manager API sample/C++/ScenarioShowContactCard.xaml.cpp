//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#include "pch.h"
#include "ScenarioShowContactCard.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::ContactManager;
using namespace Windows::ApplicationModel::Contacts;

using namespace Windows::Foundation;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;

ScenarioShowContactCard::ScenarioShowContactCard()
{
    InitializeComponent();
}

void SDKSample::ContactManager::ScenarioShowContactCard::ShowContactCardButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    if ((this->EmailAddress->Text->Length() == 0) && (this->PhoneNumber->Text->Length() == 0))
    {
        MainPage::Current->NotifyUser("You must enter an email address and/or phone number of the contact for the system to search and show the contact card.", NotifyType::ErrorMessage);
    }
    else
    {
        Contact^ contact = ref new Contact();
        if (this->EmailAddress->Text->Length() > 0)
        {
            if (this->EmailAddress->Text->Length() <= MAX_EMAIL_ADDRESS_LENGTH)
            {
                ContactEmail^ email = ref new ContactEmail();
                email->Address = this->EmailAddress->Text;
                contact->Emails->Append(email);
            }
            else
            {
                MainPage::Current->NotifyUser("The email address you entered is too long.", NotifyType::ErrorMessage);
                return;
            }
        }

        if (this->PhoneNumber->Text->Length() > 0)
        {
            if (this->PhoneNumber->Text->Length() <= MAX_PHONE_NUMBER_LENGTH)
            {

                ContactPhone^ phone = ref new ContactPhone();
                phone->Number = this->PhoneNumber->Text;
                contact->Phones->Append(phone);
            }
            else
            {
                MainPage::Current->NotifyUser("The phone number you entered is too long.", NotifyType::ErrorMessage);
                return;
            }
        }

        // Get the selection rect of the button pressed to show contact card.
        Rect rect = GetElementRect(safe_cast<FrameworkElement^>(sender));

        Windows::ApplicationModel::Contacts::ContactManager::ShowContactCard(contact, rect, Windows::UI::Popups::Placement::Default);
        MainPage::Current->NotifyUser("ContactManager.ShowContactCard() was called.", NotifyType::StatusMessage);
    }
}
