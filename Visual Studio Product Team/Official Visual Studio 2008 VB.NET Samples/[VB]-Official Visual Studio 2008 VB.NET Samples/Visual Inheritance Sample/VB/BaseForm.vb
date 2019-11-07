' Copyright (c) Microsoft Corporation. All rights reserved.
Public Class BaseForm

    Private Sub btnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

    Private Sub frmBase_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        lblDateTime.Text = My.Computer.Clock.LocalTime.ToLongDateString & " - " & _
  My.Computer.Clock.LocalTime.ToShortTimeString

        lblPrivate.Text = ""
        lblProtected.Text = ""
    End Sub
End Class



