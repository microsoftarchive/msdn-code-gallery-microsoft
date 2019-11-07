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
// Scenario2.xaml.h
// Declaration of the Scenario2 class
//

#pragma once

#include "pch.h"
#include "Scenario2.g.h"
#include "MainPage.xaml.h"
#include "Collection.h"

namespace SDKSample
{
    namespace BackgroundTransfer
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        #if WINAPI_FAMILY == WINAPI_FAMILY_PHONE_APP
        public ref class Scenario2 sealed : public IFileOpenPickerContinuable
        #else
        public ref class Scenario2 sealed
        #endif
        {
        public:
            Scenario2();

            #if WINAPI_FAMILY == WINAPI_FAMILY_PHONE_APP
            virtual void ContinueFileOpenPicker(Windows::ApplicationModel::Activation::FileOpenPickerContinuationEventArgs^ args);
            #endif

        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

        private:
            MainPage^ rootPage;
            Concurrency::cancellation_token_source* cancellationToken;

            static const UINT32 maxUploadFileSize = 100 * 1024 * 1024; // 100 MB (arbitrary limit chosen for this sample)

            ref class SizeCounter sealed
            {
            public:
                SizeCounter();

                void Add(INT64 size);

                INT64 GetSize();

            private:
                INT64 size;
            };

            void StartUpload_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void StartMultipartUpload_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void CancelAll_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void DiscoverActiveUploads();
            void UploadProgress(
                Windows::Foundation::IAsyncOperationWithProgress<Windows::Networking::BackgroundTransfer::UploadOperation^, Windows::Networking::BackgroundTransfer::UploadOperation^>^ operation,
                Windows::Networking::BackgroundTransfer::UploadOperation^);
            void HandleUploadAsync(Windows::Networking::BackgroundTransfer::UploadOperation^ upload, bool start);
            void LogException(Platform::String^ title, Platform::Exception^ ex);
            void MarshalLog(Platform::String^ value);
            void Log(Platform::String^ message);
            void LogStatus(Platform::String^ message, NotifyType type);

            Concurrency::task<void> UploadSingleFileAsync(Windows::Foundation::Uri^ uri, Windows::Storage::IStorageFile^ file);
            Concurrency::task<void> UploadMultipleFilesAsync(
                Windows::Foundation::Uri^ uri,
                Windows::Foundation::Collections::IVectorView<Windows::Storage::StorageFile^>^ files);
            Concurrency::task<Platform::Collections::Vector<Windows::Networking::BackgroundTransfer::BackgroundTransferContentPart^>^> CreatePartsAsync(
                Windows::Foundation::Collections::IVectorView<Windows::Storage::StorageFile^>^ files);

            ~Scenario2();
        };
    }
}
