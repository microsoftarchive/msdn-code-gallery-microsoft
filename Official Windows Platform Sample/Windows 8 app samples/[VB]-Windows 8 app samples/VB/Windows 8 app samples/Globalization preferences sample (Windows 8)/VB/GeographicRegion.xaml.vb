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

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class GeographicRegionScenario
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
        ' This scenario uses the Windows.Globalization.GeographicRegion class to
        ' obtain the geographic region characteristics.

        ' Stores results of the scenario
        Dim results As New StringBuilder()

        ' Display characteristics of user's geographic region.
        Dim userRegion As New GeographicRegion()
        results.AppendLine("User's region display name: " & userRegion.DisplayName)
        results.AppendLine("User's region native name: " & userRegion.NativeName)
        ' results.AppendLine("User's region currencies in use: " & String.Join(",", userRegion.CurrenciesInUse))
        results.AppendLine("User's region codes: " & userRegion.CodeTwoLetter & "," & userRegion.CodeThreeLetter & "," & userRegion.CodeThreeDigit)
        results.AppendLine()

        ' Display characteristics of example region.
        Dim ruRegion As New GeographicRegion("RU")
        results.AppendLine("RU region display name: " & ruRegion.DisplayName)
        results.AppendLine("RU region native name: " & ruRegion.NativeName)
        results.AppendLine("RU region currencies in use: " & String.Join(",", ruRegion.CurrenciesInUse))
        results.AppendLine("RU region codes: " & ruRegion.CodeTwoLetter & "," & ruRegion.CodeThreeLetter & "," & ruRegion.CodeThreeDigit)

        ' Display the results
        rootPage.NotifyUser(results.ToString, NotifyType.StatusMessage)
    End Sub
End Class
