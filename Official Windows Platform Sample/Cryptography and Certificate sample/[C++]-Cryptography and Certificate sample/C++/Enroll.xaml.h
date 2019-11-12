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
// Enroll.xaml.h
// Declaration of the Enroll class
//

#pragma once

#include "pch.h"
#include "Enroll.g.h"
#include "MainPage.xaml.h"
#include "HttpRequest.h"

namespace SDKSample
{
    namespace CryptographyAndCertificate
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class Enroll sealed
        {
        public:
            Enroll();

        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

        private:
            MainPage^ rootPage;
            Platform::String^ certRequest;
            
            void RunSampleUserEnroll(Platform::String^ url);
            void RunSampleAppEnroll(Platform::String^ url);
            Platform::String^ FormatHttpRequest(Platform::String^ encodedRequest);
            Platform::String^ GetCertFromXmlResponse(Platform::String^ xmlResponse);
            void RunSample_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
