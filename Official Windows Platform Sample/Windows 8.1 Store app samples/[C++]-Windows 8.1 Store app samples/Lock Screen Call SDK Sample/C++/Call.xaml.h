//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Call.xaml.h
// Declaration of the CallPage class
//

#pragma once
#include "Call.g.h"

namespace SDKSample
{
    namespace LockScreenCall
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class CallPage sealed
        {
        public:
            CallPage();

        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
            virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

        private:
            void OnEndRequested(Windows::ApplicationModel::Calls::LockScreenCallUI^ sender, Windows::ApplicationModel::Calls::LockScreenCallEndRequestedEventArgs^ e);
            void OnClosed(Windows::ApplicationModel::Calls::LockScreenCallUI^ sender, Platform::Object^ e);
            void EndCallButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

            void FadeToBlackThen(Windows::Foundation::EventHandler<Platform::Object^>^ agileCallback);
            void NavigateToMainPage();
            void OtherPartyHangsUp(Windows::System::Threading::ThreadPoolTimer^ sender);
            static Windows::UI::Xaml::Media::SolidColorBrush^ RandomLightSolidColor();
            void QueueOnUIThread(Windows::UI::Core::DispatchedHandler^ agileCallback);

        private:
            // The UI on the lock screen, if the call is being displayed on the lock screen.
            Windows::ApplicationModel::Calls::LockScreenCallUI^ callUI;

            // Tokens for event handlers.
            Windows::Foundation::EventRegistrationToken endRequestedToken;
            Windows::Foundation::EventRegistrationToken closedToken;

            // The timer that simulates the caller hanging up.
            Windows::System::Threading::ThreadPoolTimer^ hangUpTimer;
        };
    }
}
