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
Imports Windows.UI.Xaml.Media.Animation
Imports Windows.UI.Xaml.Shapes
Imports Windows.UI.Xaml.Media
Imports Windows.UI.ViewManagement

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario3
    Inherits SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()

        AddHandler Scenario3FunctionSelector.SelectionChanged, AddressOf Scenario3EasingFunctionChanged
        AddHandler Scenario3EasingModeSelector.SelectionChanged, AddressOf Scenario3EasingFunctionChanged
    End Sub

    Private Sub rootPage_MainPageResized(ByVal sender As Object, ByVal e As MainPageSizeChangedEventArgs)
        If Window.Current.Bounds.Width < 768 Then
            InputPanel.Orientation = Orientation.Vertical
            FunctionPanel.Margin = New Thickness(0.0, 0.0, 0.0, 10.0)
        Else
            InputPanel.Orientation = Orientation.Horizontal
        End If
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        AddHandler rootPage.MainPageResized, AddressOf rootPage_MainPageResized

        Scenario3FunctionSelector.SelectedIndex = 0
        Scenario3EasingModeSelector.SelectedIndex = 0
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be navigated away from in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>

    Protected Overrides Sub OnNavigatedFrom(ByVal e As NavigationEventArgs)
        RemoveHandler rootPage.MainPageResized, AddressOf rootPage_MainPageResized
    End Sub

    Private Sub Scenario3EasingFunctionChanged(ByVal sender As Object, ByVal e As SelectionChangedEventArgs)
        ' stop the storyboard
        Scenario3Storyboard.Stop()

        Dim easingFunction As EasingFunctionBase = Nothing

        ' select an easing function based on the user's selection
        Dim selectedFunctionItem As ComboBoxItem = TryCast(Scenario3FunctionSelector.SelectedItem, ComboBoxItem)
        If selectedFunctionItem IsNot Nothing Then
            Select Case selectedFunctionItem.Content.ToString()
                Case "BounceEase"
                    easingFunction = New BounceEase()
                Case "CircleEase"
                    easingFunction = New CircleEase()
                Case "ExponentialEase"
                    easingFunction = New ExponentialEase()
                Case "PowerEase"
                    easingFunction = New PowerEase() With {.Power = 0.5}
                Case Else
            End Select
        End If

        ' if no valid easing function was specified, let the storyboard stay stopped and do not continue
        If easingFunction Is Nothing Then
            Return
        End If

        Dim selectedEasingModeItem As ComboBoxItem = TryCast(Scenario3EasingModeSelector.SelectedItem, ComboBoxItem)
        ' select an easing mode based on the user's selection, defaulting to EaseIn if no selection was given
        If selectedEasingModeItem IsNot Nothing Then
            Select Case selectedEasingModeItem.Content.ToString()
                Case "EaseOut"
                    easingFunction.EasingMode = EasingMode.EaseOut
                Case "EaseInOut"
                    easingFunction.EasingMode = EasingMode.EaseInOut
                Case Else
                    easingFunction.EasingMode = EasingMode.EaseIn
            End Select
        End If

        ' plot a graph of the easing function
        PlotEasingFunctionGraph(easingFunction, 0.005)

        RectanglePositionAnimation.EasingFunction = easingFunction
        GraphPositionMarkerYAnimation.EasingFunction = easingFunction

        ' start the storyboard
        Scenario3Storyboard.Begin()
    End Sub

    ' Plots a graph of the passed easing function using the given sampling interval on the "Graph" Canvas control 
    Private Sub PlotEasingFunctionGraph(ByVal easingFunction As EasingFunctionBase, ByVal samplingInterval As Double)
        Dim UserSettings As New UISettings()
        GraphCanvas.Children.Clear()

        Dim path As New Path()
        Dim pathGeometry As New PathGeometry()
        Dim pathFigure As New PathFigure() With {.StartPoint = New Point(0, 0)}
        Dim pathSegmentCollection As New PathSegmentCollection()

        ' Note that an easing function is just like a regular function that operates on doubles.
        ' Here we plot the range of the easing function's output on the y-axis of a graph.
        For i As Double = 0 To 1 Step samplingInterval
            Dim x As Double = i * GraphContainer.Width
            Dim y As Double = easingFunction.Ease(i) * GraphContainer.Height

            Dim segment As New LineSegment()
            segment.Point = New Point(x, y)
            pathSegmentCollection.Add(segment)
        Next i

        pathFigure.Segments = pathSegmentCollection
        pathGeometry.Figures.Add(pathFigure)
        path.Data = pathGeometry
        path.Stroke = New SolidColorBrush(UserSettings.UIElementColor(UIElementType.ButtonText))
        path.StrokeThickness = 1

        ' Add the path to the Canvas
        GraphCanvas.Children.Add(path)
    End Sub

End Class
