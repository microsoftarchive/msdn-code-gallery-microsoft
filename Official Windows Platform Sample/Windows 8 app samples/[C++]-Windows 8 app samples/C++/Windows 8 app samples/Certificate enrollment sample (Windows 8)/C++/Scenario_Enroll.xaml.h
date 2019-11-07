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
// Scenario_Enroll.xaml.h
// Declaration of the Scenario_Enroll class
//

#pragma once

#include "pch.h"
#include "Scenario_Enroll.g.h"
#include "MainPage.xaml.h"
#include "HttpRequest.h"

namespace SDKSample
{
    namespace CertificateEnrollment
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class Scenario_Enroll sealed
        {
        public:
            Scenario_Enroll();
    
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        private:
            MainPage^ rootPage;
            Platform::String ^ certificateRequest;
			
            Web::HttpRequest httpRequest;
            void CreateRequest_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void InstallCertifiate_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void SubmitCertificateRequestAndGetResponse(Platform::String^ certificateRequest, Platform::String^ url);
        };
    }
}
