//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// S3_GetStatus.xaml.h
// Declaration of the S3_GetStatus class
//

#pragma once
#include "S3_GetStatus.g.h"

namespace SDKSample
{
    namespace FileRevocation
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        public ref class S3_GetStatus sealed
        {
        public:
            S3_GetStatus();

        private:
            MainPage^ RootPage;
            void GetStatus_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
