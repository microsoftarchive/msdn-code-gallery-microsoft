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
Imports Windows.Foundation
Imports Windows.Foundation.Collections
Imports Windows.Globalization
Imports Windows.Globalization.DateTimeFormatting

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class OverrideSettings
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
        ' This scenario uses the Windows.Globalization.DateTimeFormatting.DateTimeFormatter class
        ' to format a date/time by using a formatter that provides specific languages,
        ' geographic region, calendar and clock

        ' Get the current application language value
        Dim currentLanguage = ApplicationLanguages.Languages.First

        ' We keep results in this variable
        Dim results As New StringBuilder()
        results.AppendLine("Current default application context language: " & currentLanguage)
        results.AppendLine()

        ' Create formatters with individual format specifiers for date/time elements.
        ' Example formatters for dates using string templates.

        ' Example formatters using paranetrized date templates.

        ' Example formatters for times.

        ' Example formatters using paranetrized time templates.
        Dim templateFormatters As DateTimeFormatter() = {New DateTimeFormatter("longdate", {"ja-JP", "en-US"}, "JP", CalendarIdentifiers.Japanese, ClockIdentifiers.TwelveHour), New DateTimeFormatter("month day", {"fr-FR", "en-US"}, "FR", CalendarIdentifiers.Gregorian, ClockIdentifiers.TwentyFourHour), New DateTimeFormatter(YearFormat.Abbreviated, MonthFormat.Abbreviated, DayFormat.[Default], DayOfWeekFormat.None, HourFormat.None, MinuteFormat.None,
                                                                                                                                                                                                                                                                                                                                                    SecondFormat.None, {"de-DE", "en-US"}, "DE", CalendarIdentifiers.Gregorian, ClockIdentifiers.TwelveHour), New DateTimeFormatter("longtime", {"ja-JP", "en-US"}, "JP", CalendarIdentifiers.Japanese, ClockIdentifiers.TwelveHour), New DateTimeFormatter("hour minute", {"fr-FR", "en-US"}, "FR", CalendarIdentifiers.Gregorian, ClockIdentifiers.TwentyFourHour), New DateTimeFormatter(YearFormat.None, MonthFormat.None, DayFormat.None, DayOfWeekFormat.None, HourFormat.[Default], MinuteFormat.[Default],
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 SecondFormat.None, {"de-DE"}, "DE", CalendarIdentifiers.Gregorian, ClockIdentifiers.TwelveHour)}

        ' Create date/time to format and display.
        Dim dateTime__1 As DateTime = DateTime.Now

        ' Try to format and display date/time if calendar supports it.
        For Each formatter As DateTimeFormatter In templateFormatters
            Try
                ' Format and display date/time.
                results.AppendLine(formatter.Template & ": (" & formatter.ResolvedLanguage & ") " & formatter.Format(dateTime__1))
            Catch generatedExceptionName As ArgumentException
                ' Retrieve and display formatter properties. 
                results.AppendLine(String.Format("Unable to format Gregorian DateTime {0} using formatter with template {1} for languages [{2}], region {3}, calendar {4} and clock {5}", dateTime__1, formatter.Template, String.Join(",", formatter.Languages), formatter.GeographicRegion, formatter.Calendar, _
                    formatter.Clock))
            End Try
        Next

        ' Display the results
        rootPage.NotifyUser(results.ToString, NotifyType.StatusMessage)
    End Sub
End Class
