Imports System

Class GuidList
    Private Sub New()
    End Sub

    Public Const guidWinformsControlsInstallerPkgString As String = "575f0df7-740f-406e-9c14-44a06eacb049"
    Public Const guidWinformsControlsInstallerCmdSetString As String = "27aa2dc6-a3de-4bee-9ebe-1034f147f199"

    Public Shared ReadOnly guidWinformsControlsInstallerCmdSet As New Guid(guidWinformsControlsInstallerCmdSetString)
End Class