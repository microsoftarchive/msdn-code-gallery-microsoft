' Copyright (c) Microsoft Corporation. All rights reserved.
Public Class Options

    Private Sub Options_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.DataGrid1.DataSource = HighScores.GetHighScores()
    End Sub

    Private Sub resetScores_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles resetScores.Click
        HighScores.ResetScores()
        Me.DataGrid1.DataSource = HighScores.GetHighScores()
    End Sub

    Public Property SoundOn() As Boolean
        Get
            Return Me.isSoundOn.Checked
        End Get
        Set(ByVal Value As Boolean)
            Me.isSoundOn.Checked = Value
        End Set
    End Property


End Class