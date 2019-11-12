//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// S1_Protect.xaml.h
// Declaration of the S1_Protect class
//

#pragma once
#include "S1_Protect.g.h"

namespace SDKSample
{
    namespace FileRevocation
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        public ref class S1_Protect sealed
        {
        public:
            S1_Protect();

        private:
            MainPage^ RootPage;
            void FileRevocation::S1_Protect::Initialize();
            void Setup_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void ProtectFile_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void ProtectFolder_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
