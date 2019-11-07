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
Imports System.Xml

Namespace Microsoft.Samples.VisualStudio.CodeSweep.Scanner
    ''' <summary>
    ''' Factory for publicly visible types in the Scanner component.
    ''' </summary>
    Public Class Factory
        ''' <summary>
        ''' Creates an IScanner object.
        ''' </summary>
        Private Sub New()
        End Sub
        Public Shared Function GetScanner() As IScanner
            If _scanner Is Nothing Then
                _scanner = New Scanner()
            End If
            Return _scanner
        End Function

        ''' <summary>
        ''' Creates an ITermTable object for the term table stored in a given file.
        ''' </summary>
        ''' <param name="filePath">The file containing the XML specification for a term table.</param>
        Public Shared Function GetTermTable(ByVal filePath As String) As ITermTable
            Return New TermTable(filePath)
        End Function

#Region "Private Members"

        Private Shared _scanner As IScanner

#End Region ' Private Members
    End Class
End Namespace
