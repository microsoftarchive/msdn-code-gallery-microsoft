//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#pragma once

namespace SDKSample
{
    /// <summary>
    /// Namespace containing utility functions 
    /// </summary
    namespace Utils
    {
        Windows::UI::Xaml::Media::Imaging::BitmapImage^ GetImageFromFile(Windows::Storage::Streams::IRandomAccessStream^ stream);

        void SetImageSourceFromFile(_In_ Windows::Storage::StorageFile^ file, _Inout_ Windows::UI::Xaml::Controls::Image^ img);		

        void SetImageSourceFromStream(_In_ Windows::Storage::Streams::IRandomAccessStream^ stream, _Inout_ Windows::UI::Xaml::Controls::Image^ img);
        
        void DisplayImageAndScanCompleteMessage(_In_ Windows::Foundation::Collections::IVectorView<Windows::Storage::StorageFile^>^ FileStorageList, _Inout_ Windows::UI::Xaml::Controls::Image^ img);

        void UpdateFileListData(_In_ Windows::Foundation::Collections::IVectorView<Windows::Storage::StorageFile^>^ FileStorageList, _Inout_ Common::ModelDataContext^ Data);

        void OnScenarioException(_In_ Platform::Exception^ e, _Inout_ Common::ModelDataContext^ Data);

        void DisplayExceptionErrorMessage(_In_ Platform::Exception^ e);

        void DisplayScanCancelationMessage();
    };
}

