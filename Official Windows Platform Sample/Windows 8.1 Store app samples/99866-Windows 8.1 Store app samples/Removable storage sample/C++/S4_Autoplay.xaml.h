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
// S4_Autoplay.xaml.h
// Declaration of the S4_Autoplay class
//

#pragma once

#include "pch.h"
#include "S4_Autoplay.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace RemovableStorageCPP
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class S4_Autoplay sealed
        {
        public:
            S4_Autoplay();

        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        private:
            MainPage^ rootPage;
            void GetImage_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void GetFirstImageFromStorageAsync(Windows::Storage::StorageFolder^ storage);
            void DisplayImageAsync(Windows::Storage::StorageFile^ imageFile);
        };
    }
}
