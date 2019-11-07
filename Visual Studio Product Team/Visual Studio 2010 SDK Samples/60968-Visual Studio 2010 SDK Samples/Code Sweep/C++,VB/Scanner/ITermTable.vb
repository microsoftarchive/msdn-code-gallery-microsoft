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
    ''' A table of search terms.
    ''' </summary>
    Public Interface ITermTable
        ''' <summary>
        ''' Gets the full path of the file from which this table was loaded.
        ''' </summary>
        ReadOnly Property SourceFile() As String

        ''' <summary>
        ''' Gets the terms defined in this table.
        ''' </summary>
        ReadOnly Property Terms() As IEnumerable(Of ISearchTerm)
    End Interface
End Namespace
