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
    ''' The result of a scan across zero or more files.
    ''' </summary>
    Public Interface IMultiFileScanResult
        ''' <summary>
        ''' Gets the number of files CodeSweep attempted to scan.
        ''' </summary>
        ReadOnly Property Attempted() As Integer

        ''' <summary>
        ''' Gets the number of files for which no search hits were found.
        ''' </summary>
        ReadOnly Property PassedScan() As Integer

        ''' <summary>
        ''' Gets the number of files for which one or more search hits were found.
        ''' </summary>
        ReadOnly Property FailedScan() As Integer

        ''' <summary>
        ''' Gets the number of files which could not be opened for scanning.
        ''' </summary>
        ReadOnly Property UnableToScan() As Integer

        ''' <summary>
        ''' Gets the search hits that were found by the scan.
        ''' </summary>
        ReadOnly Property Results() As IEnumerable(Of IScanResult)
    End Interface
End Namespace
