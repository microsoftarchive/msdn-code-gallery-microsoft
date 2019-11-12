//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

#pragma once

#include "pch.h"
#include "Scenario4.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace SDKTemplate
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
    	[Windows::Foundation::Metadata::WebHostHidden]
        public ref class Scenario4 sealed
        {
        public:
            Scenario4();
    
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        private:
            MainPage^ rootPage;
            void StartTilePolling_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
    		void StopTilePolling_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
    		bool AddURIIfValid(Windows::Foundation::Collections::IVector<Windows::Foundation::Uri^>^ vectorToReceive, Platform::String^ polledUrl);
        };
    }
}
