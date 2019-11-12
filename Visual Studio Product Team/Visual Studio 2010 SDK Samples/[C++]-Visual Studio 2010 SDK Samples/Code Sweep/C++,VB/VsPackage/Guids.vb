'**************************************************************************

'Copyright (c) Microsoft Corporation. All rights reserved.
'This code is licensed under the Visual Studio SDK license terms.
'THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
'ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
'IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
'PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

'**************************************************************************

' MUST match guid.h (for the satellite DLL)

Imports Microsoft.VisualBasic
Imports System

Namespace Microsoft.Samples.VisualStudio.CodeSweep.VSPackage
    Public Class GuidStrings
        Public Const GuidVSPackagePkg As String = "32E9CE10-DFC9-4b7d-B22A-396DC0B8F6E3"
        Public Const GuidVSPackageCmdSet As String = "4D0C001B-DA7A-4db9-BA55-C76CB3105EEA"
    End Class

    Friend Class GuidList
        Private Sub New()
        End Sub

        Public Shared ReadOnly guidVSPackagePkg As New Guid(GuidStrings.GuidVSPackagePkg)
        Public Shared ReadOnly guidVSPackageCmdSet As New Guid(GuidStrings.GuidVSPackageCmdSet)
    End Class
End Namespace