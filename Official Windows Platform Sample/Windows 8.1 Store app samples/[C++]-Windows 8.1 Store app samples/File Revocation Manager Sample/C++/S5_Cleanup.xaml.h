//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// S5_Cleanup.xaml.h
// Declaration of the S5_Cleanup class
//

#pragma once
#include "S5_Cleanup.g.h"

namespace SDKSample
{
    namespace FileRevocation
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        public ref class S5_Cleanup sealed
        {
        public:
            S5_Cleanup();

        private:
            MainPage^ RootPage;
            void Cleanup_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
