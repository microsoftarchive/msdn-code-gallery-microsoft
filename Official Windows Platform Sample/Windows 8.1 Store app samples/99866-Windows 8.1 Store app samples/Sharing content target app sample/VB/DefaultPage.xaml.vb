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

Partial Public NotInheritable Class DefaultPage
    Inherits Page

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Private Async Sub Footer_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Await Windows.System.Launcher.LaunchUriAsync(New Uri(CType(sender, HyperlinkButton).Tag.ToString()))
    End Sub
End Class
