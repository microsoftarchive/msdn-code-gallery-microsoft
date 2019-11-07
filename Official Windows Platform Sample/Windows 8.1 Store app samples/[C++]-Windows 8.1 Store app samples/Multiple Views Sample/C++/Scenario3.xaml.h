//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario3.xaml.h
// Declaration of the Scenario3 class
//

#pragma once
#include "Scenario3.g.h"

namespace SDKSample
{
    namespace MultipleViews
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class Scenario3 sealed
        {
        public:
            Scenario3();
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
            virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        private:
            void AnimatedSwitch_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void Fade_Completed(Platform::Object^ sender, Platform::Object^ e);
            Concurrency::task<void> FadeOutContents();
            void Current_VisibilityChanged(Platform::Object^ sender, Windows::UI::Core::VisibilityChangedEventArgs^ e);
            unsigned long long GetCurrentTime();

            Windows::Foundation::EventRegistrationToken visibilityToken;
            Windows::Foundation::EventRegistrationToken fadeOutToken;
            SDKSample::MainPage^ rootPage;
            Windows::UI::Xaml::Media::Animation::Storyboard^ fadeOutStoryboard;
            unsigned long long lastFadeOutTime;
            std::unique_ptr<Concurrency::task_completion_event<void>> animationTaskPtr;
        };
    }
}
