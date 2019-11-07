//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// S1_AddWithAPI.xaml.h
// Declaration of the S1_AddWithAPI class
//

#pragma once
#include "S1_AddWithAPI.g.h"

namespace SDKSample
{
    namespace Indexer
    {
        public ref class S1_AddWithAPI sealed
        {
        public:
            S1_AddWithAPI();

        private:
            void AddToIndex_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
