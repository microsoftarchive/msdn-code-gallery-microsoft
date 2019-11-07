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
Imports Windows.Globalization.NumberFormatting


''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class DecimalFormatting
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
        ' This scenario uses the Windows.Globalization.NumberFormatting.DecimalFormatter class
        ' to format a number.

        ' Keep results of the scenario in a StringBuilder
        Dim results As New StringBuilder()

        ' Create formatter initialized using the current user's preference settings.
        Dim decimalFormat As DecimalFormatter = New Windows.Globalization.NumberFormatting.DecimalFormatter()

        ' Make a random number.
        Dim randomNumber As Double = (New Random().NextDouble() * 100000)

        ' Format with the user's default preferences.
        Dim decimalCurrent As String = decimalFormat.Format(randomNumber)

        ' Format with grouping.
        Dim decimalFormat1 As DecimalFormatter = New Windows.Globalization.NumberFormatting.DecimalFormatter()
        decimalFormat1.IsGrouped = True
        Dim decimal1 As String = decimalFormat1.Format(randomNumber)

        ' Format with grouping using French.
        Dim decimalFormatFR As DecimalFormatter = New Windows.Globalization.NumberFormatting.DecimalFormatter(New String() {"fr-FR"}, "ZZ")
        decimalFormatFR.IsGrouped = True
        Dim decimalFR As String = decimalFormatFR.Format(randomNumber)

        ' Illustrate how automatic digit substitution works
        Dim decimalFormatAR As DecimalFormatter = New Windows.Globalization.NumberFormatting.DecimalFormatter(New String() {"ar"}, "ZZ")
        decimalFormatAR.IsGrouped = True
        Dim decimalAR As String = decimalFormatAR.Format(randomNumber)

        ' Get a fixed number.
        Dim fixedNumber As ULong = 500

        ' Format with the user's default preferences.
        Dim decimal2 As String = decimalFormat.Format(fixedNumber)

        ' Format always with a decimal point.
        Dim decimalFormat3 As DecimalFormatter = New Windows.Globalization.NumberFormatting.DecimalFormatter()
        decimalFormat3.IsDecimalPointAlwaysDisplayed = True
        decimalFormat3.FractionDigits = 0
        Dim decimal3 As String = decimalFormat3.Format(fixedNumber)

        ' Format with no fractional digits or decimal point.
        Dim decimalFormat4 As DecimalFormatter = New Windows.Globalization.NumberFormatting.DecimalFormatter()
        decimalFormat4.FractionDigits = 0
        Dim decimal4 As String = decimalFormat4.Format(fixedNumber)

        ' Display the results.
        results.AppendLine("Random number (" & randomNumber & ")")
        results.AppendLine("With current user preferences: " & decimalCurrent)
        results.AppendLine("With grouping separators: " & decimal1)
        results.AppendLine("With grouping separators (fr-FR): " & decimalFR)
        results.AppendLine("With digit substitution (ar): " & decimalAR)
        results.AppendLine()
        results.AppendLine("Fixed number (" & fixedNumber & ")")
        results.AppendLine("With current user preferences: " & decimal2)
        results.AppendLine("Always with a decimal point: " & decimal3)
        results.AppendLine("With no fraction digits or decimal points: " & decimal4)

        ' Display the results
        rootPage.NotifyUser(results.ToString, NotifyType.StatusMessage)
    End Sub
End Class
