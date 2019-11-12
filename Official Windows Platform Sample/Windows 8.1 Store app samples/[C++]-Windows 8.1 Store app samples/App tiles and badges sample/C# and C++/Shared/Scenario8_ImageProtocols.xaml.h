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
// ImageProtocols.xaml.h
// Declaration of the ImageProtocols class
//

#pragma once

#include "pch.h"
#include "Scenario8_ImageProtocols.g.h"
#include "MainPage.xaml.h"

#if WINAPI_FAMILY == WINAPI_FAMILY_PHONE_APP
#include "ContinuationManager.h"
using namespace Windows::ApplicationModel::Activation;
#endif

namespace SDKSample
{
    namespace Tiles
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
#if WINAPI_FAMILY == WINAPI_FAMILY_PHONE_APP
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class ImageProtocols sealed : IFileOpenPickerContinuable
            #else
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class ImageProtocols sealed
#endif
        {
        public:
            ImageProtocols();

#if WINAPI_FAMILY == WINAPI_FAMILY_PHONE_APP
            virtual void ContinueFileOpenPicker(FileOpenPickerContinuationEventArgs^ args);
#endif

        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

        private:
            MainPage^ rootPage;
            Platform::String^ imageRelativePath;
            void ProtocolList_SelectionChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::SelectionChangedEventArgs^ e);
            void SendTileNotification_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void PickImage_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void CopyImageToLocalFolder(Windows::Storage::StorageFile^ file);
        };
    }
}