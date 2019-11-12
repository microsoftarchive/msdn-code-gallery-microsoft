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
Imports Windows.UI.Xaml.Input
Imports Windows.UI.Xaml.Media
Imports Windows.UI

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
        AddHandler bTapped.Tapped, AddressOf bTapped_Tapped
        AddHandler bDoubleTapped.DoubleTapped, AddressOf bDoubleTapped_DoubleTapped
        AddHandler bRightTapped.RightTapped, AddressOf bRightTapped_RightTapped
        AddHandler bHolding.Holding, AddressOf bHolding_Holding
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
    End Sub

    Private Sub Scenario3UpdateVisuals(border As Border, gesture As String)
        Select Case gesture.ToLower()
            Case "holding"
                border.Background = New SolidColorBrush(Colors.Yellow)
                Exit Select
            Case Else
                border.Background = New SolidColorBrush(Colors.Green)
                Exit Select
        End Select

        DirectCast(border.Child, TextBlock).Text = gesture
    End Sub

    Private Sub bHolding_Holding(sender As Object, e As HoldingRoutedEventArgs)
        Dim holdingState As String = If((e.HoldingState = Windows.UI.Input.HoldingState.Started), "Holding", "Held")
        Scenario3UpdateVisuals(TryCast(sender, Border), holdingState)
    End Sub

    Private Sub bDoubleTapped_DoubleTapped(sender As Object, e As DoubleTappedRoutedEventArgs)
        Scenario3UpdateVisuals(TryCast(sender, Border), "Double Tapped")
    End Sub
    Private Sub bRightTapped_RightTapped(sender As Object, e As RightTappedRoutedEventArgs)
        Scenario3UpdateVisuals(TryCast(sender, Border), "Right Tapped")
    End Sub
    Private Sub bTapped_Tapped(sender As Object, e As TappedRoutedEventArgs)
        Scenario3UpdateVisuals(TryCast(sender, Border), "Tapped")
    End Sub

    Private Sub Scenario3ResetMethod(sender As Object, e As RoutedEventArgs)
        Reset()
    End Sub

    Private Sub Reset()
        bTapped.Background = New SolidColorBrush(Colors.Red)
        bHolding.Background = New SolidColorBrush(Colors.Red)
        bDoubleTapped.Background = New SolidColorBrush(Colors.Red)
    End Sub

   

End Class
