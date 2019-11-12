' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

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

Partial Public NotInheritable Class ScenarioOutput3
    Inherits Page

    ' A pointer back to the main page which is used to gain access to the input and output frames and their content
    Private rootPage As MainPage = Nothing

    Public Sub New()
        InitializeComponent()

        '#Region "ViewState and Resolution change code for THIS page - Remove unless you need it"
        AddHandler Loaded, AddressOf ScenarioOutput3_Loaded

        ' Hook the ViewState and Resolution changed events.  This is only necessary if you need to modify your
        ' content to fit well in the various view states and/or orientations.
        AddHandler Window.Current.SizeChanged, AddressOf ScenarioOutput3_SizeChanged
        AddHandler DisplayProperties.LogicalDpiChanged, AddressOf DisplayProperties_LogicalDpiChanged
        '#End Region

    End Sub

#Region "ViewState and Resolution change code for THIS page - Remove unless you need it"
    Private Sub ScenarioOutput3_Loaded(sender As Object, e As RoutedEventArgs)
        CheckResolutionAndViewState()
    End Sub

    ' You may or may not need to handle resolution and view state changes in your specific scenario page content.
    ' It will simply depend on your content.  In the case of this specific example, we need to adjust the content
    ' to fit well when the application is in portrait or when snapped.

    Private Sub DisplayProperties_LogicalDpiChanged(sender As Object)
        CheckResolutionAndViewState()
    End Sub

    Private Sub CheckResolutionAndViewState()
        VisualStateManager.GoToState(Me, ApplicationView.Value.ToString & DisplayProperties.ResolutionScale.ToString, False)
    End Sub

    Private Sub ScenarioOutput3_SizeChanged(sender As Object, args As Windows.UI.Core.WindowSizeChangedEventArgs)
        CheckResolutionAndViewState()
    End Sub
#End Region

#Region "Template-Related Code - Do not remove"
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        ' Get a pointer to our main page.
        rootPage = TryCast(e.Parameter, MainPage)

        ' We want to be notified with the OutputFrame is loaded so we can get to the content.
        AddHandler rootPage.InputFrameLoaded, AddressOf rootPage_InputFrameLoaded
    End Sub

    Protected Overrides Sub OnNavigatedFrom(e As NavigationEventArgs)
        RemoveHandler rootPage.InputFrameLoaded, AddressOf rootPage_InputFrameLoaded
    End Sub
#End Region

#Region "Use this code if you need access to elements in the input frame - otherwise delete"
    Private Sub rootPage_InputFrameLoaded(sender As Object, e As Object)
        ' At this point, we know that the Input Frame has been loaded and we can go ahead
        ' and reference elements in the page contained in the Input Frame.

        ' Get a pointer to the content within the IntputFrame.
        Dim inputFrame As Page = DirectCast(rootPage.InputFrame.Content, Page)

        ' Go find the elements that we need for this scenario.
        ' ex: flipView1 = TryCast(inputFrame.FindName("FlipView1"), FlipView)
    End Sub
#End Region

End Class
