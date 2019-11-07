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
    Friend Class Exclusion
        Implements IExclusion
        Private ReadOnly _text As String
        Private ReadOnly _term As ISearchTerm

        Public Sub New(ByVal text As String, ByVal term As ISearchTerm)
            _text = text
            _term = term
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

#End Region
    End Class
End Namespace
