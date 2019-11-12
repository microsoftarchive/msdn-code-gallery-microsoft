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
// Scenario2.xaml.h
// Declaration of the Scenario2 class
//

#pragma once

#include "pch.h"
#include "Scenario2.g.h"
#include "MainPage.xaml.h"
#include "Collection.h"

namespace SDKSample
{
    namespace BackgroundTransfer
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
    	[Windows::Foundation::Metadata::WebHostHidden]
        public ref class Scenario2 sealed
        {
        public:
            Scenario2();
    
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
    
        private:
            MainPage^ rootPage;
            Concurrency::cancellation_token_source* cancellationToken;
    
            void StartUpload_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void StartMultipartUpload_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void CancelAll_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void DiscoverActiveUploads();
            void UploadProgress(
                Windows::Foundation::IAsyncOperationWithProgress<Windows::Networking::BackgroundTransfer::UploadOperation^, Windows::Networking::BackgroundTransfer::UploadOperation^>^ operation,
                Windows::Networking::BackgroundTransfer::UploadOperation^);
            void HandleUploadAsync(Windows::Networking::BackgroundTransfer::UploadOperation^ upload, bool start);
            void LogException(Platform::String^ title, Platform::Exception^ ex);
            void MarshalLog(Platform::String^ value);
            void Log(Platform::String^ message);
            void LogStatus(Platform::String^ message, NotifyType type);
            ~Scenario2();
        };
    }
}
