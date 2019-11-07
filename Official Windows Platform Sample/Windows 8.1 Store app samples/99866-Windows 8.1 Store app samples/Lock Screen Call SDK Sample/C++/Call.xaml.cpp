//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Call.xaml.cpp
// Implementation of the CallPage class
//

#include "pch.h"
#include "Call.xaml.h"
#include "MainPage.xaml.h"
#include <string>
#include <sstream>
#include <random>
#include <agents.h>


using namespace SDKSample;
using namespace SDKSample::LockScreenCall;

using namespace Platform;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::ApplicationModel::Calls;
using namespace Windows::Foundation;
using namespace Windows::System::Threading;
using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Interop;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;
using namespace concurrency;

CallPage::CallPage()
{
    InitializeComponent();
}

void CallPage::OnNavigatedTo(NavigationEventArgs^ e)
{
    __super::OnNavigatedTo(e);

    auto args = dynamic_cast<ILaunchActivatedEventArgs^>(e->Parameter);

    // This sample uses the same page for both lock screen calls and normal calls.
    // The OnLaunched method in App.xaml.cpp handles  normal call activation,
    // and the OnActivated method handles lock screen call activation.
    // Both pass the ILaunchActivatedEventArgs as the page parameter. If activated from
    // a lock screen call, the parameter will be a LockScreenCallActivatedEventArgs,
    // and the CallUI member gives us access to the lock screen.
    // The Arguments member is the toast command string specified in the XML generated
    // in the xmlPayload variable in Toast.xaml.cpp. It takes the form
    //   "<call mode> <caller identity> <simulated call duration>"

    std::wistringstream argsParser(args->Arguments->Data());
    std::wstring callMode;
    std::wstring callerIdentity;
    int callDuration;
    argsParser >> callMode;
    argsParser >> callerIdentity;
    argsParser >> callDuration;

    std::wstring imageText;

    if (callerIdentity == L"Dad")
    {
        imageText = L"\xd83d\xdc68";
    }
    else if (callerIdentity == L"Mom")
    {
        imageText = L"\xd83d\xdc69";
    }
    else if (callerIdentity == L"Grandpa")
    {
        imageText = L"\xd83d\xdc74";
    }
    else if (callerIdentity == L"Grandma")
    {
        imageText = L"\xd83d\xdc75";
    }

    if (callMode == L"Voice") {
        imageText = L"\xd83d\xdd0a";
    }

    std::wstring callTitle = callMode + L" call from " + callerIdentity;

    auto lockScreenArgs = dynamic_cast<LockScreenCallActivatedEventArgs^>(args);
    if (lockScreenArgs != nullptr)
    {
        callUI = lockScreenArgs->CallUI;

        // Hide controls since they are not interactive on the lock screen.
        EndCallButton->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
            
        // Hook up events.
        endRequestedToken = callUI->EndRequested += ref new TypedEventHandler<LockScreenCallUI^, LockScreenCallEndRequestedEventArgs^>(this, &CallPage::OnEndRequested);
        closedToken = callUI->Closed += ref new TypedEventHandler<LockScreenCallUI^, Object^>(this, &CallPage::OnClosed);

        // Set the title.
        callUI->CallTitle = ref new String(callTitle.c_str());
    }

    CallImage->Text = ref new String(imageText.c_str());
    
    // Assign a random light background color so that each call looks
    // slightly different.
    LayoutRoot->Background = RandomLightSolidColor();
            
    CallTitle->Text = ref new String(callTitle.c_str());

    if (callDuration > 0)
    {
        // Wait callDuration seconds, then call OtherPartyHangsUp.
        TimeSpan delay;
        delay.Duration = callDuration * 10000000L;
        hangUpTimer = ThreadPoolTimer::CreateTimer(ref new TimerElapsedHandler(this, &CallPage::OtherPartyHangsUp), delay);
    }
}

// When the user leaves the page, throw away the CallUI object and cancel
// any pending simulated "hang up" operation.
void CallPage::OnNavigatedFrom(NavigationEventArgs^ e)
{
    if (callUI != nullptr)
    {
        callUI->Closed -= closedToken;
        callUI->EndRequested -= endRequestedToken;
        callUI = nullptr;
    }
    if (hangUpTimer != nullptr)
    {
        hangUpTimer->Cancel();
        hangUpTimer = nullptr;
    }

    __super::OnNavigatedFrom(e);
}

SolidColorBrush^ CallPage::RandomLightSolidColor()
{
    std::default_random_engine generator;
    std::uniform_int_distribution<> r(128, 255);
    Windows::UI::Color color;
    color.A = 255;
    color.R = static_cast<byte>(r(generator));
    color.G = static_cast<byte>(r(generator));
    color.B = static_cast<byte>(r(generator));
    return ref new SolidColorBrush(color);
}

void CallPage::NavigateToMainPage()
{
    Frame->Navigate(TypeName(MainPage::typeid), nullptr);
}

void CallPage::QueueOnUIThread(DispatchedHandler^ agileCallback)
{
    Dispatcher->RunAsync(CoreDispatcherPriority::Normal, agileCallback);
}

// Fade to black then invokes the provided handler.
void CallPage::FadeToBlackThen(EventHandler<Object^>^ agileCallback)
{
    CallFadeOut->Visibility = Windows::UI::Xaml::Visibility::Visible;
    auto eventToken = new EventRegistrationToken;
    auto completed = ref new EventHandler<Object^>([this, eventToken, agileCallback](Object^ sender, Object^ o)
    {
        FadeToBlack->Completed -= *eventToken;
        delete eventToken;
        agileCallback->Invoke(sender, o);
    });
    
    *eventToken = FadeToBlack->Completed += completed;
    FadeToBlack->Begin();
}

void CallPage::OtherPartyHangsUp(Windows::System::Threading::ThreadPoolTimer^ sender)
{
    QueueOnUIThread(ref new DispatchedHandler([this]()
    {
        if (callUI != nullptr)
        {
            FadeToBlackThen(ref new EventHandler<Object^>([this](Object^ sender, Object^ o)
            {
                // Re-check callUI because it may have been nulled out while the animation is running.
                if (callUI != nullptr)
                {
                    callUI->Dismiss();
                }
                NavigateToMainPage();
            }));
        }
        else
        {
            NavigateToMainPage();
        }
    }));
}

void CallPage::OnEndRequested(LockScreenCallUI^ sender, LockScreenCallEndRequestedEventArgs^ e)
{
    auto deferral = e->GetDeferral();
    QueueOnUIThread(ref new DispatchedHandler([this, deferral]()
    {
        FadeToBlackThen(ref new EventHandler<Object^>([this, deferral](Object^ sender, Object^ o)
        {
            deferral->Complete();
            NavigateToMainPage();
        }));
    }));
}

void CallPage::OnClosed(LockScreenCallUI^ sender, Object^ e)
{
    QueueOnUIThread(ref new DispatchedHandler([this]()
    {
        callUI = nullptr;

        // Show the "End Call" button in our app.
        EndCallButton->Visibility = Windows::UI::Xaml::Visibility::Visible;
    }));
}

void CallPage::EndCallButton_Click(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    NavigateToMainPage();
}
