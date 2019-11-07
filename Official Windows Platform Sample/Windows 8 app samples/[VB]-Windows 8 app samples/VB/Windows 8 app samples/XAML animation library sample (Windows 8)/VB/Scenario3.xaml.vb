'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation
Imports SDKTemplate
Imports System
Imports Windows.UI.Xaml.Media.Animation
Imports Windows.UI.Xaml.Shapes
Imports Windows.UI.Xaml.Media
Imports Windows.Foundation
Imports Windows.UI
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

        AddHandler rootPage.MainPageResized, AddressOf rootPage_MainPageResized
    End Sub


    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        Scenario3FunctionSelector.SelectedIndex = 0
        Scenario3EasingModeSelector.SelectedIndex = 0
    End Sub

    Private Sub Scenario3EasingFunctionChanged(sender As Object, e As SelectionChangedEventArgs)
        ' stop the storyboard
        Scenario3Storyboard.[Stop]()

        Dim easingFunction As EasingFunctionBase = Nothing

        ' select an easing function based on the user's selection
        Dim selectedFunctionItem As ComboBoxItem = TryCast(Scenario3FunctionSelector.SelectedItem, ComboBoxItem)
        If selectedFunctionItem IsNot Nothing Then
            Select Case selectedFunctionItem.Content.ToString
                Case "BounceEase"
                    easingFunction = New BounceEase()
                    Exit Select
                Case "CircleEase"
                    easingFunction = New CircleEase()
                    Exit Select
                Case "ExponentialEase"
                    easingFunction = New ExponentialEase()
                    Exit Select
                Case "PowerEase"
                    easingFunction = New PowerEase() With {.Power = 0.5}
                    Exit Select
                Case Else
                    Exit Select
            End Select
        End If

        ' if no valid easing function was specified, let the storyboard stay stopped and do not continue
        If easingFunction Is Nothing Then
            Exit Sub
        End If

        Dim selectedEasingModeItem As ComboBoxItem = TryCast(Scenario3EasingModeSelector.SelectedItem, ComboBoxItem)
        ' select an easing mode based on the user's selection, defaulting to EaseIn if no selection was given
        If selectedEasingModeItem IsNot Nothing Then
            Select Case selectedEasingModeItem.Content.ToString
                Case "EaseOut"
                    easingFunction.EasingMode = EasingMode.EaseOut
                    Exit Select
                Case "EaseInOut"
                    easingFunction.EasingMode = EasingMode.EaseInOut
                    Exit Select
                Case Else
                    easingFunction.EasingMode = EasingMode.EaseIn
                    Exit Select
            End Select
        End If

        ' plot a graph of the easing function
        PlotEasingFunctionGraph(easingFunction, 0.005)

        RectanglePositionAnimation.EasingFunction = easingFunction
        'GraphPositionMarkerXAnimation.EasingFunction = easingFunction;
        GraphPositionMarkerYAnimation.EasingFunction = easingFunction

        ' start the storyboard
        Scenario3Storyboard.Begin()
    End Sub

    ' Plots a graph of the passed easing function using the given sampling interval on the "Graph" Canvas control 
    Private Sub PlotEasingFunctionGraph(easingFunction As EasingFunctionBase, samplingInterval As Double)
        Dim UserSettings As New UISettings()
        Graph.Children.Clear()

        Dim path As New Path()
        Dim pathGeometry As New PathGeometry()
        Dim pathFigure As New PathFigure() With { _
            .StartPoint = New Point(0, 0) _
        }
        Dim pathSegmentCollection As New PathSegmentCollection()

        ' Note that an easing function is just like a regular function that operates on doubles.
        ' Here we plot the range of the easing function's output on the y-axis of a graph.
        Dim i As Double = 0
        While i < 1
            Dim x As Double = i * GraphContainer.Width
            Dim y As Double = easingFunction.Ease(i) * GraphContainer.Height

            Dim segment As New LineSegment()
            segment.Point = New Point(x, y)
            pathSegmentCollection.Add(segment)
            i += samplingInterval
        End While

        pathFigure.Segments = pathSegmentCollection
        pathGeometry.Figures.Add(pathFigure)
        path.Data = pathGeometry
        path.Stroke = New SolidColorBrush(UserSettings.UIElementColor(UIElementType.ButtonText))
        path.StrokeThickness = 1

        ' Add the path to the Canvas
        Graph.Children.Add(path)
    End Sub

    Private Sub rootPage_MainPageResized(sender As Object, e As Global.SDKTemplate.MainPage.MainPageSizeChangedEventArgs)
        Select Case e.ViewState
            Case Windows.UI.ViewManagement.ApplicationViewState.Snapped
                InputPanel.Orientation = Orientation.Vertical
                FunctionPanel.Margin = New Thickness(0.0, 0.0, 0.0, 10.0)
                Exit Select
            Case Else
                InputPanel.Orientation = Orientation.Horizontal
                Exit Select
        End Select
    End Sub

End Class
