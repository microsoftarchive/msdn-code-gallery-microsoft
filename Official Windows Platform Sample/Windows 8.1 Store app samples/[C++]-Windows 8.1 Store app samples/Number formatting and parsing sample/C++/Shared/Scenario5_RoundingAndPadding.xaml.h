//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

//
// RoundingAndPadding.xaml.h
// Declaration of the RoundingAndPadding class
//

#pragma once

#include "pch.h"
#include "Scenario5_RoundingAndPadding.g.h"
#include "MainPage.xaml.h"

using namespace Platform;
using namespace Windows::Globalization::NumberFormatting;

namespace SDKSample
{
    namespace NumberFormatting
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class RoundingAndPadding sealed
        {
        public:
            RoundingAndPadding();
    
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        private:
            MainPage^ rootPage;
            
            // Class methods that implement the scenarios
            String^ DisplayRoundingAlgorithmAsString(RoundingAlgorithm roundingAlgorithm); 
            String^ DoPaddingScenarios(INumberFormatter^ formatter);
            String^ DoPaddingAndRoundingScenarios(RoundingAlgorithm roundingAlgorithm, unsigned int significantDigits, int integerDigits, int fractionalDigits);
            String^ DoCurrencyRoundingScenarios(String^ currencyCode, RoundingAlgorithm roundingAlgorithm);
            String^ DoIncrementRoundingScenarios(RoundingAlgorithm roundingAlgorithm);
            String^ DoSignificantDigitRoundingScenarios(RoundingAlgorithm roundingAlgorithm);

            void Display_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
