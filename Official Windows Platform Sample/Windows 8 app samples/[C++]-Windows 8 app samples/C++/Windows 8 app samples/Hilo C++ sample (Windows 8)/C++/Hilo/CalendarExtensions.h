// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#pragma once

namespace Hilo
{
    // The CalendarExtensions class contains helper functions for manipulating dates.
    class CalendarExtensions 
    {
    public:
        static Platform::String^ CreateMonthRangeFromYearAndMonth(int year, int month);
        static Platform::String^ CreateMonthRangeFromDate(const Windows::Foundation::DateTime date);
        static int GetNumberOfMonthsInYear(const Windows::Foundation::DateTime yearDate);
        static Platform::String^ GetLocalizedAbbreviatedMonthName(int year, int month);
        static Platform::String^ GetLocalizedMonthAndYear(const Windows::Foundation::DateTime date);
        static Platform::String^ GetLocalizedYear(const Windows::Foundation::DateTime yearDate);
        static int GetNumericYear(const Windows::Foundation::DateTime yearDate);
        static void WriteLocalizedYearAndMonth(const Windows::Foundation::DateTime date, int& yearResult, int& monthResult);
        static Platform::String^ ResolvedLanguage();
        static Platform::String^ ResolvedGeographicRegion();

    private:
        static std::wstring GetAqsFormattedDate(const Windows::Foundation::DateTime date);
        static Platform::String^ CreateMonthRangeFromDate(Windows::Globalization::Calendar^ cal);
        static Windows::Globalization::Calendar^ GetCalendar();
    };
}