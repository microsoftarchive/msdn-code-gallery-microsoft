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
Imports Windows.Globalization.Fonts
Imports Windows.Graphics.Display
Imports Windows.UI.Text
Imports Windows.UI.Xaml.Data
Imports Windows.UI.Xaml.Media

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario2
    Inherits SDKTemplate.Common.LayoutAwarePage
    Private oriHeadingDoc As LocalFontInfo
    Private oriTextDoc As LocalFontInfo

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
        If Me.oriHeadingDoc IsNot Nothing Then
            Dim headingDoc = DirectCast(Me.FindName("Scenario2Heading"), Windows.UI.Xaml.Controls.TextBlock)
            oriHeadingDoc.Reset(headingDoc)

            Dim textDoc = DirectCast(Me.FindName("Scenario2Text"), Windows.UI.Xaml.Controls.TextBlock)
            oriTextDoc.Reset(textDoc)
        End If
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Scenario2InputButton' button.  You would replace this with your own handler
    ''' if you have a button or buttons on this page.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Scenario2InputButton_Click(sender As Object, e As RoutedEventArgs)
        Dim b As Button = TryCast(sender, Button)
        If b IsNot Nothing Then
            Dim languageFontGroup = New LanguageFontGroup("hi")
            Dim headingUI = DirectCast(Me.Scenario2Heading, Windows.UI.Xaml.Controls.TextBlock)
            Dim textUI = DirectCast(Me.Scenario2Text, Windows.UI.Xaml.Controls.TextBlock)

            If Me.oriHeadingDoc Is Nothing Then
                Me.oriHeadingDoc = New LocalFontInfo()
                Me.oriTextDoc = New LocalFontInfo()
                Me.oriHeadingDoc.[Set](headingUI)
                Me.oriTextDoc.[Set](textUI)
            End If

            Me.SetFont(headingUI, languageFontGroup.DocumentHeadingFont)

            ' Not all scripts have recommended fonts for "document alternate"
            ' categories, so need to verify before using it. Note that Hindi does
            ' have document alternate fonts, so in this case the fallback logic is
            ' unnecessary, but (for example) Japanese does not have recommendations
            ' for the document alternate 2 category.
            If languageFontGroup.DocumentAlternate2Font IsNot Nothing Then
                Me.SetFont(textUI, languageFontGroup.DocumentAlternate2Font)
            ElseIf languageFontGroup.DocumentAlternate1Font IsNot Nothing Then
                Me.SetFont(textUI, languageFontGroup.DocumentAlternate1Font)
            Else
                Me.SetFont(textUI, languageFontGroup.ModernDocumentFont)
            End If


            Me.Output.Visibility = Visibility.Visible
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
