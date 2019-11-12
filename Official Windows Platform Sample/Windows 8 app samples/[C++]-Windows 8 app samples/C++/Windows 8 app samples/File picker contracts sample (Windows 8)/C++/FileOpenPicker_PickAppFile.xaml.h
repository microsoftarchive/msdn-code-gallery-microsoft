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
// FileOpenPicker_PickAppFile.xaml.h
// Declaration of the FileOpenPicker_PickAppFile class
//

#pragma once

#include "pch.h"
#include "FileOpenPicker_PickAppFile.g.h"

namespace SDKSample
{
    namespace FilePickerContracts
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class FileOpenPicker_PickAppFile sealed
        {
        public:
            FileOpenPicker_PickAppFile();

        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
            virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

        internal:
            Windows::Storage::Pickers::Provider::FileOpenPickerUI^ fileOpenPickerUI;

        private:
            Platform::String^ fileID;
            Windows::Foundation::EventRegistrationToken token;

            void AddLocalFileButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void RemoveLocalFileButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void OnFileRemoved(Windows::Storage::Pickers::Provider::FileOpenPickerUI^ sender, Windows::Storage::Pickers::Provider::FileRemovedEventArgs^ e);
            void UpdateButtonState(bool fileInBasket);
        };
    }
}
