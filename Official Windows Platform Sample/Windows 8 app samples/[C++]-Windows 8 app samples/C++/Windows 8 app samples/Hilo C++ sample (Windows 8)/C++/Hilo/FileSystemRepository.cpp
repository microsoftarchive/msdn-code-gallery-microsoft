// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#include "pch.h"
#include "FileSystemRepository.h"
#include "CalendarExtensions.h"
#include "NullPhotoGroup.h"
#include "Photo.h"
#include "QueryChange.h"

using namespace concurrency;
using namespace Hilo;
using namespace Platform;
using namespace Platform::Collections;
using namespace std;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Storage;
using namespace Windows::Storage::BulkAccess;
using namespace Windows::Storage::FileProperties;
using namespace Windows::Storage::Search;

// See http://go.microsoft.com/fwlink/?LinkId=267277 for info about the FileSystemRepository class.

#pragma region File System Queries

const std::array<String^, 6> items = { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tif" };

inline task<IVectorView<StorageFile^>^> FileSystemRepository::GetPhotoStorageFilesAsync(String^ query, unsigned int maxNumberOfItems)
{
    auto fileQuery = CreateFileQuery(KnownFolders::PicturesLibrary, query);
    return create_task(fileQuery->GetFilesAsync(0, maxNumberOfItems));
}

inline StorageFileQueryResult^ FileSystemRepository::CreateFileQuery(IStorageFolderQueryOperations^ folder, String^ query, IndexerOption indexerOption)
{
    auto fileTypeFilter = ref new Vector<String^>(items);
    auto queryOptions = ref new QueryOptions(CommonFileQuery::OrderByDate, fileTypeFilter);
    queryOptions->FolderDepth = FolderDepth::Deep;
    queryOptions->IndexerOption = indexerOption;
    queryOptions->ApplicationSearchFilter = query;
    queryOptions->SetThumbnailPrefetch(ThumbnailMode::PicturesView, 190, ThumbnailOptions::UseCurrentScale);
    queryOptions->Language = CalendarExtensions::ResolvedLanguage();
    return folder->CreateFileQueryWithOptions(queryOptions);
}

#pragma endregion

FileSystemRepository::FileSystemRepository(shared_ptr<ExceptionPolicy> exceptionPolicy) : m_exceptionPolicy(exceptionPolicy)
{
}

#pragma region ObservableQuery Methods
void FileSystemRepository::AddObserver(const std::function<void()> callback, PageType pageType)
{
    switch (pageType)
    {
    case PageType::Hub:
        m_hubViewModelCallback = callback;
        break;
    case PageType::Browse:
        m_imageBrowserViewModelCallback = callback;
        break;
    case PageType::Image:
        m_imageViewModelCallback = callback;
        break;
    default:
        assert(false);
    }
}

void FileSystemRepository::RemoveObserver(PageType pageType)
{
    switch (pageType)
    {
    case PageType::Hub:
        m_pictureHubGroupQueryChange = nullptr;
        m_hubViewModelCallback = nullptr;
        break;
    case PageType::Browse:
        m_monthQueryChange = nullptr;
        m_imageBrowserViewModelCallback = nullptr;
        break;
    case PageType::Image:
        m_allPhotosQueryChange = nullptr;
        m_imageViewModelCallback = nullptr;
        break;
    default:
        assert(false);
    }
}  
#pragma endregion


void FileSystemRepository::NotifyAllObservers()
{
    if (m_hubViewModelCallback != nullptr)
    {
        m_hubViewModelCallback();
    }
    if (m_imageBrowserViewModelCallback != nullptr)
    {
        m_imageBrowserViewModelCallback();
    }
    if (m_imageViewModelCallback = nullptr)
    {
        m_imageViewModelCallback();
    }
}

// Create month groups by querying the file system.
task<IVectorView<IPhotoGroup^>^> FileSystemRepository::GetMonthGroupedPhotosWithCacheAsync(shared_ptr<PhotoCache> photoCache, concurrency::cancellation_token token)
{
    auto queryOptions = ref new QueryOptions(CommonFolderQuery::GroupByMonth);
    queryOptions->FolderDepth = FolderDepth::Deep;
    queryOptions->IndexerOption = IndexerOption::UseIndexerWhenAvailable;
    queryOptions->Language = CalendarExtensions::ResolvedLanguage();
    auto fileQuery = KnownFolders::PicturesLibrary->CreateFolderQueryWithOptions(queryOptions);
    auto fileInformationFactory = ref new FileInformationFactory(fileQuery, ThumbnailMode::PicturesView);

    m_monthQueryChange = (m_imageBrowserViewModelCallback != nullptr) ? ref new QueryChange(fileQuery, m_imageBrowserViewModelCallback) : nullptr;

    shared_ptr<ExceptionPolicy> policy = m_exceptionPolicy;
    auto sharedThis = shared_from_this();
    return create_task(fileInformationFactory->GetFoldersAsync()).then([this, fileInformationFactory, photoCache, sharedThis, policy](IVectorView<FolderInformation^>^ folders) 
    {
        auto temp = ref new Vector<IPhotoGroup^>();
        for (auto folder : folders)
        {
            auto photoGroup = ref new MonthGroup(photoCache, folder, sharedThis, policy);
            temp->Append(photoGroup);
        }
        return temp->GetView();
    }, token);
}

// Query the file system using a pathname.
task<IPhoto^> FileSystemRepository::GetSinglePhotoAsync(String^ photoPath)
{
    String^ query = "System.ParsingPath:=\"" + photoPath + "\"";    
    auto fileQuery = CreateFileQuery(KnownFolders::PicturesLibrary, query, IndexerOption::DoNotUseIndexer);
    auto fileInformationFactory = ref new FileInformationFactory(fileQuery, ThumbnailMode::PicturesView);
    shared_ptr<ExceptionPolicy> policy = m_exceptionPolicy;
    return create_task(fileInformationFactory->GetFilesAsync(0, 1)).then([policy](IVectorView<FileInformation^>^ files) 
    {
        IPhoto^ photo = nullptr;
        auto size = files->Size;
        if (size > 0)
        {
            photo = ref new Photo(files->GetAt(0), ref new NullPhotoGroup(), policy);
        }
        return photo;
    }, task_continuation_context::use_current());
}

// Count the number of photos in a given folder query.
task<unsigned int> FileSystemRepository::GetFolderPhotoCountAsync(IStorageFolderQueryOperations^ folderQuery)
{
    auto fileTypeFilter = ref new Vector<String^>(items);
    auto queryOptions = ref new QueryOptions(CommonFileQuery::OrderByDate, fileTypeFilter);  // ordered query allows use of indexer
    queryOptions->FolderDepth = FolderDepth::Deep;
    queryOptions->IndexerOption = IndexerOption::UseIndexerWhenAvailable;
    queryOptions->Language = CalendarExtensions::ResolvedLanguage();
    auto fileQuery = folderQuery->CreateFileQueryWithOptions(queryOptions);
    return create_task(fileQuery->GetFilesAsync(0, 1)).then([fileQuery](IVectorView<StorageFile^>^ files)
    {
        return fileQuery->GetItemCountAsync();
    });
}

// Retrieves a given number of photos from a folder query.
task<IVectorView<IPhoto^>^> FileSystemRepository::GetPhotoDataForMonthGroup(IPhotoGroup^ photoGroup, IStorageFolderQueryOperations^ folderQuery, unsigned int maxNumberOfItems)
{
    auto fileQuery = CreateFileQuery(folderQuery, "");
    auto fileInformationFactory = ref new FileInformationFactory(fileQuery, ThumbnailMode::PicturesView, 190, ThumbnailOptions::UseCurrentScale, true);  // speed up query by deferring thumbnails
    shared_ptr<ExceptionPolicy> policy = m_exceptionPolicy;
    return create_task(fileInformationFactory->GetFilesAsync(0, maxNumberOfItems)).then([photoGroup, policy, maxNumberOfItems](IVectorView<FileInformation^>^ files) 
    {
        auto photos = ref new Vector<IPhoto^>();
        for (auto item : files)
        {
            auto photo = ref new Photo(item, photoGroup, policy);
            photos->Append(photo);
        }
        return photos->GetView();
    }, task_continuation_context::use_current());
}

// Returns true if there are any photos in the given folder within the specified date range. Returns false otherwise.
task<bool> FileSystemRepository::HasPhotosInRangeAsync(Platform::String^ dateRangeQuery, IStorageFolderQueryOperations^ folderQuery)
{
    auto fileTypeFilter = ref new Vector<String^>(items);
    auto queryOptions = ref new QueryOptions(CommonFileQuery::OrderByDate, fileTypeFilter);  // ordered query allows use of indexer
    queryOptions->FolderDepth = FolderDepth::Deep;
    queryOptions->IndexerOption = IndexerOption::UseIndexerWhenAvailable;
    queryOptions->Language = CalendarExtensions::ResolvedLanguage();
    queryOptions->ApplicationSearchFilter = dateRangeQuery;
    auto fileQuery = folderQuery->CreateFileQueryWithOptions(queryOptions);
    return create_task(fileQuery->GetFilesAsync(0, 1)).then([](IVectorView<StorageFile^>^ files)
    {
        return (files->Size > 0);
    });
}

// Retrieves the photos from the user's picture library with dates in a given range.
task<IVectorView<IPhoto^>^> FileSystemRepository::GetPhotosForDateRangeQueryAsync(String^ dateRangeQuery)
{
    auto fileQuery = CreateFileQuery(KnownFolders::PicturesLibrary, dateRangeQuery);
    auto fileInformationFactory = ref new FileInformationFactory(fileQuery, ThumbnailMode::PicturesView);
    shared_ptr<ExceptionPolicy> policy = m_exceptionPolicy;

    m_allPhotosQueryChange = (m_imageViewModelCallback != nullptr) ? ref new QueryChange(fileQuery, m_imageViewModelCallback) : nullptr;

    return create_task(fileInformationFactory->GetFilesAsync()).then([policy](IVectorView<FileInformation^>^ files) 
    {
        auto photos = ref new Vector<IPhoto^>();
        for (auto file : files)
        {
            auto photo = ref new Photo(file, ref new NullPhotoGroup(), policy);
            photos->Append(photo);
        }
        return photos->GetView();
    }, task_continuation_context::use_current());
}

// Selects a set of recent photos.
task<IVectorView<IPhoto^>^> FileSystemRepository::GetPhotosForPictureHubGroupAsync(IPhotoGroup^ photoGroup, unsigned int maxNumberOfItems)
{
    auto fileQuery = CreateFileQuery(KnownFolders::PicturesLibrary, "");
    auto fileInformationFactory = ref new FileInformationFactory(fileQuery, ThumbnailMode::PicturesView);
    m_pictureHubGroupQueryChange = (m_hubViewModelCallback != nullptr) ? ref new QueryChange(fileQuery, m_hubViewModelCallback) : nullptr;
    shared_ptr<ExceptionPolicy> policy = m_exceptionPolicy;
    return create_task(fileInformationFactory->GetFilesAsync(0, maxNumberOfItems)).then([photoGroup, policy](IVectorView<FileInformation^>^ files) 
    {
        auto photos = ref new Vector<IPhoto^>();
        for (auto item : files)
        {
            auto photo = ref new Photo(item, photoGroup, policy);
            photos->Append(photo);
        }
        return photos->GetView();

    }, task_continuation_context::use_current());
}

// Queries the file system for photos grouped by year. There will be one year group for each year that has photos.
task<IVectorView<IYearGroup^>^> FileSystemRepository::GetYearGroupedMonthsAsync(cancellation_token token)
{
    shared_ptr<ExceptionPolicy> policy = m_exceptionPolicy;
    auto queryOptions = ref new QueryOptions(CommonFolderQuery::GroupByYear);
    queryOptions->FolderDepth = FolderDepth::Deep;
    queryOptions->IndexerOption = IndexerOption::UseIndexerWhenAvailable;
    queryOptions->Language = CalendarExtensions::ResolvedLanguage();
    auto fileQuery =  KnownFolders::PicturesLibrary->CreateFolderQueryWithOptions(queryOptions);
    auto fileInformationFactory = ref new FileInformationFactory(fileQuery, ThumbnailMode::PicturesView);
    return create_task(fileInformationFactory->GetFoldersAsync()).then([this, policy](IVectorView<FolderInformation^>^ folders) 
    {
        vector<task<YearGroup^>> yearGroupTasks;
        for (auto folder : folders)
        {
            yearGroupTasks.push_back(GetDateTimeForYearFolderAsync(folder, policy));
        }
        return when_all(begin(yearGroupTasks), end(yearGroupTasks)).then([](vector<YearGroup^> groups)
        {
            auto result = ref new Vector<IYearGroup^>();
            for (auto group : groups)
            {
                if (group != nullptr)
                {
                    result->Append(group);
                }
            }
            return result->GetView();
        });
    }, token, task_continuation_context::use_arbitrary());
}

// Helper method for GetYearGroupedMonthsAsync. Creates a YearGroup object for each folder created by the year query.
task<YearGroup^> FileSystemRepository::GetDateTimeForYearFolderAsync(IStorageFolderQueryOperations^ folderQuery, shared_ptr<ExceptionPolicy> exceptionPolicy)
{
    auto maxNumberOfItems = 1;
    auto dateTakenProperties = ref new Vector<String^>();
    dateTakenProperties->Append("System.ItemDate");
    auto sharedThis = shared_from_this();
    auto fileTypeFilter = ref new Vector<String^>(items);
    auto queryOptions = ref new QueryOptions(CommonFileQuery::OrderByDate, fileTypeFilter);
    queryOptions->IndexerOption = IndexerOption::UseIndexerWhenAvailable;
    queryOptions->SetPropertyPrefetch(PropertyPrefetchOptions::ImageProperties, dateTakenProperties);
    queryOptions->Language = CalendarExtensions::ResolvedLanguage();
    auto fileQuery = folderQuery->CreateFileQueryWithOptions(queryOptions);
    auto fileInformationFactory = ref new FileInformationFactory(fileQuery, ThumbnailMode::PicturesView);
    return create_task(fileInformationFactory->GetFilesAsync(0, maxNumberOfItems)).then([sharedThis, folderQuery, exceptionPolicy](IVectorView<FileInformation^>^ files) -> YearGroup^
    {
        if (files->Size > 0)
        {
            auto file = files->GetAt(0);
            auto yearDate = file->ImageProperties->DateTaken;
            if (yearDate.UniversalTime == 0)
            {
                yearDate = file->BasicProperties->DateModified;
            }
            return (yearDate.UniversalTime != 0) ? ref new YearGroup(yearDate, folderQuery, sharedThis, exceptionPolicy) : nullptr; 
        }
        else
        {
            return nullptr;
        }
    }, task_continuation_context::use_arbitrary()).then(ObserveException<YearGroup^>(exceptionPolicy));
}


