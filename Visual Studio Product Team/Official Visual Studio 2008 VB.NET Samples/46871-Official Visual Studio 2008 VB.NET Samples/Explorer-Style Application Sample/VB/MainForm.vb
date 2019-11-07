' Copyright (c) Microsoft Corporation. All rights reserved.
Public Class MainForm

    ' Handles the Click event for the "Launch Directory Scanner" button.
    Private Sub btnSimple_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Simple.Click
        Dim sdv As New DirectoryScanner()
        sdv.Show()
    End Sub

    ' Handles the Click event for the "Launch Explorer-Style Viewer" button.
    Private Sub btnExplorer_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Explorer.Click
        Dim esfv As New ExplorerStyleViewer()
        esfv.Show()
    End Sub


    Private Sub exitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles exitToolStripMenuItem.Click
        Me.Close()
    End Sub
End Class
