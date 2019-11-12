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
Imports Microsoft.Build.Utilities
Imports Microsoft.Samples.VisualStudio.CodeSweep.Scanner
Imports System.IO
Imports Microsoft.Build.Framework
Imports System.ComponentModel
Imports Microsoft.Samples.VisualStudio.CodeSweep.BuildTask.Properties
Imports System.Globalization

Namespace Microsoft.Samples.VisualStudio.CodeSweep.BuildTask
    ''' <summary>
    ''' MSBuild task which performs a CodeSweep scan across items in a project.
    ''' </summary>
    <Description("CodeSweepTaskEntry")> _
    Public Class ScannerTask
        Inherits Task
        Private _duplicateTerms As New List(Of String)()

        ''' <summary>
        ''' Performs a scan over all files in the project.
        ''' </summary>
        ''' <returns>False if any violations are found, true otherwise.</returns>
        ''' <remarks>
        ''' If any violations are found, a message will be sent to standard output.  If this task
        ''' is running the VS IDE with the CodeSweep package loaded, the message will also be
        ''' placed in the task list.
        ''' </remarks>
        Public Overrides Function Execute() As Boolean
            _ignoreInstances = New List(Of IIgnoreInstance)(Factory.DeserializeIgnoreInstances(_ignoreInstancesString, Path.GetDirectoryName(_project)))

            Dim result As IMultiFileScanResult = Scanner.Factory.GetScanner().Scan(GetFilesToScan(), GetTermTables(), AddressOf OutputScanResults)

            If _signalErrorIfTermsFound Then
                Return result.PassedScan = result.Attempted
            Else
                Return True
            End If
        End Function

        ''' <summary>
        ''' Gets or sets the list of term table files, expressed as paths relative to the project
        ''' folder, delimited by semicolons.
        ''' </summary>
        <Required()> _
        Public Property TermTables() As String
            Set(ByVal value As String)
                If (value IsNot Nothing) Then
                    _termTables = value.Split(";"c)
                Else
                    _termTables = Nothing
                End If
            End Set
            Get
                If (_termTables IsNot Nothing) Then
                    Return Utilities.Concatenate(_termTables, ";")
                Else
                    Return Nothing
                End If
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the list of files to scan.
        ''' </summary>
        <Required()> _
        Public Property FilesToScan() As ITaskItem()
            Set(ByVal value As ITaskItem())
                _filesToScan = value
            End Set
            Get
                Return _filesToScan
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the full path of the project file being built.
        ''' </summary>
        <Required()> _
        Public Property Project() As String
            Get
                Return _project
            End Get
            Set(ByVal value As String)
                _project = value
            End Set
        End Property

        Private _ignoreInstancesString As String

        ''' <summary>
        ''' Gets or sets the list of "ignore instances".
        ''' </summary>
        Public Property IgnoreInstances() As String
            Get
                Return _ignoreInstancesString
            End Get
            Set(ByVal value As String)
                _ignoreInstancesString = value
            End Set
        End Property

        Private _signalErrorIfTermsFound As Boolean = False

        ''' <summary>
        ''' Controls whether the task will indicate an error when it finds a search term.
        ''' </summary>
        Public Property SignalErrorIfTermsFound() As Boolean
            Get
                Return _signalErrorIfTermsFound
            End Get
            Set(ByVal value As Boolean)
                _signalErrorIfTermsFound = value
            End Set
        End Property

#Region "Private Members"

        Private _termTables As String()
        Private _filesToScan As ITaskItem()
        Private _project As String
        Private _ignoreInstances As List(Of IIgnoreInstance)

        Private NotInheritable Class AnonymousClass9
            Public hit As IScanHit

            Public Function AnonymousMethod(ByVal item As String) As Boolean
                Return String.Compare(item, hit.Term.Text, StringComparison.OrdinalIgnoreCase) = 0
            End Function
        End Class

        Private Sub OutputScanResults(ByVal result As IScanResult)
            Dim locals As New AnonymousClass9()
            If result.Scanned Then
                If (Not result.Passed) Then
                    For Each locals.hit In result.Results
                        Dim thisIgnoreInstance As IIgnoreInstance = Factory.GetIgnoreInstance(result.FilePath, locals.hit.LineText, locals.hit.Term.Text, locals.hit.Column)

                        If (Not _ignoreInstances.Contains(thisIgnoreInstance)) Then
                            If locals.hit.Warning IsNot Nothing Then
                                If Nothing Is _duplicateTerms.Find(AddressOf locals.AnonymousMethod) Then
                                    Log.LogWarning(locals.hit.Warning)
                                    _duplicateTerms.Add(locals.hit.Term.Text)
                                End If
                            End If

                            Dim outputText As String

                            If locals.hit.Term.RecommendedTerm IsNot Nothing AndAlso locals.hit.Term.RecommendedTerm.Length > 0 Then
                                outputText = String.Format(CultureInfo.CurrentUICulture, Resources.HitFormatWithReplacement, result.FilePath, locals.hit.Line + 1, locals.hit.Column + 1, locals.hit.Term.Text, locals.hit.Term.Severity, locals.hit.Term.Class, locals.hit.Term.Comment, locals.hit.Term.RecommendedTerm)
                            Else
                                outputText = String.Format(CultureInfo.CurrentUICulture, Resources.HitFormat, result.FilePath, locals.hit.Line + 1, locals.hit.Column + 1, locals.hit.Term.Text, locals.hit.Term.Severity, locals.hit.Term.Class, locals.hit.Term.Comment)
                            End If

                            If HostObject IsNot Nothing AndAlso TypeOf HostObject Is IScannerHost Then
                                ' We're piping the results to the task list, so we don't want to use
                                ' LogWarning, which would create an entry in the error list.
                                Log.LogMessage(MessageImportance.High, outputText)
                            Else
                                Log.LogWarning(outputText)
                            End If
                        End If
                    Next locals.hit
                End If
            Else
                Log.LogWarning(String.Format(CultureInfo.CurrentUICulture, Resources.FileNotScannedError, result.FilePath))
            End If

            If HostObject IsNot Nothing AndAlso TypeOf HostObject Is IScannerHost Then
                Dim scannerHost As IScannerHost = TryCast(HostObject, IScannerHost)
                scannerHost.AddResult(result, _project)
            End If
        End Sub

        Private Function GetTermTables() As IEnumerable(Of ITermTable)
            Dim result As New List(Of ITermTable)()

            For Each file As String In _termTables
                Try
                    result.Add(Scanner.Factory.GetTermTable(file))
                Catch ex As Exception
                    If TypeOf ex Is ArgumentException OrElse TypeOf ex Is System.Xml.XmlException Then
                        Log.LogWarning(String.Format(CultureInfo.CurrentUICulture, Resources.TermTableLoadFailed, file))
                    Else
                        Throw
                    End If
                End Try
            Next file

            Return result
        End Function

        Private Function GetFilesToScan() As IEnumerable(Of String)
            Dim results As New List(Of String)
            For Each item As ITaskItem In _filesToScan
                results.Add(item.ItemSpec)
            Next item
            Return results
        End Function

#End Region
    End Class
End Namespace
