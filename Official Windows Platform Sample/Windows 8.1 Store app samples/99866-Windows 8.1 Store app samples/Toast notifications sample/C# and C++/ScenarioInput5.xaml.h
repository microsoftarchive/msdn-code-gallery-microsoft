// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//
// ScenarioInput5.xaml.h
// Declaration of the ScenarioInput5 class.
//

#pragma once

#include "pch.h"
#include "ScenarioInput5.g.h"
#include "MainPage.xaml.h"

using namespace Windows::UI::Xaml;

namespace ToastsSampleCPP
{
	[Windows::Foundation::Metadata::WebHostHidden]
    public ref class ScenarioInput5 sealed
    {
    public:
        ScenarioInput5();

    protected:
        virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

    private:
        ~ScenarioInput5();

		MainPage^ rootPage;
        void rootPage_OutputFrameLoaded(Object^ sender, Object^ e);
        void OutputText(Platform::String^ text);
        void Scenario5DisplayToastWithCallbacks_Click(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void toast_Failed(Windows::UI::Notifications::ToastNotification^ sender, Windows::UI::Notifications::ToastFailedEventArgs^ e);
        void toast_Dismissed(Windows::UI::Notifications::ToastNotification^ sender, Windows::UI::Notifications::ToastDismissedEventArgs^ e);

        void Scenario5HideToast_Click(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

        Windows::Foundation::EventRegistrationToken _frameLoadedToken;
        Windows::UI::Xaml::Controls::TextBlock^ outputText;
        Windows::UI::Core::CoreDispatcher^ dispatcher;
        Platform::String^ toastContext;

        Windows::UI::Notifications::ToastNotification^ _toast;
    };
}
