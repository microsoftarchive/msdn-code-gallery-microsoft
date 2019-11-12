' Copyright (c) Microsoft Corporation. All rights reserved.
Public Class MainForm
    Inherits InheritForm.BaseForm

    Private Sub LinkLabel1_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        Dim mySamplesForm As New MySamples
        mySamplesForm.Show()
    End Sub
    Private Sub LinkLabel2_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabel2.LinkClicked
        Dim controlsForm As New ControlSamples
        controlsForm.Show()
    End Sub

    Private Sub MainForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        lblTitle.Text = "Windows Forms Inheritance Demos"
    End Sub

End Class


