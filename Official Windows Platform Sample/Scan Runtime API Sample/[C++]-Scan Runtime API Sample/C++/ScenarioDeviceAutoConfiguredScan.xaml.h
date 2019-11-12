//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// ScenarioAutoConfiguration.xaml.h
// Declaration of the ScenarioAutoConfiguration class
//

#pragma once
#include "ScenarioDeviceAutoConfiguredScan.g.h"

namespace SDKSample
{
    namespace ScanRuntimeAPI
    {
        /// <summary>
        /// Class for implementation of Auto Configuraiton Scenario
        /// </summary>
        [ Windows::Foundation::Metadata::WebHostHidden ]
        public ref class ScenarioDeviceAutoConfiguredScan sealed
        {
        public:
            ScenarioDeviceAutoConfiguredScan();
            virtual void OnNavigatedTo(_In_ Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
            virtual void OnNavigatedFrom(_In_ Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

        private:
            void StartScenario_Click(_In_ Platform::Object^ sender, _In_ Windows::UI::Xaml::RoutedEventArgs^ e);
            void CancelScenario_Click(_In_ Platform::Object^ sender, _In_ Windows::UI::Xaml::RoutedEventArgs^ e);
            void Progress(_In_ Windows::Foundation::IAsyncOperationWithProgress< Windows::Devices::Scanners::ImageScannerScanResult^, UINT32>^ operation, UINT32 numberOfScannedFiles);
            void ScanToFolder(_In_ Platform::String^ deviceId, _In_ Windows::Storage::StorageFolder^ destinationFolder);
            void CancelScanning();

            Concurrency::cancellation_token_source cancellationToken;
        };
    }
}
