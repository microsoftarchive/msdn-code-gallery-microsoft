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

    // The RandomPhotoSelector static class creates a randomly shuffled vector of image files.
    class RandomPhotoSelector
    {
    public:
        static concurrency::task<Windows::Foundation::Collections::IVector<Windows::Storage::StorageFile^>^> SelectFilesAsync(Windows::Foundation::Collections::IVectorView<Windows::Storage::StorageFile^>^ photos, unsigned int count );
        static std::vector<unsigned int> CreateRandomizedVector(unsigned int vectorSize, unsigned int sampleSize);
    };
}