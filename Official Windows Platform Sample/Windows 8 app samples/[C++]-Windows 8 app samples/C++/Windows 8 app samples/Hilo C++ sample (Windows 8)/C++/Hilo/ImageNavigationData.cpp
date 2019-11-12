// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#include "pch.h"
#include "ImageNavigationData.h"
#include "IPhoto.h"
#include "CalendarExtensions.h"


using namespace Hilo;
using namespace Platform;
using namespace std;
using namespace Windows::Foundation;


ImageNavigationData::ImageNavigationData(IPhoto^ photo)
{
    m_fileDate = photo->DateTaken;
    m_filePath = photo->Path;
}

ImageNavigationData::ImageNavigationData(String^ serializedData)
{
    wstring data(serializedData->Data());
    auto index = data.find('|');
    assert(index > 0);

    auto path = data.substr(0, index);
    auto date = data.substr(index+1, data.length());
    DateTime fileDate;
    fileDate.UniversalTime = _wtoi64(date.c_str());

    m_filePath = ref new String(path.c_str());
    m_fileDate = fileDate;
}

DateTime ImageNavigationData::GetFileDate() const
{
    return m_fileDate;
}

String^ ImageNavigationData::GetFilePath() const
{
    return m_filePath;
}

String^ ImageNavigationData::GetDateQuery()
{
    if (nullptr == m_dateQuery)
    {
        m_dateQuery = CalendarExtensions::CreateMonthRangeFromDate(m_fileDate);
    }
    return m_dateQuery;
}

String^ ImageNavigationData::SerializeToString()
{
    wstringstream stringStream;
    stringStream << m_filePath->Data() << L"|" << m_fileDate.UniversalTime ;
    return ref new String(stringStream.str().c_str());
}
