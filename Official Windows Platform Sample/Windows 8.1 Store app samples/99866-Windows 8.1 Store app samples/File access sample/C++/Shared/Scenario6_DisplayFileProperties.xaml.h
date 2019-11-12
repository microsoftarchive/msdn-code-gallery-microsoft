// Copyright (c) Microsoft. All rights reserved.

#pragma once

#include "pch.h"
#include "Scenario6_DisplayFileProperties.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace FileAccess
    {
        /// <summary>
        /// Displaying file properties.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class Scenario6 sealed
        {
        public:
            Scenario6();

        private:
            static Windows::Globalization::DateTimeFormatting::DateTimeFormatter^ dateFormat;
            static Windows::Globalization::DateTimeFormatting::DateTimeFormatter^ timeFormat;
            static Platform::String^ dateAccessedProperty;
            static Platform::String^ fileOwnerProperty;

            MainPage^ rootPage;

            void ShowPropertiesButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
