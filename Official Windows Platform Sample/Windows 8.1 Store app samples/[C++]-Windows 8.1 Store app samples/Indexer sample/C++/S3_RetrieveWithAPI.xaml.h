//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// S3_RetrieveWithAPI.xaml.h
// Declaration of the S3_RetrieveWithAPI class
//

#pragma once
#include "S3_RetrieveWithAPI.g.h"

namespace SDKSample
{
    namespace Indexer
    {
        public ref class S3_RetrieveWithAPI sealed
        {
        public:
            S3_RetrieveWithAPI();

        private:
            void AddToIndex_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void RetrieveAllItems_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void RetrieveMatchingItems_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
