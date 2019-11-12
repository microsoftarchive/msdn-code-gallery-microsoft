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
// FileSavePicker_SaveToCachedFile.xaml.h
// Declaration of the FileSavePicker_SaveToCachedFile class
//

#pragma once

#include "pch.h"
#include "FileSavePicker_SaveToCachedFile.g.h"

namespace SDKSample
{
    namespace FilePickerContracts
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class FileSavePicker_SaveToCachedFile sealed
        {
        public:
            FileSavePicker_SaveToCachedFile();

        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
            virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

        internal:
            Windows::Storage::Pickers::Provider::FileSavePickerUI^ fileSavePickerUI;

        private:
            Platform::String^ fileID;
            Windows::Foundation::EventRegistrationToken token;

            void OnTargetFileRequested(Windows::Storage::Pickers::Provider::FileSavePickerUI^ sender,  Windows::Storage::Pickers::Provider::TargetFileRequestedEventArgs^ e);
        };
    }
}