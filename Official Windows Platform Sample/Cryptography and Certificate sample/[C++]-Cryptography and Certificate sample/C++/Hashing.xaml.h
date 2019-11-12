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
// Hashing.xaml.h
// Declaration of the Hashing class
//

#pragma once
#include "pch.h"
#include "MainPage.xaml.h"
#include "Hashing.g.h"

namespace SDKSample
{
    namespace CryptographyAndCertificate
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class Hashing sealed
        {
        public:
            Hashing();

        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

        private:
            MainPage^ rootPage;
            Windows::Storage::Streams::IBuffer^ digest;

            void RunSample_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            Windows::Security::Cryptography::Core::CryptographicHash^ CreateHashCryptographicHash();
            Windows::Security::Cryptography::Core::CryptographicHash^ CreateHmacCryptographicHash();
            void bHash_Checked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void bHmac_Checked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
