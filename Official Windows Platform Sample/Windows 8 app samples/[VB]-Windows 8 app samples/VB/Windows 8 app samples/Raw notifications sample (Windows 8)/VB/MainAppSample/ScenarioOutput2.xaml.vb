' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports System
Imports System.Linq
Imports System.Collections.Generic
Imports Windows.Foundation
Imports Windows.Foundation.Collections
Imports Windows.Graphics.Display
Imports Windows.UI.ViewManagement
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Controls.Primitives
Imports Windows.UI.Xaml.Data
Imports Windows.UI.Xaml.Input
Imports Windows.UI.Xaml.Media
Imports Windows.UI.Xaml.Navigation
Imports SDKTemplate

Partial Public NotInheritable Class ScenarioOutput2
    Inherits Page
    ' A pointer back to the main page which is used to gain access to the input and output frames and their content
    Private rootPage As MainPage = Nothing

    Public Sub New()
        InitializeComponent()
    End Sub

#Region "Template-Related Code - Do not remove"
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        ' Get a pointer to our main page
        rootPage = TryCast(e.Parameter, MainPage)

        ' We want to be notified with the OutputFrame is loaded so we can get to the content
        AddHandler rootPage.InputFrameLoaded, AddressOf rootPage_InputFrameLoaded
    End Sub

    Protected Overrides Sub OnNavigatedFrom(e As NavigationEventArgs)
        RemoveHandler rootPage.InputFrameLoaded, AddressOf rootPage_InputFrameLoaded
    End Sub
#End Region

#Region "Use this code if you need access to elements in the input frame - otherwise delete"
    Private Sub rootPage_InputFrameLoaded(sender As Object, e As Object)
        ' At this point, we know that the Input Frame has been loaded and we can go ahead
        ' and reference elements in the page contained in the Input Frame

        ' Get a pointer to the content within the IntputFrame
        Dim inputFrame As Page = DirectCast(rootPage.InputFrame.Content, Page)

        ' Go find the elements that we need for this scenario
        ' ex: flipView1 = inputFrame.FindName("FlipView1") as FlipView;
    End Sub
#End Region
End Class
