//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#pragma once
#include "ScenarioShowContactCard.g.h"

namespace SDKSample
{
    namespace ContactManager
    {
        /// <summary>
        /// A page for the 'Show contact card' scenario that can be used on its own or navigated to within a Frame.
        /// </summary>
        public ref class ScenarioShowContactCard sealed
        {
        public:
            ScenarioShowContactCard();

        private:
            void ShowContactCardButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

            // Length limits allowed by the API
            static const UINT MAX_EMAIL_ADDRESS_LENGTH = 321;
            static const UINT MAX_PHONE_NUMBER_LENGTH = 50;
        };
    }
}
