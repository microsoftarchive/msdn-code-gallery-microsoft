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
// GetCurrentTextSegmentFromIndex.xaml.h
// Declaration of the GetCurrentTextSegmentFromIndex class
//

#pragma once
#include "GetCurrentTextSegmentFromIndex.g.h"

namespace SDKSample
{
    namespace TextSegmentation
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        public ref class GetCurrentTextSegmentFromIndex sealed
        {
        public:
            GetCurrentTextSegmentFromIndex();

        private:
            void WordSegmentButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void SelectionSegmentButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
