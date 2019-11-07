'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports System.Collections.ObjectModel

Partial Public NotInheritable Class Scenario7
    Inherits SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current
    Private ocTeams As ObservableCollection(Of Team)

    Public Sub New()
        Me.InitializeComponent()

        AddHandler btnRemoveTeam.Click, AddressOf BtnRemoveTeam_Click
        Scenario7Reset(Nothing, Nothing)
    End Sub

    Private Sub Scenario7Reset(sender As Object, e As RoutedEventArgs)
        If ocTeams IsNot Nothing Then
            RemoveHandler ocTeams.CollectionChanged, AddressOf _ocTeams_CollectionChanged
        End If

        ocTeams = New ObservableCollection(Of Team)(New Teams())
        AddHandler ocTeams.CollectionChanged, AddressOf _ocTeams_CollectionChanged

        teamsCVS.Source = ocTeams

        tbCollectionChangeStatus.Text = String.Empty
    End Sub

    Private Sub BtnRemoveTeam_Click(sender As Object, e As RoutedEventArgs)
        If ocTeams.Count > 0 Then
            Dim index As Integer = 0
            If lbTeams.SelectedItem IsNot Nothing Then
                index = lbTeams.SelectedIndex
            End If
            ocTeams.RemoveAt(index)
        End If
    End Sub

    Private Sub _ocTeams_CollectionChanged(sender As Object, e As System.Collections.Specialized.NotifyCollectionChangedEventArgs)
        tbCollectionChangeStatus.Text = String.Format("Collection was changed. Count = {0}", ocTeams.Count)
    End Sub
End Class
