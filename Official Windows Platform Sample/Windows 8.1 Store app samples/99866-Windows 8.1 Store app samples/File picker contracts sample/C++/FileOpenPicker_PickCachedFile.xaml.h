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
// FileOpenPicker_PickCachedFile.xaml.h
// Declaration of the FileOpenPicker_PickCachedFile class
//

#pragma once

#include "pch.h"
#include "FileOpenPicker_PickCachedFile.g.h"

namespace SDKSample
{
    namespace FilePickerContracts
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class FileOpenPicker_PickCachedFile sealed
        {
        public:
            FileOpenPicker_PickCachedFile();

        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
            virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

        internal:
            Windows::Storage::Pickers::Provider::FileOpenPickerUI^ fileOpenPickerUI;

        private:
            Platform::String^ fileID;
            Windows::Foundation::EventRegistrationToken token;

            void AddFileButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void RemoveFileButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void OnFileRemoved(Windows::Storage::Pickers::Provider::FileOpenPickerUI^ sender, Windows::Storage::Pickers::Provider::FileRemovedEventArgs^ e);
            void UpdateButtonState(bool fileInBasket);
        };
    }
}
