//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// S7_RetrieveWithAppContent.xaml.h
// Declaration of the S7_RetrieveWithAppContent class
//

#pragma once
#include "S7_RetrieveWithAppContent.g.h"

namespace SDKSample
{
    namespace Indexer
    {
        public ref class S7_RetrieveWithAppContent sealed
        {
        public:
            S7_RetrieveWithAppContent();

        private:
            void AddToIndex_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void RetrieveAllItems_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void RetrieveMatchedItems_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
