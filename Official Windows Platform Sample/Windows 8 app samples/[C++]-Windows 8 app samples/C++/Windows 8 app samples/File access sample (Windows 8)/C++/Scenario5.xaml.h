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
// Scenario5.xaml.h
// Declaration of the Scenario5 class
//

#pragma once

#include "pch.h"
#include "Scenario5.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace FileAccess
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        public ref class Scenario5 sealed
        {
        public:
            Scenario5();
    
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
