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
// Scenario1.xaml.h
// Declaration of the Scenario1 class
//

#pragma once

#include "pch.h"
#include "Scenario1.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace BackgroundTransfer
    {
        struct GuidComparer
        {
          bool operator() (const Platform::Guid& lhs, const Platform::Guid& rhs) const
          {
              return false;
          }
        };
    
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
    	[Windows::Foundation::Metadata::WebHostHidden]
        public ref class Scenario1 sealed
        {
        public:
            Scenario1();
    
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
    
        private:
            MainPage^ rootPage;
            Concurrency::cancellation_token_source* cancellationToken;
            std::map<int /*Platform::Guid*/, Windows::Networking::BackgroundTransfer::DownloadOperation^> activeDownloads;
    
            void StartDownload_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void CancelAll_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void ResumeAll_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void PauseAll_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void HandleDownloadAsync(Windows::Networking::BackgroundTransfer::DownloadOperation^ download, boolean start);
            void DownloadProgress(
                Windows::Foundation::IAsyncOperationWithProgress<Windows::Networking::BackgroundTransfer::DownloadOperation^, Windows::Networking::BackgroundTransfer::DownloadOperation^>^ operation,
                Windows::Networking::BackgroundTransfer::DownloadOperation^);
            void DiscoverActiveDownloads();
            void LogException(Platform::String^ title, Platform::Exception^ ex);
            void MarshalLog(Platform::String^ value);
            void Log(Platform::String^ message);
            void LogStatus(Platform::String^ message, NotifyType type);
            ~Scenario1();
        };
    }
}
