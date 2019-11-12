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
Imports Windows.UI.Xaml.Media

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario2
    Inherits SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current
    Private _pointerCount As Integer

    Public Sub New()
        Me.InitializeComponent()
        AddHandler bEnteredExited.PointerEntered, AddressOf bEnteredExited_PointerEntered
        AddHandler bEnteredExited.PointerExited, AddressOf bEnteredExited_PointerExited
        AddHandler bEnteredExited.PointerPressed, AddressOf bEnteredExited_PointerPressed
        AddHandler bEnteredExited.PointerReleased, AddressOf bEnteredExited_PointerReleased
        AddHandler bEnteredExited.PointerMoved, AddressOf bEnteredExited_PointerMoved

        'To code for multiple Pointers (i.e. Fingers) we track how many entered/exited.
        _pointerCount = 0
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
    End Sub

    Private Sub bEnteredExited_PointerMoved(ByVal sender As Object, ByVal e As PointerRoutedEventArgs)
        Scenario2UpdateVisuals(TryCast(sender, Border), "Moved")
    End Sub

    Private Sub bEnteredExited_PointerReleased(ByVal sender As Object, ByVal e As PointerRoutedEventArgs)
        CType(sender, Border).ReleasePointerCapture(e.Pointer)
        txtCaptureStatus.Text = String.Empty
    End Sub

    'Can only get capture on PointerPressed (i.e. touch down, mouse click, pen press)
    Private Sub bEnteredExited_PointerPressed(ByVal sender As Object, ByVal e As PointerRoutedEventArgs)
        If tbPointerCapture.IsOn Then
            Dim _hasCapture As Boolean = CType(sender, Border).CapturePointer(e.Pointer)
            txtCaptureStatus.Text = "Got Capture: " & _hasCapture
        End If
    End Sub

    Private Sub bEnteredExited_PointerExited(ByVal sender As Object, ByVal e As PointerRoutedEventArgs)
        _pointerCount -= 1
        Scenario2UpdateVisuals(TryCast(sender, Border), "Exited")
    End Sub

    Private Sub bEnteredExited_PointerEntered(ByVal sender As Object, ByVal e As PointerRoutedEventArgs)
        _pointerCount += 1
        Scenario2UpdateVisuals(TryCast(sender, Border), "Entered")

    End Sub

    Private Sub Scenario2UpdateVisuals(ByVal border As Border, ByVal eventDescription As String)
        Select Case eventDescription.ToLower()
            Case "exited"
                If _pointerCount <= 0 Then
                    border.Background = New SolidColorBrush(Colors.Red)
                    bEnteredExitedTextBlock.Text = eventDescription
                End If
            Case "moved"

                Dim rt As RotateTransform = CType(bEnteredExitedTimer.RenderTransform, RotateTransform)
                rt.Angle += 2
                If rt.Angle > 360 Then
                    rt.Angle -= 360
                End If
            Case Else
                border.Background = New SolidColorBrush(Colors.Green)
                bEnteredExitedTextBlock.Text = eventDescription

        End Select
    End Sub

    Private Sub Scenario2_Reset(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Scenario2_Reset()
    End Sub

    Private Sub Scenario2_Reset()
        bEnteredExited.Background = New SolidColorBrush(Colors.Red)
        bEnteredExitedTextBlock.Text = String.Empty
    End Sub
End Class
