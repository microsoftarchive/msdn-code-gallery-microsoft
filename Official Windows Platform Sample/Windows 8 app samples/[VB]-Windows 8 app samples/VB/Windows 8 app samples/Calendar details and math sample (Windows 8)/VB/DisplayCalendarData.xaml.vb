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

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class DisplayCalendarData
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
    ''' This is the click handler for the 'Display' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Display_Click(sender As Object, e As RoutedEventArgs)
        ' This scenario uses the Windows.Globalization.Calendar class to display the parts of a date.

        ' Store results here.
        Dim results As New StringBuilder()

        ' Create Calendar objects using different constructors.
        Dim calendar As New Calendar()
        Dim japaneseCalendar As New Calendar({"ja-JP"}, CalendarIdentifiers.Japanese, ClockIdentifiers.TwelveHour)
        Dim hebrewCalendar As New Calendar({"he-IL"}, CalendarIdentifiers.Hebrew, ClockIdentifiers.TwentyFourHour)

        ' Display individual date/time elements.
        results.AppendLine("User's default calendar system: " & calendar.GetCalendarSystem())
        results.AppendLine("Name of Month: " & calendar.MonthAsSoloString())
        results.AppendLine("Day of Month: " & calendar.DayAsPaddedString(2))
        results.AppendLine("Day of Week: " & calendar.DayOfWeekAsSoloString())
        results.AppendLine("Year: " & calendar.YearAsString())
        results.AppendLine()
        results.AppendLine("Calendar system: " & japaneseCalendar.GetCalendarSystem())
        results.AppendLine("Name of Month: " & japaneseCalendar.MonthAsSoloString())
        results.AppendLine("Day of Month: " & japaneseCalendar.DayAsPaddedString(2))
        results.AppendLine("Day of Week: " & japaneseCalendar.DayOfWeekAsSoloString())
        results.AppendLine("Year: " & japaneseCalendar.YearAsString())
        results.AppendLine()
        results.AppendLine("Calendar system: " & hebrewCalendar.GetCalendarSystem())
        results.AppendLine("Name of Month: " & hebrewCalendar.MonthAsSoloString())
        results.AppendLine("Day of Month: " & hebrewCalendar.DayAsPaddedString(2))
        results.AppendLine("Day of Week: " & hebrewCalendar.DayOfWeekAsSoloString())
        results.AppendLine("Year: " & hebrewCalendar.YearAsString())
        results.AppendLine()

        ' Display the results
        rootPage.NotifyUser(results.ToString, NotifyType.StatusMessage)
    End Sub
End Class
