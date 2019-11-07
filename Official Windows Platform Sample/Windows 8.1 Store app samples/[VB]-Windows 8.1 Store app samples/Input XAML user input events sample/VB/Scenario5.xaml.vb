'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports System
Imports Windows.UI.Xaml.Media

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario5
    Inherits SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
        AddHandler scenario5Reset.Loaded, AddressOf scenario5Reset_Loaded
    End Sub

    Private Sub scenario5Reset_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
        scenario5Reset.Focus(Windows.UI.Xaml.FocusState.Programmatic)
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
    End Sub

    Private Sub Scenario5_Reset(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Scenario5_Reset()
    End Sub

    Private Sub Scenario5_Reset()
        Scenario5UpdateVisuals(bChange, "")
        keyState.Text = ""
    End Sub

    Private Sub Output_KeyDown_1(ByVal sender As Object, ByVal e As Windows.UI.Xaml.Input.KeyRoutedEventArgs)
        keyState.Text = """" & e.Key.ToString() & """" & " KeyDown"
    End Sub

    Private Sub Output_KeyUp_1(ByVal sender As Object, ByVal e As Windows.UI.Xaml.Input.KeyRoutedEventArgs)
        keyState.Text = """" & e.Key.ToString() & """" & " KeyUp"
        Scenario5UpdateVisuals(bChange, e.Key.ToString())
    End Sub

    Private Sub Scenario5UpdateVisuals(ByVal border As Border, ByVal color As String)
        Select Case color.ToLower()
            Case "v"
                border.Background = New SolidColorBrush(Colors.Violet)
                CType(border.Child, TextBlock).Text = "Violet"
            Case "i"
                border.Background = New SolidColorBrush(Colors.Indigo)
                CType(border.Child, TextBlock).Text = "Indigo"
            Case "b"
                border.Background = New SolidColorBrush(Colors.Blue)
                CType(border.Child, TextBlock).Text = "Blue"
            Case "g"
                border.Background = New SolidColorBrush(Colors.Green)
                CType(border.Child, TextBlock).Text = "Green"
            Case "y"
                border.Background = New SolidColorBrush(Colors.Yellow)
                CType(border.Child, TextBlock).Text = "Yellow"
            Case "o"
                border.Background = New SolidColorBrush(Colors.Orange)
                CType(border.Child, TextBlock).Text = "Orange"
            Case "r"
                border.Background = New SolidColorBrush(Colors.Red)
                CType(border.Child, TextBlock).Text = "Red"
            Case Else
                border.Background = New SolidColorBrush(Colors.White)
                CType(border.Child, TextBlock).Text = "White"
        End Select
    End Sub
End Class
