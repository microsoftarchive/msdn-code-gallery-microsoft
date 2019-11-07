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
// Files.xaml.h
// Declaration of the Files class
//

#pragma once

#include "pch.h"
#include "Scenario1_Files.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace ApplicationDataSample
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class Files sealed
        {
        public:
            Files();

        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        private:
            MainPage^ rootPage;

            Windows::Storage::StorageFolder^ localFolder;
            int localCounter;
            Windows::Storage::StorageFolder^ localCacheFolder;
            int localCacheCounter;
            Windows::Storage::StorageFolder^ roamingFolder;
            int roamingCounter;
            Windows::Storage::StorageFolder^ temporaryFolder;
            int temporaryCounter;

            void Increment_Local_Click(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void Read_Local_Counter();
            void Increment_LocalCache_Click(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void Read_LocalCache_Counter();
            void Increment_Roaming_Click(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void Read_Roaming_Counter();
            void Increment_Temporary_Click(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void Read_Temporary_Counter();
            void DisplayOutput();
        };
    }
}
