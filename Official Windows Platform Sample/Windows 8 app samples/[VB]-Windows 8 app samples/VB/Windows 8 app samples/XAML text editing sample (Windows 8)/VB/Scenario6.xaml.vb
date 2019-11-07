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
Imports Windows.UI.Xaml.Controls.Primitives
Imports Windows.UI.Xaml.Data
Imports Windows.UI.Xaml.Input
Imports Windows.UI.Xaml.Media
Imports Windows.UI.Xaml.Navigation
Imports Windows.UI.Text
Imports Windows.UI
Imports Windows.Storage
Imports Windows.Storage.Streams
Imports SDKTemplate
Imports System
Imports System.Collections.Generic

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario6
    Inherits SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Private m_highlightedWords As List(Of ITextRange) = Nothing

    Public Sub New()
        Me.InitializeComponent()

        m_highlightedWords = New List(Of ITextRange)()
    End Sub

    Private Sub BoldButtonClick(sender As Object, e As RoutedEventArgs)
        Dim selectedText As ITextSelection = editor.Document.Selection
        If selectedText IsNot Nothing Then
            Dim charFormatting As ITextCharacterFormat = selectedText.CharacterFormat
            charFormatting.Bold = FormatEffect.Toggle
            selectedText.CharacterFormat = charFormatting
        End If
    End Sub

    Private Sub ItalicButtonClick(sender As Object, e As RoutedEventArgs)
        Dim selectedText As ITextSelection = editor.Document.Selection
        If selectedText IsNot Nothing Then
            Dim charFormatting As ITextCharacterFormat = selectedText.CharacterFormat
            charFormatting.Italic = FormatEffect.Toggle
            selectedText.CharacterFormat = charFormatting
        End If
    End Sub

    Private Sub FontColorButtonClick(sender As Object, e As RoutedEventArgs)
        fontColorPopup.IsOpen = True
        fontColorButton.Focus(Windows.UI.Xaml.FocusState.Keyboard)
    End Sub

    Private Sub FindBoxTextChanged(sender As Object, e As TextChangedEventArgs)
        Dim textToFind As String = findBox.Text

        If textToFind IsNot Nothing Then
            FindAndHighlightText(textToFind)
        End If
    End Sub

    Private Sub FontColorButtonLostFocus(sender As Object, e As RoutedEventArgs)
        fontColorPopup.IsOpen = False
    End Sub

    Private Sub FindBoxLostFocus(sender As Object, e As RoutedEventArgs)
        ClearAllHighlightedWords()
    End Sub

    Private Sub FindBoxGotFocus(sender As Object, e As RoutedEventArgs)
        Dim textToFind As String = findBox.Text

        If textToFind IsNot Nothing Then
            FindAndHighlightText(textToFind)
        End If
    End Sub

    Private Sub ClearAllHighlightedWords()
        Dim charFormat As ITextCharacterFormat
        For i As Integer = 0 To m_highlightedWords.Count - 1
            charFormat = m_highlightedWords(i).CharacterFormat
            charFormat.BackgroundColor = Colors.Transparent
            m_highlightedWords(i).CharacterFormat = charFormat
        Next

        m_highlightedWords.Clear()
    End Sub

    Private Sub FindAndHighlightText(textToFind As String)
        ClearAllHighlightedWords()

        Dim searchRange As ITextRange = editor.Document.GetRange(0, TextConstants.MaxUnitCount)
        searchRange.Move(0, 0)

        Dim textFound As Boolean = True
        Do
            If searchRange.FindText(textToFind, TextConstants.MaxUnitCount, FindOptions.None) < 1 Then
                textFound = False
            Else
                m_highlightedWords.Add(searchRange.GetClone())

                Dim charFormatting As ITextCharacterFormat = searchRange.CharacterFormat
                charFormatting.BackgroundColor = Colors.Yellow
                searchRange.CharacterFormat = charFormatting
            End If
        Loop While textFound
    End Sub

    Private Sub ColorButtonClick(sender As Object, e As RoutedEventArgs)
        Dim clickedColor As Button = DirectCast(sender, Button)

        Dim charFormatting As ITextCharacterFormat = editor.Document.Selection.CharacterFormat
        Select Case clickedColor.Name
            Case "black"
                If True Then
                    charFormatting.ForegroundColor = Colors.Black
                    Exit Select
                End If

            Case "gray"
                If True Then
                    charFormatting.ForegroundColor = Colors.Gray
                    Exit Select
                End If

            Case "darkgreen"
                If True Then
                    charFormatting.ForegroundColor = Colors.DarkGreen
                    Exit Select
                End If

            Case "green"
                If True Then
                    charFormatting.ForegroundColor = Colors.Green
                    Exit Select
                End If

            Case "blue"
                If True Then
                    charFormatting.ForegroundColor = Colors.Blue
                    Exit Select
                End If
            Case Else

                If True Then
                    charFormatting.ForegroundColor = Colors.Black
                    Exit Select
                End If
        End Select
        editor.Document.Selection.CharacterFormat = charFormatting

        editor.Focus(Windows.UI.Xaml.FocusState.Keyboard)
        fontColorPopup.IsOpen = False
    End Sub

    Private Async Sub LoadContentAsync()
        Dim file As StorageFile = Await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync("lorem.rtf")
        Dim randAccStream As IRandomAccessStream = Await file.OpenAsync(FileAccessMode.Read)
        editor.Document.LoadFromStream(TextSetOptions.FormatRtf, randAccStream)
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
    End Sub


    ''' <summary>
    ''' This is the click handler for the 'Default' button.  You would replace this with your own handler
    ''' if you have a button or buttons on this page.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Default_Click(sender As Object, e As RoutedEventArgs)
        Dim b As Button = TryCast(sender, Button)
        If b IsNot Nothing Then
            rootPage.NotifyUser("You clicked the " & b.Name & " button", NotifyType.StatusMessage)
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
            rootPage.NotifyUser("You clicked the " & b.Name & " button", NotifyType.StatusMessage)
        End If
    End Sub

End Class
