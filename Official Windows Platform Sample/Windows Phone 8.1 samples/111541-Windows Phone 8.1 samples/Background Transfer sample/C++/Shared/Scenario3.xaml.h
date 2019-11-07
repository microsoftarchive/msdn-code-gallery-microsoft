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
// Scenario3.xaml.h
// Declaration of the Scenario3 class
//

#pragma once

#include "pch.h"
#include "Scenario3.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace BackgroundTransfer
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
    	[Windows::Foundation::Metadata::WebHostHidden]
        public ref class Scenario3 sealed
        {
        public:
            Scenario3();
    
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

        private:
            MainPage^ rootPage;
            Windows::Networking::BackgroundTransfer::BackgroundTransferGroup^ notificationsGroup;

            static UINT32 runId;

            static const UINT64 tenMinutesIn100NanoSecondUnits = 6000000000;

            enum ScenarioType
            {
                Toast,
                Tile
            };

            void ToastNotificationButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void TileNotificationButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            
            void StartDownload(
                Windows::Networking::BackgroundTransfer::BackgroundTransferPriority priority,
                boolean requestUnconstrainedDownload);
            void RunDownloads(
                Windows::Networking::BackgroundTransfer::BackgroundDownloader^ downloader,
                ScenarioType type);
            bool TryCreateDownloadAsync(
                Windows::Networking::BackgroundTransfer::BackgroundDownloader^ downloader, 
                UINT8 delaySeconds, 
                Platform::String^ fileNameSuffix, 
                ScenarioType type,
                Platform::Collections::Vector<Windows::Networking::BackgroundTransfer::DownloadOperation^>^ downloadOperations,
                Concurrency::task<void>& resultTask);
            void RunDownload(Windows::Networking::BackgroundTransfer::DownloadOperation^ download);
            void CancelActiveDownloads();
            void LogException(
                Platform::String^ title, 
                Platform::Exception^ ex, 
                Windows::Networking::BackgroundTransfer::DownloadOperation^ download = nullptr);
            void Log(Platform::String^ message);
            void LogStatus(Platform::String^ message, NotifyType type);
            
            ~Scenario3();
        };
    }
}
