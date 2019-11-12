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
// UsingUnicodeExtensions.xaml.h
// Declaration of the UsingUnicodeExtensions class
//

#pragma once

#include "pch.h"
#include "Scenario7_UsingUnicodeExtensions.g.h"
#include "MainPage.xaml.h"

using namespace Platform;
using namespace Windows::Globalization::NumberFormatting;

namespace SDKSample
{
    namespace NumberFormatting
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class UsingUnicodeExtensions sealed
        {
        public:
            UsingUnicodeExtensions();
    
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        private:
            MainPage^ rootPage;
            void Display_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
