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
// S1_ListStorages.xaml.h
// Declaration of the S1_ListStorages class
//

#pragma once

#include "pch.h"
#include "S1_ListStorages.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace RemovableStorageCPP
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        public ref class S1_ListStorages sealed
        {
        public:
            S1_ListStorages();

        private:
            void ListStorages_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            MainPage^ rootPage;
        };
    }
}
