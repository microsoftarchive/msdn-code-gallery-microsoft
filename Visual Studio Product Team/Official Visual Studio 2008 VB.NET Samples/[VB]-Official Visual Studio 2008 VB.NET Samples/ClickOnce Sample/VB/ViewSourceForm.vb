' Copyright (c) Microsoft Corporation. All rights reserved.
Public Class ViewSourceForm

    Private Sub ViewSourceForm_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.SourceTextbox.Text = My.Resources.MainFormSourceCode
    End Sub
End Class