// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#pragma once

namespace Hilo
{
    // See http://go.microsoft.com/fwlink/?LinkId=267275 for info about Hilo's implementation of tiles.

    class ExceptionPolicy;

    // The ThumbnailGenerator class creates thumbnail images for a vector of image files.
    class ThumbnailGenerator
    {
    public:
        ThumbnailGenerator(std::shared_ptr<ExceptionPolicy> policy);
        concurrency::task<Platform::Collections::Vector<Windows::Storage::StorageFile^>^> Generate(Windows::Foundation::Collections::IVector<Windows::Storage::StorageFile^>^ files, Windows::Storage::StorageFolder^ thumbnailsFolder) ;

    private:
        static concurrency::task<Windows::Storage::Streams::InMemoryRandomAccessStream^> CreateThumbnailFromPictureFileAsync(Windows::Storage::StorageFile^ sourceFile, unsigned int thumbSize);
        static concurrency::task<Windows::Storage::StorageFile^> CreateLocalThumbnailAsync(Windows::Storage::StorageFolder^ folder, Windows::Storage::StorageFile^ imageFile, Platform::String^ localFileName, unsigned int thumbSize, std::shared_ptr<ExceptionPolicy> policy);
        static concurrency::task<Windows::Storage::StorageFile^> InternalSaveToFile(Windows::Storage::StorageFolder^ folder, Windows::Storage::Streams::InMemoryRandomAccessStream^ stream, Platform::String^ filename);

        std::shared_ptr<ExceptionPolicy> m_exceptionPolicy;
    } ; 
}

