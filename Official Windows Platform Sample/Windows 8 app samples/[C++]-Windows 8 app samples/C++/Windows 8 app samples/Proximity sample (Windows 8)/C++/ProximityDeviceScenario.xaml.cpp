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
// ProximityDeviceScenario.xaml.cpp
// Implementation of the ProximityDeviceScenario class
//


#include "pch.h"
#include "ProximityDeviceScenario.xaml.h"

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::Core;
using namespace Windows::UI::ViewManagement;
using namespace Windows::System;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Networking::Proximity;
using namespace Windows::Storage::Streams;
using namespace Platform;
using namespace ProximityCPP;



ProximityDeviceScenario::ProximityDeviceScenario()
{
    InitializeComponent();

    m_proximityDevice = ProximityDevice::GetDefault();
    if (m_proximityDevice != nullptr)
    {
        // there is a proximity device, so set up the scenarios
        m_publishedMessageId = -1;
        m_subscribedMessageId = -1;
        ProximityDevice_PublishMessageButton->Click += ref new RoutedEventHandler(this, &ProximityDeviceScenario::ProximityDevice_PublishMessage);
        ProximityDevice_SubscribeForMessageButton->Click += ref new RoutedEventHandler(this, &ProximityDeviceScenario::ProximityDevice_SubscribeForMessage);
        ProximityDevice_PublishMessageButton->Visibility =  Windows::UI::Xaml::Visibility::Visible;
        ProximityDevice_SubscribeForMessageButton->Visibility =  Windows::UI::Xaml::Visibility::Visible;
        ProximityDevice_PublishMessageText->Visibility =  Windows::UI::Xaml::Visibility::Visible;
    }

}

void ProximityDeviceScenario::ProximityDevice_PublishMessage(Object^ sender, RoutedEventArgs^ e) 
{
    if (m_publishedMessageId == -1)
    {
        m_rootPage->NotifyUser("", NotifyType::ErrorMessage);
        String^ publishText = ProximityDevice_PublishMessageText->Text;
        ProximityDevice_PublishMessageText->Text = ""; // clear the input after publishing.
        if (publishText->Length() > 0)
        {
            m_publishedMessageId = m_proximityDevice->PublishMessage("Windows.SampleMessageType", publishText);
            m_rootPage->NotifyUser("Message published, tap another device to transmit.", NotifyType::StatusMessage);
        }
        else
        {
            m_rootPage->NotifyUser("Please type a message", NotifyType::ErrorMessage);
        }
    } 
    else 
    {
        m_rootPage->NotifyUser("This sample only supports publishing one message at a time.", NotifyType::ErrorMessage);
    }
}

void ProximityDeviceScenario::MessageReceived(ProximityDevice^ device, ProximityMessage^ message) 
{
    m_rootPage->LogInfo("Message received: " + message->DataAsString, ProximityDeviceOutputText);
}

void ProximityDeviceScenario::ProximityDevice_SubscribeForMessage(Object^ sender, RoutedEventArgs^ e)
{
    if (m_subscribedMessageId == -1)
    {
        m_subscribedMessageId = m_proximityDevice->SubscribeForMessage("Windows.SampleMessageType", 
            ref new MessageReceivedHandler(this, &ProximityDeviceScenario::MessageReceived, CallbackContext::Same));
        m_rootPage->NotifyUser("Subscribed for proximity message, enter proximity to receive.", NotifyType::StatusMessage);
    } 
    else 
    {
        m_rootPage->NotifyUser("This sample only supports subscribing for one message at a time.", NotifyType::ErrorMessage);
    }
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void ProximityDeviceScenario::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in ProximityDeviceScenario such
    // as NotifyUser()
    m_rootPage = MainPage::Current;
    ProximityDevice_PublishMessageText->Text = "Hello World";
    if (m_proximityDevice == nullptr)
    {
        m_rootPage->NotifyUser("No proximity device found", NotifyType::ErrorMessage);
    }
    m_rootPage->ClearLog(ProximityDeviceOutputText);
}


void ProximityDeviceScenario::OnNavigatingFrom(NavigatingCancelEventArgs^ e)
{
    if (m_proximityDevice != nullptr)
    {
        if (m_publishedMessageId != -1)
        {
            m_proximityDevice->StopPublishingMessage(m_publishedMessageId);
            m_publishedMessageId = -1;
        }
        if (m_subscribedMessageId != -1)
        {
            m_proximityDevice->StopSubscribingForMessage(m_subscribedMessageId);
            m_subscribedMessageId = 1;
        }
    }
}
