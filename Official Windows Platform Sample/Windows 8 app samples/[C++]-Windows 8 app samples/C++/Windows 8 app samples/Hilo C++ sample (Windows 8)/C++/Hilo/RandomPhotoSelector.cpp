// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#include "pch.h"
#include "RandomPhotoSelector.h"
#include <numeric>
#include <time.h>

using namespace concurrency;
using namespace std;
using namespace Windows::Storage;
using namespace Windows::Foundation::Collections;
using namespace Hilo;

// See http://go.microsoft.com/fwlink/?LinkId=267275 for info about Hilo's implementation of tiles.

task<IVector<StorageFile^>^> RandomPhotoSelector::SelectFilesAsync(IVectorView<StorageFile^>^ photos, unsigned int count )
{
    return create_task(
        [photos, count]() -> IVector<StorageFile^>^
    {
        auto selectedImages = RandomPhotoSelector::CreateRandomizedVector(photos->Size, count);

        auto selectedFiles = ref new Platform::Collections::Vector<Windows::Storage::StorageFile^>();

        for (unsigned int imageCounter = 0; imageCounter < selectedImages.size(); imageCounter++)
        {
            auto imageFile = photos->GetAt(selectedImages[imageCounter]);
            selectedFiles->Append(imageFile);
        }

        return selectedFiles;
    });
}

vector<unsigned int> RandomPhotoSelector::CreateRandomizedVector(unsigned int vectorSize, unsigned int sampleSize)
{
    // Seed the rand() function, which is used by random_shuffle.
    srand(static_cast<unsigned int>(time(nullptr)));

    // The resulting set of random numbers.
    vector<unsigned int> result(vectorSize);

    // Fill with [0..vectorSize).
    iota(begin(result), end(result), 0);

    // Shuffle the elements.
    random_shuffle(begin(result), end(result));

    // Trim the list to the first sampleSize elements if the collection size is greater than the sample size.
    if (vectorSize > sampleSize)
    {
        result.resize(sampleSize);
    }

    return result;
}
