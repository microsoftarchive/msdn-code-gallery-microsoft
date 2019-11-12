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
    namespace Scaling
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
    
        private:
            static const int DEFAULT_LOGICALPPI = 96;
            static const int PERCENT = 100;
    
            Windows::UI::Xaml::Media::Imaging::BitmapImage^ image100;
            Windows::UI::Xaml::Media::Imaging::BitmapImage^ image140;
            Windows::UI::Xaml::Media::Imaging::BitmapImage^ image180;
    
            Platform::String^ GetMinDPI();
            Platform::String^ GetMinResolution();
            void SetManualLoadImage();
            void ResetOutput();
            void DisplayProperties_LogicalDpiChanged(Platform::Object^ sender);
        };
    }
}
