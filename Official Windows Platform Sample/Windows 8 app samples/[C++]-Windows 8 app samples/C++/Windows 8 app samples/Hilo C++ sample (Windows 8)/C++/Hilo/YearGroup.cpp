// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#include "pch.h"
#include "YearGroup.h"
#include "MonthBlock.h"
#include "ExceptionPolicy.h"
#include "LocalResourceLoader.h"
#include "Repository.h"
#include "CalendarExtensions.h"

using namespace concurrency;
using namespace Hilo;
using namespace Platform;
using namespace Platform::Collections;
using namespace std;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Storage::Search;

YearGroup::YearGroup(DateTime yearDate, IStorageFolderQueryOperations^ folderQuery, shared_ptr<Repository> repository, shared_ptr<ExceptionPolicy> exceptionPolicy) : 
    m_yearDate(yearDate), m_folderQuery(folderQuery), m_repository(repository), m_exceptionPolicy(exceptionPolicy)
{
    m_year = CalendarExtensions::GetNumericYear(yearDate);
    m_name = CalendarExtensions::GetLocalizedYear(yearDate);
}

void YearGroup::OnPropertyChanged(String^ propertyName)
{
    assert(IsMainThread());
    PropertyChanged(this, ref new Windows::UI::Xaml::Data::PropertyChangedEventArgs(propertyName));
}

IObservableVector<IMonthBlock^>^ YearGroup::Items::get()
{
    if (nullptr == m_months)
    {
        m_months = ref new Vector<IMonthBlock^>();
        auto yearDate = m_yearDate;
        auto nMonths = CalendarExtensions::GetNumberOfMonthsInYear(yearDate);
        vector<IMonthBlock^> monthBlocks;
        monthBlocks.reserve(nMonths);
        for (int month = 1; month <= nMonths; month++)
        {
            auto monthBlock = ref new MonthBlock(this, month, m_folderQuery, m_repository, m_exceptionPolicy);
            monthBlocks.push_back(monthBlock);
        }
        m_months = ref new Vector<IMonthBlock^>(std::move(monthBlocks));
    }
    return m_months;
}

String^ YearGroup::Title::get()
{
    return m_name;
}

int YearGroup::Year::get()
{
    return m_year;
}