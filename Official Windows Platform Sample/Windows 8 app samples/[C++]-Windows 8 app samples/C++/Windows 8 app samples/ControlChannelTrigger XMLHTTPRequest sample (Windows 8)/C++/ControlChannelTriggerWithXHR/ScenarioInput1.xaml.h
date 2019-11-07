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
// ScenarioInput1.xaml.h
// Declaration of the ScenarioInput1 class
//

#pragma once

#include "pch.h"
#include "ScenarioInput1.g.h"
#include "MainPage.xaml.h"
using namespace Windows::Foundation;
using namespace Windows::UI::Xaml::Controls;
using namespace XHRTransportHelper;
using namespace Windows::UI::Core;

namespace ControlChannelTrigger
{
    public enum class connectionStates
    {
        notConnected = 0,
        connecting = 1,
        connected = 2,
    };

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class ScenarioInput1 sealed
    {
    public:
        ScenarioInput1();
        void rootPage_OutputFrameLoaded(Object^ sender, Object^ e);
        String^ GetUrl();
        String^ GetPostData();
        void ClientInit();

    protected:
        virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
    private:
        void CanaryButton_Click(Object^ sender, RoutedEventArgs^ e);
        void ConnectButton_Click(Object^ sender, RoutedEventArgs^ e);
        void RegisterNetworkChangeTask();
        static connectionStates connectionState;
    internal:
        MainPage^ rootPage;
        bool lockScreenAdded;
        Page outputFrame;
        EventRegistrationToken outputFrameERToken;
        CommModule^ commModule;
    };
}
