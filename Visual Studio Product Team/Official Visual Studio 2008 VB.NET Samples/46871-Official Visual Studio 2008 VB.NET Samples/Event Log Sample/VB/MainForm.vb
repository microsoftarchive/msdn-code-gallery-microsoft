' Copyright (c) Microsoft Corporation. All rights reserved.
Public Class MainForm

    '' When writing to an event log you need to pass the machine name where 
    '' the log resides.  Here the MachineName Property of the Environment class 
    '' is used to determine the name of the local machine.  Assuming you have 
    '' the appropriate permissions it is also easy to write to event logs on 
    '' other machines.
    'Private machineName As String = Environment.MachineName
    'Private logType As String = ""
    'Private entryType As EventLogEntryType = EventLogEntryType.Error

    Private Sub btnWrite_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnWrite.Click
        Dim frmWrite As New WriteForm()
        frmWrite.ShowDialog()
    End Sub

    Private Sub btnRead_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRead.Click
        Dim frmRead As New ReadForm()
        frmRead.ShowDialog()
    End Sub

    Private Sub btnCreateDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCreateDelete.Click
        Dim frmCreateDelete As New CreateDeleteForm()
        frmCreateDelete.ShowDialog()
    End Sub

    Private Sub exitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles exitToolStripMenuItem.Click
        Me.Close()
    End Sub
End Class