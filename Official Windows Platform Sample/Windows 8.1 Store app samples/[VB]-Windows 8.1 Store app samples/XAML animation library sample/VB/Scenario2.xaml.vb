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

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario2
    Inherits SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Private isScenario2StoryboardRunning As Boolean = False

    Public Sub New()
        Me.InitializeComponent()
        AddHandler Scenario2ToggleStoryboard.Click, AddressOf Scenario2ToggleStoryboard_Click
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
    End Sub

    Private Sub Scenario2ToggleStoryboard_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' toggle the storyboards to stop or begin them depending on the current state
        If isScenario2StoryboardRunning Then
            ToggleScenario2(False)
        Else
            ToggleScenario2(True)
        End If
    End Sub

    Private Sub ToggleScenario2(ByVal startScenario As Boolean)
        If Not startScenario Then
            ' stop the storyboards
            Scenario2ContinuousStoryboard.Stop()
            Scenario2KeyFrameStoryboard.Stop()
            Scenario2ToggleStoryboard.Content = "Begin storyboards"
        Else
            ' start the storyboards
            Scenario2ContinuousStoryboard.Begin()
            Scenario2KeyFrameStoryboard.Begin()
            Scenario2ToggleStoryboard.Content = "Stop storyboards"
        End If
        isScenario2StoryboardRunning = startScenario
    End Sub

End Class
