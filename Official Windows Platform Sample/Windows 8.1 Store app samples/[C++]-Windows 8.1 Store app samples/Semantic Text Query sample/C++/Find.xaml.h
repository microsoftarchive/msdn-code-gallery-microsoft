//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Find.xaml.h
// Declaration of the Scenario1 class
//

#pragma once
#include "Find.g.h"

namespace SDKSample
{
    namespace SemanticTextQuery
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        public ref class Scenario1 sealed
        {
        public:
            Scenario1();

        private:
            void Find_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
