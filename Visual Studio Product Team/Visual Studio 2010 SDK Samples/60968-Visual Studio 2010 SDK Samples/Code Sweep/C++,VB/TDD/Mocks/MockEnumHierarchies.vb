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
    Friend Class MockEnumHierarchies
        Implements IEnumHierarchies
        Private _projects As List(Of MockIVsProject)
        Private _next As Integer = 0

        Public Sub New(ByVal projects As IEnumerable(Of MockIVsProject))
            _projects = New List(Of MockIVsProject)(projects)
        End Sub

#Region "IEnumHierarchies Members"

        Public Function Clone(<System.Runtime.InteropServices.Out()> ByRef ppenum As IEnumHierarchies) As Integer Implements IEnumHierarchies.Clone
            ppenum = New MockEnumHierarchies(_projects)
            Return VSConstants.S_OK
        End Function

        Public Function [Next](ByVal celt As UInteger, ByVal rgelt As IVsHierarchy(), <System.Runtime.InteropServices.Out()> ByRef pceltFetched As UInteger) As Integer Implements IEnumHierarchies.Next
            pceltFetched = 0

            Do While pceltFetched < celt AndAlso _next < _projects.Count
                rgelt(CInt(pceltFetched)) = _projects(_next)
                pceltFetched = CUInt(pceltFetched + 1)
                _next += 1
            Loop

            If pceltFetched = celt Then
                Return VSConstants.S_OK
            Else
                Return VSConstants.S_FALSE
            End If
        End Function

        Public Function Reset() As Integer Implements IEnumHierarchies.Reset
            _next = 0
            Return VSConstants.S_OK
        End Function

        Public Function Skip(ByVal celt As UInteger) As Integer Implements IEnumHierarchies.Skip
            Dim items As IVsHierarchy() = New IVsHierarchy(CInt(celt - 1)) {}
            Dim fetched As UInteger

            Return [Next](celt, items, fetched)
        End Function

#End Region
    End Class
End Namespace
