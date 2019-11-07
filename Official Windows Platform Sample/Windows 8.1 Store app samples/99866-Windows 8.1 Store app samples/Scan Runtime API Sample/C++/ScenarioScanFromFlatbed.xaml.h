//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// ScenarioScanFromFlatbed.xaml.h
// Declaration of the ScenarioScanFromFlatbed class
//

#pragma once
#include "ScenarioScanFromFlatbed.g.h"

namespace SDKSample
{
    namespace ScanRuntimeAPI
    {
        /// <summary>
        /// Class for implementation of Scanning from flatbed scenario
        /// </summary>
        [ Windows::Foundation::Metadata::WebHostHidden ]
        public ref class ScenarioScanFromFlatbed sealed
        {
        public:
            ScenarioScanFromFlatbed();
            virtual void OnNavigatedTo(_In_ Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
            virtual void OnNavigatedFrom(_In_ Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

        private:
            void StartScenario_Click(_In_ Platform::Object^ sender, _In_ Windows::UI::Xaml::RoutedEventArgs^ e);
            void CancelScenario_Click(_In_ Platform::Object^ sender, _In_ Windows::UI::Xaml::RoutedEventArgs^ e);
            void ScanToFolder(_In_ Platform::String^ deviceId, _In_ Windows::Storage::StorageFolder^ destinationFolder);
            void CancelScanning();

            Concurrency::cancellation_token_source cancellationToken;
            
        };
    }
}
