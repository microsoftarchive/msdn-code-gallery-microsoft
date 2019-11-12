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
#include "Files.g.h"
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
            Windows::Storage::StorageFolder^ roamingFolder;
            int counter;

            void Increment_Click(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void ReadCounter();
            void DisplayOutput();
        };
    }
}
