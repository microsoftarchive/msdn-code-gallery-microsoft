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
    Friend Class MockTermTable
        Implements ITermTable
        Private _termList As New List(Of ISearchTerm)()
        Private _file As String

        Public Sub New(ByVal sourceFile As String)
            _file = sourceFile
        End Sub

        Public Sub AddSearchTerm(ByVal term As ISearchTerm)
            _termList.Add(term)
        End Sub

#Region "ITermTable Members"

        Public ReadOnly Property SourceFile() As String Implements ITermTable.SourceFile
            Get
                Return _file
            End Get
        End Property

        Public ReadOnly Property Terms() As IEnumerable(Of ISearchTerm) Implements ITermTable.Terms
            Get
                Return _termList
            End Get
        End Property

#End Region
    End Class
End Namespace
