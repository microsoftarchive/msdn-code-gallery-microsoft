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
// TrialMode.xaml.h
// Declaration of the TrialMode class
//

#pragma once

#include "pch.h"
#include "TrialMode.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace StoreSample
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class TrialMode sealed
        {
        public:
            TrialMode();

        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
            virtual void OnNavigatingFrom(Windows::UI::Xaml::Navigation::NavigatingCancelEventArgs^ e) override;

        private:
            MainPage^ rootPage;
            Windows::Foundation::EventRegistrationToken eventRegistrationToken;

            void LoadTrialModeProxyFile();
            void TrialModeRefreshScenario();

            void TrialTime_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void TrialProduct_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void FullProduct_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void ConvertTrial_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}