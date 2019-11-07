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
Partial Public NotInheritable Class CurrencyFormatting
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
        ' This scenario uses the Windows.Globalization.NumberFormatting.CurrencyFormatter class
        ' to format a number as a currency.

        ' Keep results of the scenario in a StringBuilder
        Dim results As New StringBuilder()

        ' Determine the current user's default currency.
        Dim currency As String = GlobalizationPreferences.Currencies(0)

        ' Generate numbers used for formatting.
        Dim wholeNumber As ULong = 12345
        Dim fractionalNumber As Double = 12345.67

        ' Create currency formatter initialized with current number formatting preferences.
        Dim defaultCurrencyFormatter As New CurrencyFormatter(currency)

        Dim usdCurrencyFormatter As New CurrencyFormatter("USD")
        Dim eurFRCurrencyFormatter As New CurrencyFormatter("EUR", {"fr-FR"}, "FR")
        Dim eurIECurrencyFormatter As New CurrencyFormatter("EUR", {"gd-IE"}, "IE")

        ' Format numbers as currency.
        results.AppendLine("Fixed number (" & fractionalNumber & ")")
        results.AppendLine("Formatted with user's default currency: " & defaultCurrencyFormatter.Format(fractionalNumber))
        results.AppendLine("Formatted US Dollar: " & usdCurrencyFormatter.Format(fractionalNumber))
        results.AppendLine("Formatted Euro (fr-FR defaults): " & eurFRCurrencyFormatter.Format(fractionalNumber))
        results.AppendLine("Formatted Euro (gd-IE defaults): " & eurIECurrencyFormatter.Format(fractionalNumber))
        results.AppendLine()

        ' Format currency with fraction digits always included.
        usdCurrencyFormatter.FractionDigits = 2
        results.AppendLine("Formatted US Dollar (with fractional digits): " & usdCurrencyFormatter.Format(wholeNumber))

        ' Format currenccy with grouping.
        usdCurrencyFormatter.IsGrouped = True
        results.AppendLine("Formatted US Dollar (with grouping separators): " & usdCurrencyFormatter.Format(fractionalNumber))

        ' Display the results
        rootPage.NotifyUser(results.ToString, NotifyType.StatusMessage)
    End Sub
End Class
