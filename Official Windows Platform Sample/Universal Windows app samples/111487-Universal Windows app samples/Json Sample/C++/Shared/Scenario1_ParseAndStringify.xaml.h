//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// S1_ParseAndStringify.xaml.h
// Declaration of the S1_ParseAndStringify class
//

#pragma once
#include "Scenario1_ParseAndStringify.g.h"

namespace SDKSample
{
    namespace Json
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        public ref class S1_ParseAndStringify sealed
        {
        public:
            S1_ParseAndStringify();

        private:
            void Parse_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void Stringify_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void Add_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            bool IsExceptionHandled(Platform::Exception^ ex);
        };
    }
}
