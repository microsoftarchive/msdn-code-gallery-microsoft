'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.ApplicationModel.Resources.Core
Imports Windows.Globalization
Imports System.Text

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario8
    Inherits SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
        UpdateCurrentAppLanguageMessage()
        AddHandler LanguageOverrideCombo.LanguageOverrideChanged, AddressOf LanguageOverrideCombo_LanguageOrverrideChanged
    End Sub

    Private Sub LanguageOverrideCombo_LanguageOrverrideChanged(ByVal sender As Object, ByVal e As EventArgs)
        UpdateCurrentAppLanguageMessage()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)

    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Scenario8Button_Show' button.  You would replace this with your own handler
    ''' if you have a button or buttons on this page.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Scenario8Button_Show_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim b As Button = TryCast(sender, Button)
        If b IsNot Nothing Then
            Dim defaultContextForCurrentView As ResourceContext = ResourceContext.GetForCurrentView()
            Dim stringResourcesResourceMap As ResourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("Resources")
            Scenario8MessageTextBlock.Text = stringResourcesResourceMap.GetValue("string1", defaultContextForCurrentView).ValueAsString
        End If
    End Sub


    Private Sub UpdateCurrentAppLanguageMessage()
        Me.Scenario8AppLanguagesTextBlock.Text = "Current app language(s): " & GetAppLanguagesAsFormattedString()
    End Sub

    Private Function GetAppLanguagesAsFormattedString() As String
        Dim countLanguages = ApplicationLanguages.Languages.Count
        Dim sb As New StringBuilder()
        For i = 0 To countLanguages - 2
            sb.Append(ApplicationLanguages.Languages(i))
            sb.Append(", ")
        Next i
        sb.Append(ApplicationLanguages.Languages(countLanguages - 1))
        Return sb.ToString()
    End Function
End Class
