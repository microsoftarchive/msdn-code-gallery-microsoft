//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario1.xaml.h
// Declaration of the Scenario1 class
//

#pragma once
#include "S1_LoggingSession.g.h"
#include "LoggingScenario.h"

namespace SDKSample
{
	namespace LoggingSession
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
		public ref class S1_LoggingSession sealed
        {
        public:
            S1_LoggingSession();

        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

        private:

            property LoggingSession::LoggingScenario^ LoggingScenario
            {
                LoggingSession::LoggingScenario^ get() { return LoggingSession::LoggingScenario::GetLoggingScenarioSingleton(); }
            }

            task<void> UpdateLogMessageCountDispatchAsync();
            void AddMessage(Platform::String^ message);
            task<void> AddMessageDispatch(Platform::String^ message);
            task<void> AddLogFileMessageDispatch(Platform::String^ message, const std::wstring& logFileFullPath);
            void UpdateControls();
            void DoScenario(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void EnableDisableLogging(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void OnStatusChanged(SDKSample::LoggingSession::LoggingScenario ^sender, SDKSample::LoggingSession::LoggingScenarioEventArgs ^args);
            Windows::UI::Xaml::Controls::ScrollViewer^ FindScrollViewer(DependencyObject^ depObject);
        };
    }
}
