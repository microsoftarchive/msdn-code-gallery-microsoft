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
Imports SDKTemplate
Imports Windows.Graphics.Display
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation

Partial Public NotInheritable Class Scenario3
    Inherits SDKTemplate.Common.LayoutAwarePage
    Public Sub New()
        InitializeComponent()
    End Sub

    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        screenOrientation.Text = DisplayProperties.CurrentOrientation.ToString
        AddHandler DisplayProperties.OrientationChanged, AddressOf OnOrientationChanged
    End Sub

    Protected Overrides Sub OnNavigatedFrom(e As NavigationEventArgs)
        RemoveHandler DisplayProperties.OrientationChanged, AddressOf OnOrientationChanged
    End Sub

    Private Sub OnOrientationChanged(sender As Object)
        screenOrientation.Text = DisplayProperties.CurrentOrientation.ToString
    End Sub
End Class
