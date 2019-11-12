//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// ScenarioScanFromFeeder.xaml.h
// Declaration of the ScenarioScanFromFeeder class
//

#pragma once
#include "ScenarioScanFromFeeder.g.h"

namespace SDKSample
{
    namespace ScanRuntimeAPI
    {
        /// <summary>
        /// Class for implementing Scan from Feeder Scenario
        /// </summary>
        [Windows::UI::Xaml::Data::Bindable]
        [Windows::Foundation::Metadata::WebHostHidden ]
        public ref class ScenarioScanFromFeeder sealed
        {
        public:
            ScenarioScanFromFeeder();
            virtual void OnNavigatedTo(_In_ Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;	
            virtual void OnNavigatedFrom(_In_ Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;		

        private:
            void StartScenario_Click(_In_ Platform::Object^ sender, _In_ Windows::UI::Xaml::RoutedEventArgs^ e);
            void CancelScenario_Click(_In_ Platform::Object^ sender, _In_ Windows::UI::Xaml::RoutedEventArgs^ e);
            void ScanToFolder(_In_ Platform::String^ deviceId, _In_ Windows::Storage::StorageFolder^ destinationFolder);
            void Progress(_In_ Windows::Foundation::IAsyncOperationWithProgress<Windows::Devices::Scanners::ImageScannerScanResult^, UINT32>^ operation, UINT32 numberOfScannedFiles);
            void CancelScanning();

            Concurrency::cancellation_token_source cancellationToken;
            
        };
    }
}
