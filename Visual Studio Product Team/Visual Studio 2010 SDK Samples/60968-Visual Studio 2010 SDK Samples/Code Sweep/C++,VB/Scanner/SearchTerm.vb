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

Namespace Microsoft.Samples.VisualStudio.CodeSweep.Scanner
    Friend Class SearchTerm
        Implements ISearchTerm
        Private ReadOnly _text As String = ""
        Private ReadOnly _table As ITermTable
        Private ReadOnly _class As String
        Private ReadOnly _severity As Integer
        Private ReadOnly _comment As String
        Private ReadOnly _recommended As String
        Private ReadOnly _exclusions As New List(Of IExclusion)()

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

#End Region
    End Class
End Namespace
