' Copyright (c) Microsoft Corporation. All rights reserved.
Public Class PictureForm

    Private Sub PictureForm_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.MediaPictureBox.Image = System.Drawing.Image.FromFile("Winter.jpg")
    End Sub
End Class