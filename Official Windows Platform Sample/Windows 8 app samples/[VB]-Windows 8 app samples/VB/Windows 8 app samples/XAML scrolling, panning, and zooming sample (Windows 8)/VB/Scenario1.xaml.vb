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


''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario1
    Inherits SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
        Scenario1Reset()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
    End Sub


    Private Sub CheckBox_Checked_HorizontalRailed(sender As Object, e As RoutedEventArgs)
        ScrollViewer.IsHorizontalRailEnabled = True
    End Sub

    Private Sub CheckBox_Unchecked_HorizontalRailed(sender As Object, e As RoutedEventArgs)
        ScrollViewer.IsHorizontalRailEnabled = False
    End Sub

    Private Sub CheckBox_Checked_VerticalRailed(sender As Object, e As RoutedEventArgs)
        ScrollViewer.IsVerticalRailEnabled = True
    End Sub

    Private Sub CheckBox_Unchecked_VerticalRailed(sender As Object, e As RoutedEventArgs)
        ScrollViewer.IsVerticalRailEnabled = False
    End Sub

    Private Sub hsmCombo_SelectionChanged_1(sender As Object, e As SelectionChangedEventArgs)
        If ScrollViewer Is Nothing Then
            Return
        End If

        Dim cb As ComboBox = TryCast(sender, ComboBox)

        If cb IsNot Nothing Then
            Select Case cb.SelectedIndex
                Case 0
                    ' Auto
                    ScrollViewer.HorizontalScrollMode = ScrollMode.Auto
                    Exit Select
                Case 1
                    'Enabled
                    ScrollViewer.HorizontalScrollMode = ScrollMode.Enabled
                    Exit Select
                Case 2
                    ' Disabled
                    ScrollViewer.HorizontalScrollMode = ScrollMode.Disabled
                    Exit Select
                Case Else
                    ScrollViewer.HorizontalScrollMode = ScrollMode.Enabled
                    Exit Select
            End Select
        End If
    End Sub

    Private Sub hsbvCombo_SelectionChanged_1(sender As Object, e As SelectionChangedEventArgs)
        If ScrollViewer Is Nothing Then
            Return
        End If

        Dim cb As ComboBox = TryCast(sender, ComboBox)

        If cb IsNot Nothing Then
            Select Case cb.SelectedIndex
                Case 0
                    ' Auto
                    ScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto
                    Exit Select
                Case 1
                    'Visible
                    ScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible
                    Exit Select
                Case 2
                    ' Hidden
                    ScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden
                    Exit Select
                Case 3
                    ' Disabled
                    ScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled
                    Exit Select
                Case Else
                    ScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled
                    Exit Select
            End Select
        End If
    End Sub

    Private Sub vsmCombo_SelectionChanged_1(sender As Object, e As SelectionChangedEventArgs)
        If ScrollViewer Is Nothing Then
            Return
        End If

        Dim cb As ComboBox = TryCast(sender, ComboBox)

        If cb IsNot Nothing Then
            Select Case cb.SelectedIndex
                Case 0
                    ' Auto
                    ScrollViewer.VerticalScrollMode = ScrollMode.Auto
                    Exit Select
                Case 1
                    'Enabled
                    ScrollViewer.VerticalScrollMode = ScrollMode.Enabled
                    Exit Select
                Case 2
                    ' Disabled
                    ScrollViewer.VerticalScrollMode = ScrollMode.Disabled
                    Exit Select
                Case Else
                    ScrollViewer.VerticalScrollMode = ScrollMode.Enabled
                    Exit Select
            End Select
        End If
    End Sub

    Private Sub vsbvCombo_SelectionChanged_1(sender As Object, e As SelectionChangedEventArgs)
        If ScrollViewer Is Nothing Then
            Return
        End If

        Dim cb As ComboBox = TryCast(sender, ComboBox)

        If cb IsNot Nothing Then
            Select Case cb.SelectedIndex
                Case 0
                    ' Auto
                    ScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto
                    Exit Select
                Case 1
                    'Visible
                    ScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Visible
                    Exit Select
                Case 2
                    ' Hidden
                    ScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden
                    Exit Select
                Case 3
                    ' Disabled
                    ScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled
                    Exit Select
                Case Else
                    ScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Visible
                    Exit Select
            End Select
        End If
    End Sub

    Private Sub Scenario1Reset(sender As Object, e As RoutedEventArgs)
        Scenario1Reset()
    End Sub

    Private Sub Scenario1Reset()
        'Restore to defaults
        hsbvCombo.SelectedIndex = 3
        hsmCombo.SelectedIndex = 1
        vsbvCombo.SelectedIndex = 1
        vsmCombo.SelectedIndex = 1
        hrCheckbox.IsChecked = True
        vrCheckbox.IsChecked = True
    End Sub

End Class
