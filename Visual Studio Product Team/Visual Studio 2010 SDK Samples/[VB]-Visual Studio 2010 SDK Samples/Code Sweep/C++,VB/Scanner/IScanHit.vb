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
    ''' A single hit on a specific term found during a scan.
    ''' </summary>
    Public Interface IScanHit
        ''' <summary>
        ''' Gets the full path of the file in which the term was found.
        ''' </summary>
        ReadOnly Property FilePath() As String

        ''' <summary>
        ''' Gets the zero-based line number of the line on which the term was found.
        ''' </summary>
        ReadOnly Property Line() As Integer

        ''' <summary>
        ''' Gets the zero-based column number of the character at which the term begins.
        ''' </summary>
        ReadOnly Property Column() As Integer

        ''' <summary>
        ''' Gets the full text of the line on which the term was found.
        ''' </summary>
        ReadOnly Property LineText() As String

        ''' <summary>
        ''' Gets the search term that was found.
        ''' </summary>
        ReadOnly Property Term() As ISearchTerm

        ''' <summary>
        ''' Gets the warning associated with this search hit, or null if there is no warning.
        ''' </summary>
        ReadOnly Property Warning() As String
    End Interface
End Namespace
