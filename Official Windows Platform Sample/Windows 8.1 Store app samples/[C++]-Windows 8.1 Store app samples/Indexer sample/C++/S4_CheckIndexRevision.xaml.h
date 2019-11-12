//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// S4_CheckIndexRevision.xaml.h
// Declaration of the S4_CheckIndexRevision class
//

#pragma once
#include "S4_CheckIndexRevision.g.h"

namespace SDKSample
{
    namespace Indexer
    {
        public ref class S4_CheckIndexRevision sealed
        {
        public:
            S4_CheckIndexRevision();

        private:
            void CheckIndexRevision_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
