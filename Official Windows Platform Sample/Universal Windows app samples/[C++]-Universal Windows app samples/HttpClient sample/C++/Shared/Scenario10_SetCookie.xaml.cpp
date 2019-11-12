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
// Scenario10.xaml.cpp
// Implementation of the Scenario10 class
//

#include "pch.h"
#include "Scenario10_SetCookie.xaml.h"

using namespace SDKSample::HttpClientSample;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Web::Http;
using namespace Windows::Web::Http::Filters;

Scenario10::Scenario10()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Scenario10::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void Scenario10::SetCookie_Click(Object^ sender, RoutedEventArgs^ e)
{
    try
    {
        HttpCookie^ cookie = ref new HttpCookie(NameField->Text, DomainField->Text, PathField->Text);
        cookie->Value = ValueField->Text;

        if (NullCheckBox->IsChecked->Value)
        {
            cookie->Expires = nullptr;
        }
        else
        {
            DateTime expires = ExpiresDatePicker->Date;
            expires.UniversalTime += ExpiresTimePicker->Time.Duration;
            cookie->Expires = expires;
        }

        cookie->Secure = SecureToggle->IsOn;
        cookie->HttpOnly = HttpOnlyToggle->IsOn;

        HttpBaseProtocolFilter^ filter = ref new HttpBaseProtocolFilter();
        bool replaced = filter->CookieManager->SetCookie(cookie, false);

        if (replaced)
        {
            rootPage->NotifyUser("Cookie replaced.", NotifyType::StatusMessage);
        }
        else
        {
            rootPage->NotifyUser("Cookie set.", NotifyType::StatusMessage);
        }
    }
    catch (InvalidArgumentException^ ex)
    {
        rootPage->NotifyUser(ex->Message, NotifyType::StatusMessage);
    }
    catch (Exception^ ex)
    {
        rootPage->NotifyUser("Error: " + ex->Message, NotifyType::ErrorMessage);
    }
}
