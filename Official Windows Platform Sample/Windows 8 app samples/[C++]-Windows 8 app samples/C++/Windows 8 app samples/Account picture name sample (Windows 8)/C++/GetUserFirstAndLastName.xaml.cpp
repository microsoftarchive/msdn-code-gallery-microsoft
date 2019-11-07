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
// GetUserFirstAndLastName.xaml.cpp
// Implementation of the GetUserFirstAndLastName class
//

#include "pch.h"
#include "GetUserFirstAndLastName.xaml.h"

using namespace AccountPictureName;
using namespace concurrency;
using namespace Platform;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::System::UserProfile;
using namespace Windows::Foundation;
using namespace Microsoft::WRL;
using namespace Microsoft::WRL::Wrappers;

GetUserFirstAndLastName::GetUserFirstAndLastName()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void GetUserFirstAndLastName::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void GetUserFirstAndLastName::GetFirstNameButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    if (UserInformation::NameAccessAllowed)
    {
        create_task(UserInformation::GetFirstNameAsync()).then([this](String^ firstName)
        {
            if (firstName != nullptr)
            {
                rootPage->NotifyUser("First name = " + firstName, NotifyType::StatusMessage);
            }
            else
            {
                rootPage->NotifyUser("No first name returned for current user.", NotifyType::ErrorMessage);
            }
        });
    }
    else
    {
        rootPage->NotifyUser("Access to Name disabled by Privacy Setting or Group Policy", NotifyType::ErrorMessage);
    }
}

void GetUserFirstAndLastName::GetLastNameButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    if (UserInformation::NameAccessAllowed)
    {
        create_task(UserInformation::GetLastNameAsync()).then([this](String^ lastName)
        {
            if (lastName != nullptr)
            {
                rootPage->NotifyUser("Last name = " + lastName, NotifyType::StatusMessage);
            }
            else
            {
                rootPage->NotifyUser("No last name returned for current user.", NotifyType::ErrorMessage);
            }
        });
    }
    else
    {
        rootPage->NotifyUser("Access to Name disabled by Privacy Setting or Group Policy", NotifyType::ErrorMessage);
    }
}
