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
Imports System.Threading

Namespace Microsoft.Samples.VisualStudio.CodeSweep.Scanner
    ''' <summary>
    ''' Delegate type for a callback from <c>IScanner.Scan</c> which is called after each file scan
    ''' completes.
    ''' </summary>
    ''' <param name="result">The result of the scan of a single file.</param>
    Public Delegate Sub FileScanCompleted(ByVal result As IScanResult)

    ''' <summary>
    ''' Delegate type for an argument to <c>IScanner.Scan</c> which is called to get the text of a
    ''' file instead of reading it from disk.
    ''' </summary>
    ''' <param name="filePath">The full path of the file to get.</param>
    ''' <returns>The text of the file, or null if it is not provided by this delegate and should be read from disk.</returns>
    Public Delegate Function FileContentGetter(ByVal filePath As String) As String

    ''' <summary>
    ''' Delegate type for an argument to <c>IScanner.Scan</c> which is called to determine whether
    ''' the scan should be aborted before it is finished.
    ''' </summary>
    ''' <returns>True if the scan should be aborted immediately, false otherwise.</returns>
    Public Delegate Function ScanStopper() As Boolean

    ''' <summary>
    ''' Performs the scanning process.
    ''' </summary>
    Public Interface IScanner
        ''' <summary>
        ''' Scans a collection of files for terms defined in a collection of term tables.
        ''' </summary>
        ''' <param name="filePaths">The full paths of the files to scan.</param>
        ''' <param name="termTables">The term tables containing the search terms to scan for.</param>
        ''' <returns>The result of the scan.</returns>
        ''' <exception cref="System.ArgumentNullException">Thrown if <c>filePaths</c> or <c>termTables</c> is null.</exception>
        Function Scan(ByVal filePaths As IEnumerable(Of String), ByVal termTables As IEnumerable(Of ITermTable)) As IMultiFileScanResult

        ''' <summary>
        ''' Scans a collection of files for terms defined in a collection of term tables, and calls a callback delegate after each file is scanned.
        ''' </summary>
        ''' <param name="filePaths">The full paths of the files to scan.</param>
        ''' <param name="termTables">The term tables containing the search terms to scan for.</param>
        ''' <param name="callback">The delegate to be called after each file is scanned; may be null.</param>
        ''' <returns>The result of the scan.</returns>
        ''' <exception cref="System.ArgumentNullException">Thrown if <c>filePaths</c> or <c>termTables</c> is null.</exception>
        Function Scan(ByVal filePaths As IEnumerable(Of String), ByVal termTables As IEnumerable(Of ITermTable), ByVal callback As FileScanCompleted) As IMultiFileScanResult

        ''' <summary>
        ''' Scans a collection of files for terms defined in a collection of term tables, and calls a callback delegate after each file is scanned.
        ''' </summary>
        ''' <param name="filePaths">The full paths of the files to scan.</param>
        ''' <param name="termTables">The term tables containing the search terms to scan for.</param>
        ''' <param name="callback">The delegate to be called after each file is scanned; may be null.</param>
        ''' <param name="contentGetter">The delegate which may be called to provide the text of a file instead of reading it from disk; may be null.</param>
        ''' <returns>The result of the scan.</returns>
        ''' <exception cref="System.ArgumentNullException">Thrown if <c>filePaths</c> or <c>termTables</c> is null.</exception>
        Function Scan(ByVal filePaths As IEnumerable(Of String), ByVal termTables As IEnumerable(Of ITermTable), ByVal callback As FileScanCompleted, ByVal contentGetter As FileContentGetter) As IMultiFileScanResult

        ''' <summary>
        ''' Scans a collection of files for terms defined in a collection of term tables, and calls a callback delegate after each file is scanned.
        ''' </summary>
        ''' <param name="filePaths">The full paths of the files to scan.</param>
        ''' <param name="termTables">The term tables containing the search terms to scan for.</param>
        ''' <param name="callback">The delegate to be called after each file is scanned; may be null.</param>
        ''' <param name="contentGetter">The delegate which may be called to provide the text of a file instead of reading it from disk; may be null.</param>
        ''' <param name="stopper">The delegate which is called frequently during processing to determine if the scan should be aborted; may be null.</param>
        ''' <returns>The result of the scan.</returns>
        ''' <exception cref="System.ArgumentNullException">Thrown if <c>filePaths</c> or <c>termTables</c> is null.</exception>
        Function Scan(ByVal filePaths As IEnumerable(Of String), ByVal termTables As IEnumerable(Of ITermTable), ByVal callback As FileScanCompleted, ByVal contentGetter As FileContentGetter, ByVal stopper As ScanStopper) As IMultiFileScanResult
    End Interface
End Namespace
