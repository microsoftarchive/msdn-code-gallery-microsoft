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
Imports System.Runtime.Serialization

Namespace Microsoft.Samples.VisualStudio.CodeSweep.Scanner
    <Serializable>
    Friend Class ScanHit
        Implements IScanHit, ISerializable

        Const FilePathKey As String = "FilePath"
        Const LineKey As String = "Line"
        Const ColumnKey As String = "Column"
        Const TermKey As String = "Term"
        Const LineTextKey As String = "LineText"
        Const WarningKey As String = "Warning"

        Private ReadOnly _filePath As String
        Private ReadOnly _line As Integer
        Private ReadOnly _column As Integer
        Private ReadOnly _term As ISearchTerm
        Private ReadOnly _lineText As String
        Private ReadOnly _warning As String

        Public Sub New(ByVal filePath As String, ByVal line As Integer, ByVal column As Integer, ByVal lineText As String, ByVal term As ISearchTerm, ByVal warning As String)
            _filePath = filePath
            _line = line
            _column = column
            _term = term
            _lineText = lineText
            _warning = warning
        End Sub

        Protected Sub New(info As SerializationInfo, context As StreamingContext)
            _filePath = info.GetString(FilePathKey)
            _line = info.GetInt32(LineKey)
            _column = info.GetInt32(ColumnKey)
            _term = CType(info.GetValue(TermKey, GetType(ISearchTerm)), ISearchTerm)
            _lineText = info.GetString(LineTextKey)
            _warning = info.GetString(WarningKey)
        End Sub

#Region "IScanHit Members"

        Public ReadOnly Property FilePath() As String Implements IScanHit.FilePath
            Get
                Return _filePath
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

#End Region ' IScanHit Members

#Region "ISerializable Members"

        Public Sub GetObjectData(info As SerializationInfo, context As StreamingContext) Implements ISerializable.GetObjectData
            info.AddValue(FilePathKey, _filePath)
            info.AddValue(LineKey, _line)
            info.AddValue(ColumnKey, _column)
            info.AddValue(TermKey, _term)
            info.AddValue(LineTextKey, _lineText)
            info.AddValue(WarningKey, _warning)
        End Sub

#End Region ' ISerializable Members
    End Class
End Namespace
