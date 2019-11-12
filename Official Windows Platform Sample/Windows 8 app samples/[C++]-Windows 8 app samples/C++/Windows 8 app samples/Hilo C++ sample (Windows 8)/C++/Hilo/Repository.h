// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#pragma once

#include "ObservableQuery.h"

namespace Hilo
{
    interface class IPhoto;
    interface class IPhotoGroup;
    interface class IYearGroup;
    class PhotoCache;

    // The Repository class provides the app's interface to file system queries.
    // See http://go.microsoft.com/fwlink/?LinkId=267277 for more info about this class.
    class Repository : public ObservableQuery
    {
    public:
        virtual concurrency::task<Windows::Foundation::Collections::IVectorView<IPhotoGroup^>^> GetMonthGroupedPhotosWithCacheAsync(std::shared_ptr<PhotoCache> photoCache, concurrency::cancellation_token token) = 0;
        virtual concurrency::task<IPhoto^> GetSinglePhotoAsync(Platform::String^ photoPath) = 0;
        virtual concurrency::task<unsigned int> GetFolderPhotoCountAsync(Windows::Storage::Search::IStorageFolderQueryOperations^ folderQuery) = 0;
        virtual concurrency::task<Windows::Foundation::Collections::IVectorView<IPhoto^>^> GetPhotoDataForMonthGroup(IPhotoGroup^ photoGroup, Windows::Storage::Search::IStorageFolderQueryOperations^ folderQuery, unsigned int maxNumberOfItems) = 0;
        virtual concurrency::task<Windows::Foundation::Collections::IVectorView<IPhoto^>^> GetPhotosForDateRangeQueryAsync(Platform::String^ dateRangeQuery) = 0;
                virtual concurrency::task<bool> HasPhotosInRangeAsync(Platform::String^ dateRangeQuery, Windows::Storage::Search::IStorageFolderQueryOperations^ folderQuery) = 0;
        virtual concurrency::task<Windows::Foundation::Collections::IVectorView<IPhoto^>^> GetPhotosForPictureHubGroupAsync(IPhotoGroup^ photoGroup, unsigned int maxNumberOfItems) = 0;
        virtual concurrency::task<Windows::Foundation::Collections::IVectorView<IYearGroup^>^> GetYearGroupedMonthsAsync(concurrency::cancellation_token token) = 0;
        virtual concurrency::task<Windows::Foundation::Collections::IVectorView<Windows::Storage::StorageFile^>^> GetPhotoStorageFilesAsync(Platform::String^ query, unsigned int maxNumberOfItems) = 0;
        virtual void NotifyAllObservers() = 0;
    };
}
