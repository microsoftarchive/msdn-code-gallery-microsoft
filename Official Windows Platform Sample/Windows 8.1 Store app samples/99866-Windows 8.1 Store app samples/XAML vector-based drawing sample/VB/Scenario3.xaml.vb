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
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        Color1Selection.SelectedIndex = 0
        Color2Selection.SelectedIndex = 1
        Color3Selection.SelectedIndex = 2
    End Sub

    Private Sub Color1SelectionChanged(ByVal sender As Object, ByVal e As SelectionChangedEventArgs)
        ' Updates the first gradient stop's color based on the color selected in the ComboBox
        Scenario3GradientStop1.Color = rootPage.ConvertIndexToColor(Color1Selection.SelectedIndex)
    End Sub


    Private Sub Color2SelectionChanged(ByVal sender As Object, ByVal e As SelectionChangedEventArgs)
        ' Updates the second GradientStop's color based on the color selected in the ComboBox
        Scenario3GradientStop2.Color = rootPage.ConvertIndexToColor(Color2Selection.SelectedIndex)
    End Sub

    Private Sub Color3SelectionChanged(ByVal sender As Object, ByVal e As SelectionChangedEventArgs)
        ' Updates the third gradient stop's color based on the color selected in the ComboBox
        Scenario3GradientStop3.Color = rootPage.ConvertIndexToColor(Color3Selection.SelectedIndex)
    End Sub

End Class
