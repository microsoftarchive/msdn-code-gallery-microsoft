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
// SpellingTextSuggestions.xaml.h
// Declaration of the SpellingTextSuggestions class
//

#pragma once

#include "pch.h"
#include "SpellingTextSuggestions.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace TouchKeyboardTextInput
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
    	[Windows::Foundation::Metadata::WebHostHidden]
        public ref class SpellingTextSuggestions sealed
        {
        public:
            SpellingTextSuggestions();
    
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        private:
            MainPage^ rootPage;
        };
    }
}
