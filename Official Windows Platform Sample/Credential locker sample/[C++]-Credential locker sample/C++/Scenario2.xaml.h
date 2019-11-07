//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario2.xaml.h
// Declaration of the Scenario2 class
//

#pragma once
#include "Scenario2.g.h"

namespace SDKSample
{
    namespace PasswordVaultCPP
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        public ref class Scenario2 sealed
        {
        public:
            Scenario2();
		
//		protected:
//            virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;	

        private:
            void Manage_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
			void OnCommandsRequested(Windows::UI::ApplicationSettings::SettingsPane ^sender, Windows::UI::ApplicationSettings::SettingsPaneCommandsRequestedEventArgs ^args);
			void OnAccountCommandsRequested(Windows::UI::ApplicationSettings::AccountsSettingsPane ^sender, Windows::UI::ApplicationSettings::AccountsSettingsPaneCommandsRequestedEventArgs ^args);
			void ProviderInvoked(Windows::UI::ApplicationSettings::WebAccountProviderCommand^ providerCmd);
			void AccountInvoked(Windows::UI::ApplicationSettings::WebAccountCommand^ accountCmd, Windows::UI::ApplicationSettings::WebAccountInvokedArgs^ args);
			void CredentialDeleted(Windows::UI::ApplicationSettings::CredentialCommand^ credCmd);
			void HandleAppSpecificCmd(Windows::UI::Popups::IUICommand^ command);
			void DebugPrint(Platform::String^ Message);
        };
    }
}
