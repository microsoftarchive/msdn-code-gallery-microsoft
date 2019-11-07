// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#include "pch.h"
#include "MonthBlock.h"
#include "ExceptionPolicy.h"
#include "IYearGroup.h"
#include "IResourceLoader.h"
#include "TaskExceptionsExtensions.h"
#include "Repository.h"
#include "CalendarExtensions.h"
using namespace concurrency;
using namespace std;
using namespace Hilo;
using namespace Platform;
using namespace Windows::Globalization;
using namespace Windows::Globalization::DateTimeFormatting;
using namespace Windows::System::UserProfile;
using namespace Windows::Storage::Search;

MonthBlock::MonthBlock(IYearGroup^ yearGroup, int month, IStorageFolderQueryOperations^ folderQuery, shared_ptr<Repository> repository, shared_ptr<ExceptionPolicy> exceptionPolicy) : 
    m_weakYearGroup(yearGroup), m_month(month), m_name(nullptr), m_folderQuery(folderQuery),
    m_repository(repository), m_exceptionPolicy(exceptionPolicy), m_count(0ul), m_runOperation(false), m_runningOperation(false)
{
}

void MonthBlock::OnPropertyChanged(String^ propertyName)
{
    assert(IsMainThread());
    PropertyChanged(this, ref new Windows::UI::Xaml::Data::PropertyChangedEventArgs(propertyName));
}

unsigned int MonthBlock::Month::get()
{
    return m_month;
}

String^ MonthBlock::Name::get()
{
    if (nullptr == m_name)
    {
        m_name = CalendarExtensions::GetLocalizedAbbreviatedMonthName(Group->Year, m_month);
    }
    return m_name;
}

IYearGroup^ MonthBlock::Group::get()
{
    return m_weakYearGroup.Resolve<IYearGroup>();
}

bool MonthBlock::HasPhotos::get()
{
    if (!m_runOperation && !m_runningOperation)
    {
        m_runningOperation = true;
        run_async_non_interactive([this]()
        {
            QueryPhotoCount().then([this]
            {
                assert(IsMainThread());
                OnPropertyChanged("HasPhotos");
            }, task_continuation_context::use_current())
                .then(ObserveException<void>(m_exceptionPolicy));
        });
    }
    return (m_count > 0);
}

task<void> MonthBlock::QueryPhotoCount()
{
    auto t = create_task_from_result(true);

    if (Group != nullptr)
    {
        // If there is a valid group we can actually build a query to get the real count.
        auto dateRangeQuery = BuildDateQuery();
        t = m_repository->HasPhotosInRangeAsync(dateRangeQuery, m_folderQuery);
    }

    return t.then([this](task<bool> priorTask)
    {
        assert(IsMainThread());
        m_runningOperation = false;
        m_count = priorTask.get() ? 1u : 0u;
        m_runOperation = true;  
    }, task_continuation_context::use_current());
}

String^ MonthBlock::BuildDateQuery()
{
    return CalendarExtensions::CreateMonthRangeFromYearAndMonth(Group->Year, m_month);
}