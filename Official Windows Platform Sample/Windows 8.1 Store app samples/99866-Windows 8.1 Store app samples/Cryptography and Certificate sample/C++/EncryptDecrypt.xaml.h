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
// EncryptDecrypt.xaml.h
// Declaration of the EncryptDecrypt class
//

#pragma once

#include "pch.h"
#include "EncryptDecrypt.g.h"
#include "MainPage.xaml.h"
#include <windows.security.cryptography.core.h>

namespace SDKSample
{
    namespace CryptographyAndCertificate
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class EncryptDecrypt sealed
        {
        public:
            EncryptDecrypt();

        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
            void RunEncryption_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void RunDataProtection_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

        private:
            MainPage^ rootPage;
            Platform::Array<BYTE>^ NonceBytes;
        
            Windows::Security::Cryptography::Core::CryptographicKey^ GenerateAsymmetricKey();
            Windows::Security::Cryptography::Core::CryptographicKey^ GenerateSymmetricKey();
            void SampleDataProtection(Platform::String^ descriptor);
            void SampleDataProtectionStream(Platform::String^ descriptor);
            void bSymAlgs_Checked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void bAsymAlgs_Checked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void bAuthEncrypt_Checked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void bEncryption_Checked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void bDataProtection_Checked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
