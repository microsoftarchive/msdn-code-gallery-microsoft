' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports SDKTemplate
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

Partial Public NotInheritable Class ScenarioOutput5
    Inherits Page

    ' A pointer back to the main page which is used to gain access to the input and output frames and their content
    Private rootPage As MainPage = Nothing

    Public Property SelectedText As String
        Get
            Return m_SelectedText
        End Get
        Set(value As String)
            m_SelectedText = value
        End Set
    End Property
    Private m_SelectedText As String

    Public Sub New()
        Me.InitializeComponent()
        '#Region "ViewState and Resolution change code for THIS page"
        AddHandler Loaded, AddressOf ScenarioOutput5_Loaded

        ' Hook the ViewState and Resolution changed events.  This is only necessary if you need to modify your
        ' content to fit well in the various view states and/or orientations.
        AddHandler DisplayProperties.LogicalDpiChanged, AddressOf DisplayProperties_LogicalDpiChanged
        '#End Region

        AddHandler textContent.SelectionChanged, AddressOf textContent_SelectionChanged
    End Sub

    Private Sub textContent_SelectionChanged(sender As Object, e As RoutedEventArgs)
        SelectedText = textContent.SelectedText
    End Sub

    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        ' Get a pointer to our main page.
        rootPage = TryCast(e.Parameter, MainPage)
    End Sub

#Region "ViewState and Resolution change code for THIS page"
    Private Sub ScenarioOutput5_Loaded(sender As Object, e As RoutedEventArgs)
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
#End Region

End Class
