//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// GetMatchingPropertiesWithRanges.xaml.h
// Declaration of the Scenario3 class
//

#pragma once
#include "GetMatchingPropertiesWithRanges.g.h"

namespace SDKSample
{
    namespace SemanticTextQuery
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        public ref class Scenario3 sealed
        {
        public:
            Scenario3();

        private:
            void Find_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
