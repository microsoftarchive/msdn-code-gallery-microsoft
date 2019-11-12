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
Imports System.Linq
Imports System.Text
Imports Microsoft.Build.Utilities
Imports Microsoft.Samples.VisualStudio.CodeSweep.Scanner
Imports System.IO
Imports Microsoft.Build.Framework
Imports System.ComponentModel
Imports Microsoft.Samples.VisualStudio.CodeSweep.BuildTask.Properties
Imports System.Globalization
Imports System.Runtime.Remoting
Imports System.Runtime.Remoting.Channels
Imports System.Runtime.Remoting.Channels.Tcp

Namespace Microsoft.Samples.VisualStudio.CodeSweep.BuildTask
    ''' <summary>
    ''' MSBuild task which performs a CodeSweep scan across items in a project.
    ''' </summary>
    <Description("CodeSweepTaskEntry")> _
    Public Class ScannerTask
        Inherits Task

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
            If Not String.IsNullOrEmpty(HostIdeProcId) Then
                _host = GetHostObject(Integer.Parse(HostIdeProcId))
            End If

            _ignoreInstancesList = New List(Of IIgnoreInstance)(Factory.DeserializeIgnoreInstances(IgnoreInstances, Path.GetDirectoryName(Project)))

            Dim result As IMultiFileScanResult = Scanner.Factory.GetScanner().Scan(GetFilesToScan(), GetTermTables(), AddressOf OutputScanResults)

            If SignalErrorIfTermsFound Then
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

        ''' <summary>
        ''' Gets or sets the full path of the project file being built.
        ''' </summary>
        <Required()> _
        Public Property Project() As String

        ''' <summary>
        ''' Gets or sets the list of "ignore instances".
        ''' </summary>
        Public Property IgnoreInstances() As String

        ''' <summary>
        ''' Controls whether the task will indicate an error when it finds a search term.
        ''' </summary>
        Public Property SignalErrorIfTermsFound() As Boolean

        ''' <summary>
        ''' Sets the process ID of the host IDE.
        ''' </summary>
        Public Property HostIdeProcId As String

#Region "Private Members"

        Private _termTables As String()
        Private _ignoreInstancesList As List(Of IIgnoreInstance)
        Private ReadOnly _duplicateTerms As New List(Of String)()
        Private _host As IScannerHost

        Private Sub OutputScanResults(ByVal result As IScanResult)
            If result.Scanned Then
                If (Not result.Passed) Then
                    For Each hit As IScanHit In result.Results
                        Dim thisIgnoreInstance As IIgnoreInstance = Factory.GetIgnoreInstance(result.FilePath, hit.LineText, hit.Term.Text, hit.Column)

                        If _ignoreInstancesList.Contains(thisIgnoreInstance) Then
                            Continue For
                        End If

                        If hit.Warning IsNot Nothing Then
                            If _duplicateTerms.Any(Function(item) String.Compare(item, hit.Term.Text, StringComparison.OrdinalIgnoreCase) = 0) Then
                                Log.LogWarning(hit.Warning)
                                _duplicateTerms.Add(hit.Term.Text)
                            End If
                        End If

                        Dim outputText As String

                        If hit.Term.RecommendedTerm IsNot Nothing AndAlso hit.Term.RecommendedTerm.Length > 0 Then
                            outputText = String.Format(CultureInfo.CurrentUICulture, Resources.HitFormatWithReplacement, result.FilePath, hit.Line + 1, hit.Column + 1, hit.Term.Text, hit.Term.Severity, hit.Term.Class, hit.Term.Comment, hit.Term.RecommendedTerm)
                        Else
                            outputText = String.Format(CultureInfo.CurrentUICulture, Resources.HitFormat, result.FilePath, hit.Line + 1, hit.Column + 1, hit.Term.Text, hit.Term.Severity, hit.Term.Class, hit.Term.Comment)
                        End If

                        If _host IsNot Nothing Then
                            ' We're piping the results to the task list, so we don't want to use
                            ' LogWarning, which would create an entry in the error list.
                            Log.LogMessage(MessageImportance.High, outputText)
                        Else
                            Log.LogWarning(outputText)
                        End If
                    Next
                End If
            Else
                Log.LogWarning(String.Format(CultureInfo.CurrentUICulture, Resources.FileNotScannedError, result.FilePath))
            End If

            If _host IsNot Nothing Then
                _host.AddResult(result, Project)
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
            Return FilesToScan.Select(Function(item) item.ItemSpec)
        End Function

        Function GetHostObject(hostProcId As Integer) As IScannerHost
            Try
                ChannelServices.RegisterChannel(New TcpChannel(), False)
            Catch e As RemotingException
                ' The channel may already have been registered.
            End Try

            Try
                Return CType(Activator.GetObject(GetType(IScannerHost), Utilities.GetRemotingUri(hostProcId, True)), IScannerHost)
            Catch
                Return Nothing
            End Try
        End Function

#End Region ' Private Members
    End Class
End Namespace
