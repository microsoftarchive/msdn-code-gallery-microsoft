' Copyright (c) Microsoft Corporation. All rights reserved.
Public Class Status

    Public Overloads Sub Show(ByVal Message As String)
        Label1.Text = Message
        Me.Show()
        Application.DoEvents()
    End Sub



End Class