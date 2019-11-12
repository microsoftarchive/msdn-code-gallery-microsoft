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
// AuthByForegroundApp.xaml.h
// Declaration of the AuthByForegroundApp class
//

#pragma once

#include "pch.h"
#include "AuthByForegroundApp.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace HotspotAuthenticationApp
    {
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class AuthByForegroundApp sealed
        {
        public:
            AuthByForegroundApp();
    
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        private:
            MainPage^ rootPage;
            Windows::Networking::NetworkOperators::HotspotAuthenticationContext^ authenticationContext;
    
            void AuthenticateButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void SkipButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void AbortButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void InitializeForegroundAppAuthentication();
            void ClearAuthenticationToken();
        };
    }
}
