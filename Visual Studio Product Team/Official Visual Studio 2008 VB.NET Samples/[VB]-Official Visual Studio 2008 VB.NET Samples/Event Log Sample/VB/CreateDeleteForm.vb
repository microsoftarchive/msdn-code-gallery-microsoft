' Copyright (c) Microsoft Corporation. All rights reserved.
Public Class CreateDeleteForm
    Private Sub cmdCreateLog_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCreateLog.Click
        Try
            ' Next check for the existence of the log that the user wants to create.
            If Not EventLog.Exists(txtLogNameToCreate.Text) Then
                ' If the event source is already registered we want to delete it 
                ' before recreating it on the call to CreateEventSource
                If EventLog.SourceExists("Event Log Sample") Then
                    EventLog.DeleteEventSource("Event Log Sample")
                End If
                EventLog.CreateEventSource("Event Log Sample", txtLogNameToCreate.Text)
                MsgBox("Event log created.")
            Else
                MsgBox("Log already exists: " & txtLogNameToCreate.Text, MsgBoxStyle.OKOnly, Me.Text & " Error")
            End If
        Catch ex As Exception
            MsgBox("Unable to create event log.", MsgBoxStyle.OKOnly, Me.Text & " Error")
        End Try
    End Sub

    Private Sub cmdDeleteLog_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDeleteLog.Click
        Try
            If EventLog.Exists(txtLogNameToDelete.Text) Then
                'Next call the delete method of the log that the user wants to delete.
                EventLog.Delete(txtLogNameToDelete.Text)
                MsgBox("Event log deleted.")
            Else
                MsgBox("Log does not exist: " & txtLogNameToDelete.Text, MsgBoxStyle.OKOnly, Me.Text & " Error")
            End If
        Catch ex As Exception
            MsgBox("Unable to delete event log.", MsgBoxStyle.OKOnly, Me.Text & " Error")
        End Try
    End Sub

    Private Sub CloseToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CloseToolStripMenuItem.Click
        Me.Close()
    End Sub
End Class