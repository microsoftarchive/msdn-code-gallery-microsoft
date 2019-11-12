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
// Scenario7.xaml.h
// Declaration of the Scenario7 class
//

#pragma once

#include "pch.h"
#include "Scenario7.g.h"
#include "MainPage.xaml.h"
#include <agile.h>

namespace SDKSample
{
    namespace WebViewControl
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class Scenario7 sealed
        {
        public:
            Scenario7();
    
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        private:
            MainPage^ rootPage;
    		Platform::Agile<Windows::ApplicationModel::DataTransfer::DataTransferManager> dataTransferManager;
    		void WebView7_LoadCompleted(Platform::Object^ sender, Windows::UI::Xaml::Navigation::NavigationEventArgs^ e);
    		void Share_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
    		void dataTransferManager_DataRequested(Windows::ApplicationModel::DataTransfer::DataTransferManager^ sender, Windows::ApplicationModel::DataTransfer::DataRequestedEventArgs^ args);
            Windows::Foundation::EventRegistrationToken dataRequestedToken;
    	};
    }}
