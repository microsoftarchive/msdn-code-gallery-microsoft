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
        Implements IScannerHost
        Private _serviceProvider As IServiceProvider

        Public Sub New(ByVal serviceProvider As IServiceProvider)
            _serviceProvider = serviceProvider
        End Sub

#Region "IScannerHost Members"

        Public Sub AddResult(ByVal result As IScanResult, ByVal projectFile As String) Implements IScannerHost.AddResult
            Factory.GetTaskProvider().AddResult(result, projectFile)
        End Sub

        Public Function GetTextOfFileIfOpenInIde(ByVal filePath As String) As String Implements IScannerHost.GetTextOfFileIfOpenInIde
            Dim rdt As IVsRunningDocumentTable = TryCast(_serviceProvider.GetService(GetType(SVsRunningDocumentTable)), IVsRunningDocumentTable)

            Dim hierarchy As IVsHierarchy = Nothing
            Dim itemid As UInteger = 0
            Dim docDataUnk As IntPtr = IntPtr.Zero
            Dim lockCookie As UInteger = 0

            Dim hr As Integer = rdt.FindAndLockDocument(CUInt(_VSRDTFLAGS.RDT_ReadLock), filePath, hierarchy, itemid, docDataUnk, lockCookie)
            Try
                If hr = VSConstants.S_OK Then
                    Dim textLines As IVsTextLines = TryCast(Marshal.GetUniqueObjectForIUnknown(docDataUnk), IVsTextLines)

                    If textLines IsNot Nothing Then
                        Dim text As String = Nothing
                        Dim endLine As Integer = 0
                        Dim endIndex As Integer = 0

                        hr = textLines.GetLastLineIndex(endLine, endIndex)
                        Debug.Assert(hr = VSConstants.S_OK, "GetLastLineIndex did not return S_OK.")

                        hr = textLines.GetLineText(0, 0, endLine, endIndex, text)
                        Debug.Assert(hr = VSConstants.S_OK, "GetLineText did not return S_OK.")

                        Return text
                    End If
                End If

                Return Nothing
            Finally
                If lockCookie <> 0 Then
                    rdt.UnlockDocument(CUInt(_VSRDTFLAGS.RDT_ReadLock), lockCookie)
                End If
            End Try
        End Function

#End Region
    End Class
End Namespace
