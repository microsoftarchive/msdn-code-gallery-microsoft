' Copyright (c) Microsoft Corporation. All rights reserved.
Imports System.ServiceProcess

Public Class Form1

    ' Used to access an instance of the selected service.
    Private msvc As ServiceController
    Private controllers As New System.Collections.Generic.SortedList(Of String, ServiceController)

    Private Sub cmdPause_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdPause.Click
        Try
            msvc.Pause()
            UpdateUIForSelectedService()
        Catch exp As Exception
            MsgBox("Cannot pause service.", MsgBoxStyle.OKOnly, Me.Text)
        End Try
    End Sub

    Private Sub cmdResume_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdResume.Click
        Try
            msvc.Continue()
            UpdateUIForSelectedService()
        Catch exp As Exception
            MsgBox("Cannot resume service.", MsgBoxStyle.OKOnly, Me.Text)
        End Try
    End Sub

    Private Sub cmdStart_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdStart.Click
        Try
            msvc.Start()
            UpdateUIForSelectedService()
        Catch exp As Exception
            MsgBox("Cannot start service.", MsgBoxStyle.OKOnly, Me.Text)
        End Try
    End Sub

    Private Sub cmdStop_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdStop.Click
        Try
            msvc.Stop()
            UpdateUIForSelectedService()
        Catch exp As Exception
            MsgBox("Cannot stop service.", MsgBoxStyle.OKOnly, Me.Text)
        End Try
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        EnumServices()
    End Sub

    Private Sub lvServices_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lvServices.SelectedIndexChanged
        UpdateUIForSelectedService()
    End Sub

    Private Sub EnumServices()
        ' Get the list of available services and
        ' load the list view control with the information
        Try
            Me.ToolStripStatusLabel1.Text = "Loading Service List, pleasse wait"

            Me.lvServices.Items.Clear()

            If controllers IsNot Nothing Then
                controllers = New Generic.SortedList(Of String, ServiceController)
            End If

            Dim services As ServiceController() = ServiceController.GetServices()
            For Each controller As ServiceController In services
                With Me.lvServices.Items.Add(controller.DisplayName)
                    .SubItems.Add(controller.Status.ToString())
                    .SubItems.Add(controller.ServiceType.ToString())
                End With
                controllers.Add(controller.DisplayName, controller)
            Next controller
        Catch exp As Exception
            MsgBox("Cannot enumerate the services.")
        Finally
            ToolStripStatusLabel1.Text = "Ready"
        End Try
    End Sub

    Private Sub UpdateServiceStatus()
        ' Check each service
        Try
            ToolStripStatusLabel1.Text = "Checking Service Status . . "
            Dim item As ListViewItem
            For Each item In Me.lvServices.Items
                msvc = controllers.Item(item.Text)
                msvc.Refresh()
                item.SubItems(1).Text = msvc.Status.ToString()
            Next item
            UpdateUIForSelectedService()
        Catch exp As Exception
            MessageBox.Show(exp.Message, exp.Source, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            ToolStripStatusLabel1.Text = "Ready"
        End Try
    End Sub

    Private Sub UpdateUIForSelectedService()
        ' Update the command buttons for the selected service.
        Dim strName As String

        Try
            If Me.lvServices.SelectedItems.Count = 1 Then
                strName = Me.lvServices.SelectedItems(0).SubItems(0).Text
                msvc = controllers.Item(strName)
                With msvc
                    ' If it's stopped, we should be able to start it
                    Me.cmdStart.Enabled = (.Status = ServiceControllerStatus.Stopped)
                    ' Check if we're allowed to try and stop it and make sure it's not
                    ' already stopped.
                    Me.cmdStop.Enabled = (.CanStop AndAlso (Not .Status = ServiceControllerStatus.Stopped))
                    ' Check if we're allowed to pause it and see if it is not paused
                    ' already.
                    Me.cmdPause.Enabled = (.CanPauseAndContinue AndAlso (Not .Status = ServiceControllerStatus.Paused))
                    ' If it's paused, we must be able to resume it.
                    Me.cmdResume.Enabled = (.Status = ServiceControllerStatus.Paused)
                End With
            End If
        Catch exp As Exception
            MsgBox("Cannot update UI.", MsgBoxStyle.OKOnly, Me.Text)
        End Try
    End Sub

    Private Sub RefreshToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RefreshToolStripMenuItem.Click
        UpdateServiceStatus()
    End Sub

    Private Sub RelistMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripMenuItem2.Click
        EnumServices()
    End Sub

    Private Sub exitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles exitToolStripMenuItem.Click
        Me.Close()
    End Sub
End Class
