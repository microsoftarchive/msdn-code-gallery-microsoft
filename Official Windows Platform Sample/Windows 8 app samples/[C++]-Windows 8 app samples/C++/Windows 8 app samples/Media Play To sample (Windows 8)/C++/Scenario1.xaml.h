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
// Scenario1.xaml.h
// Declaration of the Scenario1 class
//

#pragma once

#include "pch.h"
#include "Scenario1.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace PlayTo
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
    	[Windows::Foundation::Metadata::WebHostHidden]
        public ref class Scenario1 sealed
        {
        public:
            Scenario1();
    		
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
    		virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        private:
            MainPage^ rootPage;
    		Windows::UI::Core::CoreDispatcher^ dispatcher;
    		Windows::Media::PlayTo::PlayToManager^ playToManager;
    		Windows::Storage::StorageFile^ localFile;
    		Windows::Foundation::EventRegistrationToken sourceRequestedToken;
    		void webContent_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
    		void videoFile_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
    		void playButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
    		void pauseButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
    		void playToManager_SourceRequested(Windows::Media::PlayTo::PlayToManager^ sender, Windows::Media::PlayTo::PlayToSourceRequestedEventArgs^ e);
    	};
    }
}
