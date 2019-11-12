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
// OperatorMessage.xaml.h
// Declaration of the OperatorMessage class
//

#pragma once

#include "pch.h"
#include "OperatorMessage.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace MobileBroadband
    {
        //
        /// This page registers a completion handler for the mobile broadband operator notification background task
        //
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class OperatorMessage sealed
        {
        public:
            OperatorMessage();
    
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        private:
            MainPage^ rootPage;
    
            Windows::UI::Core::CoreDispatcher^ sampleDispatcher;
            Platform::String^ operatorNotificationTaskEntryPoint;
            Platform::String^ operatorNotificationTaskName;
            void RegisterOperatorNotificationTask();
            void OnCompleted(Windows::ApplicationModel::Background::BackgroundTaskRegistration^ sender, Windows::ApplicationModel::Background::BackgroundTaskCompletedEventArgs^ e);
            void OnProgress(Windows::ApplicationModel::Background::BackgroundTaskRegistration^ sender, Windows::ApplicationModel::Background::BackgroundTaskProgressEventArgs^ e);
        };
    }
}
