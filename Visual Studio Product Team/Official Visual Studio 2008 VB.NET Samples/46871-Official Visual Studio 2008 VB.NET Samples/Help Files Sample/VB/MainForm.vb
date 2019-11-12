' Copyright (c) Microsoft Corporation. All rights reserved.
Public Class MainForm


    Private Sub txtNumberValue_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txtNumberValue.Validating
        If Not IsNumeric(txtNumberValue.Text) Then
            ' Activate the error provider to notify the user of a
            ' problem.
            ErrorProvider1.SetError(txtNumberValue, "Not a numeric value.")
        Else
            ' Clear the Error
            ErrorProvider1.SetError(txtNumberValue, "")
        End If
    End Sub


    Private Sub exitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles exitToolStripMenuItem.Click
        Me.Close()
    End Sub

    Private Sub contentsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles contentsToolStripMenuItem.Click
        ' Show the contents of the help file.
        Help.ShowHelp(Me, hpAdvancedCHM.HelpNamespace)
    End Sub

    Private Sub indexToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles indexToolStripMenuItem.Click
        ' Show index of the help file.
        Help.ShowHelpIndex(Me, hpAdvancedCHM.HelpNamespace)
    End Sub

    Private Sub searchToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles searchToolStripMenuItem.Click
        ' Show the search tab of the help file.
        Help.ShowHelp(Me, hpAdvancedCHM.HelpNamespace, HelpNavigator.Find, "")
    End Sub

    Private Sub ExitMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Me.Close()
    End Sub

End Class