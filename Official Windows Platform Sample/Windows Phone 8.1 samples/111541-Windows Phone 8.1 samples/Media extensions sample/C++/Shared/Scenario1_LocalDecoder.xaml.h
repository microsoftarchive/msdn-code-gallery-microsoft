//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// LocalDecoder.xaml.h
// Declaration of the LocalDecoder class
//

#pragma once
#include "Scenario1_LocalDecoder.g.h"

namespace SDKSample
{
    namespace MediaExtensions
    {
        /// <summary>
        /// Local decoder scenario.
        /// </summary>
        public ref class LocalDecoder sealed
        {
        public:
            LocalDecoder();

        private:
			void Open_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
			void Stop_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

			Windows::Media::MediaExtensionManager^ extensionManager;
        };
    }
}
