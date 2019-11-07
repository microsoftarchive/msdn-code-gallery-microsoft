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
// Scenario3.xaml.h
// Declaration of the Scenario3 class
//

#pragma once

#include "pch.h"
#include "Scenario3_oAuthFlickr.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace WebAuthentication
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class Scenario3 sealed
        {
        public:
            Scenario3();
    
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;		
			virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;		
        private:
            MainPage^ rootPage;
            void Show_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

			void OnCommandsRequested(Windows::UI::ApplicationSettings::SettingsPane ^sender, Windows::UI::ApplicationSettings::SettingsPaneCommandsRequestedEventArgs ^args);
			void OnAccountCommandsRequested(Windows::UI::ApplicationSettings::AccountsSettingsPane ^sender, Windows::UI::ApplicationSettings::AccountsSettingsPaneCommandsRequestedEventArgs ^args);
			void WebAccountProviderInvokedHandler(Windows::UI::ApplicationSettings::WebAccountProviderCommand^ providerCmd);
			void WebAccountInvokedHandler(Windows::UI::ApplicationSettings::WebAccountCommand^ accountCmd, Windows::UI::ApplicationSettings::WebAccountInvokedArgs^ args);


			void HandleAppSpecificCmd(Windows::UI::Popups::IUICommand^ command);


			void InitializeWebAccountProviders();
            void InitializeWebAccounts();
			void AuthenticateToFacebook();
			void AuthenticateToGoogle();
			void GetFacebookUserName(Platform::String^ webAuthResultResponseData);
			void GetYouTubeUserName(Platform::String^ webAuthResultResponseData);
			
			
			Windows::Security::Credentials::WebAccountProvider ^facebookProvider;
			Windows::Security::Credentials::WebAccount ^facebookAccount;

			Windows::Security::Credentials::WebAccountProvider ^googleProvider;
			Windows::Security::Credentials::WebAccount ^googleAccount;
			

			Windows::Storage::ApplicationDataContainer^ roamingSettings ;
			bool isFacebookUserLoggedIn;
			bool isGoogleUserLoggedIn;
			Platform::String ^facebookUserName;
			Platform::String ^googleUserName;

			Windows::Foundation::EventRegistrationToken commandsRequestedEventRegistrationToken;
			Windows::Foundation::EventRegistrationToken accountCommandsRequestedEventRegistrationToken;

    	};
    }
}
