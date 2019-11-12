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
Imports Windows.Foundation
Imports Windows.Globalization.Fonts
Imports Windows.Graphics.Display
Imports Windows.System
Imports Windows.UI.Text
Imports Windows.UI.Xaml.Data
Imports Windows.UI.Xaml.Media


''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario1
    Inherits SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private oriHeadingUI As LocalFontInfo
    Private oriTextUI As LocalFontInfo

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
        If Me.oriHeadingUI IsNot Nothing Then
            Dim headingUI = DirectCast(Me.Scenario1Heading, Windows.UI.Xaml.Controls.TextBlock)
            Me.oriHeadingUI.Reset(headingUI)

            Dim textUI = DirectCast(Me.Scenario1Text, Windows.UI.Xaml.Controls.TextBlock)
            Me.oriTextUI.Reset(textUI)
        End If
    End Sub


    ''' <summary>
    ''' This is the click handler for the 'Scenario1InputButton' button.  You would replace this with your own handler
    ''' if you have a button or buttons on this page.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Scenario1InputButton_Click(sender As Object, e As RoutedEventArgs)
        Dim b As Button = TryCast(sender, Button)
        If b IsNot Nothing Then
            Dim languageFontGroup = New LanguageFontGroup("ja-JP")
            Dim headingUI = DirectCast(Me.Scenario1Heading, Windows.UI.Xaml.Controls.TextBlock)
            Dim textUI = DirectCast(Me.Scenario1Text, Windows.UI.Xaml.Controls.TextBlock)

            If Me.oriHeadingUI Is Nothing Then
                ' Store original font style for Reset
                Me.oriHeadingUI = New LocalFontInfo()
                Me.oriTextUI = New LocalFontInfo()
                Me.oriHeadingUI.[Set](headingUI)
                Me.oriTextUI.[Set](textUI)
            End If

            ' Change the Font value with selected font from LanguageFontGroup API
            Me.SetFont(headingUI, languageFontGroup.UIHeadingFont)
            Me.SetFont(textUI, languageFontGroup.UITextFont)

            Me.Output.Visibility = Visibility.Visible
        End If
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Other' button.  You would replace this with your own handler
    ''' if you have a button or buttons on this page.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Other_Click(sender As Object, e As RoutedEventArgs)
        Dim b As Button = TryCast(sender, Button)
        If b IsNot Nothing Then
            rootPage.NotifyUser("You clicked the " + b.Name + " button", NotifyType.StatusMessage)
        End If
    End Sub

    Private Sub SetFont(textBlock As TextBlock, languageFont As Windows.Globalization.Fonts.LanguageFont)
        Dim fontFamily As New FontFamily(languageFont.FontFamily)
        textBlock.FontFamily = fontFamily
        textBlock.FontWeight = languageFont.FontWeight
        textBlock.FontStyle = languageFont.FontStyle
        textBlock.FontStretch = languageFont.FontStretch
    End Sub

End Class
