//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#pragma once
#include "ScenarioShowContactCardDelayLoad.g.h"

namespace SDKSample
{
    namespace ContactManager
    {
        /// <summary>
        /// A page for the 'Show contact card with delay loaded data' scenario that can be used on its own or navigated to within a Frame.
        /// </summary>
        public ref class ScenarioShowContactCardDelayLoad sealed
        {
        public:
            ScenarioShowContactCardDelayLoad();

        private:
            void ShowContactCardDelayLoadButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            Windows::Foundation::IAsyncOperation<Windows::ApplicationModel::Contacts::Contact^>^ DownloadContactDataAsync(Windows::ApplicationModel::Contacts::Contact^ contact);
        };
    }
}
