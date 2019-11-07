//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// S2_CopyProtection.xaml.h
// Declaration of the S2_CopyProtection class
//

#pragma once
#include "S2_CopyProtection.g.h"

namespace SDKSample
{
    namespace FileRevocation
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        public ref class S2_CopyProtection sealed
        {
        public:
            S2_CopyProtection();

        private:
            MainPage^ RootPage;
            void CopyProtectionToFile_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void CopyProtectionToFolder_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
