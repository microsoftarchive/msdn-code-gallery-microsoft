//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// ScenarioScanPreview.xaml.h
// Declaration of the ScenarioScanPreview class
//

#pragma once
#include "ScenarioPreviewFromFlatbed.g.h"

namespace SDKSample
{
    namespace ScanRuntimeAPI
    {
        /// <summary>
        /// Class for implementing the scenario of getting the preview from flatbed
        /// </summary>
        [ Windows::Foundation::Metadata::WebHostHidden ]
        public ref class ScenarioPreviewFromFlatbed sealed
        {
        public:
            ScenarioPreviewFromFlatbed();
            virtual void OnNavigatedTo(_In_ Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

        private:
            void StartScenario_Click(_In_ Platform::Object^ sender, _In_ Windows::UI::Xaml::RoutedEventArgs^ e);  
            void ScanPreview(_In_ Platform::String^ deviceId, _In_ Windows::Storage::Streams::IRandomAccessStream^ stream);
        };
    }
}
