//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// S1-DirectContentUris.xaml.h
// Declaration of the DirectContentUriScenario class
//

#pragma once
#include "S1-DirectContentUris.g.h"

namespace SDKSample
{
    namespace SDKTemplate
    {
        public ref class DirectContentUriScenario sealed
        {
        public:
            DirectContentUriScenario();

        private:
            void AddDirectUris_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void ClearDirectUris_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

            void UpdateUriTable();
            void UpdateIfUriIsInCache(Windows::Foundation::Uri ^uri, Windows::UI::Xaml::Controls::TextBlock ^cacheStatusTextBlock);
        };
    }
}
