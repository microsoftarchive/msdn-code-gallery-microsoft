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
Imports Windows.System.UserProfile
Imports Windows.Foundation
Imports Windows.Foundation.Collections
Imports Windows.Globalization
Imports Windows.Globalization.NumberFormatting

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class NumberParsing
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
        ' This scenario uses the Windows.Globalization.NumberFormatting.DecimalFormatter,
        ' Windows.Globalization.NumberFormatting.CurrencyFormatter and 
        ' Windows.Globalization.NumberFormatting.PercentFormatter classes to format and parse a number
        ' percentage or currency.

        ' Keep results of the scenario in a StringBuilder
        Dim results As New StringBuilder()

        ' Define percent formatters.
        Dim percentFormat As New PercentFormatter()
        Dim percentFormatJP As New PercentFormatter(New String() {"ja"}, "ZZ")
        Dim percentFormatFR As New PercentFormatter(New String() {"fr-FR"}, "ZZ")

        ' Define decimal formatters.
        Dim decimalFormat As New DecimalFormatter()
        decimalFormat.IsGrouped = True
        Dim decimalFormatJP As New DecimalFormatter(New String() {"ja"}, "ZZ")
        decimalFormatJP.IsGrouped = True
        Dim decimalFormatFR As New DecimalFormatter(New String() {"fr-FR"}, "ZZ")
        decimalFormatFR.IsGrouped = True

        ' Define currency formatters
        Dim userCurrency As String = GlobalizationPreferences.Currencies(0)
        Dim currencyFormat As New CurrencyFormatter(userCurrency)
        Dim currencyFormatJP As New CurrencyFormatter("JPY", New String() {"ja"}, "ZZ")
        Dim currencyFormatFR As New CurrencyFormatter("EUR", New String() {"fr-FR"}, "ZZ")

        ' Generate numbers for parsing.
        Dim percentNumber As Double = 0.523
        Dim decimalNumber As Double = 12345.67
        Dim currencyNumber As Double = 1234.56

        ' Roundtrip the percent numbers by formatting and parsing.
        Dim percent1 As String = percentFormat.Format(percentNumber)
        Dim percent1Parsed As Double = percentFormat.ParseDouble(percent1).Value

        Dim percent1JP As String = percentFormatJP.Format(percentNumber)
        Dim percent1JPParsed As Double = percentFormatJP.ParseDouble(percent1JP).Value

        Dim percent1FR As String = percentFormatFR.Format(percentNumber)
        Dim percent1FRParsed As Double = percentFormatFR.ParseDouble(percent1FR).Value

        ' Generate the results for percent parsing.
        results.AppendLine("Percent parsing of " & percentNumber)
        results.AppendLine("Formatted for current user: " & percent1 & " Parsed as current user: " & percent1Parsed)
        results.AppendLine("Formatted for ja-JP: " & percent1JP & " Parsed for ja-JP: " & percent1JPParsed)
        results.AppendLine("Formatted for fr-FR: " & percent1FR & " Parsed for fr-FR: " & percent1FRParsed)
        results.AppendLine()

        ' Roundtrip the decimal numbers for formatting and parsing.
        Dim decimal1 As String = decimalFormat.Format(decimalNumber)
        Dim decimal1Parsed As Double = decimalFormat.ParseDouble(decimal1).Value

        Dim decimal1JP As String = decimalFormatJP.Format(decimalNumber)
        Dim decimal1JPParsed As Double = decimalFormatJP.ParseDouble(decimal1JP).Value

        Dim decimal1FR As String = decimalFormatFR.Format(decimalNumber)
        Dim decimal1FRParsed As Double = decimalFormatFR.ParseDouble(decimal1FR).Value

        ' Generate the results for decimal parsing.
        results.AppendLine("Decimal parsing of " & decimalNumber)
        results.AppendLine("Formatted for current user: " & decimal1 & " Parsed as current user: " & decimal1Parsed)
        results.AppendLine("Formatted for ja-JP: " & decimal1JP & " Parsed for ja-JP: " & decimal1JPParsed)
        results.AppendLine("Formatted for fr-FR: " & decimal1FR & " Parsed for fr-FR: " & decimal1FRParsed)
        results.AppendLine()

        ' Roundtrip the currency numbers for formatting and parsing.
        Dim currency1 As String = currencyFormat.Format(currencyNumber)
        Dim currency1Parsed As Double = currencyFormat.ParseDouble(currency1).Value

        Dim currency1JP As String = currencyFormatJP.Format(currencyNumber)
        Dim currency1JPParsed As Double = currencyFormatJP.ParseDouble(currency1JP).Value

        Dim currency1FR As String = currencyFormatFR.Format(currencyNumber)
        Dim currency1FRParsed As Double = currencyFormatFR.ParseDouble(currency1FR).Value

        ' Generate the results for decimal parsing.
        results.AppendLine("Currency parsing of " & currencyNumber)
        results.AppendLine("Formatted for current user: " & currency1 & " Parsed as current user: " & currency1Parsed)
        results.AppendLine("Formatted for ja-JP: " & currency1JP & " Parsed for ja-JP: " & currency1JPParsed)
        results.AppendLine("Formatted for fr-FR: " & currency1FR & " Parsed for fr-FR: " & currency1FRParsed)
        results.AppendLine()

        ' Display the results.
        rootPage.NotifyUser(results.ToString, NotifyType.StatusMessage)
    End Sub
End Class
