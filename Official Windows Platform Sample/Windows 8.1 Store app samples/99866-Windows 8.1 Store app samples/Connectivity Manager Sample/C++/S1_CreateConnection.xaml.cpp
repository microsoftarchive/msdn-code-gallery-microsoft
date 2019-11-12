//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// S1_CreateConnection.xaml.cpp
// Implementation of the S1_CreateConnection class
//

#include "pch.h"
#include "S1_CreateConnection.xaml.h"
#include "MainPage.xaml.h"
#include "Constants.h"

using namespace SDKSample;
using namespace SDKSample::ConnectivityManager;
using namespace Windows::Networking::Connectivity;
using namespace Windows::UI;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::System;
using namespace Platform;
using namespace concurrency;
using namespace Windows::Foundation;

/// <summary>
/// Class constructor
/// </summary>

S1_CreateConnection::S1_CreateConnection()
{
    InitializeComponent();
    // start handling network state change
    Register_NetworkStateChange();
    // keeps track of the current connection
    MainPage::Current->connectionSession = nullptr;
}


/// <summary>
/// register for network status change to handle connectivity changes
/// </summary>
void S1_CreateConnection::Register_NetworkStateChange()
{
    //create handler for network status changed event
    networkStatusCallback = ref new NetworkStatusChangedEventHandler(this, 
                                                                     &S1_CreateConnection::OnNetworkStatusChange, 
                                                                     CallbackContext::Same);
    // register for network state change
    cookie = NetworkInformation::NetworkStatusChanged += networkStatusCallback;

}

/// <summary>
/// Event handler for Network Status Change event
/// </summary>
void S1_CreateConnection::OnNetworkStatusChange(Object^ sender)
{
    // we only care about on demand connection
    if (nullptr != MainPage::Current->connectionSession)
    {
        ConnectionProfile^ p = MainPage::Current->connectionSession->ConnectionProfile;
        NetworkConnectivityLevel connectivity = p->GetNetworkConnectivityLevel();

        DisplayInfo("Connected to " + p->ProfileName + "\n" + "Connectivity Level: " + connectivity.ToString());
    }

    return;
}

/// <summary>
/// Unregister Network Status Change notifications
/// </summary>
void S1_CreateConnection::UnRegisterForNetworkStatusChangeNotif()
{
    try
    {
        NetworkInformation::NetworkStatusChanged -= cookie;
    }
    catch (Exception^ ex)
    {		
        DisplayError(ex->ToString());
    }
}

/// <summary>
/// Create a new on demand connection
/// </summary>
void S1_CreateConnection::Connect_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    String^ status = ConnectButton->Content->ToString();
    bool	fConnecting = false;
    if (status == "Connect")
    {
        fConnecting = true;
        ConnectButton->Content = "Disconnect";
    }
    else
    {
        fConnecting = false;
        ConnectButton->Content = "Connect";
    }
    if (fConnecting == true)
    {
        CellularApnContext^ context = PopulateApnContextFromUI();
        IAsyncOperation<ConnectionSession^>^ op = Windows::Networking::Connectivity::ConnectivityManager::AcquireConnectionAsync(context);
        auto acquireConnectionTask = create_task(op);

        acquireConnectionTask.then([this](task<ConnectionSession^> tsession)
        {
            try
            {
                auto session = tsession.get();

                // store the current Connection in the Global Data
                MainPage::Current->connectionSession = session;

                DisplayInfo("Connected to " + session->ConnectionProfile->ProfileName + "\n" +
                    "Connectivity Level: " + session->ConnectionProfile->GetNetworkConnectivityLevel().ToString());
            }
            catch(Exception^ e)
            {
                DisplayError(e->ToString());
                ConnectButton->Content = "Connect";
            }

        });

    }
    else
    {	// DISCONNECTING
        if (MainPage::Current->connectionSession != nullptr)
        {
            delete MainPage::Current->connectionSession;
            MainPage::Current->connectionSession = nullptr;
            DisplayInfo("Disconnected");
        }
    }
}

/// <summary>
/// Fill in and return an Windows::Networking::Connectivity::CellularApnContext
//	Based on user input
/// </summary>
CellularApnContext^ S1_CreateConnection::PopulateApnContextFromUI()
{
    CellularApnContext^ context = ref new CellularApnContext();
    context->ProviderId = ProviderId->Text;
    context->AccessPointName = AccessPointName->Text;
    context->UserName = UserName->Text;
    context->Password = Password->Text;
    context->AuthenticationType = GetAuthenticationTypeFromString(((ComboBoxItem^) (Authentication->SelectedItem))->Content->ToString());
    context->IsCompressionEnabled = (((ComboBoxItem^) (Compression->SelectedItem))->Content->ToString() == "Yes") ? true : false;
    return context;
}

CellularApnAuthenticationType S1_CreateConnection::GetAuthenticationTypeFromString(String^ authType)
{
    CellularApnAuthenticationType ret = CellularApnAuthenticationType::None;
    if (authType == "Pap")
    {
        ret = CellularApnAuthenticationType::Pap;
    }
    else if (authType == "Chap")
    {
        ret = CellularApnAuthenticationType::Chap; 
    }
    else if (authType == "Mschapv2")
    {
        ret = CellularApnAuthenticationType::Mschapv2;
    }	
    return ret;
}


void S1_CreateConnection::DisplayError(String^ message)
{
    OutputText->Foreground = ref new SolidColorBrush(Colors::Red);
    OutputText->Text = message;
}

void S1_CreateConnection::DisplayWarning(String^ message)
{
    OutputText->Foreground = ref new SolidColorBrush(Colors::Yellow);
    OutputText->Text = message;
}

void S1_CreateConnection::DisplayInfo(String^ message)
{
    OutputText->Foreground = ref new SolidColorBrush(Colors::Green);
    OutputText->Text = message;
}