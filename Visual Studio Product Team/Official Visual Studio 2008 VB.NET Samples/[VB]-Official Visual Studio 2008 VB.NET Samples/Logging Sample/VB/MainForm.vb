' Copyright (c) Microsoft Corporation. All rights reserved.
Public Class MainForm

#Region "Event Handlers"
    Private CurrentMessageType As TraceEventType
    Private Shared LogDirectory As String = "C:\temp\logs\"


    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        For Each enabledListener As TraceListener In My.Application.Log.TraceSource.Listeners
            Me.DataGridView1.Rows.Add(enabledListener.GetType.FullName)
        Next
    End Sub

    Private Sub InformationRadioButton_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles InformationRadioButton.CheckedChanged
        Me.CurrentMessageType = TraceEventType.Information
    End Sub

    Private Sub WarningRadioButton_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles WarningRadioButton.CheckedChanged
        Me.CurrentMessageType = TraceEventType.Warning
    End Sub

    Private Sub ErrorRadioButton_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ErrorRadioButton.CheckedChanged
        Me.CurrentMessageType = TraceEventType.Error
    End Sub

    Private Sub CriticalRadioButton_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CriticalRadioButton.CheckedChanged
        Me.CurrentMessageType = TraceEventType.Critical
    End Sub
#End Region

    Private Sub WriteMessageButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles WriteMessageButton.Click
        If (Me.MessageTextBox.Text <> String.Empty) Then
            My.Application.Log.WriteEntry(Me.MessageTextBox.Text, Me.CurrentMessageType)
            ' Flush all of the listeners.  This typically should not be done in production applcations
            ' in the interest of performance.  It is done here so that you can view the log files while 
            ' the sample is executing.
            My.Application.Log.TraceSource.Flush()
            Me.MessageTextBox.Text = String.Empty
        End If
    End Sub

    Private Sub exitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles exitToolStripMenuItem.Click
        Me.Close()
    End Sub
End Class