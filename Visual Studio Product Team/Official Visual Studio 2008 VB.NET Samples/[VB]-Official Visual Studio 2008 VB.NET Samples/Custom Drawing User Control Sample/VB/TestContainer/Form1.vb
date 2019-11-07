' Copyright (c) Microsoft Corporation. All rights reserved.
Public Class Form1

    Private Sub ShowZerosCheckBox_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ShowZerosCheckBox.CheckedChanged
        If ShowZerosCheckBox.Checked Then
            ScoreBoard1.LeadingZeros = True
        Else
            ScoreBoard1.LeadingZeros = False
        End If
    End Sub

    Private Sub ChangeFillColorButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ChangeFillColorButton.Click
        If ColorDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            ScoreBoard1.NumberColor = ColorDialog1.Color
            BeadedScoreBoard1.BeadColor = ColorDialog1.Color
        End If
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    End Sub

    Private Sub ShowOutlineCheckBox_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ShowOutlineCheckBox.CheckedChanged
        If ShowOutlineCheckBox.Checked Then
            ScoreBoard1.Outline = True
            BeadedScoreBoard1.Outline = True
        Else
            ScoreBoard1.Outline = False
            BeadedScoreBoard1.Outline = False
        End If
    End Sub

    Private Sub ChangeOutlineColorButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ChangeOutlineColorButton.Click

        If ColorDialog2.ShowDialog = Windows.Forms.DialogResult.OK Then
            ScoreBoard1.OutlineColor = ColorDialog2.Color
            BeadedScoreBoard1.BeadOutlineColor = ColorDialog2.Color
        End If

    End Sub

    Private Sub TrackBar1_Scroll(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TrackBar1.Scroll
        ScoreBoard1.Digits = TrackBar1.Value
        TrackBar2.Maximum = CInt(10 ^ TrackBar1.Value - 1)
    End Sub

    Private Sub TrackBar2_Scroll(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TrackBar2.Scroll
        ScoreBoard1.Score = TrackBar2.Value
    End Sub

    Private Sub TrackBar3_Scroll(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TrackBar3.Scroll
        BeadedScoreBoard1.BeadCount = TrackBar3.Value
    End Sub

    Private Sub AddOneButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddOneButton.Click
        BeadedScoreBoard1.Score += 1
    End Sub

    Private Sub SubtractOneButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SubtractOneButton.Click
        BeadedScoreBoard1.Score -= 1
    End Sub

    Private Sub ExitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitToolStripMenuItem.Click
        End
    End Sub
End Class
