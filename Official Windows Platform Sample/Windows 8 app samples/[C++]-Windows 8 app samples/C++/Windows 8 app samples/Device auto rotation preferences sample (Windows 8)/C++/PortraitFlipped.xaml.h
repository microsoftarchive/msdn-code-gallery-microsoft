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
// PortraitFlipped.xaml.h
// Declaration of the PortraitFlipped class
//

#pragma once

#include "pch.h"
#include "PortraitFlipped.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace Rotation
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
    	[Windows::Foundation::Metadata::WebHostHidden]
        public ref class PortraitFlipped sealed
        {
        public:
            PortraitFlipped();
    
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        private:
            MainPage^ rootPage;
            void SetPortraitFlipped_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
    	};
    }
}
