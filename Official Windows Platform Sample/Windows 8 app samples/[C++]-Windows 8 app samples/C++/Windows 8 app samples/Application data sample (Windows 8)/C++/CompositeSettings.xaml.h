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
// CompositeSettings.xaml.h
// Declaration of the CompositeSettings class
//

#pragma once

#include "pch.h"
#include "CompositeSettings.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace ApplicationDataSample
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
    	[Windows::Foundation::Metadata::WebHostHidden]
        public ref class CompositeSettings sealed
        {
        public:
            CompositeSettings();

        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        private:
            MainPage^ rootPage;
            Windows::Storage::ApplicationDataContainer^ roamingSettings;

            void WriteCompositeSetting_Click(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void DeleteCompositeSetting_Click(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void DisplayOutput();
        };
    }
}
