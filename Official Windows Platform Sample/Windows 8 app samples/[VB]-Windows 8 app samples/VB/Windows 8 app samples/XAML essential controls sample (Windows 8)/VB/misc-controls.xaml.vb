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
Imports Windows.UI.Xaml.Navigation
Imports SDKTemplate
Imports System

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class MiscControls
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

    Private Sub StretchModeButton_Checked(sender As Object, e As RoutedEventArgs)
        Dim button As RadioButton = TryCast(sender, RadioButton)
        If Image1 IsNot Nothing Then
            Select Case button.Name
                Case "FillButton"
                    Image1.Stretch = Windows.UI.Xaml.Media.Stretch.Fill
                    Exit Select
                Case "NoneButton"
                    Image1.Stretch = Windows.UI.Xaml.Media.Stretch.None
                    Exit Select
                Case "UniformButton"
                    Image1.Stretch = Windows.UI.Xaml.Media.Stretch.Uniform
                    Exit Select
                Case "UniformToFillButton"
                    Image1.Stretch = Windows.UI.Xaml.Media.Stretch.UniformToFill
                    Exit Select
                Case Else
                    Exit Select
            End Select
        End If
    End Sub

    Private Sub PopupButton_Click(sender As Object, e As RoutedEventArgs)
        If Not popup.IsOpen Then
            popup.Child = New PopupPanel()
            popup.VerticalOffset = 300.0
            popup.HorizontalOffset = 700.0
            popup.IsOpen = True
        End If

    End Sub

    Private popup As New Popup()

End Class
