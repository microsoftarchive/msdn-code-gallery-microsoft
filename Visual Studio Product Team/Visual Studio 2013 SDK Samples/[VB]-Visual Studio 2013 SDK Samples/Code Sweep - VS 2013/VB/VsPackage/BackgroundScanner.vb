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
Imports System.Diagnostics
Imports System.Text
Imports Microsoft.VisualStudio.Shell.Interop
Imports Microsoft.Samples.VisualStudio.CodeSweep.Scanner
Imports System.Threading
Imports Microsoft.VisualStudio
Imports Microsoft.VisualStudio.TextManager.Interop
Imports System.Runtime.InteropServices

Namespace Microsoft.Samples.VisualStudio.CodeSweep.VSPackage
    Friend Class BackgroundScanner
        Implements IBackgroundScanner
        ''' <summary>
        ''' Creates a new background scanner.
        ''' </summary>
        ''' <param name="serviceProvider">The service provider that is used to get VS services.</param>
        ''' <exception cref="System.ArgumentNullException">Thrown if <c>serviceProvider</c> is null.</exception>
        Public Sub New(ByVal serviceProvider As IServiceProvider)
            If serviceProvider Is Nothing Then
                Throw New ArgumentNullException("serviceProvider")
            End If

            _serviceProvider = serviceProvider
        End Sub

        ''' <summary>
        ''' Fired just before the background scan starts.  The call occurs on the thread on which
        ''' <c>Start</c> is called, not on the background thread.
        ''' </summary>
        Public Event Started As EventHandler Implements IBackgroundScanner.Started

        ''' <summary>
        ''' Fired after the background scan stops.  The call occurs on the background thread.
        ''' </summary>
        Public Event Stopped As EventHandler Implements IBackgroundScanner.Stopped

        ''' <summary>
        ''' Starts a scan on a background thread.
        ''' </summary>
        ''' <param name="projects">The projects which will be scanned.</param>
        ''' <exception cref="System.ArgumentNullException">Thrown if <c>project</c> is null.</exception>
        ''' <exception cref="System.InvalidOperationException">Thrown if a background scan is already running.</exception>
        ''' <remarks>
        ''' Before this method returns, the CodeSweep task list will be cleared, and the tasklist
        ''' window will be activated with the CodeSweep task list visible.
        ''' While the scan is running, the status bar will display a progress indicator.
        ''' As each file is processed, the results (if any) will be placed in the task list.
        ''' If <c>project</c> does not contain a CodeSweep configuration, this method will return
        ''' immediately without creating a background thread.  <c>Started</c> is not fired.
        ''' </remarks>
        Public Sub Start(ByVal projects As IEnumerable(Of IVsProject)) Implements IBackgroundScanner.Start
            If projects Is Nothing Then
                Throw New ArgumentNullException("projects")
            End If
            If IsRunning Then
                Throw New InvalidOperationException(Resources.BackgroundScanAlreadyRunning)
            End If

            GatherProjectInfo(projects)

            StartWithExistingConfigs()
        End Sub

        ''' <summary>
        ''' Starts a new scan, using the same project as the previous scan.
        ''' </summary>
        ''' <exception cref="System.InvalidOperationException">Thrown if a background scan is already running or if there was no previous scan to repeat.</exception>
        Public Sub RepeatLast() Implements IBackgroundScanner.RepeatLast
            If _projectsToScan.Count = 0 Then
                Throw New InvalidOperationException(Resources.NoPreviousScan)
            End If

            StartWithExistingConfigs()
        End Sub

        ''' <summary>
        ''' Gets a boolean value indicating whether a background scan is currently running.
        ''' </summary>
        Public ReadOnly Property IsRunning() As Boolean Implements IBackgroundScanner.IsRunning
            Get
                Return _running
            End Get
        End Property

        ''' <summary>
        ''' Stops a scan in progress.
        ''' </summary>
        ''' <param name="blockUntilDone">Controls whether this method returns immediately or waits until the background thread has finished.</param>
        Public Sub StopIfRunning(ByVal blockUntilDone As Boolean) Implements IBackgroundScanner.StopIfRunning
            Dim doneEvent As New ManualResetEvent(False)
            If blockUntilDone Then
                AddHandler Stopped, Sub(sender, e) doneEvent.Set()
            End If

            SyncLock _lockObj
                If IsRunning Then
                    _stopPending = True
                Else
                    Return
                End If
            End SyncLock

            If blockUntilDone Then
                doneEvent.WaitOne()
            End If
        End Sub

#Region "Private members"

        Private Class ProjectConfiguration
            Private ReadOnly _filesToScan As List(Of String)
            Private ReadOnly _termTableFiles As List(Of String)
            Private ReadOnly _projectPath As String

            Public Sub New(ByVal filesToScan As IEnumerable(Of String), ByVal termTableFiles As IEnumerable(Of String), ByVal projectPath As String)
                _filesToScan = New List(Of String)(filesToScan)
                _termTableFiles = New List(Of String)(termTableFiles)
                _projectPath = projectPath
            End Sub

            Public ReadOnly Property FilesToScan() As ICollection(Of String)
                Get
                    Return _filesToScan
                End Get
            End Property

            Public ReadOnly Property TermTableFiles() As ICollection(Of String)
                Get
                    Return _termTableFiles
                End Get
            End Property

            Public ReadOnly Property ProjectPath() As String
                Get
                    Return _projectPath
                End Get
            End Property
        End Class

        Private Delegate Function ScanDelegate(ByVal filePaths As IEnumerable(Of String), ByVal termTables As IEnumerable(Of ITermTable), ByVal callback As FileScanCompleted, ByVal contentGetter As FileContentGetter, ByVal stopper As ScanStopper) As IMultiFileScanResult

        Private ReadOnly _serviceProvider As IServiceProvider
        Private _running As Boolean = False
        Private _stopPending As Boolean = False
        Private _filesProcessed As UInteger = 0
        Private _totalFiles As UInteger = 0
        Private _statusBarCookie As UInteger = 0
        Private _projectsToScan As New List(Of ProjectConfiguration)()
        Private _currentProject As Integer = 0
        Private _lockObj As New Object()

        Private Sub ScanCompleted(ByVal result As IAsyncResult)
            SyncLock _lockObj
                _currentProject += 1
                If _currentProject = _projectsToScan.Count OrElse _stopPending Then
                    _currentProject = 0
                    _running = False
                    _stopPending = False
                    UpdateStatusBar(False, String.Empty, 0, 0)

                    RaiseEvent Stopped(Me, EventArgs.Empty)
                Else
                    StartCurrentConfig()
                End If
            End SyncLock
        End Sub

        Private Sub UpdateStatusBar(ByVal usingStatusBar As Boolean, ByVal text As String, ByVal soFar As UInteger, ByVal total As UInteger)
            Dim statusBar As IVsStatusbar = TryCast(_serviceProvider.GetService(GetType(SVsStatusbar)), IVsStatusbar)
            If statusBar IsNot Nothing Then
                If usingStatusBar Then
                    statusBar.Progress(_statusBarCookie, 1, text, soFar, total)
                Else
                    statusBar.Progress(_statusBarCookie, 0, text, soFar, total)
                End If
            Else
                Debug.Fail("Failed to get SVsStatusbar service.")
            End If

            If (Not usingStatusBar) Then
                _statusBarCookie = 0
            End If
        End Sub

        Private Sub ScanResultRecieved(ByVal result As IScanResult)
            If (Not _stopPending) Then
                Factory.GetTaskProvider().AddResult(result, _projectsToScan(_currentProject).ProjectPath)
                _filesProcessed = CUInt(_filesProcessed + 1)
                UpdateStatusBar(True, Resources.AppName, _filesProcessed, _totalFiles)
            End If
        End Sub

        Private Sub StartCurrentConfig()
            Dim scanDelegate As ScanDelegate = AddressOf CodeSweep.Scanner.Factory.GetScanner().Scan

            Dim termTables As New List(Of ITermTable)()
            For Each tableFile As String In _projectsToScan(_currentProject).TermTableFiles
                Try
                    termTables.Add(CodeSweep.Scanner.Factory.GetTermTable(tableFile))
                Catch ex As Exception
                    If Not (TypeOf ex Is ArgumentException OrElse TypeOf ex Is System.Xml.XmlException) Then
                        Throw
                    End If
                End Try
            Next tableFile

            UpdateStatusBar(True, Resources.AppName, _filesProcessed, _totalFiles)

            RaiseEvent Started(Me, EventArgs.Empty)

            Dim stopper As ScanStopper = Function() _stopPending

            _running = True
            scanDelegate.BeginInvoke(
                _projectsToScan(_currentProject).FilesToScan,
                termTables,
                New FileScanCompleted(AddressOf ScanResultRecieved),
                New FileContentGetter(AddressOf GetTextOfFileIfOpenInIde),
                stopper,
                AddressOf ScanCompleted,
                Nothing)
        End Sub

        Private Sub StartWithExistingConfigs()
            If IsRunning Then
                Throw New InvalidOperationException(Resources.AlreadyScanning)
            End If

            ResetScanRun()

            If _totalFiles = 0 Then
                Return
            End If

            StartCurrentConfig()
        End Sub

        Private Sub ResetScanRun()
            _currentProject = 0
            _filesProcessed = 0

            Factory.GetTaskProvider().Clear()
            Factory.GetTaskProvider().ShowTaskList()
            Factory.GetTaskProvider().SetAsActiveProvider()
        End Sub

        Private Sub GatherProjectInfo(ByVal projects As IEnumerable(Of IVsProject))
            _projectsToScan.Clear()
            _totalFiles = 0

            For Each project As IVsProject In projects
                Dim filesToScan As New List(Of String)()
                Dim termTableFiles As New List(Of String)()

                If Factory.GetProjectConfigurationStore(project).HasConfiguration Then
                    filesToScan.AddRange(Factory.GetBuildManager().AllItemsInProject(project))
                    termTableFiles.AddRange(Factory.GetProjectConfigurationStore(project).TermTableFiles)
                    _totalFiles += CUInt(filesToScan.Count)
                End If

                _projectsToScan.Add(New ProjectConfiguration(filesToScan, termTableFiles, ProjectUtilities.GetProjectFilePath(project)))
            Next project
        End Sub

        Public Function GetTextOfFileIfOpenInIde(ByVal filePath As String) As String
            Dim rdt As IVsRunningDocumentTable = TryCast(_serviceProvider.GetService(GetType(SVsRunningDocumentTable)), IVsRunningDocumentTable)

            Dim hierarchy As IVsHierarchy = Nothing
            Dim itemid As UInteger = 0
            Dim docDataUnk As IntPtr = IntPtr.Zero
            Dim lockCookie As UInteger = 0

            Dim hr As Integer = rdt.FindAndLockDocument(CUInt(_VSRDTFLAGS.RDT_ReadLock), filePath, hierarchy, itemid, docDataUnk, lockCookie)
            Try
                If hr = VSConstants.S_OK Then
                    Dim textLines As IVsTextLines = TryCast(Marshal.GetUniqueObjectForIUnknown(docDataUnk), IVsTextLines)

                    If textLines IsNot Nothing Then
                        Dim text As String = Nothing
                        Dim endLine As Integer = 0
                        Dim endIndex As Integer = 0

                        hr = textLines.GetLastLineIndex(endLine, endIndex)
                        Debug.Assert(hr = VSConstants.S_OK, "GetLastLineIndex did not return S_OK.")

                        hr = textLines.GetLineText(0, 0, endLine, endIndex, text)
                        Debug.Assert(hr = VSConstants.S_OK, "GetLineText did not return S_OK.")

                        Return text
                    End If
                End If

                Return Nothing
            Finally
                If lockCookie <> 0 Then
                    rdt.UnlockDocument(CUInt(_VSRDTFLAGS.RDT_ReadLock), lockCookie)
                End If
            End Try
        End Function

#End Region ' Private Methods
    End Class
End Namespace
