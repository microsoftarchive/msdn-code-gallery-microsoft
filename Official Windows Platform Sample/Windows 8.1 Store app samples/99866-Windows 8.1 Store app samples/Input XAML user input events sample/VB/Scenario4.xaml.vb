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
Partial Public NotInheritable Class Scenario4
    Inherits SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current
    Private _transformGroup As TransformGroup
    Private _previousTransform As MatrixTransform
    Private _compositeTransform As CompositeTransform
    Private forceManipulationsToEnd As Boolean

    Public Sub New()
        Me.InitializeComponent()
        forceManipulationsToEnd = False
        AddHandler ManipulateMe.ManipulationStarting, AddressOf ManipulateMe_ManipulationStarting
        AddHandler ManipulateMe.ManipulationStarted, AddressOf ManipulateMe_ManipulationStarted
        AddHandler ManipulateMe.ManipulationDelta, AddressOf ManipulateMe_ManipulationDelta
        AddHandler ManipulateMe.ManipulationCompleted, AddressOf ManipulateMe_ManipulationCompleted
        AddHandler ManipulateMe.ManipulationInertiaStarting, AddressOf ManipulateMe_ManipulationInertiaStarting
        InitManipulationTransforms()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
    End Sub

    Private Sub InitManipulationTransforms()
        _transformGroup = New TransformGroup()
        _compositeTransform = New CompositeTransform()
        _previousTransform = New MatrixTransform() With {.Matrix = Matrix.Identity}

        _transformGroup.Children.Add(_previousTransform)
        _transformGroup.Children.Add(_compositeTransform)

        ManipulateMe.RenderTransform = _transformGroup
    End Sub

    Private Sub ManipulateMe_ManipulationStarting(ByVal sender As Object, ByVal e As ManipulationStartingRoutedEventArgs)
        forceManipulationsToEnd = False
        e.Handled = True
    End Sub

    Private Sub ManipulateMe_ManipulationStarted(ByVal sender As Object, ByVal e As ManipulationStartedRoutedEventArgs)
        e.Handled = True
    End Sub

    Private Sub ManipulateMe_ManipulationInertiaStarting(ByVal sender As Object, ByVal e As ManipulationInertiaStartingRoutedEventArgs)
        e.Handled = True
    End Sub

    Private Sub ManipulateMe_ManipulationDelta(ByVal sender As Object, ByVal e As ManipulationDeltaRoutedEventArgs)
        If forceManipulationsToEnd Then
            e.Complete()
            Return
        End If

        _previousTransform.Matrix = _transformGroup.Value

        Dim center As Point = _previousTransform.TransformPoint(New Point(e.Position.X, e.Position.Y))
        _compositeTransform.CenterX = center.X
        _compositeTransform.CenterY = center.Y

        _compositeTransform.Rotation = e.Delta.Rotation
        _compositeTransform.ScaleY = e.Delta.Scale
        _compositeTransform.ScaleX = _compositeTransform.ScaleY
        _compositeTransform.TranslateX = e.Delta.Translation.X
        _compositeTransform.TranslateY = e.Delta.Translation.Y

        e.Handled = True
    End Sub

    Private Sub ManipulateMe_ManipulationCompleted(ByVal sender As Object, ByVal e As ManipulationCompletedRoutedEventArgs)
        e.Handled = True
    End Sub

    Private Sub Scenario4_Reset(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Scenario4_Reset()
    End Sub

    Private Sub Scenario4_Reset()
        forceManipulationsToEnd = True
        ManipulateMe.RenderTransform = Nothing
        InitManipulationTransforms()
    End Sub
End Class
