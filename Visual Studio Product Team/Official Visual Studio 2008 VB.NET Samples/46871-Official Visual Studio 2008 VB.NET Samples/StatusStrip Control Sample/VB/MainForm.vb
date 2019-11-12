' Copyright (c) Microsoft Corporation. All rights reserved.
Public Class MainForm

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ' Set the title of the application form.
        CheckLockKeys()
        dateLabel.Text = Today.ToShortDateString
    End Sub

    Private Sub ShowPanelsCheckBox_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ShowPanelsCheckBox.CheckedChanged
        ' If we select the ability to show the panels we want to enable the ability to select which panels to show on the statusbar.
        GroupBox1.Enabled = ShowPanelsCheckBox.Checked

        ' Change the statusbar showpanels property based on the checkbox on the form.
        Me.status.Visible = ShowPanelsCheckBox.Checked
        Me.numLock.Visible = ShowPanelsCheckBox.Checked
        Me.capsLock.Visible = ShowPanelsCheckBox.Checked
        Me.dateLabel.Visible = ShowPanelsCheckBox.Checked
        Me.progressBarStrip.Visible = ShowPanelsCheckBox.Checked

        ' Disable the timer so the progress bar stops progressing
        If ProgressBarPanelCheckBox.Checked And ShowPanelsCheckBox.Checked Then
            Timer1.Enabled = True
        ElseIf Not ShowPanelsCheckBox.Checked Then
            Timer1.Enabled = False
        End If
    End Sub

    Private Sub ProgressBarPanelCheckBox_CheckChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ProgressBarPanelCheckBox.CheckedChanged
        ' Disable the timer if we don't show the progressbar panel on the statusbar.
        Timer1.Enabled = ProgressBarPanelCheckBox.Checked
        Me.progressBarStrip.Visible = Me.ProgressBarPanelCheckBox.Checked
    End Sub

    Private Sub NumLockPanelCheckBox_CheckChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NumLockPanelCheckBox.CheckedChanged
        numLock.Visible = NumLockPanelCheckBox.Checked
    End Sub


    Private Sub CapsLockpanelCheckBox_CheckChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CapsLockpanelCheckBox.CheckedChanged
        capsLock.Visible = CapsLockpanelCheckBox.Checked
    End Sub

    Private Sub DatePanelCheckBox_CheckChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DatePanelCheckBox.CheckedChanged
        Me.dateLabel.Visible = DatePanelCheckBox.Checked
    End Sub

    Private Sub ShowTextPanelCheckBox_CheckChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ShowTextPanelCheckBox.CheckedChanged
        status.Visible = ShowTextPanelCheckBox.Checked
    End Sub

    Private Sub TextBox1_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox1.TextChanged
        ' Update both the status bar text and the text contained within the 1st status bar panel.
        StatusStrip1.Text = TextBox1.Text
        status.Text = TextBox1.Text
    End Sub

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        If Me.progressBarStrip.Value < progressBarStrip.Maximum Then
            progressBarStrip.Increment(progressBarStrip.Step)
        Else
            progressBarStrip.Value = progressBarStrip.Minimum
        End If
        ' We call DoEvents and refresh the status bar so that the drawitem event fires.
        Me.StatusStrip1.Invalidate()
    End Sub


    Private Sub CheckLockKeys()
        ' If the capslock key changes then change the value of the capslock statusbarpanel.
        If My.Computer.Keyboard.CapsLock Then
            capsLock.Text = "CAP"
            capsLock.BorderStyle = Border3DStyle.Raised
        Else
            capsLock.Text = ""
            capsLock.BorderStyle = Border3DStyle.Sunken
        End If

        ' If the numlock key changes then change the value of the numlock statusbarpanel.
        If My.Computer.Keyboard.NumLock Then
            numLock.Text = "NUM"
            numLock.BorderStyle = Border3DStyle.Raised
        Else
            numLock.Text = ""
            numLock.BorderStyle = Border3DStyle.Sunken
        End If
    End Sub

    Private Sub Form1_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown, MyBase.KeyDown, MyBase.KeyDown, MyBase.KeyDown, DatePanelCheckBox.KeyDown, CapsLockpanelCheckBox.KeyDown, ShowTextPanelCheckBox.KeyDown, NumLockPanelCheckBox.KeyDown, ProgressBarPanelCheckBox.KeyDown, ShowPanelsCheckBox.KeyDown, TextBox1.KeyDown, MyBase.KeyDown, MyBase.KeyDown, MyBase.KeyDown, MyBase.KeyDown
        ' Check to see if the key pressed was either of the lock keys we're interested in.
        If e.KeyCode = Keys.CapsLock Or e.KeyCode = Keys.NumLock Then
            CheckLockKeys()
        End If
    End Sub

    Private Sub exitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles exitToolStripMenuItem.Click
        Me.Close()
    End Sub
End Class
