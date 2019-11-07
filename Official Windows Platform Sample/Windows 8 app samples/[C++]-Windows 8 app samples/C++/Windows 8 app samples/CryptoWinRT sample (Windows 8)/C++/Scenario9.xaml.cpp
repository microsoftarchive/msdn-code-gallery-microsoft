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
// Scenario9.xaml.cpp
// Implementation of the Scenario9 class
//

#include "pch.h"
#include "Scenario9.xaml.h"

using namespace SDKSample::CryptoWinRT;

using namespace concurrency;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Security::Cryptography;
using namespace Windows::Security::Cryptography::Core;
using namespace Windows::Security::Cryptography::DataProtection;
using namespace Windows::Storage::Streams;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario9::Scenario9()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Scenario9::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void SDKSample::CryptoWinRT::Scenario9::RunSample_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Scenario9Text->Text = "";
    String^ descriptor = tbDescriptor->Text;

    Scenario9Text->Text += "*** Sample Data Protection for " + descriptor + " ***\n";

    DataProtectionProvider^ Provider = ref new DataProtectionProvider(descriptor);
    Scenario9Text->Text += "    DataProtectionProvider is Ready\n";

    // Create random data for protection
    IBuffer^ data = CryptographicBuffer::GenerateRandom(73);
    Scenario9Text->Text += "    Original Data: " + CryptographicBuffer::EncodeToHexString(data) + "\n";

    // Protect the random data
    create_task(Provider->ProtectAsync(data)).then([=](task<IBuffer ^> protectedDataTask)
    {
        IBuffer ^ protectedData;
        Scenario9Text->Text += "    ProtectAsync completed\n";
        protectedData = protectedDataTask.get();
        Scenario9Text->Text += "    Protected Data: " + CryptographicBuffer::EncodeToHexString(protectedData) + "\n";

        if(protectedData==nullptr)
        {
            Scenario9Text->Text += "    ProtectedData is a nullptr\n";
            return;
        }
        else
        {
            DataProtectionProvider ^ testProvider2 = ref new DataProtectionProvider();
            Scenario9Text->Text += "    Starting UnprotectAsync()\n";

            create_task(testProvider2->UnprotectAsync(protectedData)).then([=](task<IBuffer^> unprotectTask)
            {
                IBuffer ^ data2 = unprotectTask.get();

                Scenario9Text->Text += "    Data protected and unprotected\n";

                if(data2==nullptr)
                {
                    Scenario9Text->Text += "    UnprotectAsync failed!\n";
                    return;
                }
                else if(!CryptographicBuffer::Compare(data, data2))
                {
                    Scenario9Text->Text += "    UnprotectAsync returned invalid data\n";
                    return;
                }
                Scenario9Text->Text += "    UnprotectAsync completed successfully\n";
                Scenario9Text->Text += "    Unprotected Data: " + CryptographicBuffer::EncodeToHexString(data2) + "\n";
            });
        }
    });
}
