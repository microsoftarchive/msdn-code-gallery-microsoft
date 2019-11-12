// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#include "pch.h"
#include "CalendarExtensions.h"

using namespace Hilo;
using namespace Platform;
using namespace Platform::Collections;
using namespace std;
using namespace Windows::Foundation;
using namespace Windows::Globalization;
using namespace Windows::Globalization::DateTimeFormatting;
using namespace Windows::System::UserProfile;

const int DATE_BUFFER_SIZE = 128;

// Creates a formatted date string that will be compatible with Advanced Query Syntax (AQS).
wstring CalendarExtensions::GetAqsFormattedDate(const DateTime date)
{
    auto utime = date.UniversalTime;

    FILETIME time;
    time.dwHighDateTime = (DWORD)(utime >> 32);
    time.dwLowDateTime = (DWORD)(utime & 0xFFFFFFFF);
    
    BOOL result;
    SYSTEMTIME systime;

    result = FileTimeToSystemTime(
        /* __in   const FILETIME *lpFileTime */ &time, 
        /* __out  LPSYSTEMTIME lpSystemTime */  &systime
        );
    if (!result)
    {
        throw Exception::CreateException(HRESULT_FROM_WIN32(GetLastError()));
    }

    SYSTEMTIME localSystime;
    TIME_ZONE_INFORMATION timeZoneInfo;
    GetTimeZoneInformation(&timeZoneInfo);
    result = SystemTimeToTzSpecificLocalTime(
        /* _In_opt_  LPTIME_ZONE_INFORMATION lpTimeZone */ &timeZoneInfo,
        /* _In_      LPSYSTEMTIME lpUniversalTime*/        &systime,
        /* _Out_     LPSYSTEMTIME lpLocalTime */           &localSystime
        );
    if (!result)
    {
        throw Exception::CreateException(HRESULT_FROM_WIN32(GetLastError()));
    }

    WCHAR dateStr[DATE_BUFFER_SIZE];
    result = GetDateFormatEx(
        /* __in_opt   LPCWSTR lpLocaleName */     LOCALE_NAME_USER_DEFAULT,
        /* __in       DWORD dwFlags */            DATE_SHORTDATE,
        /* __in_opt   const SYSTEMTIME *lpDate */ &localSystime,
        /* __in_opt   LPCWSTR lpFormat */         NULL,
        /* __out_opt  LPWSTR lpDateStr */         dateStr,
        /* __in       int cchDate */              DATE_BUFFER_SIZE,
        /* __in_opt   LPCWSTR lpCalendar */       NULL
        );
    if (!result)
    {
        throw Exception::CreateException(HRESULT_FROM_WIN32(GetLastError()));
    }

    return wstring(dateStr);
}

// private helper-- creates an AQS date-range expression that spans the month that contains the given date in the given calendar system.
String^ CalendarExtensions::CreateMonthRangeFromDate(Calendar^ cal)
{
    wstringstream dateRange;
    dateRange << L"System.ItemDate:" ;

    cal->Day = cal->FirstDayInThisMonth;
    cal->Period = cal->FirstPeriodInThisDay;
    cal->Hour = cal->FirstHourInThisPeriod;
    cal->Minute = cal->FirstMinuteInThisHour;
    cal->Second = cal->FirstSecondInThisMinute;
    cal->Nanosecond = 0;
    dateRange << GetAqsFormattedDate(cal->GetDateTime()); 

    dateRange << "..";

    cal->Day = cal->LastDayInThisMonth;
    cal->Period = cal->LastPeriodInThisDay;
    cal->Hour = cal->LastHourInThisPeriod;
    cal->Minute = cal->LastMinuteInThisHour;
    cal->Second = cal->LastSecondInThisMinute;
    cal->Nanosecond = 999999;
    dateRange << GetAqsFormattedDate(cal->GetDateTime()); 

    return ref new String(dateRange.str().c_str());
}

Calendar^ CalendarExtensions::GetCalendar()
{
    return ref new Calendar();
}

// Uses the appropriate calendar to return an Advanced Query Syntax (AQS) date range 
// expression that includes all dates in this month.
String^ CalendarExtensions::CreateMonthRangeFromYearAndMonth(int year, int month)
{
    auto cal = GetCalendar();

    cal->Year = year;
    cal->Month = month;
    cal->Day = cal->FirstDayInThisMonth;
    return CreateMonthRangeFromDate(cal);
}

// Uses the appropriate calendar to create an Advanced Query Syntax (AQS) date range 
// expression that spans the month that contains the given date.
String^ CalendarExtensions::CreateMonthRangeFromDate(const DateTime date)
{
    auto cal = GetCalendar();

    cal->SetDateTime(date);
    return CreateMonthRangeFromDate(cal);
}

// Returns the number of months in a given year according to the appropriate calendar
int CalendarExtensions::GetNumberOfMonthsInYear(DateTime yearDate)
{
    auto cal = GetCalendar();

    cal->SetDateTime(yearDate);
    return cal->NumberOfMonthsInThisYear;
}

// Returns the short name of given month for the app's resolved language. For example, "Jan" for January in English.
String^ CalendarExtensions::GetLocalizedAbbreviatedMonthName(int year, int month)
{
    auto cal = GetCalendar();
    cal->Year = year;
    cal->Month = month;
    cal->Day = cal->FirstDayInThisMonth;

    auto lang = ref new Vector<String^>();
    lang->Append(ResolvedLanguage());
    auto dtf = ref new DateTimeFormatter(
        YearFormat::None, 
        MonthFormat::Abbreviated, 
        DayFormat::None, 
        DayOfWeekFormat::None,  
        HourFormat::None, 
        MinuteFormat::None, 
        SecondFormat::None, 
        lang, ResolvedGeographicRegion(), cal->GetCalendarSystem(), cal->GetClock());

    return dtf->Format(cal->GetDateTime());
}

// Returns a localized month and year string for a given date in the app's resolved language.
String^ CalendarExtensions::GetLocalizedMonthAndYear(const DateTime date)
{
    auto cal = GetCalendar();
    auto lang = ref new Vector<String^>();
    lang->Append(ResolvedLanguage());
    auto dtf = ref new DateTimeFormatter(
        YearFormat::Full, 
        MonthFormat::Full, 
        DayFormat::None, 
        DayOfWeekFormat::None,  
        HourFormat::None, 
        MinuteFormat::None, 
        SecondFormat::None, 
        lang, ResolvedGeographicRegion(), cal->GetCalendarSystem(), cal->GetClock());

    cal->SetDateTime(date);
    return dtf->Format(cal->GetDateTime());
}

// Returns a localized year string for a given date using the app's resolved language.
String^ CalendarExtensions::GetLocalizedYear(DateTime yearDate)
{
    auto cal = GetCalendar();

    cal->SetDateTime(yearDate);

    auto lang = ref new Vector<String^>();
    lang->Append(ResolvedLanguage());
    auto dtf = ref new DateTimeFormatter(
        YearFormat::Full, 
        MonthFormat::None, 
        DayFormat::None, 
        DayOfWeekFormat::None,  
        HourFormat::None, 
        MinuteFormat::None, 
        SecondFormat::None, 
        lang, ResolvedGeographicRegion(), cal->GetCalendarSystem(), cal->GetClock());

    return dtf->Format(cal->GetDateTime());
}

int CalendarExtensions::GetNumericYear(const Windows::Foundation::DateTime yearDate)
{
    auto cal = GetCalendar();

    cal->SetDateTime(yearDate);
    return cal->Year;
}

// Sets year and month to localized values (based on the appropriate calendar) for a given date
void CalendarExtensions::WriteLocalizedYearAndMonth(const Windows::Foundation::DateTime date, int& yearResult, int& monthResult)
{
    auto cal = GetCalendar();

    cal->SetDateTime(date);
    yearResult = cal->Year;
    monthResult = cal->Month;
}

Platform::String^ CalendarExtensions::ResolvedLanguage()
{
    return (ref new DateTimeFormatter("shortdate"))->ResolvedLanguage;
}

Platform::String^ CalendarExtensions::ResolvedGeographicRegion()
{
   return (ref new DateTimeFormatter("shortdate"))->ResolvedGeographicRegion;
}