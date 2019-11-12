Imports System

Class GuidList
    Private Sub New()
    End Sub

    Public Const guidCommandTargetRGBPkgString As String = "a7dcebeb-e498-4d25-a554-e59003a2b7ee"
    Public Const guidCommandTargetRGBCmdSetString As String = "00dcae55-4379-40a6-b152-3a38de753f29"
    Public Const guidToolWindowPersistanceString As String = "0bdb1e08-ed8b-47e8-91b2-e9bd814b4ebb"

    Public Shared ReadOnly guidCommandTargetRGBCmdSet As New Guid(guidCommandTargetRGBCmdSetString)
End Class