' Copyright (c) Microsoft Corporation. All rights reserved.
Public Class Status

    Public Overloads Sub Show(ByVal message As String)
        lblStatus.Text = message
        Me.Show()
        Application.DoEvents()
    End Sub

End Class