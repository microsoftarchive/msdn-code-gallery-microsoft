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
Imports Windows.UI.Xaml.Media

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario2
    Inherits SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    ' Used to track the orientation state of the FlipView for Scenario #2
    Private bHorizontal As Boolean = True

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        Dim sampleData = New Controls_FlipView.Data.SampleDataSource()
        FlipView2Horizontal.ItemsSource = sampleData.Items
        FlipView2Vertical.ItemsSource = sampleData.Items
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Orientation' button.  
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Orientation_Click(sender As Object, e As RoutedEventArgs)
        bHorizontal = Not bHorizontal
        If bHorizontal Then
            Orientation.Content = "Vertical"
            FlipView2Horizontal.SelectedIndex = FlipView2Vertical.SelectedIndex
            FlipView2Horizontal.Visibility = Windows.UI.Xaml.Visibility.Visible
            FlipView2Vertical.Visibility = Windows.UI.Xaml.Visibility.Collapsed
        Else
            Orientation.Content = "Horizontal"
            FlipView2Vertical.SelectedIndex = FlipView2Horizontal.SelectedIndex
            FlipView2Vertical.Visibility = Windows.UI.Xaml.Visibility.Visible
            FlipView2Horizontal.Visibility = Windows.UI.Xaml.Visibility.Collapsed
        End If
    End Sub
End Class
