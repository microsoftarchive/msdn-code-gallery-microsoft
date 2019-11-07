// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// Scenario1Input.xaml.cpp
// Implementation of the Scenario1Input class
//

#include "pch.h"
#include "ScenarioInput5.xaml.h"

using namespace ToastsSampleCPP;

using namespace NotificationsExtensions::ToastContent;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Graphics::Display;
using namespace Windows::UI::Core;
using namespace Windows::UI::Notifications;
using namespace Windows::UI::ViewManagement;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;

ScenarioInput5::ScenarioInput5()
{
    InitializeComponent();
    Scenario5DisplayToastWithCallbacks->Click += ref new RoutedEventHandler(this, &ScenarioInput5::Scenario5DisplayToastWithCallbacks_Click);
    Scenario5HideToast->Click += ref new RoutedEventHandler(this, &ScenarioInput5::Scenario5HideToast_Click);
    dispatcher = Window::Current->Dispatcher;
}

ScenarioInput5::~ScenarioInput5()
{
}

#pragma region Template-Related Code - Do not remove
void ScenarioInput5::OnNavigatedTo(NavigationEventArgs^ e)
{
    // Get a pointer to our main page.
    auto pageAndContext = dynamic_cast<MainPageAndContext^>(e->Parameter);
    rootPage = pageAndContext->MainPage;
    toastContext = pageAndContext->ToastContext;

    // We want to be notified with the OutputFrame is loaded so we can get to the content.
    _frameLoadedToken = rootPage->OutputFrameLoaded += ref new Windows::Foundation::EventHandler<Platform::Object^>(this, &ScenarioInput5::rootPage_OutputFrameLoaded);
}

void ScenarioInput5::OnNavigatedFrom(NavigationEventArgs^ e)
{
    rootPage->OutputFrameLoaded -= _frameLoadedToken;
}

void ScenarioInput5::rootPage_OutputFrameLoaded(Object^ sender, Object^ e)
{
    // Get a pointer to the content within the OutputFrame.
    Page^ outputFrame = dynamic_cast<Page^>(rootPage->OutputFrame->Content);

    if (toastContext != "") 
    {
        OutputText("A toast was clicked on with activation arguments: " + toastContext);
    }
}

#pragma endregion

void ScenarioInput5::OutputText(String^ text) {
    if (outputText == nullptr) 
    {
        // Get a pointer to the content within the OutputFrame.
        Page^ outputFrame = dynamic_cast<Page^>(rootPage->OutputFrame->Content);

        // Go find the elements that we need for this scenario
        outputText = dynamic_cast<TextBlock^>(outputFrame->FindName("Scenario5OutputText"));
    }

    outputText->Text = text;
}

void ScenarioInput5::Scenario5DisplayToastWithCallbacks_Click(Object^ sender, RoutedEventArgs^ e)
{
    IToastText02^ toastContent = ToastContentFactory::CreateToastText02();
    toastContent->TextHeading->Text = "Tap toast";
    toastContent->TextBodyWrap->Text = "Or swipe to dismiss";

    toastContent->Launch = "Context123";

    OutputText(toastContent->GetContent());

    // You can listen for the "Activated" event provided on the toast object
    // or listen to the "OnLaunched" event off Windows::UI::Xaml::Application
    // object to tell when the user clicks the toast.
    //
    // The difference is that the OnLaunched event will
    // be raised by local, scheduled and cloud toasts, while the event provided by the 
    // toast object will only be raised by local toasts. 
    //
    // In this example, we'll use the event off the CoreApplication object.
    _toast = toastContent->CreateNotification();

    _toast->Dismissed += ref new TypedEventHandler<ToastNotification^, ToastDismissedEventArgs^>(this, &ScenarioInput5::toast_Dismissed);
    _toast->Failed += ref new TypedEventHandler<ToastNotification^, ToastFailedEventArgs^>(this, &ScenarioInput5::toast_Failed);

    ToastNotificationManager::CreateToastNotifier()->Show(_toast);
}

void ScenarioInput5::toast_Failed(ToastNotification^ sender, ToastFailedEventArgs^ e)
{
    dispatcher->RunAsync(CoreDispatcherPriority::Normal, ref new DispatchedHandler([this]() 
    {
        OutputText("The toast encountered an error");
    }, CallbackContext::Any));
}

void ScenarioInput5::toast_Dismissed(ToastNotification^ sender, ToastDismissedEventArgs^ e)
{
    String^ output = "";

    switch (e->Reason)
    {
        case ToastDismissalReason::ApplicationHidden:
            output = "The app hid the toast using ToastNotifier->Hide(toast)";
            break;
        case ToastDismissalReason::UserCanceled:
            output = "The user dismissed this toast";
            break;
        case ToastDismissalReason::TimedOut:
            output = "The toast has timed out";
            break;
    }

    dispatcher->RunAsync(CoreDispatcherPriority::Normal, ref new DispatchedHandler([this, output]()
    {
        OutputText(output);
    }, CallbackContext::Any));
}

void ScenarioInput5::Scenario5HideToast_Click(Object^ sender, RoutedEventArgs^ e)
{
    if (_toast != nullptr)
    {
        ToastNotificationManager::CreateToastNotifier()->Hide(_toast);
        _toast = nullptr;
    }
    else
    {
        OutputText("No toast has been displayed from Scenario 5");
    }
}