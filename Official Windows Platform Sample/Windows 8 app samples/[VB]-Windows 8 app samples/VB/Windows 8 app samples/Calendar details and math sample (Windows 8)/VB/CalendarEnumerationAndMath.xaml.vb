'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation
Imports SDKTemplate
Imports System
Imports System.Text
Imports Windows.Globalization
Imports Windows.Globalization.DateTimeFormatting

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class CalendarEnumerationAndMath
    Inherits SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
    End Sub


    ''' <summary>
    ''' This is the click handler for the 'Default' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Display_Click(sender As Object, e As RoutedEventArgs)
        ' This scenario uses the Windows.Globalization.Calendar class to enumerate through a calendar and
        ' perform calendar math
        Dim results As New StringBuilder()

        results.AppendLine("The number of years in each era of the Japanese era calendar is not regular. It is determined by the length of the given imperial era:" & vbCrLf)

        ' Create Japanese calendar.
        Dim calendar As New Calendar({"en-US"}, CalendarIdentifiers.Japanese, ClockIdentifiers.TwentyFourHour)

        ' Enumerate all supported years in all supported Japanese eras.
        Calendar.Era = Calendar.FirstEra
        While True
            ' Process current era.
            results.AppendLine("Era " & calendar.EraAsString() & " contains " & calendar.NumberOfYearsInThisEra & " year(s)")

            ' Enumerate all years in this era.
            Calendar.Year = Calendar.FirstYearInThisEra
            While True
                ' Begin sample processing of current year.

                ' Move to first day of year. Change of month can affect day so order of assignments is important.
                Calendar.Month = Calendar.FirstMonthInThisYear
                Calendar.Day = Calendar.FirstDayInThisMonth

                ' Set time to midnight (UTC).
                Calendar.Period = Calendar.FirstPeriodInThisDay
                ' All days have 1 or 2 periods depending on clock type
                Calendar.Hour = Calendar.FirstHourInThisPeriod
                ' Hours start from 12 or 0 depending on clock type
                Calendar.Minute = 0
                Calendar.Second = 0
                Calendar.Nanosecond = 0

                results.Append(".")

                ' End sample processing of current year.

                ' Break after processing last year.
                If Calendar.Year = Calendar.LastYearInThisEra Then
                    Exit While
                End If
                Calendar.AddYears(1)
            End While
            results.AppendLine()

            ' Break after processing last era.
            If Calendar.Era = Calendar.LastEra Then
                Exit While
            End If
            Calendar.AddYears(1)
        End While

        ' This section shows enumeration through the hours in a day to demonstrate that the number of time units in a given period (hours in a day, minutes in an hour, etc.)
        ' should not be regarded as fixed. With Daylight Saving Time and other local calendar adjustments, a given day may have not have 24 hours, and
        ' a given hour may not have 60 minutes, etc.
        results.AppendLine(vbCrLf & "The number of hours in a day is not invariable. The US calendar transitions from DST to standard time on 4 November 2012. Set your system time zone to a US time zone to see the effect on the number of hours in the day:" & vbCrLf)

        ' Create a DateTimeFormatter to display dates
        Dim displayDate As New Windows.Globalization.DateTimeFormatting.DateTimeFormatter("longdate")

        ' Create a gregorian calendar for the US with 12-hour clock format
        Dim currentCal As New Windows.Globalization.Calendar({"en-US"}, CalendarIdentifiers.Gregorian, ClockIdentifiers.TwentyFourHour)

        ' Set the calendar to a the date of the Daylight Saving Time-to-Standard Time transition for the US in 2012.
        ' DST ends in the US at 02:00 on 4 November 2012
        Dim dstDate As New DateTime(2012, 11, 4)
        currentCal.SetDateTime(dstDate)

        ' Set the current calendar to one day before DST change. Create a second calendar for comparision and set it to one day after DST change.
        Dim endDate = currentCal.Clone()
        currentCal.AddDays(-1)
        endDate.AddDays(1)

        ' Enumerate the day before, the day of, and the day after the 2012 DST-to-Standard time transition
        While currentCal.Day <= endDate.Day

            ' Process current day.
            Dim currentDate = currentCal.GetDateTime()
            results.AppendFormat("{0} contains {1} hour(s)", displayDate.Format(currentDate), currentCal.NumberOfHoursInThisPeriod)
            results.AppendLine()

            ' Enumerate all hours in this day.
            ' Create a calendar to represent the following day.
            Dim nextDay = currentCal.Clone()
            nextDay.AddDays(1)

            ' Start with the first hour in the period
            currentCal.Hour = currentCal.FirstHourInThisPeriod
            While True

                ' Display the hour for each hour in the day.             
                results.AppendFormat("{0} ", currentCal.HourAsPaddedString(2))

                ' Break upon reaching the next period (i.e. the first period in the following day).
                If currentCal.Day = nextDay.Day And currentCal.Period = nextDay.Period Then
                    Exit While
                End If

                ' Move to the next hour
                currentCal.AddHours(1)
            End While

            results.AppendLine()

        End While

        ' Display results
        rootPage.NotifyUser(results.ToString, NotifyType.StatusMessage)
    End Sub
End Class
