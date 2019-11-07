//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// ScenarioScannerEnumeration.xaml.h
// Declaration of the ScenarioScannerEnumeration class
//

#pragma once
#include "ScenarioScannerEnumeration.g.h"

#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace ScanRuntimeAPI
    {
        /// <summary>
        /// Class for implementing scanner enumeration
        /// </summary>
        [ Windows::Foundation::Metadata::WebHostHidden ]
        public ref class ScenarioScannerEnumeration sealed
        {
        public:
            ScenarioScannerEnumeration();
            virtual void OnNavigatedTo(_In_ Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
            
        private:
            void Start_Enumeration_Watcher_Click(_In_ Platform::Object^ sender, _In_ Windows::UI::Xaml::RoutedEventArgs^ e);
            void Stop_Enumeration_Watcher_Click(_In_ Platform::Object^ sender, _In_ Windows::UI::Xaml::RoutedEventArgs^ e);
            void UpdateStartStopButtons();
        };
    }
}
