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
Imports Windows.UI.Xaml.Controls.Primitives
Imports Windows.UI.Xaml.Media
Imports Windows.Foundation

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
        AddHandler Color1Selection.SelectionChanged, AddressOf ColorSelectionChanged
        AddHandler Color2Selection.SelectionChanged, AddressOf ColorSelectionChanged
        AddHandler Color3Selection.SelectionChanged, AddressOf ColorSelectionChanged

        AddHandler Color1OffsetSelection.ValueChanged, AddressOf ColorOffsetValueChanged
        AddHandler Color2OffsetSelection.ValueChanged, AddressOf ColorOffsetValueChanged
        AddHandler Color3OffsetSelection.ValueChanged, AddressOf ColorOffsetValueChanged
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        Color1Selection.SelectedIndex = 0
        Color2Selection.SelectedIndex = 1
        Color3Selection.SelectedIndex = 2

        Color1OffsetSelection.Value = Color1OffsetSelection.Minimum
        Color2OffsetSelection.Value = (Color2OffsetSelection.Maximum - Color2OffsetSelection.Minimum) / 2
        Color3OffsetSelection.Value = Color3OffsetSelection.Maximum
    End Sub

    ' Any time a slider or combobox is changed in the description of scenario 3 
    ' an associated event handler calls the LoadGradient function.          
    Private Sub ColorSelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        LoadGradient()
    End Sub

    Private Sub ColorOffsetValueChanged(sender As Object, e As RangeBaseValueChangedEventArgs)
        LoadGradient()
    End Sub

    ' LoadGradient uses the values selected by the scenario 3 description 
    ' comboboxes and sliders to generate a LinearGradientBrush, which is assigned
    ' to the Fill property of the Secenario3Rectangle.
    Private Sub LoadGradient()
        Dim Gradient As New LinearGradientBrush()
        Gradient.StartPoint = New Point(0, 0)
        Gradient.EndPoint = New Point(1, 1)

        Dim Color1 As New GradientStop()
        Color1.Color = rootPage.ConvertIndexToColor(Color1Selection.SelectedIndex)
        Color1.Offset = Color1OffsetSelection.Value / 100.0

        Dim Color2 As New GradientStop()
        Color2.Color = rootPage.ConvertIndexToColor(Color2Selection.SelectedIndex)
        Color2.Offset = Color2OffsetSelection.Value / 100.0

        Dim Color3 As New GradientStop()
        Color3.Color = rootPage.ConvertIndexToColor(Color3Selection.SelectedIndex)
        Color3.Offset = Color3OffsetSelection.Value / 100.0

        Gradient.GradientStops.Add(Color1)
        Gradient.GradientStops.Add(Color2)
        Gradient.GradientStops.Add(Color3)

        Scenario3Rectangle.Fill = Gradient
    End Sub

End Class
