//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// S6_DeleteWithAppContent.xaml.h
// Declaration of the S6_DeleteWithAppContent class
//

#pragma once
#include "S6_DeleteWithAppContent.g.h"

namespace SDKSample
{
    namespace Indexer
    {
        public ref class S6_DeleteWithAppContent sealed
        {
        public:
            S6_DeleteWithAppContent();

        private:
            void AddToIndex_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void DeleteSingleItem_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void DeleteAllItems_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
