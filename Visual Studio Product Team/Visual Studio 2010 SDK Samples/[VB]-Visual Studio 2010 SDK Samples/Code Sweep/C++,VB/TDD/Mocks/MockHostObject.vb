'**************************************************************************

'Copyright (c) Microsoft Corporation. All rights reserved.
'This code is licensed under the Visual Studio SDK license terms.
'THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
'ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
'IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
'PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

'**************************************************************************


Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.Samples.VisualStudio.CodeSweep.VSPackage
Imports Microsoft.Samples.VisualStudio.CodeSweep.Scanner
Imports Microsoft.Samples.VisualStudio.CodeSweep.BuildTask

Namespace Microsoft.Samples.VisualStudio.CodeSweep.UnitTests
    Friend Class MockHostObject
        Implements IScannerHost

        Public Class AddResultArgs
            Inherits EventArgs
            Public ReadOnly Result As IScanResult
            Public ReadOnly ProjectFile As String

            Public Sub New(ByVal result As IScanResult, ByVal projectFile As String)
                Me.Result = result
                Me.ProjectFile = projectFile
            End Sub
        End Class

        Public Event OnAddResult As EventHandler(Of AddResultArgs)


#Region "IScannerHost Members"

        Public Sub AddResult(ByVal result As IScanResult, ByVal projectFile As String) Implements IScannerHost.AddResult
            If OnAddResultEvent IsNot Nothing Then
                RaiseEvent OnAddResult(Me, New AddResultArgs(result, projectFile))
            End If
        End Sub

        Public Function GetTextOfFileIfOpenInIde(ByVal filePath As String) As String Implements IScannerHost.GetTextOfFileIfOpenInIde
            Return Nothing
        End Function

#End Region
    End Class
End Namespace
