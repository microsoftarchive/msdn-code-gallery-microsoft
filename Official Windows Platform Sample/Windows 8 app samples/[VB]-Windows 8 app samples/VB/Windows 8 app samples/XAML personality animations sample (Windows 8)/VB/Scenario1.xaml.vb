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
Imports Windows.UI.Xaml.Media.Animation

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario1
    Inherits SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
        PopIn.IsEnabled = false
    End Sub

    Private Sub PopInClick(sender As Object, e As RoutedEventArgs)
        PopInStoryboard.Begin()
        PopIn.IsEnabled = false
        PopOut.IsEnabled = true
    End Sub

    Private Sub PopOutClick(sender As Object, e As RoutedEventArgs)
        PopOutStoryboard.Begin()
        PopIn.IsEnabled = true
        PopOut.IsEnabled = false
    End Sub
End Class
