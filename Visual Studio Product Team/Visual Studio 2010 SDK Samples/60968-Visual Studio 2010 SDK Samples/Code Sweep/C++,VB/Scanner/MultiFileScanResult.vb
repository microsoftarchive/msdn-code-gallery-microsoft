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
    Friend Class MultiFileScanResult
        Implements IMultiFileScanResult
        Private _fileResults As New List(Of IScanResult)()

        ''' <summary>
        ''' Creates a new MultiFileScanResult object with default values indicating no files have
        ''' been scanned.
        ''' </summary>
        Public Sub New()
        End Sub

        ''' <summary>
        ''' Appends the results of a single-file scan to this multi-file result.
        ''' </summary>
        ''' <param name="fileResult">The results of the single-file scan.</param>
        ''' <exception cref="System.ArgumentNullException">Thrown if <c>fileResult</c> is null.</exception>
        ''' <remarks>
        ''' As a result of a successfull call to this method:
        '''   * 'Attempted' will be incremented by one.
        '''   * One of 'PassedScan', 'FailedScan', or 'UnableToScan' will be incremented by one.
        '''   * An entry will be added to 'Results'.
        ''' </remarks>
        Public Sub Append(ByVal fileResult As IScanResult)
            If fileResult Is Nothing Then
                Throw New ArgumentNullException("fileResult")
            End If

            _fileResults.Add(fileResult)
        End Sub

#Region "IMultiFileScanResult Members"

        ''' <summary>
        ''' Gets the number of files on which a scan has been attempted.
        ''' </summary>
        Public ReadOnly Property Attempted() As Integer Implements IMultiFileScanResult.Attempted
            Get
                Return _fileResults.Count
            End Get
        End Property
        Private NotInheritable Class AnonymousClass3
            Public total As Integer = 0
            Public Sub AnonymousMethod(ByVal result As IScanResult)
                If result.Passed Then
                    total += 1
                End If
            End Sub
        End Class

        ''' <summary>
        ''' Gets the number of files that have passed the scan.
        ''' </summary>
        Public ReadOnly Property PassedScan() As Integer Implements IMultiFileScanResult.PassedScan
            Get
                Dim locals As New AnonymousClass3()
                _fileResults.ForEach(New Action(Of IScanResult)(AddressOf locals.AnonymousMethod))
                Return locals.total
            End Get
        End Property
        Private NotInheritable Class AnonymousClass4
            Public total As Integer = 0
            Public Sub AnonymousMethod(ByVal result As IScanResult)
                If result.Scanned AndAlso (Not result.Passed) Then
                    total += 1
                End If
            End Sub
        End Class

        ''' <summary>
        ''' Gets the number of files that have failed the scan.
        ''' </summary>
        Public ReadOnly Property FailedScan() As Integer Implements IMultiFileScanResult.FailedScan
            Get
                Dim locals As New AnonymousClass4()
                _fileResults.ForEach(New Action(Of IScanResult)(AddressOf locals.AnonymousMethod))
                Return locals.total
            End Get
        End Property
        Private NotInheritable Class AnonymousClass5
            Public total As Integer = 0
            Public Sub AnonymousMethod(ByVal result As IScanResult)
                If (Not result.Scanned) Then
                    total += 1
                End If
            End Sub
        End Class

        ''' <summary>
        ''' Gets the number of files that could not be scanned, due to invalid file paths,
        ''' insufficient permissions, etc.
        ''' </summary>
        Public ReadOnly Property UnableToScan() As Integer Implements IMultiFileScanResult.UnableToScan
            Get
                Dim locals As New AnonymousClass5()
                _fileResults.ForEach(New Action(Of IScanResult)(AddressOf locals.AnonymousMethod))
                Return locals.total
            End Get
        End Property

        ''' <summary>
        ''' Gets the results of the scan of each file.
        ''' </summary>
        Public ReadOnly Property Results() As IEnumerable(Of IScanResult) Implements IMultiFileScanResult.Results
            Get
                Return _fileResults
            End Get
        End Property

#End Region
    End Class
End Namespace
