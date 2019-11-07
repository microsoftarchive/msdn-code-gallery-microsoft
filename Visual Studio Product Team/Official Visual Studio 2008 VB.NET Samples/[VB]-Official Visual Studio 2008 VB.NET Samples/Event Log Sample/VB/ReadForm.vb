' Copyright (c) Microsoft Corporation. All rights reserved.
Public Class ReadForm
    ' Stores the name of the log that the user wants to view.
    Private logType As String = ""

    Private Sub cmdViewLogEntries_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnViewLogEntries.Click
        Try
            Const EntriesToDisplay As Integer = 10

            ' In this case the EventLog constructor is passed a string variable for the log name.
            ' This is because the user of the application can choose which log they wish to view 
            ' from the listbox on the form.
            Dim ev As New EventLog(logType, System.Environment.MachineName, _
                "Event Log Sample")

            rchEventLogOutput.Text = "Event log entries (maximum of 10), newest to oldest." & vbCrLf & vbCrLf

            Dim lastLogToShow As Integer = ev.Entries.Count - EntriesToDisplay
            If lastLogToShow < 0 Then
                lastLogToShow = 0
            End If

            ' Display the last 10 records in the chosen log.
            For index As Integer = ev.Entries.Count - 1 To lastLogToShow Step -1
                Dim CurrentEntry As EventLogEntry = ev.Entries(index)
                rchEventLogOutput.Text &= "Event ID : " & CurrentEntry.InstanceId & vbCrLf
                rchEventLogOutput.Text &= "Entry Type : " & _
                    CurrentEntry.EntryType.ToString() & vbCrLf
                rchEventLogOutput.Text &= "Message : " & _
                    CurrentEntry.Message & vbCrLf & vbCrLf
            Next
        Catch secEx As System.Security.SecurityException
            MsgBox("Security exception in reading the event log.", MsgBoxStyle.OKOnly, Me.Text & " Error")
        Catch ex As Exception
            MsgBox("Error accessing logs on the local machine.", MsgBoxStyle.OKOnly, Me.Text & " Error")
        End Try
    End Sub

    Private Sub lstEntryType_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles lstEntryType.SelectedIndexChanged
        ' Store the log that the user selected in the ListBox
        logType = CType(lstEntryType.Items(lstEntryType.SelectedIndex()), String)
    End Sub

    Private Sub ReadForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            For Each currentLog As EventLog In EventLog.GetEventLogs()
                lstEntryType.Items.Add(currentLog.LogDisplayName)
            Next
            lstEntryType.SelectedIndex = 0
        Catch secEx As System.Security.SecurityException
            MsgBox("Security exception in reading the event log.", MsgBoxStyle.OKOnly, Me.Text & " Error")
        Catch ex As Exception
            MsgBox("Error accessing logs on the local machine.", MsgBoxStyle.OKOnly, Me.Text & " Error")
        End Try
    End Sub

    Private Sub CloseToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CloseToolStripMenuItem.Click
        Me.Close()
    End Sub

End Class