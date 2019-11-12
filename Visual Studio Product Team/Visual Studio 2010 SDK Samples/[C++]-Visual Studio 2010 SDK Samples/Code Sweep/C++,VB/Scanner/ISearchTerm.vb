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
    ''' <summary>
    ''' A term to search for.
    ''' </summary>
    Public Interface ISearchTerm
        ''' <summary>
        ''' Gets the case-insensitive text of the term.
        ''' </summary>
        ReadOnly Property Text() As String

        ''' <summary>
        ''' Gets the term class, such as "Geopolitical".
        ''' </summary>
        ReadOnly Property [Class]() As String

        ''' <summary>
        ''' Gets the term severity, normally ranging from 1 (most severe) to 3 (least severe).
        ''' </summary>
        ReadOnly Property Severity() As Integer

        ''' <summary>
        ''' Gets the comment explaining why the term is undesirable and what to do about it.
        ''' </summary>
        ReadOnly Property Comment() As String

        ''' <summary>
        ''' Gets the recommended replacement term; this may be null if there is no recommended
        ''' replacement.
        ''' </summary>
        ReadOnly Property RecommendedTerm() As String

        ''' <summary>
        ''' Gets the exclusions that apply to this term.
        ''' </summary>
        ReadOnly Property Exclusions() As IEnumerable(Of IExclusion)

        ''' <summary>
        ''' Gets the term table to which this term belongs.
        ''' </summary>
        ReadOnly Property Table() As ITermTable
    End Interface
End Namespace
