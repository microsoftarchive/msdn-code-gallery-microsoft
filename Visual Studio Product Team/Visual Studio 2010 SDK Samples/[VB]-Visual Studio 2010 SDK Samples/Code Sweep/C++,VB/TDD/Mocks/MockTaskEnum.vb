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
Imports Microsoft.VisualStudio.Shell.Interop
Imports Microsoft.VisualStudio

Namespace Microsoft.Samples.VisualStudio.CodeSweep.UnitTests
    Friend Class MockTaskEnum
        Implements IVsEnumTaskItems
        Private ReadOnly _items As IList(Of IVsTaskItem)
        Private _next As Integer = 0

        Public Sub New(ByVal items As IList(Of IVsTaskItem))
            _items = items
        End Sub

#Region "IVsEnumTaskItems Members"

        Public Function Clone(<System.Runtime.InteropServices.Out()> ByRef ppenum As IVsEnumTaskItems) As Integer Implements IVsEnumTaskItems.Clone
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function [Next](ByVal celt As UInteger, ByVal rgelt() As Microsoft.VisualStudio.Shell.Interop.IVsTaskItem, Optional ByVal pceltFetched() As UInteger = Nothing) As Integer Implements IVsEnumTaskItems.Next
            pceltFetched(0) = 0
            Do While celt > 0
                If _next >= _items.Count Then
                    Return VSConstants.S_FALSE
                End If
                rgelt(CInt(pceltFetched(0))) = _items(_next)
                _next += 1
                celt = CUInt(celt - 1)
                pceltFetched(0) = CUInt(pceltFetched(0) + 1)
            Loop
            Return VSConstants.S_OK
        End Function

        Public Function Reset() As Integer Implements IVsEnumTaskItems.Reset
            _next = 0
            Return VSConstants.S_OK
        End Function

        Public Function Skip(ByVal celt As UInteger) As Integer Implements IVsEnumTaskItems.Skip
            Throw New Exception("The method or operation is not implemented.")
        End Function

#End Region
    End Class
End Namespace
