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
// PhoneCall.xaml.h
// Declaration of the PhoneCall class
//

#pragma once

#include "pch.h"
#include "PhoneCall.g.h"
#include "MainPage.xaml.h"

using namespace Windows::UI::Xaml;
using namespace Windows::Media;
using namespace Platform;

namespace  SDKSample
{
namespace CallControl
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
	[Windows::Foundation::Metadata::WebHostHidden]
    public ref class PhoneCall sealed
    {
    public:
        PhoneCall();

    protected:
        virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
    private:
        MainPage^ rootPage;
        void ButtonInitialize_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void ButtonRing_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void ButtonHangUp_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

        void CallControlError(String^ ErrorText);
        void CallControlStatus(String^ StatusText);

        void CallControls_AnswerRequested(Devices::CallControl^ sender);
        void CallControls_HangUpRequested(Devices::CallControl^ sender);
        void CallControls_AudioTransferRequested(Devices::CallControl^ sender);
        void CallControls_RedialRequested(Devices::CallControl^ sender, Devices::RedialRequestedEventArgs^ e);
        void CallControls_DialRequested(Devices::CallControl^ sender, Devices::DialRequestedEventArgs^ e);

        void enableIncomingCallButton();
        void disableIncomingCallButton();
        void enableInitializeButton();
        void disableInitializeButton();
        void enableHangUpButton();
        void disableHangUpButton();

        void SetAudioSource();
        void PlayAudio();
        void StopAudio();
        void PauseAudio();

    private:
        Devices::CallControl^ CallControls;
        Windows::UI::Core::CoreDispatcher^ _dispatcher;
        unsigned long long CallToken;
	
	};
}
}