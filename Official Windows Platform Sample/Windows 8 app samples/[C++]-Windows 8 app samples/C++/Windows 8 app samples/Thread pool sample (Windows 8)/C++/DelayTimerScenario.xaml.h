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
// DelayTimerScenario.xaml.h
// Declaration of the DelayTimerScenario class
//

#pragma once

#include "pch.h"
#include "Constants.h"
#include "DelayTimerScenario.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace ThreadPool
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
    	[Windows::Foundation::Metadata::WebHostHidden]
        public ref class DelayTimerScenario sealed
        {
        public:
            DelayTimerScenario();
            void UpdateUI(ThreadPool::Status status);
    
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
            virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        private:
            MainPage^ rootPage;
            void CreateDelayTimer(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void CancelDelayTimer(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
