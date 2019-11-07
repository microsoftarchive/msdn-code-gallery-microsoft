//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

#pragma once

#include "pch.h"
#include "OvenClient.g.h"
#include "MainPage.xaml.h"

namespace ProxyStubsForWinRTComponents
{
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class OvenClient sealed
    {
    public:
        OvenClient();

    protected:
        virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
    private:
        MainPage^ rootPage;
        void OvenClientRun(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void BreadCompletedHandler1(Microsoft::SDKSamples::Kitchen::Oven^ oven, Microsoft::SDKSamples::Kitchen::Bread^ bread);
        void BreadCompletedHandler2(Microsoft::SDKSamples::Kitchen::Oven^ oven, Microsoft::SDKSamples::Kitchen::Bread^ bread);
        void BreadCompletedHandler3(Microsoft::SDKSamples::Kitchen::Oven^ oven, Microsoft::SDKSamples::Kitchen::Bread^ bread);
    };
}
