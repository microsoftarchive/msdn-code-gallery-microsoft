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
// ShowConnection.xaml.cpp
// Implementation of the ShowConnection class
//

#include "pch.h"
#include "ShowConnection.xaml.h"

using namespace SDKSample::MobileBroadband;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Networking::NetworkOperators;

ShowConnection::ShowConnection()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void ShowConnection::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;

    PrepareScenario();
}

void SDKSample::MobileBroadband::ShowConnection::ShowConnectionUI_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ b = safe_cast<Button^>(sender);
    if (b != nullptr)
    {
        try
        {
            auto mobileBroadbandAccount = MobileBroadbandAccount::CreateFromNetworkAccountId(deviceAccountId->GetAt(0));
            mobileBroadbandAccount->CurrentNetwork->ShowConnectionUI();
        }
        catch (Platform::Exception^ ex)
        {
            rootPage->NotifyUser("Error:" + ex->Message, NotifyType::ErrorMessage);
        }
    }
}


void SDKSample::MobileBroadband::ShowConnection::PrepareScenario()
{
    rootPage->NotifyUser("", NotifyType::StatusMessage);

    try
    {
        deviceAccountId = MobileBroadbandAccount::AvailableNetworkAccountIds;

        if (deviceAccountId->Size != 0)
        {
            ShowConnectionUI->Content = "Show Connection UI";
            ShowConnectionUI->IsEnabled = true;
        }
        else
        {
            ShowConnectionUI->Content = "No available accounts detected";
            ShowConnectionUI->IsEnabled = false;
        }
    }
    catch (Platform::Exception^ ex)
    {
        rootPage->NotifyUser("Error:" + ex->Message, NotifyType::ErrorMessage);
    }
}