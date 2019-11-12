' Copyright (c) Microsoft Corporation. All rights reserved.
Public Class WriteForm

    Private entryType As System.Diagnostics.EventLogEntryType = EventLogEntryType.Information

    Private Sub cmdWriteEntry_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles btnWriteEntry.Click

        Try
            If IsNumeric(txtEventID.Text) Then
                ' The first entry is the name of the log you want to write to.  The second 
                ' parameter is the machine name.  In this case it is the local machine name.
                ' The third parameter is the source of the event.  Commonly this is set equal
                ' to the name of the application that is writing the event.
                Dim ev As New EventLog("Application", My.Computer.Name, "Event Log Sample")

                ' The first argument to WriteEntry is the text of the message.  The second 
                ' argument is the type of entry you want to create (Warning, Information, etc.)
                ' The third is the eventID of the event.  The user could use this to look up 
                ' further information in a help file.
                ev.WriteEntry(txtEntry.Text, entryType, CInt(txtEventID.Text))

                ev.Close()

                MsgBox("Entry written to the event log", MsgBoxStyle.OKOnly, Me.Text)
            Else
                ' The EventID was not numeric
                MsgBox("Value entered into EventID text box must be numeric.", MsgBoxStyle.OKOnly, _
                    Me.Text & " Error")
            End If
        Catch secEx As System.Security.SecurityException
            MsgBox("Security error writing to the event log.", _
                MsgBoxStyle.OKOnly, Me.Text & " Error")
        Catch ex As Exception
            MsgBox("Error accessing logs on the local machine.", MsgBoxStyle.OKOnly, Me.Text & " Error")
        End Try
    End Sub

    Private Sub rdo_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles rdoError.Click, rdoInfo.Click, rdoWarning.Click

        ' This event procedure handles the click event for all the radio buttons in 
        ' the group box on the form.  In the event handler, we know which radio 
        ' button was clicked because it is passed in as the "sender" argument.
        ' This comes in as a generic object, however, and must be cast back to a
        ' radio button so that you can access the name property.

        Dim rdo As RadioButton = CType(sender, RadioButton)
        Select Case rdo.Name
            Case "rdoError"
                entryType = EventLogEntryType.Error
            Case "rdoWarning"
                entryType = EventLogEntryType.Warning
            Case "rdoInfo"
                entryType = EventLogEntryType.Information
            Case Else
                MsgBox("Select an entry type.")
        End Select

    End Sub

    Private Sub CloseToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CloseToolStripMenuItem.Click
        Me.Close()
    End Sub
End Class