Imports System

Class GuidList
    Private Sub New()
    End Sub

    Public Const guidBuildProgressBarPkgString As String = "646a5020-a33d-4509-864e-9dc6bf7206f0"
    Public Const guidBuildProgressBarCmdSetString As String = "413e9fca-f16d-4883-86c1-9d6f6e8af3dd"
    Public Const guidToolWindowPersistanceString As String = "1bcb49dc-47f9-4eba-8d7d-b2baefe89076"

    Public Shared ReadOnly guidBuildProgressBarCmdSet As New Guid(guidBuildProgressBarCmdSetString)
End Class