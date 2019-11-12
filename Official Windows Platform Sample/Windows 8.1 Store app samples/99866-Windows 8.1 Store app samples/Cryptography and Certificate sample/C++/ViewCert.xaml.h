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
// ViewCert.xaml.h
// Declaration of the ViewCert class
//

#pragma once

#include "pch.h"
#include "ViewCert.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace CryptographyAndCertificate
    {

        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class ViewCert sealed
        {
        public:
            ViewCert();

        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

        private:
            MainPage^ rootPage;

            Windows::Foundation::Collections::IVectorView<Windows::Security::Cryptography::Certificates::Certificate ^> ^ certList;
            void EnumerateVerifyList();
            void EnumerateCertificateList();
            void RunSample_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void DisplayCertificate(Windows::Security::Cryptography::Certificates::Certificate^ selectedcertificate);
            void CertificateList_SelectionChanged(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void VerifyCert_SelectionChaged(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
