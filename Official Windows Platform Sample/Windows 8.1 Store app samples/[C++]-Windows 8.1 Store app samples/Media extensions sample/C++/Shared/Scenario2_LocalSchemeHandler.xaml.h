//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// LocalSchemeHandler.xaml.h
// Declaration of the LocalSchemeHandler class
//

#pragma once
#include "Scenario2_LocalSchemeHandler.g.h"

namespace SDKSample
{
    namespace MediaExtensions
    {
        /// <summary>
        /// Local Scheme Handler scenario
        /// </summary>
        public ref class LocalSchemeHandler sealed
        {
        public:
            LocalSchemeHandler();

        private:
			void Circle_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
			void Square_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
			void Triangle_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

			Windows::Media::MediaExtensionManager^ extensionManager;
		};
    }
}
