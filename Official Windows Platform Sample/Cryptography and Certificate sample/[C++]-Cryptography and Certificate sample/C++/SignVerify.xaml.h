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
// SignVerify.xaml.h
// Declaration of the SignVerify class
//

#pragma once

#include "pch.h"
#include "SignVerify.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace CryptographyAndCertificate
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class SignVerify sealed
        {
        public:
            SignVerify();

        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

        private:
            MainPage^ rootPage;

            void RunSample_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void AsymmetricImportExport(Windows::Security::Cryptography::Core::CryptographicKey^ keyPair);
            Windows::Security::Cryptography::Core::CryptographicKey^ GenerateHMACKey();
            Windows::Security::Cryptography::Core::CryptographicKey^ SignVerify::GenerateAsymmetricKey();
            void bHmac_Checked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void bAsymmetric_Checked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
