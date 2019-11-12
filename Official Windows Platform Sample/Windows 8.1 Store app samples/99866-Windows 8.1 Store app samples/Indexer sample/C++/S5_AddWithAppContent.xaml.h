//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// S5_AddWithAppContent.xaml.h
// Declaration of the S5_AddWithAppContent class
//

#pragma once
#include "S5_AddWithAppContent.g.h"

namespace SDKSample
{
    namespace Indexer
    {
        public ref class S5_AddWithAppContent sealed
        {
        public:
            S5_AddWithAppContent();

        private:
            void AddToIndex_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
