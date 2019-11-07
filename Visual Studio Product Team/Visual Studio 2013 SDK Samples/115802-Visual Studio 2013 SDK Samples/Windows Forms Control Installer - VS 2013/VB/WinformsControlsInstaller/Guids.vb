'**************************************************************************
'
'Copyright (c) Microsoft Corporation. All rights reserved.
'THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
'ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
'IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
'PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'**************************************************************************

Imports System

Class GuidList
    Private Sub New()
    End Sub

    Public Const guidWinformsControlsInstallerPkgString As String = "575f0df7-740f-406e-9c14-44a06eacb049"
    Public Const guidWinformsControlsInstallerCmdSetString As String = "27aa2dc6-a3de-4bee-9ebe-1034f147f199"

    Public Shared ReadOnly guidWinformsControlsInstallerCmdSet As New Guid(guidWinformsControlsInstallerCmdSetString)
End Class