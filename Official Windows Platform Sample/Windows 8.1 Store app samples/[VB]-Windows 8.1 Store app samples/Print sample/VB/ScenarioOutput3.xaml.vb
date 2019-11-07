' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports System

Partial Public NotInheritable Class ScenarioOutput3
    Inherits Page

    ' A pointer back to the main page which is used to gain access to the input and output frames and their content
    Private rootPage As MainPage = Nothing

    Public Sub New()
        InitializeComponent()

        AddHandler Loaded, AddressOf ScenarioOutput3_Loaded

        ' Hook the Width and Resolution changed events.  This is only necessary if you need to modify your
        ' content to fit well in the various view states and/or orientations.
        AddHandler Window.Current.SizeChanged, AddressOf ScenarioOutput3_SizeChanged
    End Sub

    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        ' Get a pointer to our main page.
        rootPage = TryCast(e.Parameter, MainPage)
    End Sub

    Private Sub ScenarioOutput3_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
        CheckLayout()
    End Sub

    ' You may or may not need to handle resolution and view state changes in your specific scenario page content.
    ' It will simply depend on your content.  In the case of this specific example, we need to adjust the content 
    ' to fit well when the application is in portrait or when snapped.



    Private Sub CheckLayout()
        Dim visualState As String = If(Me.ActualWidth < 768, "Below768Layout", "DefaultLayout")
        VisualStateManager.GoToState(Me, visualState, False)
    End Sub

    Private Sub ScenarioOutput3_SizeChanged(ByVal sender As Object, ByVal args As Windows.UI.Core.WindowSizeChangedEventArgs)
        CheckLayout()
    End Sub

End Class

