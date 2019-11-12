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
// AddSettingsScenario.xaml.h
// Declaration of the AddSettingsScenario class
//

#pragma once

#include "pch.h"
#include "AddSettingsScenario.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace ApplicationSettings
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class AddSettingsScenario sealed
        {
        public:
            AddSettingsScenario();

        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
            virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

        private:
            MainPage^ rootPage;
            void addSettingsScenarioAdd_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

            bool isEventRegistered;
            Windows::Foundation::EventRegistrationToken commandsRequestedEventRegistrationToken;

            void onSettingsCommand(Windows::UI::Popups::IUICommand^ command);
            void onCommandsRequested(Windows::UI::ApplicationSettings::SettingsPane^ settingsPane, Windows::UI::ApplicationSettings::SettingsPaneCommandsRequestedEventArgs^ eventArgs);
        };
    }
}
