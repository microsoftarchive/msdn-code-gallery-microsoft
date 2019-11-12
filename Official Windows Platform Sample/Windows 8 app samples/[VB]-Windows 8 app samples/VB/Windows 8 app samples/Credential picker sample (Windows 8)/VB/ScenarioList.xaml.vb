' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports System.Linq
Imports System.Collections.Generic
Imports Windows.Foundation
Imports Windows.Foundation.Collections
Imports Windows.Graphics.Display
Imports Windows.UI.ViewManagement
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Controls.Primitives
Imports Windows.UI.Xaml.Data
Imports Windows.UI.Xaml.Input
Imports Windows.UI.Xaml.Media
Imports Windows.UI.Xaml.Navigation
Imports SDKTemplate


Partial Public NotInheritable Class ScenarioList
    Inherits Page

    ' A pointer back to the main page which is used to gain access to the input and output frames and their content.
    Private rootPage As Global.SDKTemplate.MainPage = Nothing

    Public Sub New()
        InitializeComponent()
    End Sub

#Region "Template-Related Code - Do not remove"
    Private Sub Scenarios_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        If Scenarios.SelectedItem IsNot Nothing Then
            rootPage.NotifyUser("", Global.SDKTemplate.NotifyType.StatusMessage)

            Dim selectedListBoxItem As ListBoxItem = TryCast(Scenarios.SelectedItem, ListBoxItem)
            SuspensionManager.SessionState("SelectedScenario") = selectedListBoxItem.Name

            If rootPage.InputFrame IsNot Nothing AndAlso rootPage.OutputFrame IsNot Nothing Then
                ' Load the input and output pages for the current scenario into their respective frames.

                rootPage.DoNavigation(Type.GetType(GetType(ScenarioList).Namespace & "." & "ScenarioInput" & ((Scenarios.SelectedIndex + 1).ToString)), rootPage.InputFrame, Type.GetType(GetType(ScenarioList).Namespace & "." & "ScenarioOutput" & ((Scenarios.SelectedIndex + 1).ToString)), rootPage.OutputFrame)
            End If
        End If
    End Sub

    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        rootPage = TryCast(e.Parameter, Global.SDKTemplate.MainPage)
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
