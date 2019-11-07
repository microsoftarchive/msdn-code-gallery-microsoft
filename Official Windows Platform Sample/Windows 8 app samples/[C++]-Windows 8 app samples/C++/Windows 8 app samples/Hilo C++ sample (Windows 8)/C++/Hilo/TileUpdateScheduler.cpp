// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#include "pch.h"
#include "TileUpdateScheduler.h"
#include "ThumbnailGenerator.h"
#include "Repository.h"
#include "RandomPhotoSelector.h"
#include "WideFiveImageTile.h"
#include "TaskExceptionsExtensions.h"
#include "ExceptionPolicyFactory.h"

using namespace concurrency;
using namespace Hilo;
using namespace std;
using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::Foundation::Collections;
using namespace Windows::Storage;
using namespace Windows::UI::Notifications;

// See http://go.microsoft.com/fwlink/?LinkId=267275 for info about Hilo's implementation of tiles.

// Specifies the name of the local app folder that holds the thumbnail images.
String^ ThumbnailsFolderName = "thumbnails";

// Specifies the number of photos in a batch.  A batch is applied to a single 
// tile update.
const unsigned int BatchSize = 5;
// Specifies the number of batches. The Windows Runtime rotates through 
// multiple batches.
const unsigned int SetSize = 3;

TileUpdateScheduler::TileUpdateScheduler()
{
}

// Select random pictures from the Pictures library and copy them 
// to a local app folder and then update the tile.
task<void> TileUpdateScheduler::ScheduleUpdateAsync(std::shared_ptr<Repository> repository, std::shared_ptr<ExceptionPolicy> policy)
{
    // The storage folder that holds the thumbnails.
    auto thumbnailStorageFolder = make_shared<StorageFolder^>(nullptr);

    return create_task(
        // Create a folder to hold the thumbnails.
        // The ReplaceExisting option specifies to replace the contents of any existing folder with a new, empty folder.
        ApplicationData::Current->LocalFolder->CreateFolderAsync(
            ThumbnailsFolderName, 
            CreationCollisionOption::ReplaceExisting)).then([repository, thumbnailStorageFolder](StorageFolder^ createdFolder) 
    {
        assert(IsBackgroundThread());
        (*thumbnailStorageFolder) = createdFolder;

        // Collect a multiple of the batch and set size of the most recent photos from the library. 
        // Later a random set is selected from this collection for thumbnail image generation.
        return repository->GetPhotoStorageFilesAsync("", 2 * BatchSize * SetSize);
    }, task_continuation_context::use_arbitrary()).then([](IVectorView<StorageFile^>^ files) -> task<IVector<StorageFile^>^>
    {
        assert(IsBackgroundThread());
        // If we received fewer than the number in one batch,
        // return the empty collection. 
        if (files->Size < BatchSize)
        {
            return create_task_from_result(static_cast<IVector<StorageFile^>^>(
                ref new Vector<StorageFile^>()));
        }
        auto copiedFileInfos = ref new Vector<StorageFile^>(begin(files), end(files));
        return RandomPhotoSelector::SelectFilesAsync(copiedFileInfos->GetView(), SetSize * BatchSize);
    }, task_continuation_context::use_arbitrary()).then([this, thumbnailStorageFolder, policy](IVector<StorageFile^>^ selectedFiles) -> task<Vector<StorageFile^>^>
    {
        assert(IsBackgroundThread());
        // Return the empty collection if the previous step did not
        // produce enough photos.
        if (selectedFiles->Size == 0)
        {
            return create_task_from_result(ref new Vector<StorageFile^>());
        }
        ThumbnailGenerator thumbnailGenerator(policy);
        return thumbnailGenerator.Generate(selectedFiles, *thumbnailStorageFolder);
    }, task_continuation_context::use_arbitrary()).then([this](Vector<StorageFile^>^ files)
    {
        assert(IsBackgroundThread());
        // Update the tile.
        UpdateTile(files);
    }, concurrency::task_continuation_context::use_arbitrary()).then(ObserveException<void>(policy));
}

void TileUpdateScheduler::UpdateTile(IVector<StorageFile^>^ files)
{
    // Create a tile updater.
    TileUpdater^ tileUpdater = TileUpdateManager::CreateTileUpdaterForApplication();
    tileUpdater->Clear();

    unsigned int imagesCount = files->Size;
    unsigned int imageBatches = imagesCount / BatchSize;

    tileUpdater->EnableNotificationQueue(imageBatches > 0);

    for(unsigned int batch = 0; batch < imageBatches; batch++)
    {
        vector<wstring> imageList;

        // Add the selected images to the wide tile template.
        for(unsigned int image = 0; image < BatchSize; image++)
        {
            StorageFile^ file = files->GetAt(image + (batch * BatchSize));
            wstringstream imageSource;
            imageSource << L"ms-appdata:///local/" 
                << ThumbnailsFolderName->Data() 
                << L"/" 
                << file->Name->Data();
            imageList.push_back(imageSource.str());
        }

        WideFiveImageTile wideTile;
        wideTile.SetImageFilePaths(imageList);

        // Create the notification and update the tile.
        auto notification = wideTile.GetTileNotification();
        tileUpdater->Update(notification);
    }
}
