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

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario2
    Inherits SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
        AddHandler FillSelection.SelectionChanged, AddressOf FillSelection_SelectionChanged
        AddHandler StrokeThicknessSelection.ValueChanged, AddressOf StrokeWidthSelection_ValueChanged
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        FillSelection.SelectedIndex = 0
    End Sub

    Private Sub FillSelection_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        Scenario2Rectangle.Fill = New SolidColorBrush(rootPage.ConvertIndexToColor(FillSelection.SelectedIndex))
    End Sub

    ' Any time a slider or combobox is changed in the description of scenario 2 
    ' the associated property of Scenario2Rectangle (StrokeThickness or Fill) is changed 
    ' appropriately.
    Private Sub StrokeWidthSelection_ValueChanged(sender As Object, e As RangeBaseValueChangedEventArgs)
        Scenario2Rectangle.StrokeThickness = StrokeThicknessSelection.Value
    End Sub

End Class
