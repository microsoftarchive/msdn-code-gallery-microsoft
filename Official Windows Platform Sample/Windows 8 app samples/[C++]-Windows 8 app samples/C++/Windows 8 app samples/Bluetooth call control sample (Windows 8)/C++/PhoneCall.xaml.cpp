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
// PhoneCall.xaml.cpp
// Implementation of the PhoneCall class
//

#include "pch.h"
#include "PhoneCall.xaml.h"
#include "ppltasks.h"

using namespace SDKSample::CallControl;

using namespace Windows::UI;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::System;
using namespace Windows::Foundation;
using namespace Windows::Media;
using namespace Windows::Storage;
using namespace Windows::Storage::Streams;
using namespace Platform;
using namespace Concurrency;

PhoneCall::PhoneCall()
{
    InitializeComponent();
    _dispatcher = Window::Current->Dispatcher;
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void PhoneCall::OnNavigatedTo(NavigationEventArgs^ e)
{
    // Get a pointer to our main page.
    rootPage = MainPage::Current;
}


void PhoneCall::ButtonInitialize_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ b = safe_cast<Button^>(sender);
    if (b != nullptr)
    {
        rootPage->NotifyUser("You clicked the " + b->Name + " button", NotifyType::StatusMessage);
    }
    try
    {
        CallControls = Windows::Media::Devices::CallControl::GetDefault();
        if (CallControls == nullptr)
        {
            CallControlError("Call Controls Initialization Failed.\tEnsure you have a bluetooth device and press the initialize the call control button.");
        }
        else
        {
            CallControls->AnswerRequested += ref new Devices::CallControlEventHandler(this, &PhoneCall::CallControls_AnswerRequested);
            CallControls->HangUpRequested += ref new Devices::CallControlEventHandler(this, &PhoneCall::CallControls_HangUpRequested);
            CallControls->AudioTransferRequested += ref new Devices::CallControlEventHandler(this, &PhoneCall::CallControls_AudioTransferRequested);
            CallControls->RedialRequested += ref new Devices::RedialRequestedEventHandler(this, &PhoneCall::CallControls_RedialRequested);
            CallControls->DialRequested += ref new Devices::DialRequestedEventHandler(this, &PhoneCall::CallControls_DialRequested);
            enableIncomingCallButton();
            disableInitializeButton();

            CallControlStatus("Call Controls Initialized");
        }
    }
    catch (Exception^ Exception)
    {
        if (Exception->Message == "Element not found.\r\n")
        {
            CallControlError("Call Controls Initialization Failed.\tEnsure you have a bluetooth device and press the initialize the call control button.");
        }
        else
        {
            CallControlError(Exception->Message);
        }
    }
}


void PhoneCall::ButtonRing_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    if (CallToken == 0 && CallControls != nullptr)
    {
        CallToken = CallControls->IndicateNewIncomingCall(true, "5555555555");
        CallControlStatus("Call Token: " + CallToken.ToString());
        disableIncomingCallButton();
    }
}


void PhoneCall::CallControlError(String^ ErrorText)
{
	rootPage->NotifyUser(ErrorText, NotifyType::ErrorMessage);
}

void PhoneCall::CallControlStatus(String^ StatusText)
{
	rootPage->NotifyUser(StatusText, NotifyType::StatusMessage);
}


void PhoneCall::CallControls_AnswerRequested(Devices::CallControl^ sender)
{
    if (CallToken != 0 && CallControls != nullptr)
    {
        _dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, ref new Windows::UI::Core::DispatchedHandler([=]()
        {
			CallControls->IndicateActiveCall(CallToken);
            SetAudioSource();
            PlayAudio();
            CallControlStatus("Answer requested: " + CallToken.ToString());
            enableHangUpButton();
        }
        , CallbackContext::Any)); 
    }
}

void PhoneCall::ButtonHangUp_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{

    if (CallToken != 0 && CallControls != nullptr)
    {
        _dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, ref new Windows::UI::Core::DispatchedHandler([=]()
        {
            CallControls->EndCall(CallToken);
            CallToken = 0;
            PauseAudio();
            CallControlStatus("Hangup requested");
            disableHangUpButton();
        }
        , CallbackContext::Any)); 
		
		enableIncomingCallButton();
    }

}

void PhoneCall::CallControls_HangUpRequested(Devices::CallControl^ sender)
{
    if (CallToken != 0 && CallControls != nullptr)
    {
        _dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, ref new Windows::UI::Core::DispatchedHandler([=]()
        {
            CallControls->EndCall(CallToken);
            CallToken = 0;
            PauseAudio();
            CallControlStatus("Hangup requested");
            disableHangUpButton();
        }
        , CallbackContext::Any)); 

        enableIncomingCallButton();
    }
}

void PhoneCall::CallControls_AudioTransferRequested(Devices::CallControl^ sender)
{
    CallControlStatus("Audio Transfer requested");
}

void PhoneCall::CallControls_RedialRequested(Devices::CallControl^ sender, Devices::RedialRequestedEventArgs^ e)
{
    if (CallToken != 0)
    {
        // Already in a call.
        CallControlStatus("Already in a call");
        return;
    }
    else
    {
        e->Handled();
        CallControlStatus("Redial requested");
    }
}

void PhoneCall::enableIncomingCallButton() {
	_dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, ref new Windows::UI::Core::DispatchedHandler([=]()
	{
		ButtonRing->IsEnabled = true;
	}
	, CallbackContext::Any));
}

void PhoneCall::disableIncomingCallButton() {
	_dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, ref new Windows::UI::Core::DispatchedHandler([=]()
	{
		ButtonRing->IsEnabled = false;
	}
	, CallbackContext::Any));
}

void PhoneCall::enableInitializeButton() {
	_dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, ref new Windows::UI::Core::DispatchedHandler([=]()
	{
		ButtonInitialize->IsEnabled = true;
	}
	, CallbackContext::Any));
}

void PhoneCall::disableInitializeButton() {
	_dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, ref new Windows::UI::Core::DispatchedHandler([=]()
	{
		ButtonInitialize->IsEnabled = false;
	}
	, CallbackContext::Any));
}

void PhoneCall::enableHangUpButton() {
	_dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, ref new Windows::UI::Core::DispatchedHandler([=]()
	{
		ButtonHangUp->IsEnabled = true;
	}
	, CallbackContext::Any));
}

void PhoneCall::disableHangUpButton() {
	_dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, ref new Windows::UI::Core::DispatchedHandler([=]()
	{
		ButtonHangUp->IsEnabled = false;
	}
	, CallbackContext::Any));
}

void PhoneCall::CallControls_DialRequested(Devices::CallControl^ sender, Devices::DialRequestedEventArgs^ e)
{
    if (CallToken != 0)
    {
        // Already in a call.
        CallControlStatus("Already in a call");
    }
}

void PhoneCall::SetAudioSource()
{
    MediaElement^ player = OutputMediaElement;
    create_task(Windows::ApplicationModel::Package::Current->InstalledLocation->GetFileAsync("folk_rock.mp3")).then([player](StorageFile^ file) 
    {
            create_task(file->OpenAsync(FileAccessMode::Read)).then([file, player](IRandomAccessStream^ stream) 
            {
                    player->SetSource(stream, file->ContentType);
            });
    });
}

void PhoneCall::PlayAudio()
{
    OutputMediaElement->Play();
}

void PhoneCall::StopAudio()
{
    OutputMediaElement->Stop();
}

void PhoneCall::PauseAudio()
{
    OutputMediaElement->Pause();
}


