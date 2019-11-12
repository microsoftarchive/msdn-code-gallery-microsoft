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
    Friend Class Exclusion
        Implements IExclusion, ISerializable

        Const TextKey As String = "Text"
        Const TermKey As String = "Term"

        Private ReadOnly _text As String
        Private ReadOnly _term As ISearchTerm

        Public Sub New(ByVal text As String, ByVal term As ISearchTerm)
            _text = text
            _term = term
        End Sub

        Protected Sub New(info As SerializationInfo, context As StreamingContext)
            _text = info.GetString(TextKey)
            _term = CType(info.GetValue(TermKey, GetType(ISearchTerm)), ISearchTerm)
        End Sub

#Region "IExclusion Members"

        Public ReadOnly Property Text() As String Implements IExclusion.Text
            Get
                Return _text
            End Get
        End Property

        Public ReadOnly Property Term() As ISearchTerm Implements IExclusion.Term
            Get
                Return _term
            End Get
        End Property

#End Region ' IExclusion Members

#Region "ISerializable Members"

        Public Sub GetObjectData(info As SerializationInfo, context As StreamingContext) Implements ISerializable.GetObjectData
            info.AddValue(TextKey, _text)
            info.AddValue(TermKey, _term)
        End Sub

#End Region ' ISerializable Members
    End Class
End Namespace
