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
// SemanticDeduplication.xaml.h
// Declaration of the Globalization class
//

#pragma once

#include "pch.h"
#include "Scenario10_ContentDeduplication.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace Tiles
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class ContentDeduplication sealed
        {
        public:
            ContentDeduplication();

        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

        private:
            MainPage^ rootPage;
            void EnableNotificationQueue_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void ClearTile_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void SendNotifications_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}