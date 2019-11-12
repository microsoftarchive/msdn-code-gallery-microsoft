//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// CustomEffects.xaml.h
// Declaration of the CustomEffects class
//

#pragma once
#include "Scenario4_CustomEffects.g.h"

namespace SDKSample
{
    namespace MediaExtensions
    {
        /// <summary>
        /// Custom Effects scenario
        /// </summary>
        public ref class CustomEffects sealed
        {
        public:
            CustomEffects();

        private:
			void OpenGrayscale_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
			void OpenFisheye_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
			void OpenPinch_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
			void OpenWarp_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
			void OpenInvert_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
			void Stop_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

	        void OpenVideoWithPolarEffect(Platform::String^ effectName);
        };
    }
}
