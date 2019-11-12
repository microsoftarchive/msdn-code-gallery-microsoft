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
Imports Windows.Foundation
Imports Windows.UI.Input
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Input
Imports Windows.UI.Xaml.Media
Imports Windows.UI.Xaml.Navigation
Imports SDKTemplate

' The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

''' <summary>
''' A basic page that provides characteristics common to most applications.
''' </summary>
Partial Public NotInheritable Class Scenario2
    Inherits SDKTemplate.Common.LayoutAwarePage
#Region "Class Variables"
    Private edgeGesture As EdgeGesture

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current
#End Region

    Public Sub New()
        Me.InitializeComponent()
        Me.edgeGesture = edgeGesture.GetForCurrentView()
        Scenario2Init()
    End Sub

#Region "EdgeGesture Event Handlers"
    Private Sub OnContextMenu(sender As Object, e As Windows.UI.Xaml.Input.RightTappedRoutedEventArgs)
        Scenario2OutputText.Text = "Invoked with right-click."
    End Sub

    Private Sub EdgeGesture_Canceled(sender As Object, e As EdgeGestureEventArgs)
        Scenario2OutputText.Text = "Canceled with touch."
    End Sub

    Private Sub EdgeGesture_Completed(sender As Object, e As EdgeGestureEventArgs)
        If e.Kind = EdgeGestureKind.Touch Then
            Scenario2OutputText.Text = "Invoked with touch."
        ElseIf e.Kind = EdgeGestureKind.Keyboard Then
            Scenario2OutputText.Text = "Invoked with keyboard."
        ElseIf e.Kind = EdgeGestureKind.Mouse Then
            Scenario2OutputText.Text = "Invoked with right-click."
        End If
    End Sub

    Private Sub EdgeGesture_Starting(sender As Object, e As EdgeGestureEventArgs)
        Scenario2OutputText.Text = "Invoking with touch."
    End Sub

    Private Sub RightClickOverride(sender As Object, e As Windows.UI.Xaml.Input.RightTappedRoutedEventArgs)
        Scenario2OutputText.Text = "The ContextMenu event was handled.  The EdgeGesture event will not fire."
        e.Handled = True
    End Sub

    Private Sub Scenario2Init()
        AddHandler MainPage.Current.RightTapped, AddressOf Me.OnContextMenu
        AddHandler Me.edgeGesture.Canceled, AddressOf Me.EdgeGesture_Canceled
        AddHandler Me.edgeGesture.Completed, AddressOf Me.EdgeGesture_Completed
        AddHandler Me.edgeGesture.Starting, AddressOf Me.EdgeGesture_Starting
        AddHandler RightClickRegion.RightTapped, AddressOf Me.RightClickOverride

        Scenario2OutputText.Text = "Sample initialized and events registered."
    End Sub

    Private Sub Scenario2Reset()
        RemoveHandler MainPage.Current.RightTapped, AddressOf Me.OnContextMenu
        RemoveHandler Me.edgeGesture.Canceled, AddressOf Me.EdgeGesture_Canceled
        RemoveHandler Me.edgeGesture.Completed, AddressOf Me.EdgeGesture_Completed
        RemoveHandler Me.edgeGesture.Starting, AddressOf Me.EdgeGesture_Starting
        RemoveHandler RightClickRegion.RightTapped, AddressOf Me.RightClickOverride
    End Sub
#End Region

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
    End Sub
End Class
