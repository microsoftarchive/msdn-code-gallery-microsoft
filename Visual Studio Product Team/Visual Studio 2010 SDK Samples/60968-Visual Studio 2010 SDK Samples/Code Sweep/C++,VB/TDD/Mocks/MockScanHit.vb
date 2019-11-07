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
    Friend Class MockScanHit
        Implements IScanHit
        Private ReadOnly _file As String
        Private ReadOnly _line As Integer
        Private ReadOnly _column As Integer
        Private ReadOnly _term As ISearchTerm
        Private ReadOnly _lineText As String
        Private ReadOnly _warning As String

        Public Sub New(ByVal file As String, ByVal line As Integer, ByVal column As Integer, ByVal lineText As String, ByVal term As ISearchTerm, ByVal warning As String)
            _file = file
            _line = line
            _column = column
            _term = term
            _lineText = lineText
            _warning = warning
        End Sub

#Region "IScanHit Members"

        Public ReadOnly Property FilePath() As String Implements IScanHit.FilePath
            Get
                Return _file
            End Get
        End Property

        Public ReadOnly Property Line() As Integer Implements IScanHit.Line
            Get
                Return _line
            End Get
        End Property

        Public ReadOnly Property Column() As Integer Implements IScanHit.Column
            Get
                Return _column
            End Get
        End Property

        Public ReadOnly Property LineText() As String Implements IScanHit.LineText
            Get
                Return _lineText
            End Get
        End Property

        Public ReadOnly Property Term() As ISearchTerm Implements IScanHit.Term
            Get
                Return _term
            End Get
        End Property

        Public ReadOnly Property Warning() As String Implements IScanHit.Warning
            Get
                Return _warning
            End Get
        End Property

#End Region
    End Class
End Namespace
