//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// AddImageFeedScenario.xaml.h
// Declaration of the AddImageFeedScenario class
//

#pragma once
#include "AddImageFeedScenario.g.h"

namespace SDKSample
{
    namespace Personalization
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        public ref class AddImageFeedScenario sealed
        {
        public:
            AddImageFeedScenario();

        private:
            void SetDefaultButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void RemoveFeedButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
