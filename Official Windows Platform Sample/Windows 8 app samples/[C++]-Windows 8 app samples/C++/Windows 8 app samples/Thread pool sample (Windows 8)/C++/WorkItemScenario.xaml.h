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
// WorkItemScenario.xaml.h
// Declaration of the WorkItemScenario class
//

#pragma once

#include "pch.h"
#include "WorkItemScenario.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace ThreadPool
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
    	[Windows::Foundation::Metadata::WebHostHidden]
        public ref class WorkItemScenario sealed
        {
        public:
            WorkItemScenario();
            void UpdateUI(ThreadPool::Status status);
            void UpdateWorkItemProgressUI(int percentComplete);
    
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
            virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        private:
            MainPage^ rootPage;
            void CreateThreadPoolWorkItem(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void CancelThreadPoolWorkItem(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
