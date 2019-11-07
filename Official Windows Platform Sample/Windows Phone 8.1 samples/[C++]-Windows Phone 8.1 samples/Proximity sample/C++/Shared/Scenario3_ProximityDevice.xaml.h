// Copyright (c) Microsoft. All rights reserved.

#pragma once

#include "pch.h"
#include "Scenario3_ProximityDevice.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class ProximityDeviceScenario sealed
    {
    public:
        ProximityDeviceScenario();

    protected:
        virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        virtual void OnNavigatingFrom(Windows::UI::Xaml::Navigation::NavigatingCancelEventArgs ^ e) override;
        void ProximityDevice_PublishMessage(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void MessageReceived(Windows::Networking::Proximity::ProximityDevice^ device, Windows::Networking::Proximity::ProximityMessage^ message);
        void ProximityDevice_SubscribeForMessage(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

        void OnLoaded(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void Current_SizeChanged(Object^ sender, Windows::UI::Core::WindowSizeChangedEventArgs^ e);
        void ChangeVisualState(double width);

    private:
        MainPage^ m_rootPage;

        long long m_publishedMessageId;
        Windows::Networking::Proximity::ProximityDevice^ m_proximityDevice;
        long long m_subscribedMessageId;
    };
}
