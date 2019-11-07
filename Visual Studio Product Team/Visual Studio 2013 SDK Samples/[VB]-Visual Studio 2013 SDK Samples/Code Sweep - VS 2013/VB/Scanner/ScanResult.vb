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
Imports System.Linq
Imports System.Text
Imports System.Runtime.Serialization

Namespace Microsoft.Samples.VisualStudio.CodeSweep.Scanner
    <Serializable>
    Friend Class ScanResult
        Implements IScanResult, ISerializable

        Const FilePathKey As String = "FilePath"
        Const HitsKey As String = "Hits"
        Const ScannedKey As String = "Scanned"

        Public Shared Function ScanOccurred(ByVal filePath As String, ByVal hits As IEnumerable(Of IScanHit)) As ScanResult
            Return New ScanResult(filePath, hits, True)
        End Function

        Public Shared Function ScanNotPossible(ByVal filePath As String) As ScanResult
            Return New ScanResult(filePath, Nothing, False)
        End Function

        Private _filePath As String
        Private _hits As IEnumerable(Of IScanHit)
        Private _scanned As Boolean

        Private Sub New(ByVal filePath As String, ByVal hits As IEnumerable(Of IScanHit), ByVal scanned As Boolean)
            _filePath = filePath
            _hits = hits
            _scanned = scanned
        End Sub

        Protected Sub New(info As SerializationInfo, context As StreamingContext)
            _filePath = info.GetString(FilePathKey)
            _hits = CType(info.GetValue(HitsKey, GetType(IEnumerable(Of IScanHit))), IEnumerable(Of IScanHit))
            _scanned = info.GetBoolean(ScannedKey)
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
                    Return Not _hits.Any()
                End If
                Return False
            End Get
        End Property

#End Region ' IScanResult Members

#Region "ISerializable Members"

        Public Sub GetObjectData(info As SerializationInfo, context As StreamingContext) Implements ISerializable.GetObjectData
            info.AddValue(FilePathKey, _filePath)
            info.AddValue(HitsKey, _hits)
            info.AddValue(ScannedKey, _scanned)
        End Sub

#End Region ' ISerializable Members
    End Class
End Namespace
