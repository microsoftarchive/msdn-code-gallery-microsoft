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
// Scenario2.xaml.h
// Declaration of the Scenario2 class
//

#pragma once

#include "pch.h"
#include "Scenario2.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace PlayTo
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
    	[Windows::Foundation::Metadata::WebHostHidden]
        public ref class Scenario2 sealed
        {
        public:
            Scenario2();
    
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
    		virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        private:
            MainPage^ rootPage;
    		Windows::UI::Core::CoreDispatcher^ dispatcher;
    		Windows::UI::Xaml::DispatcherTimer^ playlistTimer;
    		Windows::Media::PlayTo::PlayToManager^ playToManager;
    		Windows::Foundation::EventRegistrationToken sourceRequestedToken;
    		Windows::Foundation::EventRegistrationToken timerTickToken;
    
    		void playlistTimer_Tick(Platform::Object^ sender, Platform::Object^ e);
    		void playToManager_SourceRequested(Windows::Media::PlayTo::PlayToManager^ sender, Windows::Media::PlayTo::PlayToSourceRequestedEventArgs^ e);
            void playSlideshow_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
    		void pauseSlideshow_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
    		void previousItem_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
    		void nextItem_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
    		void playListPlayNext();
    		void Playlist_SelectionChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::SelectionChangedEventArgs^ e);
    	};
    }
}
