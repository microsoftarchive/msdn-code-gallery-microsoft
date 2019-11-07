// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#include "pch.h"
#include "MonthGroup.h"
#include "TaskExceptionsExtensions.h"
#include "PhotoCache.h"
#include "Repository.h"
#include "IPhoto.h"
#include "ExceptionPolicy.h"
#include "CalendarExtensions.h"

using namespace concurrency;
using namespace Hilo;
using namespace Platform;
using namespace Platform::Collections;
using namespace std;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Storage::Search;

const unsigned int MaxNumberOfPictures = 8;

MonthGroup::MonthGroup(shared_ptr<PhotoCache> photoCache, IStorageFolderQueryOperations^ folderQuery, 
    shared_ptr<Repository> repository, shared_ptr<ExceptionPolicy> exceptionPolicy) : 
    m_count(0u), m_weakPhotoCache(photoCache), m_folderQuery(folderQuery), m_repository(repository), m_exceptionPolicy(exceptionPolicy), 
    m_hasCount(false), m_runningQuery(false)
{
    m_dateTimeForTitle.UniversalTime = 0ll;
}

void MonthGroup::OnPropertyChanged(String^ propertyName)
{
    assert(IsMainThread());
    PropertyChanged(this, ref new Windows::UI::Xaml::Data::PropertyChangedEventArgs(propertyName));
}

IObservableVector<IPhoto^>^ MonthGroup::Items::get()
{
    assert(IsMainThread());
    if (nullptr == m_photos)
    {
        m_photos = ref new Vector<IPhoto^>();

        if (!m_runningQuery)
        {
            m_runningQuery = true;
            OnPropertyChanged("IsRunning");
            QueryPhotosAsync().then([this](task<DateTime> priorTask)
            {
                m_runningQuery = false;
                m_dateTimeForTitle = priorTask.get();
                OnPropertyChanged("Items");
                OnPropertyChanged("Title");                  
                OnPropertyChanged("IsRunning");                    
            }).then([this]() {
                run_async_non_interactive([this]() {
                    if (m_dateTimeForTitle.UniversalTime > 0ll)
                    {
                        m_repository->GetFolderPhotoCountAsync(m_folderQuery).then([this](size_t count)
                        {
                            assert(IsMainThread());
                            m_count = count;
                            OnPropertyChanged("Title");   
                            OnPropertyChanged("HasPhotos");
                        }, task_continuation_context::use_current()).then(ObserveException<void>(m_exceptionPolicy));
                    }
                });
            })
                .then(ObserveException<void>(m_exceptionPolicy));
        }
    }
    return m_photos;
}

bool MonthGroup::HasPhotos::get()
{
    return m_count > 0;
}

bool MonthGroup::IsRunning::get()
{
    return m_runningQuery;
}

task<DateTime> MonthGroup::QueryPhotosAsync()
{
    // for unit tests only
    if (nullptr == m_photos)
    {
        m_photos = ref new Vector<IPhoto^>();
    }

    auto photosTask = m_repository->GetPhotoDataForMonthGroup(this, m_folderQuery, MaxNumberOfPictures);
    return photosTask.then([this](IVectorView<IPhoto^>^ photos)
    {
        assert(IsMainThread());
        bool first = true;
        shared_ptr<PhotoCache> cache = m_weakPhotoCache.lock();
        DateTime dateTime = {0ll};

        for (auto item : photos)
        {
            // Add this photo to the display graph.
            m_photos->Append(item);
            if (first)
            {
                // Cache a handle to the first photo. We will need this later when we want to scroll the grid of month
                // groups to a chosen month. (We can only scroll to a given item, not to a property of that item.)
                cache->InsertPhoto(item);

                // Remember the date of the first picture. We need this later to correctly display the name of the month group.
                dateTime = item->DateTaken;
            }
            first = false;
        }
        return dateTime;
    });
}

String^ MonthGroup::Title::get()
{
    // Set title to month name if available
    if (m_title == nullptr && !m_runningQuery && m_dateTimeForTitle.UniversalTime != 0ll)
    {
        m_title = CalendarExtensions::GetLocalizedMonthAndYear(m_dateTimeForTitle);
    }

    // Add count to title if available (this will be after the month name becomes available)
    if (m_title != nullptr && m_count > 0 && !m_hasCount)
    {
        auto formatter = ref new Windows::Globalization::NumberFormatting::DecimalFormatter();
        formatter->FractionDigits = 0;
        auto count = formatter->FormatUInt(m_count);
        m_title = m_title + " (" + count + ")";
        m_hasCount = true;
    }
    return m_title;
}
