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

Namespace Microsoft.Samples.VisualStudio.CodeSweep.UnitTests
    Friend Class MockScanResult
        Implements IScanResult
        Private _filePath As String
        Private _hits As IEnumerable(Of IScanHit)
        Private _scanned As Boolean

        Public Sub New(ByVal filePath As String, ByVal hits As IEnumerable(Of IScanHit), ByVal scanned As Boolean)
            _filePath = filePath
            _hits = hits
            _scanned = scanned
        End Sub

#Region "IScanResult Members"

        Public ReadOnly Property FilePath() As String Implements IScanResult.FilePath
            Get
                Return _filePath
            End Get
        End Property

        Public ReadOnly Property Results() As IEnumerable(Of IScanHit) Implements IScanResult.Results
            Get
                Return _hits
            End Get
        End Property

        Public ReadOnly Property Scanned() As Boolean Implements IScanResult.Scanned
            Get
                Return _scanned
            End Get
        End Property

        Public ReadOnly Property Passed() As Boolean Implements IScanResult.Passed
            Get
                If _scanned AndAlso _hits IsNot Nothing Then
                    For Each hit As IScanHit In _hits
                        ' If there are any hits, it didn't pass.
                        Return False
                    Next hit
                    Return True
                End If
                Return False
            End Get
        End Property

#End Region
    End Class
End Namespace
