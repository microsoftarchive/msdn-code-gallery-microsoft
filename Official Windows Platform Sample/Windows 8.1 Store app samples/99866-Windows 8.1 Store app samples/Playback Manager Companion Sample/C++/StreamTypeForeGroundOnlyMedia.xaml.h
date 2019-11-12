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
// StreamTypeForegroundOnlyMedia.xaml.h
// Declaration of the StreamTypeForegroundOnlyMedia class
//

#pragma once

#include "pch.h"
#include "StreamTypeForegroundOnlyMedia.g.h"
#include "MainPage.xaml.h"
#include "PlaybackControl.xaml.h"

namespace SDKSample
{
    namespace PlaybackManager2
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
    	[Windows::Foundation::Metadata::WebHostHidden]
        public ref class StreamTypeForegroundOnlyMedia sealed
        {
        public:
            StreamTypeForegroundOnlyMedia();
    
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        private:
            MainPage^ rootPage;
            void Default_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
