//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// S4_Revoke.xaml.h
// Declaration of the S4_Revoke class
//

#pragma once
#include "S4_Revoke.g.h"

namespace SDKSample
{
    namespace FileRevocation
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        public ref class S4_Revoke sealed
        {
        public:
            S4_Revoke();

        private:
            MainPage^ RootPage;
            void Revoke_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
