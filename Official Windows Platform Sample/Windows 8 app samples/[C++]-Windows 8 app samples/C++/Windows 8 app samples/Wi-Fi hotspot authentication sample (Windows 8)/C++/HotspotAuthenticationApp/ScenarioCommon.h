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
// ScenarioCommon.h
// Header for class shared by all scenarios
//

#pragma once

#include "pch.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace HotspotAuthenticationApp
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class ScenarioCommon sealed
        {
        public:
            // Singleton property
            static property ScenarioCommon^ Instance
            {
                ScenarioCommon^ get();
            }
    
            // The entry point name of the background task handler:
            property Platform::String^ BackgroundTaskEntryPoint
            {
                Platform::String^ get();
            }
    
            // The (arbitrarily chosen) name assigned to the background task:
            property Platform::String^ BackgroundTaskName
            {
                Platform::String^ get();
            }
    
            // Register completion handler for registered background task.
            // Returns true if task is registered, false otherwise.
            bool RegisteredCompletionHandlerForBackgroundTask();
    
            // Background task completion handler. When authenticating through the foreground app, this triggers the
            // authentication flow if the app is currently running.
            void OnBackgroundTaskCompleted(Windows::ApplicationModel::Background::BackgroundTaskRegistration^ task,
                Windows::ApplicationModel::Background::BackgroundTaskCompletedEventArgs^ args);
    
            // An handler for subscribing for foreground authentication events
            void SetForegroundAuthenticationEventHandler(Windows::UI::Core::DispatchedHandler^ handler);
    
        private:
            ScenarioCommon();
    
            // Singleton reference to share a single instance with all pages
            static ScenarioCommon^ scenarioCommonSingleton;
    
            // A pointer back to the main page.  This is needed to call methods in MainPage such as NotifyUser()
            MainPage^ rootPage;
    
            // Variable for BackgroundTaskEntryPoint
            Platform::String^ backgroundTaskEntryPoint;
    
            // Variable for BackgroundTaskName
            Platform::String^ backgroundTaskName;
    
            // A reference to the main window dispatcher object to the UI.
            Windows::UI::Core::CoreDispatcher^ coreDispatcher;
    
            // A flag to remember if a background task handler has been registered
            bool hasRegisteredBackgroundTaskHandler;
    
            // A callback to handle foreground authentication events
            Windows::UI::Core::DispatchedHandler^ foregroundAuthenticationCallback;
        };
    }
}
