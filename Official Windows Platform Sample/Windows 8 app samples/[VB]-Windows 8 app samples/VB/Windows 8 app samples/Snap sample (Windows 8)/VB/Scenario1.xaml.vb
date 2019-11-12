'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.Foundation
Imports Windows.UI.Input
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation
Imports Windows.UI.Xaml.Input
Imports Windows.UI.ViewManagement
Imports SDKTemplate
Imports System

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario1
    Inherits SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

#Region "Class Variables"
    Private programmaticUnsnapSucceeded As Boolean = False
#End Region

    Public Sub New()
        Me.InitializeComponent()

        Scenario1Init()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        Me.ShowCurrentViewState()
    End Sub

#Region "ApplicationView event handling logic"
    Private Sub ShowCurrentViewState()
        Me.UpdateUnsnapButtonState()

        Dim currentState As ApplicationViewState = Windows.UI.ViewManagement.ApplicationView.Value

        If currentState = ApplicationViewState.Snapped Then
            Scenario1OutputText.Text = "This app is snapped."
        ElseIf currentState = ApplicationViewState.Filled Then
            Scenario1OutputText.Text = "This app is in the fill state."
        ElseIf currentState = ApplicationViewState.FullScreenLandscape Then
            Scenario1OutputText.Text = "This app is full-screen landscape."
        ElseIf currentState = ApplicationViewState.FullScreenPortrait Then
            Scenario1OutputText.Text = "This app is full-screen portrait."
        End If
    End Sub

    Public Sub OnSizeChanged(sender As Object, args As Windows.UI.Core.WindowSizeChangedEventArgs)
        Select Case Windows.UI.ViewManagement.ApplicationView.Value
            Case Windows.UI.ViewManagement.ApplicationViewState.Filled
                VisualStateManager.GoToState(Me, "Fill", False)
                Exit Select
            Case Windows.UI.ViewManagement.ApplicationViewState.FullScreenLandscape
                VisualStateManager.GoToState(Me, "Full", False)
                Exit Select
            Case Windows.UI.ViewManagement.ApplicationViewState.Snapped
                VisualStateManager.GoToState(Me, "Snapped", False)
                Exit Select
            Case Windows.UI.ViewManagement.ApplicationViewState.FullScreenPortrait
                VisualStateManager.GoToState(Me, "Portrait", False)
                Exit Select
            Case Else
                Exit Select
        End Select

        Me.ShowCurrentViewState()
    End Sub

    Private Sub UnsnapButton_Click(sender As Object, e As RoutedEventArgs)
        UnsnapButton.IsEnabled = False
        Me.programmaticUnsnapSucceeded = Windows.UI.ViewManagement.ApplicationView.TryUnsnap()
        Me.UpdateUnsnapButtonState()

        If Not Me.programmaticUnsnapSucceeded Then
            Scenario1OutputText.Text = "Programmatic unsnap did not work."
        End If
    End Sub

    Private Sub UpdateUnsnapButtonState()
        If Windows.UI.ViewManagement.ApplicationView.Value = ApplicationViewState.Snapped Then
            UnsnapButton.Visibility = Visibility.Visible
            UnsnapButton.IsEnabled = True
        Else
            UnsnapButton.Visibility = Visibility.Collapsed
            UnsnapButton.IsEnabled = False
        End If
    End Sub
#End Region

#Region "Adding and removing a SizeChanged event handler"
    Private Sub Scenario1Init()
        AddHandler Window.Current.SizeChanged, AddressOf OnSizeChanged
        AddHandler UnsnapButton.Click, AddressOf UnsnapButton_Click
    End Sub

    Private Sub Scenario1Reset()
        RemoveHandler Window.Current.SizeChanged, AddressOf OnSizeChanged
    End Sub
#End Region
End Class
