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
// AppListingUri.xaml.h
// Declaration of the AppListingUri class
//

#pragma once

#include "pch.h"
#include "AppListingUri.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace StoreSample
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class AppListingUri sealed
        {
        public:
            AppListingUri();

        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
            virtual void OnNavigatingFrom(Windows::UI::Xaml::Navigation::NavigatingCancelEventArgs^ e) override;
        private:
            MainPage^ rootPage;
            Windows::Foundation::EventRegistrationToken eventRegistrationToken;

            void LoadAppListingUriProxyFile();
            void AppListingUriRefreshScenario();

            void DisplayLink_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
