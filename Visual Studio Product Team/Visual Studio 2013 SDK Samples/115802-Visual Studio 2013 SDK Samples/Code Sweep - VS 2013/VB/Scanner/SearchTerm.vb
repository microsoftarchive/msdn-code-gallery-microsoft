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
    Friend Class SearchTerm
        Implements ISearchTerm, ISerializable

        Const TextKey As String = "Text"
        Const TableKey As String = "Table"
        Const ClassKey As String = "Class"
        Const SeverityKey As String = "Severity"
        Const CommentKey As String = "Comment"
        Const RecommendedKey As String = "Recommended"
        Const ExclusionsKey As String = "Exclusions"

        Private ReadOnly _text As String = String.Empty
        Private ReadOnly _table As ITermTable
        Private ReadOnly _class As String
        Private ReadOnly _severity As Integer
        Private ReadOnly _comment As String
        Private ReadOnly _recommended As String
        Private ReadOnly _exclusions As List(Of IExclusion)

        ''' <summary>
        ''' Initializes the search term with the specified text.
        ''' </summary>
        ''' <param name="table">The table to which this term belongs.</param>
        ''' <param name="text">The text to search for.</param>
        ''' <param name="severity">The severity of the term, normally between 1 and 3 inclusive.</param>
        ''' <param name="termClass">The class of the term, such as "Geopolitical".</param>
        ''' <param name="comment">A descriptive comment for the term.</param>
        ''' <param name="recommendedTerm">The recommended replacement; may be null.</param>
        ''' <exception cref="System.ArgumentNullException">Thrown if <c>text</c> is null.</exception>
        ''' <exception cref="System.ArgumentException">Thrown if <c>text</c> is an empty string.</exception>
        Public Sub New(ByVal table As ITermTable, ByVal text As String, ByVal severity As Integer, ByVal termClass As String, ByVal comment As String, ByVal recommendedTerm As String)
            If table Is Nothing Then
                Throw New ArgumentNullException("table")
            End If
            If text Is Nothing Then
                Throw New ArgumentNullException("text")
            End If
            If text.Length = 0 Then
                Throw New ArgumentException("Empty string not allowed", "text")
            End If

            _table = table
            _text = text
            _severity = severity
            _class = termClass
            _comment = comment
            _recommended = recommendedTerm
            _exclusions = New List(Of IExclusion)()
        End Sub

        Protected Sub New(info As SerializationInfo, context As StreamingContext)
            _text = info.GetString(TextKey)
            _table = CType(info.GetValue(TableKey, GetType(ITermTable)), ITermTable)
            _class = info.GetString(ClassKey)
            _severity = info.GetInt32(SeverityKey)
            _comment = info.GetString(CommentKey)
            _recommended = info.GetString(RecommendedKey)
            _exclusions = CType(info.GetValue(ExclusionsKey, GetType(List(Of IExclusion))), List(Of IExclusion))
        End Sub

        Public Sub AddExclusion(ByVal text As String)
            _exclusions.Add(New Exclusion(text, Me))
        End Sub

#Region "ISearchTerm Members"

        ''' <summary>
        ''' Gets the text to search for.
        ''' </summary>
        Public ReadOnly Property Text() As String Implements ISearchTerm.Text
            Get
                Return _text
            End Get
        End Property

        Public ReadOnly Property Severity() As Integer Implements ISearchTerm.Severity
            Get
                Return _severity
            End Get
        End Property

        Public ReadOnly Property [Class]() As String Implements ISearchTerm.Class
            Get
                Return _class
            End Get
        End Property

        Public ReadOnly Property Comment() As String Implements ISearchTerm.Comment
            Get
                Return _comment
            End Get
        End Property

        Public ReadOnly Property RecommendedTerm() As String Implements ISearchTerm.RecommendedTerm
            Get
                Return _recommended
            End Get
        End Property

        ''' <summary>
        ''' Gets the list of phrases containing this term which should be excluded from the results.
        ''' </summary>
        Public ReadOnly Property Exclusions() As IEnumerable(Of IExclusion) Implements ISearchTerm.Exclusions
            Get
                Return _exclusions
            End Get
        End Property

        Public ReadOnly Property Table() As ITermTable Implements ISearchTerm.Table
            Get
                Return _table
            End Get
        End Property

#End Region ' ISearchTerm Members

#Region "ISerializable Members"

        Public Sub GetObjectData(info As SerializationInfo, context As StreamingContext) Implements ISerializable.GetObjectData
            info.AddValue(TextKey, _text)
            info.AddValue(TableKey, _table)
            info.AddValue(ClassKey, _class)
            info.AddValue(SeverityKey, _severity)
            info.AddValue(CommentKey, _comment)
            info.AddValue(RecommendedKey, _recommended)
            info.AddValue(ExclusionsKey, _exclusions)
        End Sub

#End Region ' ISerializable Members
    End Class
End Namespace
