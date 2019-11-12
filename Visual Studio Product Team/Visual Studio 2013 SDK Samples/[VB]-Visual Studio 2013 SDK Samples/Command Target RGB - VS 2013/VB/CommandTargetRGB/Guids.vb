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

    Public Const guidCommandTargetRGBPkgString As String = "e738fa52-7ee1-48a2-b7ba-6ef1698b046b"
    Public Const guidCommandTargetRGBCmdSetString As String = "00dcae55-4379-40a6-b152-3a38de753f29"
    Public Const guidToolWindowPersistenceString As String = "0bdb1e08-ed8b-47e8-91b2-e9bd814b4ebb"

    Public Shared ReadOnly guidCommandTargetRGBCmdSet As New Guid(guidCommandTargetRGBCmdSetString)
End Class