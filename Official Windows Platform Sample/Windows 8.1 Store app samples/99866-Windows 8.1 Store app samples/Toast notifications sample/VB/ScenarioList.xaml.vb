' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports System

Partial Public NotInheritable Class ScenarioList
    Inherits Page

    ' A pointer back to the main page which is used to gain access to the input and output frames and their content.
    Private rootPage As MainPage = Nothing

    Public Sub New()
        InitializeComponent()
    End Sub

    Public Property SelectedIndex() As Integer
        Get
            Return Scenarios.SelectedIndex
        End Get
        Set(ByVal value As Integer)
            If value <> Scenarios.SelectedIndex Then
                Scenarios.SelectedIndex = value
            End If
        End Set
    End Property

#Region "Template-Related Code - Do not remove"
    Private Sub Scenarios_SelectionChanged(ByVal sender As Object, ByVal e As SelectionChangedEventArgs)
        If Scenarios.SelectedItem IsNot Nothing Then
            rootPage.NotifyUser("", NotifyType.StatusMessage)

            Dim selectedListBoxItem As ListBoxItem = TryCast(Scenarios.SelectedItem, ListBoxItem)
            SuspensionManager.SessionState("SelectedScenario") = selectedListBoxItem.Name

            If rootPage.InputFrame IsNot Nothing AndAlso rootPage.OutputFrame IsNot Nothing Then
                ' Load the input and output pages for the current scenario into their respective frames.

                rootPage.DoNavigation(Type.GetType(GetType(ScenarioList).Namespace & "." & "ScenarioInput" & ((Scenarios.SelectedIndex + 1).ToString())), rootPage.InputFrame, Type.GetType(GetType(ScenarioList).Namespace & "." & "ScenarioOutput" & ((Scenarios.SelectedIndex + 1).ToString())), rootPage.OutputFrame)
            End If
        End If
    End Sub

    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        rootPage = TryCast(e.Parameter, MainPage)
        AddHandler Scenarios.SelectionChanged, AddressOf Scenarios_SelectionChanged

        ' Starting scenario is the first or based upon a previous selection.
        Dim startingScenario As ListBoxItem = Nothing
        If SuspensionManager.SessionState.ContainsKey("SelectedScenario") Then
            Dim selectedScenarioName As String = TryCast(SuspensionManager.SessionState("SelectedScenario"), String)
            startingScenario = TryCast(Me.FindName(selectedScenarioName), ListBoxItem)
        End If

        Scenarios.SelectedItem = If(startingScenario IsNot Nothing, startingScenario, Scenario1)
    End Sub
#End Region
End Class
