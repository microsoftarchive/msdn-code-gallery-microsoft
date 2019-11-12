//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// S2-IndirectContentUri.xaml.h
// Declaration of the IndirectContentUriScenario class
//

#pragma once
#include "S2-IndirectContentUri.g.h"

namespace SDKSample
{
    namespace SDKTemplate
    {
        public ref class IndirectContentUriScenario sealed
        {
        public:
            IndirectContentUriScenario();

        private:
            void SetIndirectContentUri_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void ClearIndirectContentUri_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

            void UpdateIndirectUriBlock();
        };
    }
}
