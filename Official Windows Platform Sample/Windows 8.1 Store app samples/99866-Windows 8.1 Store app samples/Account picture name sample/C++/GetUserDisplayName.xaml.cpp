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
// GetUserDisplayName.xaml.cpp
// Implementation of the GetUserDisplayName class
//

#include "pch.h"
#include "GetUserDisplayName.xaml.h"

using namespace AccountPictureName;
using namespace concurrency;
using namespace Platform;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::System::UserProfile;
using namespace Windows::Foundation;
using namespace Microsoft::WRL;
using namespace Microsoft::WRL::Wrappers;

GetUserDisplayName::GetUserDisplayName()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void GetUserDisplayName::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void GetUserDisplayName::GetDisplayNameButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    if (UserInformation::NameAccessAllowed)
    {
        create_task(UserInformation::GetDisplayNameAsync()).then([this](String^ displayName)
        {
            if (displayName != nullptr)
            {
                rootPage->NotifyUser("Display name = " + displayName, NotifyType::StatusMessage);
            }
            else
            {
                rootPage->NotifyUser("No display name returned for current user.", NotifyType::ErrorMessage);
            }
        });
    }
    else
    {
        rootPage->NotifyUser("Access to Name disabled by Privacy Setting or Group Policy", NotifyType::ErrorMessage);
    }
}
