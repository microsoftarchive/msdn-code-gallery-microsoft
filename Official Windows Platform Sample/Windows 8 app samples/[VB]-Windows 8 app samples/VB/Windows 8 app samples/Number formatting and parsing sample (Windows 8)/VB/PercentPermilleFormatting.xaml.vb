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
Partial Public NotInheritable Class PercentPermilleFormatting
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
        ' This scenario uses the Windows.Globalization.NumberFormatting.PercentFormatter and
        ' the Windows.Globalization.NumberFormatting.PermilleFormatter classes to format numbers
        ' as a percent or a permille.

        ' Keep results of the scenario in a StringBuilder
        Dim results As New StringBuilder()

        ' Create numbers to format.
        Dim randomNumber As Double = New Random().NextDouble()
        Dim fixedNumber As ULong = 500

        ' Create percent formatters.
        Dim defaultPercentFormatter As New PercentFormatter()
        Dim languagePercentFormatter As New PercentFormatter({"fr-FR"}, "ZZ")

        ' Create permille formatters.
        Dim defaultPermilleFormatter As New PermilleFormatter()
        Dim languagePermilleFormatter As New PermilleFormatter({"ar"}, "ZZ")

        ' Format random numbers as percent or permille.
        results.AppendLine("Random number (" & randomNumber & ")")
        results.AppendLine("Percent formatted: " & defaultPercentFormatter.Format(randomNumber))
        results.AppendLine("Permille formatted: " & defaultPermilleFormatter.Format(randomNumber))
        results.AppendLine()
        results.AppendLine("Language-specific percent formatted: " & languagePercentFormatter.Format(randomNumber))
        results.AppendLine("Language-specific permille formatted: " & languagePermilleFormatter.Format(randomNumber))
        results.AppendLine()
        results.AppendLine("Fixed number (" & fixedNumber & ")")

        ' Format fixed number with grouping.
        defaultPercentFormatter.IsGrouped = True
        results.AppendLine("Percent formatted (grouped): " & defaultPercentFormatter.Format(fixedNumber))

        'Format with grouping using French language.
        languagePercentFormatter.IsGrouped = True
        results.AppendLine("Percent formatted (grouped as fr-FR): " & defaultPercentFormatter.Format(fixedNumber))

        ' Format with no fraction digits.
        defaultPercentFormatter.FractionDigits = 0
        results.AppendLine("Percent formatted (no fractional digits): " & defaultPercentFormatter.Format(fixedNumber))

        ' Format always with a decimal point.
        defaultPercentFormatter.IsDecimalPointAlwaysDisplayed = True
        results.AppendLine("Percent formatted (always with a decimal point): " & defaultPercentFormatter.Format(fixedNumber))

        ' Display the results
        rootPage.NotifyUser(results.ToString, NotifyType.StatusMessage)
    End Sub
End Class
