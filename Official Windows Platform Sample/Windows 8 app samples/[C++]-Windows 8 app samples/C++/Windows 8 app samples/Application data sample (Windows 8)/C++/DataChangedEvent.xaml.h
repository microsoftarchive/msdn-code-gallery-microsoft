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
// DataChangedEvent.xaml.h
// Declaration of the DataChangedEvent class
//

#pragma once

#include "pch.h"
#include "DataChangedEvent.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace ApplicationDataSample
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
    	[Windows::Foundation::Metadata::WebHostHidden]
        public ref class DataChangedEvent sealed
        {
        public:
            DataChangedEvent();

        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        private:
            MainPage^ rootPage;
            Windows::Storage::ApplicationData^ applicationData;

            void SimulateRoaming_Click(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void WriteOutputText();
            void DataChangedHandler(Windows::Storage::ApplicationData^ appData, Object^);
            void DisplayOutput();
        };
    }
}
