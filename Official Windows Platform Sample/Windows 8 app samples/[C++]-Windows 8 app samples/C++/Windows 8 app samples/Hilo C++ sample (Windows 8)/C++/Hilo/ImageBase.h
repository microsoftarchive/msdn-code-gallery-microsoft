// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#pragma once

#include "ViewModelBase.h"

namespace Hilo
{
    interface class IPhoto;
    class ExceptionPolicy;

    // The ImageBase class provides load and save functionality for image files.
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class ImageBase : public ViewModelBase
    {

    private protected:
        IPhoto^ m_photo;

    internal:
        ImageBase(std::shared_ptr<ExceptionPolicy> exceptionPolicy);

        concurrency::task<Windows::Storage::Streams::IRandomAccessStreamWithContentType^> GetStreamWithFailCheck();
        concurrency::task<Windows::Storage::StorageFile^> GetFileNameFromFileSavePickerAsync(Platform::String^ fileType);
        concurrency::task<void> SaveImageAsync(Windows::Storage::StorageFile^ file, Windows::Storage::Streams::IRandomAccessStream^ ras);
    };
}
