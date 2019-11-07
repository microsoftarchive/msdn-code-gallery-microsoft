//*********************************************************
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF 
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
// PARTICULAR PURPOSE. 
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// FindId.xaml.h
// Declaration of the FindId class
//

#pragma once
#include "FindId.g.h"

using namespace Platform;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Globalization;

namespace SDKSample
{
    namespace UnicodeSample
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        public ref class FindId sealed
        {
        public:
            FindId();

        private:
			Platform::Collections::Vector<String^>^ FindIdsInString(String^ inputString);
			String^ DoFindIdsInStringScenario(String ^scenarioString);

            void Default_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
