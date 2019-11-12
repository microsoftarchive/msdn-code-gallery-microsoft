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
// StreamTypeMedia.xaml.h
// Declaration of the StreamTypeMedia class
//

#pragma once

#include "pch.h"
#include "StreamTypeMedia.g.h"
#include "MainPage.xaml.h"
#include "Playback.h"

namespace SDKSample
{
    namespace PlaybackManager2
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
    	public ref class StreamTypeMedia sealed
        {
        public:
            StreamTypeMedia();
    
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        private:
            MainPage^ rootPage;
            void Default_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
    		Playback^ _playback;
    	};
    }
}
