'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

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
        PopIn.IsEnabled = False
    End Sub

    Private Sub PopInClick(ByVal sender As Object, ByVal e As RoutedEventArgs)
        PopInStoryboard.Begin()
        PopIn.IsEnabled = False
        PopOut.IsEnabled = True
    End Sub

    Private Sub PopOutClick(ByVal sender As Object, ByVal e As RoutedEventArgs)
        PopOutStoryboard.Begin()
        PopIn.IsEnabled = True
        PopOut.IsEnabled = False
    End Sub

End Class

