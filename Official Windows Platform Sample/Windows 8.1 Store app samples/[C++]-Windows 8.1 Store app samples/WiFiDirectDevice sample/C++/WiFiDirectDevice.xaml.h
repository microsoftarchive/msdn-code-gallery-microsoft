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
// WiFiDirectDeviceScenario.xaml.h
// Declaration of the WiFiDirectDeviceScenario class
//

#pragma once

#include "pch.h"
#include "WiFiDirectDevice.g.h"
#include "MainPage.xaml.h"



namespace WiFiDirectDeviceCPP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
	[Windows::Foundation::Metadata::WebHostHidden]
    public ref class WiFiDirectDeviceScenario sealed
    {
    public:
        WiFiDirectDeviceScenario();

    protected:
        virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        virtual void OnNavigatingFrom(Windows::UI::Xaml::Navigation::NavigatingCancelEventArgs ^ e) override;
    private:

        void GetDevices(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void Connect(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void SocketError(Platform::String^ errMessage);
        void Send(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void Disconnect(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void DisconnectNotification(Windows::Devices::WiFiDirect::WiFiDirectDevice^ sender, Object^ arg);

        //members
        MainPage^ rootPage;
		Windows::Devices::Enumeration::DeviceInformationCollection^ devInfoCollection;
		Windows::Devices::WiFiDirect::WiFiDirectDevice^ wfdDevice;
		Windows::Foundation::EventRegistrationToken disconnectToken;
    };
}
