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
// SettingContainer.xaml.h
// Declaration of the SettingContainer class
//

#pragma once

#include "pch.h"
#include "SettingContainer.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace ApplicationDataSample
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
    	[Windows::Foundation::Metadata::WebHostHidden]
        public ref class SettingContainer sealed
        {
        public:
            SettingContainer();

        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        private:
            MainPage^ rootPage;
            Windows::Storage::ApplicationDataContainer^ localSettings;

            void CreateContainer_Click(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void DeleteContainer_Click(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void WriteSetting_Click(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void DeleteSetting_Click(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void DisplayOutput();
        };
    }
}
