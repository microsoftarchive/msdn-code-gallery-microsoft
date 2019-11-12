// Copyright (c) Microsoft. All rights reserved.


#pragma once

#include "pch.h"
#include "Scenario4_ProximityDeviceEvents.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class ProximityDeviceEvents sealed
    {
    public:
        ProximityDeviceEvents();
        void DeviceArrived(Windows::Networking::Proximity::ProximityDevice^ device);
        void DeviceDeparted(Windows::Networking::Proximity::ProximityDevice^ device);

    protected:
        virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        virtual void OnNavigatingFrom(Windows::UI::Xaml::Navigation::NavigatingCancelEventArgs ^ e) override;

    private:
        MainPage^ m_rootPage;
        Windows::Foundation::EventRegistrationToken m_arrivedToken;
        Windows::Foundation::EventRegistrationToken m_departedToken;
        Windows::Networking::Proximity::ProximityDevice^ m_proximityDevice;
    };
}
