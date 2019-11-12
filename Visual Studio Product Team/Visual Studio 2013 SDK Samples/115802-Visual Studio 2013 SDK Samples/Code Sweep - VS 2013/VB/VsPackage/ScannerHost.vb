'**************************************************************************
'
'Copyright (c) Microsoft Corporation. All rights reserved.
'THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
'ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
'IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
'PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'**************************************************************************

Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.Samples.VisualStudio.CodeSweep.Scanner
Imports Microsoft.VisualStudio.TextManager.Interop
Imports Microsoft.VisualStudio.Shell.Interop
Imports System.Diagnostics
Imports Microsoft.VisualStudio
Imports System.Runtime.InteropServices
Imports System.IO
Imports Microsoft.Samples.VisualStudio.CodeSweep.BuildTask

Namespace Microsoft.Samples.VisualStudio.CodeSweep.VSPackage
    Friend Class ScannerHost
        Inherits MarshalByRefObject
        Implements IScannerHost

        Public Overrides Function InitializeLifetimeService() As Object
            Return Nothing ' infinite lifetime
        End Function

#Region "IScannerHost Members"

        Public Sub AddResult(ByVal result As IScanResult, ByVal projectFile As String) Implements IScannerHost.AddResult
            Factory.GetTaskProvider().AddResult(result, projectFile)
        End Sub

#End Region ' IScannerHost Members
    End Class
End Namespace
