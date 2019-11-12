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
// BackgroundTask.xaml.h
// Declaration of the BackgroundTask class
//

#pragma once

#include "pch.h"
#include "BackgroundTask.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace SmsBackgroundTaskSample
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class BackgroundTask sealed
        {
        public:
            BackgroundTask();
    
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        private:
            MainPage^ rootPage;
    
            Platform::String^ backgroundTaskEntryPoint;
            Platform::String^ backgroundTaskName;
            Windows::UI::Core::CoreDispatcher^ sampleDispatcher;
            bool hasDeviceAccess;
    
            void InitializeRegisteredSmsBackgroundTasks();
            void UpdateBackgroundTaskUIState(bool Registered);
            void OnCompleted(Windows::ApplicationModel::Background::BackgroundTaskRegistration^ sender, Windows::ApplicationModel::Background::BackgroundTaskCompletedEventArgs^ e);
            void RegisterBackgroundTask_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void UnregisterBackgroundTask_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
