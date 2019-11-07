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
// CachedFileUpdater_Local.xaml.h
// Declaration of the CachedFileUpdater_Local class
//

#pragma once

#include "pch.h"
#include "CachedFileUpdater_Local.g.h"

namespace SDKSample
{
    namespace FilePickerContracts
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        public ref class CachedFileUpdater_Local sealed
        {
        public:
            CachedFileUpdater_Local();

        private:
            void FileIsCurrentButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void ProvideUpdatedVersionButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void UpdateUI(Windows::Storage::Provider::UIStatus uiStatus);
            void OutputFileAsync(Windows::Storage::StorageFile^ file);
        };
    }
}
