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
Imports Windows.UI.Xaml.Media
Imports Windows.UI

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
        AddHandler scenario5ResetButton.Loaded, AddressOf scenario5Reset_Loaded
    End Sub

    Private Sub scenario5Reset_Loaded(sender As Object, e As RoutedEventArgs)
        scenario5ResetButton.Focus(Windows.UI.Xaml.FocusState.Programmatic)
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
    End Sub

    Private Sub Scenario5Reset(sender As Object, e As RoutedEventArgs)
        Scenario5Reset()
    End Sub

    Private Sub Scenario5Reset()
        Scenario5UpdateVisuals(bChange, "")
        keyState.Text = ""
    End Sub

    Private Sub Output_KeyDown_1(sender As Object, e As Windows.UI.Xaml.Input.KeyRoutedEventArgs)
        keyState.Text = """" + e.Key.ToString() + """" + " KeyDown"
    End Sub

    Private Sub Output_KeyUp_1(sender As Object, e As Windows.UI.Xaml.Input.KeyRoutedEventArgs)
        keyState.Text = """" + e.Key.ToString() + """" + " KeyUp"
        Scenario5UpdateVisuals(bChange, e.Key.ToString())
    End Sub

    Private Sub Scenario5UpdateVisuals(border As Border, color As [String])
        Select Case color.ToLower()
            Case "v"
                border.Background = New SolidColorBrush(Colors.Violet)
                DirectCast(border.Child, TextBlock).Text = "Violet"
                Exit Select
            Case "i"
                border.Background = New SolidColorBrush(Colors.Indigo)
                DirectCast(border.Child, TextBlock).Text = "Indigo"
                Exit Select
            Case "b"
                border.Background = New SolidColorBrush(Colors.Blue)
                DirectCast(border.Child, TextBlock).Text = "Blue"
                Exit Select
            Case "g"
                border.Background = New SolidColorBrush(Colors.Green)
                DirectCast(border.Child, TextBlock).Text = "Green"
                Exit Select
            Case "y"
                border.Background = New SolidColorBrush(Colors.Yellow)
                DirectCast(border.Child, TextBlock).Text = "Yellow"
                Exit Select
            Case "o"
                border.Background = New SolidColorBrush(Colors.Orange)
                DirectCast(border.Child, TextBlock).Text = "Orange"
                Exit Select
            Case "r"
                border.Background = New SolidColorBrush(Colors.Red)
                DirectCast(border.Child, TextBlock).Text = "Red"
                Exit Select
            Case Else
                border.Background = New SolidColorBrush(Colors.White)
                DirectCast(border.Child, TextBlock).Text = "White"
                Exit Select
        End Select
    End Sub
End Class
