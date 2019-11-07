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
    Friend Class MockTerm
        Implements ISearchTerm
        Private ReadOnly _text As String
        Private ReadOnly _severity As Integer
        Private ReadOnly _class As String
        Private ReadOnly _comment As String
        Private _exclusions As New List(Of IExclusion)()
        Private ReadOnly _table As ITermTable
        Private ReadOnly _recommended As String

        Public Sub New(ByVal text As String, ByVal severity As Integer, ByVal termClass As String, ByVal comment As String, ByVal recommended As String, ByVal table As ITermTable)
            _text = text
            _severity = severity
            _class = termClass
            _comment = comment
            _table = table
            _recommended = recommended
        End Sub

        Public Sub AddExclusion(ByVal exclusion As String)
            _exclusions.Add(New MockExclusion(exclusion, Me))
        End Sub

#Region "ISearchTerm Members"

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
