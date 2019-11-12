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
Imports Windows.System.UserProfile
Imports Windows.Globalization

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class LanguageScenario
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
        ' This scenario uses the Windows.Globalization.Language class to
        ' obtain the Language characteristics.

        ' Stores results of the scenario
        Dim results As New StringBuilder()

        ' Display characteristics of user's preferred Language.
        Dim topUserLanguage As String = GlobalizationPreferences.Languages(0)
        Dim userLanguage As New Language(topUserLanguage)
        results.AppendLine("User's preferred Language display name: " & userLanguage.DisplayName)
        results.AppendLine("User's preferred Language tag: " & userLanguage.LanguageTag)
        results.AppendLine("User's preferred Language native name: " & userLanguage.NativeName)
        results.AppendLine("User's preferred Language script code: " & userLanguage.Script)
        results.AppendLine()

        ' Display characteristics of the Russian Language.
        Dim ruLanguage As New Language("ru-RU")
        results.AppendLine("Russian Language display name: " & ruLanguage.DisplayName)
        results.AppendLine("Russian Language tag: " & ruLanguage.LanguageTag)
        results.AppendLine("Russian Language native name: " & ruLanguage.NativeName)
        results.AppendLine("Russian Language script code: " & ruLanguage.Script)

        ' Display results
        rootPage.NotifyUser(results.ToString, NotifyType.StatusMessage)
    End Sub
End Class
