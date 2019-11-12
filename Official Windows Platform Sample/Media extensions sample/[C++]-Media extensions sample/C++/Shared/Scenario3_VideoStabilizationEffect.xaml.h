//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// VideoStabilizationEffect.xaml.h
// Declaration of the VideoStabilizationEffect class
//

#pragma once
#include "Scenario3_VideoStabilizationEffect.g.h"

namespace SDKSample
{
    namespace MediaExtensions
    {
        /// <summary>
        /// Video Stabilization Effect Scenario
        /// </summary>
        public ref class VideoStabilizationEffect sealed
        {
        public:
            VideoStabilizationEffect();

		private:
			void Open_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
			void Stop_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
