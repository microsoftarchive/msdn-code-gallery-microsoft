// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#pragma once

#include "Repository.h"

namespace Hilo
{
    class ExceptionPolicy;
    ref class QueryChange;

    // The FileSystemRepository class queries the user's picture library for images.
    // See http://go.microsoft.com/fwlink/?LinkId=267277 for more info about this class.
    class FileSystemRepository : public Repository, public std::enable_shared_from_this<FileSystemRepository>
    {
    public:
        FileSystemRepository(std::shared_ptr<ExceptionPolicy> exceptionPolicy);

        virtual void AddObserver(const std::function<void()> callback, PageType pageType);
        virtual void RemoveObserver(PageType pageType);

        virtual concurrency::task<Windows::Foundation::Collections::IVectorView<IPhotoGroup^>^> GetMonthGroupedPhotosWithCacheAsync(std::shared_ptr<Hilo::PhotoCache> photoCache, concurrency::cancellation_token token);
        virtual concurrency::task<IPhoto^> GetSinglePhotoAsync(Platform::String^ photoPath);
        virtual concurrency::task<unsigned int> GetFolderPhotoCountAsync(Windows::Storage::Search::IStorageFolderQueryOperations^ folderQuery);
        virtual concurrency::task<Windows::Foundation::Collections::IVectorView<IPhoto^>^> GetPhotoDataForMonthGroup(IPhotoGroup^ photoGroup, Windows::Storage::Search::IStorageFolderQueryOperations^ folderQuery, unsigned int maxNumberOfItems);
        virtual concurrency::task<bool> HasPhotosInRangeAsync(Platform::String^ dateRangeQuery, Windows::Storage::Search::IStorageFolderQueryOperations^ folderQuery);
        virtual concurrency::task<Windows::Foundation::Collections::IVectorView<IPhoto^>^> GetPhotosForDateRangeQueryAsync(Platform::String^ dateRangeQuery);
        virtual concurrency::task<Windows::Foundation::Collections::IVectorView<IPhoto^>^> GetPhotosForPictureHubGroupAsync(IPhotoGroup^ photoGroup, unsigned int maxNumberOfItem);
        virtual concurrency::task<Windows::Foundation::Collections::IVectorView<IYearGroup^>^> GetYearGroupedMonthsAsync(concurrency::cancellation_token token);
        virtual inline concurrency::task<Windows::Foundation::Collections::IVectorView<Windows::Storage::StorageFile^>^> GetPhotoStorageFilesAsync(Platform::String^ query, unsigned int maxNumberOfItems = 25);
        virtual void NotifyAllObservers();

    private:
        std::shared_ptr<ExceptionPolicy> m_exceptionPolicy;
        std::function<void()> m_hubViewModelCallback;
        std::function<void()> m_imageBrowserViewModelCallback;
        std::function<void()> m_imageViewModelCallback;
        QueryChange^ m_allPhotosQueryChange;
        QueryChange^ m_pictureHubGroupQueryChange;
        QueryChange^ m_monthQueryChange;

        inline Windows::Storage::Search::StorageFileQueryResult^ CreateFileQuery(Windows::Storage::Search::IStorageFolderQueryOperations^ folder, Platform::String^ query, Windows::Storage::Search::IndexerOption option = Windows::Storage::Search::IndexerOption::UseIndexerWhenAvailable);
        concurrency::task<YearGroup^> GetDateTimeForYearFolderAsync(Windows::Storage::Search::IStorageFolderQueryOperations^ folderQuery, std::shared_ptr<ExceptionPolicy> exceptionPolicy);
    };
}
