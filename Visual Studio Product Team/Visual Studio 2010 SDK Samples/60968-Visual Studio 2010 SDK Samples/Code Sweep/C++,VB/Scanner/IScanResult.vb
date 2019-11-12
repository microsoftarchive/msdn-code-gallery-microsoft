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
    ''' The result of a scan on a single file.
    ''' </summary>
    Public Interface IScanResult
        ''' <summary>
        ''' Gets the full path of the file that was scanned.
        ''' </summary>
        ReadOnly Property FilePath() As String

        ''' <summary>
        ''' Gets the scan hits found in the file.
        ''' </summary>
        ReadOnly Property Results() As IEnumerable(Of IScanHit)

        ''' <summary>
        ''' Gets a boolean value indicating whether the scan was performed.
        ''' </summary>
        ReadOnly Property Scanned() As Boolean

        ''' <summary>
        ''' Gets a boolean value indicating whether the scan had zero hits.
        ''' </summary>
        ReadOnly Property Passed() As Boolean
    End Interface
End Namespace
