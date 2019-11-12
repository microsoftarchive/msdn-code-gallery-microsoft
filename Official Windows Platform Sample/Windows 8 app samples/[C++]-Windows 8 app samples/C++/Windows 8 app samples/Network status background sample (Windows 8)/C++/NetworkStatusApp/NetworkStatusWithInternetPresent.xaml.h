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
// NetworkStatusWithInternetPresent.xaml.h
// Declaration of the NetworkStatusWithInternetPresent class
//

#pragma once

#include "pch.h"
#include "NetworkStatusWithInternetPresent.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace NetworkStatusApp
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class NetworkStatusWithInternetPresent sealed
        {
        public:
            NetworkStatusWithInternetPresent();
    
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        private:
            MainPage^ rootPage;
            Windows::UI::Core::CoreDispatcher^ SampleBackgroundTaskWithConditionDispatcher;
            String^ internetProfile;
            String^ networkAdapter;
    
            void AttachCompletedHandler(Windows::ApplicationModel::Background::IBackgroundTaskRegistration^ task);
            void OnCompleted(Windows::ApplicationModel::Background::BackgroundTaskRegistration^ task, Windows::ApplicationModel::Background::BackgroundTaskCompletedEventArgs^ args);
            void RegisterBackgroundTask(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void UnregisterBackgroundTask(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void UpdateUI();
        };
    }
}
