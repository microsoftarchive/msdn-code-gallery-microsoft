' Copyright (c) Microsoft Corporation. All rights reserved.
Public Class MySamples

    Private Sub MySamples_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        lblTitle.Text = "Here are a few examples of using My"

        txtMySamples.Text = _
          "My.Computer.Name returns " & My.Computer.Name & vbCrLf & _
          "My.Computer.Clock.LocalTime returns " & My.Computer.Clock.LocalTime.ToString & vbCrLf & _
          "My.Computer.Name returns " & My.Computer.Name & vbCrLf & _
          "My.Application.Info.Description returns " & My.Application.Info.Description & vbCrLf & _
          "My.User.Name returns " & My.User.Name
    End Sub
End Class


