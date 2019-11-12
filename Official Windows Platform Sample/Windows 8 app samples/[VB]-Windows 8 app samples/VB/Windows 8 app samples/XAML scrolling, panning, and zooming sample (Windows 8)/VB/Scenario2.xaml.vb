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
Partial Public NotInheritable Class Scenario2
    Inherits SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
        Scenario2Reset()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
    End Sub

    Private Sub ComboBox_SelectionChanged_1(sender As Object, e As SelectionChangedEventArgs)
        If ScrollViewer Is Nothing Then
            Return
        End If

        Dim cb As ComboBox = TryCast(sender, ComboBox)

        If cb IsNot Nothing Then
            Select Case cb.SelectedIndex
                Case 0
                    ' None
                    ScrollViewer.HorizontalSnapPointsType = SnapPointsType.None
                    Exit Select
                Case 1
                    'Optional
                    ScrollViewer.HorizontalSnapPointsType = SnapPointsType.[Optional]
                    Exit Select
                Case 2
                    ' Optional Single
                    ScrollViewer.HorizontalSnapPointsType = SnapPointsType.OptionalSingle
                    Exit Select
                Case 3
                    ' Mandatory
                    ScrollViewer.HorizontalSnapPointsType = SnapPointsType.Mandatory
                    Exit Select
                Case 4
                    ' Mandatory Single
                    ScrollViewer.HorizontalSnapPointsType = SnapPointsType.MandatorySingle
                    Exit Select
                Case Else
                    ScrollViewer.HorizontalSnapPointsType = SnapPointsType.None
                    Exit Select
            End Select
        End If
    End Sub

    Private Sub Scenario2Reset(sender As Object, e As RoutedEventArgs)
        Scenario2Reset()
    End Sub

    Private Sub Scenario2Reset()
        'Restore to defaults
        ScrollViewer.HorizontalSnapPointsType = SnapPointsType.None
        snapPointsCombo.SelectedIndex = 0
    End Sub

End Class
