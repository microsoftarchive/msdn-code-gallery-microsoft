'**************************************************************************

'Copyright (c) Microsoft Corporation. All rights reserved.
'This code is licensed under the Visual Studio SDK license terms.
'THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
'ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
'IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
'PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

'**************************************************************************

' MUST match PkgCmdID.h (for the satellite DLL)

Imports Microsoft.VisualBasic
Imports System

Namespace Microsoft.Samples.VisualStudio.CodeSweep.VSPackage
    Friend Class PkgCmdIDList
        Private Sub New()
        End Sub
        Public Const cmdidConfig As UInteger = &H100
        Public Const cmdidStopScan As UInteger = &H101
        Public Const cmdidRepeatLastScan As UInteger = &H102
        Public Const cmdidIgnore As UInteger = &H103
        Public Const cmdidDoNotIgnore As UInteger = &H104
        Public Const cmdidShowIgnoredInstances As UInteger = &H105

    End Class
End Namespace