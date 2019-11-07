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
// SetVersion.xaml.h
// Declaration of the SetVersion class
//

#pragma once

#include "pch.h"
#include "SetVersion.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace ApplicationDataSample
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
    	[Windows::Foundation::Metadata::WebHostHidden]
        public ref class SetVersion sealed
        {
        public:
            SetVersion();

        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        private:
            MainPage^ rootPage;
            Windows::Storage::ApplicationData^ appData;

            void SetVersionHandler0(Windows::Storage::SetVersionRequest^ request);
            void SetVersionHandler1(Windows::Storage::SetVersionRequest^ request);

            void SetVersion0_Click(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void SetVersion1_Click(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

            void DisplayOutput();
        };
    }
}
