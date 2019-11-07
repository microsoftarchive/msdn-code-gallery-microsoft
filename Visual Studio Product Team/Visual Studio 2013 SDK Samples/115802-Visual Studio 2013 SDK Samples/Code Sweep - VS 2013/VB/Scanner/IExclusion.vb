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

Namespace Microsoft.Samples.VisualStudio.CodeSweep.Scanner
    ''' <summary>
    ''' An exclusion for a specific term, used to suppress a hit on the term if it appears in a
    ''' particular context.
    ''' </summary>
    Public Interface IExclusion
        ''' <summary>
        ''' Gets the case-insensitive text of the exclusion.
        ''' </summary>
        ReadOnly Property Text() As String

        ''' <summary>
        ''' Gets the search term to which this exclusion applies.
        ''' </summary>
        ReadOnly Property Term() As ISearchTerm
    End Interface
End Namespace
