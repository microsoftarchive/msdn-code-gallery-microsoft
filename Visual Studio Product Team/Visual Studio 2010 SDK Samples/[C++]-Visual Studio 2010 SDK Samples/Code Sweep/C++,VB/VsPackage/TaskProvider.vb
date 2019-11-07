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
Imports Microsoft.VisualStudio.Shell.Interop
Imports Microsoft.Samples.VisualStudio.CodeSweep.Scanner
Imports Microsoft.VisualStudio
Imports System.Diagnostics
Imports Microsoft.VisualStudio.Shell
Imports System.ComponentModel.Design
Imports System.Drawing
Imports Microsoft.Samples.VisualStudio.CodeSweep.VSPackage.Properties
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Globalization

Namespace Microsoft.Samples.VisualStudio.CodeSweep.VSPackage
    Friend Class TaskProvider
        Implements ITaskProvider, IVsSolutionEvents
        Public Sub New(ByVal provider As IServiceProvider)
            _imageList.ImageSize = New Size(9, 16)
            _imageList.Images.AddStrip(Resources.priority)
            _imageList.TransparentColor = Color.FromArgb(0, 255, 0)
            _serviceProvider = provider
            Dim taskList As IVsTaskList = TryCast(_serviceProvider.GetService(GetType(SVsTaskList)), IVsTaskList)
            Dim hr As Integer = taskList.RegisterTaskProvider(Me, _cookie)
            Debug.Assert(hr = VSConstants.S_OK, "RegisterTaskProvider did not return S_OK.")
            Debug.Assert(_cookie <> 0, "RegisterTaskProvider did not return a nonzero cookie.")

            SetCommandHandlers()

            ListenForProjectUnload()
        End Sub

#Region "ITaskProvider Members"

        Private NotInheritable Class AnonymousClass12
            Public hit As IScanHit
            Public Function AnonymousMethod(ByVal item As String) As Boolean
                Return String.Compare(item, hit.Term.Text, StringComparison.OrdinalIgnoreCase) = 0
            End Function
        End Class

        Public Sub AddResult(ByVal result As IScanResult, ByVal projectFile As String) Implements ITaskProvider.AddResult
            Dim fullPath As String = result.FilePath
            Dim locals As New AnonymousClass12()
            If (Not Path.IsPathRooted(fullPath)) Then
                fullPath = Utilities.AbsolutePathFromRelative(fullPath, Path.GetDirectoryName(projectFile))
            End If

            If result.Scanned Then
                For Each locals.hit In result.Results
                    If locals.hit.Warning IsNot Nothing AndAlso locals.hit.Warning.Length > 0 Then
                        ' See if we've warned about this term before; if so, don't warn again.
                        If Nothing Is _termsWithDuplicateWarning.Find(AddressOf locals.AnonymousMethod) Then
                            _tasks.Add(New Task(locals.hit.Term.Text, locals.hit.Term.Severity, locals.hit.Term.Class, locals.hit.Warning, "", "", -1, -1, "", "", Me, _serviceProvider))
                            _termsWithDuplicateWarning.Add(locals.hit.Term.Text)
                        End If
                    End If
                    _tasks.Add(New Task(locals.hit.Term.Text, locals.hit.Term.Severity, locals.hit.Term.Class, locals.hit.Term.Comment, locals.hit.Term.RecommendedTerm, fullPath, locals.hit.Line, locals.hit.Column, projectFile, locals.hit.LineText, Me, _serviceProvider))
                Next locals.hit
            Else
                _tasks.Add(New Task("", 1, "", String.Format(CultureInfo.CurrentUICulture, Resources.FileNotScannedError, fullPath), "", fullPath, -1, -1, projectFile, "", Me, _serviceProvider))
            End If
            Refresh()
        End Sub

        Public Sub Clear() Implements ITaskProvider.Clear
            _tasks.Clear()
            Refresh()
        End Sub

        Public Sub SetAsActiveProvider() Implements ITaskProvider.SetAsActiveProvider
            Dim taskList As IVsTaskList2 = TryCast(_serviceProvider.GetService(GetType(SVsTaskList)), IVsTaskList2)
            Dim ourGuid As Guid = _providerGuid
            Dim hr As Integer = taskList.SetActiveProvider(ourGuid)
            Debug.Assert(hr = VSConstants.S_OK, "SetActiveProvider did not return S_OK.")
        End Sub

        Public Sub ShowTaskList() Implements ITaskProvider.ShowTaskList
            Dim shell As IVsUIShell = TryCast(_serviceProvider.GetService(GetType(SVsUIShell)), IVsUIShell)
            Dim dummy As Object = Nothing
            Dim cmdSetGuid As Guid = VSConstants.GUID_VSStandardCommandSet97
            Dim hr As Integer = shell.PostExecCommand(cmdSetGuid, CUInt(Fix(VSConstants.VSStd97CmdID.TaskListWindow)), 0, dummy)
            Debug.Assert(hr = VSConstants.S_OK, "SetActiveProvider did not return S_OK.")
        End Sub

        ''' <summary>
        ''' Returns an image index between 0 and 2 inclusive corresponding to the specified severity.
        ''' </summary>
        Public Shared Function GetImageIndexForSeverity(ByVal severity As Integer) As Integer
            Return Math.Max(1, Math.Min(3, severity)) - 1
        End Function

        Public ReadOnly Property IsShowingIgnoredInstances() As Boolean Implements ITaskProvider.IsShowingIgnoredInstances
            Get
                Return _showingIgnoredInstances
            End Get
        End Property

#End Region

#Region "IVsTaskProvider Members"

        Public Function EnumTaskItems(<System.Runtime.InteropServices.Out()> ByRef ppenum As IVsEnumTaskItems) As Integer Implements Microsoft.VisualStudio.Shell.Interop.IVsTaskProvider.EnumTaskItems
            ppenum = New TaskEnumerator(_tasks, IsShowingIgnoredInstances)
            Return VSConstants.S_OK
        End Function

        <DllImport("comctl32.dll")> _
              Shared Function ImageList_Duplicate(ByVal original As IntPtr) As IntPtr
        End Function

        Public Function ImageList(<System.Runtime.InteropServices.Out()> ByRef phImageList As IntPtr) As Integer Implements Microsoft.VisualStudio.Shell.Interop.IVsTaskProvider.ImageList
            phImageList = ImageList_Duplicate(_imageList.Handle)
            Return VSConstants.S_OK
        End Function

        Public Function OnTaskListFinalRelease(ByVal pTaskList As IVsTaskList) As Integer Implements Microsoft.VisualStudio.Shell.Interop.IVsTaskProvider.OnTaskListFinalRelease
            If (_cookie <> 0) AndAlso (Not Nothing Is pTaskList) Then
                Dim hr As Integer = pTaskList.UnregisterTaskProvider(_cookie)
                Debug.Assert(hr = VSConstants.S_OK, "UnregisterTaskProvider did not return S_OK.")
            End If

            Return VSConstants.S_OK
        End Function

        Public Function ReRegistrationKey(<System.Runtime.InteropServices.Out()> ByRef pbstrKey As String) As Integer Implements Microsoft.VisualStudio.Shell.Interop.IVsTaskProvider.ReRegistrationKey
            pbstrKey = ""
            Return VSConstants.E_NOTIMPL
        End Function

        Public Function SubcategoryList(ByVal cbstr As UInteger, ByVal rgbstr As String(), <System.Runtime.InteropServices.Out()> ByRef pcActual As UInteger) As Integer Implements Microsoft.VisualStudio.Shell.Interop.IVsTaskProvider.SubcategoryList
            pcActual = 0
            Return VSConstants.E_NOTIMPL
        End Function

#End Region

#Region "IVsTaskProvider3 Members"

        Public Function GetColumn(ByVal iColumn As Integer, ByVal pColumn As VSTASKCOLUMN()) As Integer Implements Microsoft.VisualStudio.Shell.Interop.IVsTaskProvider3.GetColumn
            Select Case CType(iColumn, Task.TaskFields)
                Case Task.TaskFields.Class
                    pColumn(0).bstrCanonicalName = "Class"
                    pColumn(0).bstrHeading = Resources.ClassColumn
                    pColumn(0).bstrLocalizedName = Resources.ClassColumn
                    pColumn(0).bstrTip = ""
                    pColumn(0).cxDefaultWidth = 91
                    pColumn(0).cxMinWidth = 0
                    pColumn(0).fAllowHide = 1
                    pColumn(0).fAllowUserSort = 1
                    pColumn(0).fDescendingSort = 0
                    pColumn(0).fDynamicSize = 1
                    pColumn(0).fFitContent = 0
                    pColumn(0).fMoveable = 1
                    pColumn(0).fShowSortArrow = 1
                    pColumn(0).fSizeable = 1
                    pColumn(0).fVisibleByDefault = 1
                    pColumn(0).iDefaultSortPriority = -1
                    pColumn(0).iField = CInt(Fix(Task.TaskFields.Class))
                    pColumn(0).iImage = -1
                Case Task.TaskFields.Comment
                    pColumn(0).bstrCanonicalName = "Comment"
                    pColumn(0).bstrHeading = Resources.CommentColumn
                    pColumn(0).bstrLocalizedName = Resources.CommentColumn
                    pColumn(0).bstrTip = ""
                    pColumn(0).cxDefaultWidth = 400
                    pColumn(0).cxMinWidth = 0
                    pColumn(0).fAllowHide = 1
                    pColumn(0).fAllowUserSort = 1
                    pColumn(0).fDescendingSort = 0
                    pColumn(0).fDynamicSize = 1
                    pColumn(0).fFitContent = 0
                    pColumn(0).fMoveable = 1
                    pColumn(0).fShowSortArrow = 1
                    pColumn(0).fSizeable = 1
                    pColumn(0).fVisibleByDefault = 1
                    pColumn(0).iDefaultSortPriority = -1
                    pColumn(0).iField = CInt(Fix(Task.TaskFields.Comment))
                    pColumn(0).iImage = -1
                Case Task.TaskFields.File
                    pColumn(0).bstrCanonicalName = "File"
                    pColumn(0).bstrHeading = Resources.FileColumn
                    pColumn(0).bstrLocalizedName = Resources.FileColumn
                    pColumn(0).bstrTip = ""
                    pColumn(0).cxDefaultWidth = 92
                    pColumn(0).cxMinWidth = 0
                    pColumn(0).fAllowHide = 1
                    pColumn(0).fAllowUserSort = 1
                    pColumn(0).fDescendingSort = 0
                    pColumn(0).fDynamicSize = 0
                    pColumn(0).fFitContent = 0
                    pColumn(0).fMoveable = 1
                    pColumn(0).fShowSortArrow = 1
                    pColumn(0).fSizeable = 1
                    pColumn(0).fVisibleByDefault = 1
                    pColumn(0).iDefaultSortPriority = 2
                    pColumn(0).iField = CInt(Fix(Task.TaskFields.File))
                    pColumn(0).iImage = -1
                Case Task.TaskFields.Line
                    pColumn(0).bstrCanonicalName = "Line"
                    pColumn(0).bstrHeading = Resources.LineColumn
                    pColumn(0).bstrLocalizedName = Resources.LineColumn
                    pColumn(0).bstrTip = ""
                    pColumn(0).cxDefaultWidth = 63
                    pColumn(0).cxMinWidth = 0
                    pColumn(0).fAllowHide = 1
                    pColumn(0).fAllowUserSort = 1
                    pColumn(0).fDescendingSort = 0
                    pColumn(0).fDynamicSize = 0
                    pColumn(0).fFitContent = 0
                    pColumn(0).fMoveable = 1
                    pColumn(0).fShowSortArrow = 1
                    pColumn(0).fSizeable = 1
                    pColumn(0).fVisibleByDefault = 1
                    pColumn(0).iDefaultSortPriority = 3
                    pColumn(0).iField = CInt(Fix(Task.TaskFields.Line))
                    pColumn(0).iImage = -1
                Case Task.TaskFields.Priority
                    pColumn(0).bstrCanonicalName = "Priority"
                    pColumn(0).bstrHeading = "!"
                    pColumn(0).bstrLocalizedName = Resources.PriorityColumn
                    pColumn(0).bstrTip = Resources.PriorityColumn
                    pColumn(0).cxDefaultWidth = 22
                    pColumn(0).cxMinWidth = 0
                    pColumn(0).fAllowHide = 1
                    pColumn(0).fAllowUserSort = 1
                    pColumn(0).fDescendingSort = 0
                    pColumn(0).fDynamicSize = 0
                    pColumn(0).fFitContent = 0
                    pColumn(0).fMoveable = 1
                    pColumn(0).fShowSortArrow = 0
                    pColumn(0).fSizeable = 1
                    pColumn(0).fVisibleByDefault = 1
                    pColumn(0).iDefaultSortPriority = -1
                    pColumn(0).iField = CInt(Fix(Task.TaskFields.Priority))
                    pColumn(0).iImage = -1
                Case Task.TaskFields.PriorityNumber
                    pColumn(0).bstrCanonicalName = "Priority Number"
                    pColumn(0).bstrHeading = "!#"
                    pColumn(0).bstrLocalizedName = Resources.PriorityNumberColumn
                    pColumn(0).bstrTip = Resources.PriorityNumberColumn
                    pColumn(0).cxDefaultWidth = 50
                    pColumn(0).cxMinWidth = 0
                    pColumn(0).fAllowHide = 1
                    pColumn(0).fAllowUserSort = 1
                    pColumn(0).fDescendingSort = 0
                    pColumn(0).fDynamicSize = 0
                    pColumn(0).fFitContent = 0
                    pColumn(0).fMoveable = 1
                    pColumn(0).fShowSortArrow = 0
                    pColumn(0).fSizeable = 1
                    pColumn(0).fVisibleByDefault = 0
                    pColumn(0).iDefaultSortPriority = 0
                    pColumn(0).iField = CInt(Fix(Task.TaskFields.PriorityNumber))
                    pColumn(0).iImage = -1
                Case Task.TaskFields.Project
                    pColumn(0).bstrCanonicalName = "Project"
                    pColumn(0).bstrHeading = Resources.ProjectColumn
                    pColumn(0).bstrLocalizedName = Resources.ProjectColumn
                    pColumn(0).bstrTip = ""
                    pColumn(0).cxDefaultWidth = 116
                    pColumn(0).cxMinWidth = 0
                    pColumn(0).fAllowHide = 1
                    pColumn(0).fAllowUserSort = 1
                    pColumn(0).fDescendingSort = 0
                    pColumn(0).fDynamicSize = 0
                    pColumn(0).fFitContent = 0
                    pColumn(0).fMoveable = 1
                    pColumn(0).fShowSortArrow = 1
                    pColumn(0).fSizeable = 1
                    pColumn(0).fVisibleByDefault = 1
                    pColumn(0).iDefaultSortPriority = 1
                    pColumn(0).iField = CInt(Fix(Task.TaskFields.Project))
                    pColumn(0).iImage = -1
                Case Task.TaskFields.Replacement
                    pColumn(0).bstrCanonicalName = "Replacement"
                    pColumn(0).bstrHeading = Resources.ReplacementColumn
                    pColumn(0).bstrLocalizedName = Resources.ReplacementColumn
                    pColumn(0).bstrTip = ""
                    pColumn(0).cxDefaultWidth = 140
                    pColumn(0).cxMinWidth = 0
                    pColumn(0).fAllowHide = 1
                    pColumn(0).fAllowUserSort = 1
                    pColumn(0).fDescendingSort = 0
                    pColumn(0).fDynamicSize = 0
                    pColumn(0).fFitContent = 0
                    pColumn(0).fMoveable = 1
                    pColumn(0).fShowSortArrow = 1
                    pColumn(0).fSizeable = 1
                    pColumn(0).fVisibleByDefault = 0
                    pColumn(0).iDefaultSortPriority = -1
                    pColumn(0).iField = CInt(Fix(Task.TaskFields.Replacement))
                    pColumn(0).iImage = -1
                Case Task.TaskFields.Term
                    pColumn(0).bstrCanonicalName = "Term"
                    pColumn(0).bstrHeading = Resources.TermColumn
                    pColumn(0).bstrLocalizedName = Resources.TermColumn
                    pColumn(0).bstrTip = ""
                    pColumn(0).cxDefaultWidth = 103
                    pColumn(0).cxMinWidth = 0
                    pColumn(0).fAllowHide = 1
                    pColumn(0).fAllowUserSort = 1
                    pColumn(0).fDescendingSort = 0
                    pColumn(0).fDynamicSize = 1
                    pColumn(0).fFitContent = 0
                    pColumn(0).fMoveable = 1
                    pColumn(0).fShowSortArrow = 1
                    pColumn(0).fSizeable = 1
                    pColumn(0).fVisibleByDefault = 1
                    pColumn(0).iDefaultSortPriority = -1
                    pColumn(0).iField = CInt(Fix(Task.TaskFields.Term))
                    pColumn(0).iImage = -1
                Case Else
                    Return VSConstants.E_INVALIDARG
            End Select

            Return VSConstants.S_OK
        End Function

        Public Function GetColumnCount(<System.Runtime.InteropServices.Out()> ByRef pnColumns As Integer) As Integer Implements Microsoft.VisualStudio.Shell.Interop.IVsTaskProvider3.GetColumnCount
            pnColumns = System.Enum.GetValues(GetType(Task.TaskFields)).Length
            Return VSConstants.S_OK
        End Function

        Public Function GetProviderFlags(<System.Runtime.InteropServices.Out()> ByRef tpfFlags As UInteger) As Integer Implements Microsoft.VisualStudio.Shell.Interop.IVsTaskProvider3.GetProviderFlags
            tpfFlags = CUInt(__VSTASKPROVIDERFLAGS.TPF_NOAUTOROUTING Or __VSTASKPROVIDERFLAGS.TPF_ALWAYSVISIBLE)
            Return VSConstants.S_OK
        End Function

        Public Function GetProviderGuid(<System.Runtime.InteropServices.Out()> ByRef pguidProvider As Guid) As Integer Implements Microsoft.VisualStudio.Shell.Interop.IVsTaskProvider3.GetProviderGuid
            pguidProvider = _providerGuid
            Return VSConstants.S_OK
        End Function

        Public Function GetProviderName(<System.Runtime.InteropServices.Out()> ByRef pbstrName As String) As Integer Implements Microsoft.VisualStudio.Shell.Interop.IVsTaskProvider3.GetProviderName
            pbstrName = Resources.AppName
            Return VSConstants.S_OK
        End Function

        Public Function GetProviderToolbar(<System.Runtime.InteropServices.Out()> ByRef pguidGroup As Guid, <System.Runtime.InteropServices.Out()> ByRef pdwID As UInteger) As Integer Implements Microsoft.VisualStudio.Shell.Interop.IVsTaskProvider3.GetProviderToolbar
            pguidGroup = GuidList.guidVSPackageCmdSet
            pdwID = &H2020
            Return VSConstants.S_OK
        End Function

        Public Function GetSurrogateProviderGuid(<System.Runtime.InteropServices.Out()> ByRef pguidProvider As Guid) As Integer Implements Microsoft.VisualStudio.Shell.Interop.IVsTaskProvider3.GetSurrogateProviderGuid
            pguidProvider = Guid.Empty
            Return VSConstants.E_NOTIMPL
        End Function

        Public Function OnBeginTaskEdit(ByVal pItem As IVsTaskItem) As Integer Implements Microsoft.VisualStudio.Shell.Interop.IVsTaskProvider3.OnBeginTaskEdit
            Return VSConstants.E_NOTIMPL
        End Function

        Public Function OnEndTaskEdit(ByVal pItem As IVsTaskItem, ByVal fCommitChanges As Integer, <System.Runtime.InteropServices.Out()> ByRef pfAllowChanges As Integer) As Integer Implements Microsoft.VisualStudio.Shell.Interop.IVsTaskProvider3.OnEndTaskEdit
            pfAllowChanges = 0
            Return VSConstants.E_NOTIMPL
        End Function

#End Region

#Region "IVsSolutionEvents Members"

        Public Function OnAfterCloseSolution(ByVal pUnkReserved As Object) As Integer Implements IVsSolutionEvents.OnAfterCloseSolution
            Return VSConstants.S_OK
        End Function

        Public Function OnAfterLoadProject(ByVal pStubHierarchy As IVsHierarchy, ByVal pRealHierarchy As IVsHierarchy) As Integer Implements IVsSolutionEvents.OnAfterLoadProject
            Return VSConstants.S_OK
        End Function

        Public Function OnAfterOpenProject(ByVal pHierarchy As IVsHierarchy, ByVal fAdded As Integer) As Integer Implements IVsSolutionEvents.OnAfterOpenProject
            Return VSConstants.S_OK
        End Function

        Public Function OnAfterOpenSolution(ByVal pUnkReserved As Object, ByVal fNewSolution As Integer) As Integer Implements IVsSolutionEvents.OnAfterOpenSolution
            Return VSConstants.S_OK
        End Function

        Public Function OnBeforeCloseProject(ByVal pHierarchy As IVsHierarchy, ByVal fRemoved As Integer) As Integer Implements IVsSolutionEvents.OnBeforeCloseProject
            Dim projFile As String = ProjectUtilities.GetProjectFilePath(TryCast(pHierarchy, IVsProject))

            If (Not String.IsNullOrEmpty(projFile)) Then
                ' Remove all tasks for the project that is being closed.
                Dim i As Integer = 0
                Do While i < _tasks.Count
                    If _tasks(i).ProjectFile = projFile Then
                        _tasks.RemoveAt(i)
                        i -= 1
                    End If
                    i += 1
                Loop

                Refresh()
            End If

            Return VSConstants.S_OK
        End Function

        Public Function OnBeforeCloseSolution(ByVal pUnkReserved As Object) As Integer Implements IVsSolutionEvents.OnBeforeCloseSolution
            Return VSConstants.S_OK
        End Function

        Public Function OnBeforeUnloadProject(ByVal pRealHierarchy As IVsHierarchy, ByVal pStubHierarchy As IVsHierarchy) As Integer Implements IVsSolutionEvents.OnBeforeUnloadProject
            Return VSConstants.S_OK
        End Function

        Public Function OnQueryCloseProject(ByVal pHierarchy As IVsHierarchy, ByVal fRemoving As Integer, ByRef pfCancel As Integer) As Integer Implements IVsSolutionEvents.OnQueryCloseProject
            Return VSConstants.S_OK
        End Function

        Public Function OnQueryCloseSolution(ByVal pUnkReserved As Object, ByRef pfCancel As Integer) As Integer Implements IVsSolutionEvents.OnQueryCloseSolution
            Return VSConstants.S_OK
        End Function

        Public Function OnQueryUnloadProject(ByVal pRealHierarchy As IVsHierarchy, ByRef pfCancel As Integer) As Integer Implements IVsSolutionEvents.OnQueryUnloadProject
            Return VSConstants.S_OK
        End Function

#End Region

#Region "Private Members"

        Private Shared ReadOnly _providerGuid As New Guid("{9ACC41B7-15B4-4dd7-A0F3-0A935D5647F3}")

        Private _tasks As New List(Of Task)()
        Private ReadOnly _serviceProvider As IServiceProvider
        Private ReadOnly _cookie As UInteger
        Private _termsWithDuplicateWarning As New List(Of String)()
        Private _showingIgnoredInstances As Boolean = False
        Private _imageList As New System.Windows.Forms.ImageList()
        Private _solutionEventsCookie As UInteger = 0

        Private Sub ListenForProjectUnload()
            Dim solution As IVsSolution = TryCast(_serviceProvider.GetService(GetType(SVsSolution)), IVsSolution)

            Dim hr As Integer = solution.AdviseSolutionEvents(Me, _solutionEventsCookie)
            Debug.Assert(hr = VSConstants.S_OK, "AdviseSolutionEvents did not return S_OK.")
            Debug.Assert(_solutionEventsCookie <> 0, "AdviseSolutionEvents did not return a nonzero cookie.")
        End Sub

        Private Sub SetCommandHandlers()
            Dim mcs As OleMenuCommandService = TryCast(_serviceProvider.GetService(GetType(IMenuCommandService)), OleMenuCommandService)
            If mcs IsNot Nothing Then
                Dim ignoreID As New CommandID(GuidList.guidVSPackageCmdSet, CInt(Fix(PkgCmdIDList.cmdidIgnore)))
                Dim ignoreCommand As New OleMenuCommand(New EventHandler(AddressOf IgnoreSelectedItems), New EventHandler(AddressOf QueryIgnore), ignoreID)
                mcs.AddCommand(ignoreCommand)
                AddHandler ignoreCommand.BeforeQueryStatus, AddressOf QueryIgnore

                Dim dontIgnoreID As New CommandID(GuidList.guidVSPackageCmdSet, CInt(Fix(PkgCmdIDList.cmdidDoNotIgnore)))
                Dim dontIgnoreCommand As New OleMenuCommand(New EventHandler(AddressOf DontIgnoreSelectedItems), New EventHandler(AddressOf QueryDontIgnore), dontIgnoreID)
                mcs.AddCommand(dontIgnoreCommand)
                AddHandler dontIgnoreCommand.BeforeQueryStatus, AddressOf QueryDontIgnore

                Dim showIgnoredID As New CommandID(GuidList.guidVSPackageCmdSet, CInt(Fix(PkgCmdIDList.cmdidShowIgnoredInstances)))
                Dim showIgnoredCommand As New OleMenuCommand(New EventHandler(AddressOf ShowIgnoredInstances), showIgnoredID)
                mcs.AddCommand(showIgnoredCommand)
            End If
        End Sub

        Private Function SelectedTasks() As List(Of Task)
            Dim result As New List(Of Task)()

            Dim hr As Integer = VSConstants.S_OK
            Dim taskList As IVsTaskList2 = TryCast(_serviceProvider.GetService(GetType(SVsTaskList)), IVsTaskList2)

            Dim enumerator As IVsEnumTaskItems = Nothing
            hr = taskList.EnumSelectedItems(enumerator)
            Debug.Assert(hr = VSConstants.S_OK, "EnumSelectedItems did not return S_OK.")

            Dim items() As IVsTaskItem = {Nothing}
            Dim fetched() As UInteger = {0}
            enumerator.Reset()
            Do While ((enumerator.Next(1, items, fetched) = VSConstants.S_OK) And (fetched(0) = 1))
                Dim task As Task = TryCast(items(0), Task)
                If task IsNot Nothing Then
                    result.Add(task)
                End If
            Loop
            Return result
        End Function

        Private Sub QueryIgnore(ByVal sender As Object, ByVal e As EventArgs)
            Dim mcs As OleMenuCommandService = TryCast(_serviceProvider.GetService(GetType(IMenuCommandService)), OleMenuCommandService)
            Dim command As MenuCommand = mcs.FindCommand(New CommandID(GuidList.guidVSPackageCmdSet, CInt(Fix(PkgCmdIDList.cmdidIgnore))))

            If IsActiveProvider Then
                Dim anyNotIgnored As Boolean = Not SelectedTasks().TrueForAll(AddressOf AnonymousMethod1)

                command.Supported = True
                command.Enabled = anyNotIgnored
            Else
                command.Supported = False
            End If
        End Sub
        Private Function AnonymousMethod1(ByVal task As Task) As Boolean
            Return task.Ignored
        End Function

        Private Sub QueryDontIgnore(ByVal sender As Object, ByVal e As EventArgs)
            Dim mcs As OleMenuCommandService = TryCast(_serviceProvider.GetService(GetType(IMenuCommandService)), OleMenuCommandService)
            Dim command As MenuCommand = mcs.FindCommand(New CommandID(GuidList.guidVSPackageCmdSet, CInt(Fix(PkgCmdIDList.cmdidDoNotIgnore))))

            If IsActiveProvider Then
                Dim anyIgnored As Boolean = Not SelectedTasks().TrueForAll(AddressOf AnonymousMethod2)

                command.Supported = True
                command.Enabled = anyIgnored
            Else
                command.Supported = False
            End If
        End Sub
        Private Function AnonymousMethod2(ByVal task As Task) As Boolean
            Return Not task.Ignored
        End Function

        Private ReadOnly Property IsActiveProvider() As Boolean
            Get
                Dim taskList As IVsTaskList2 = TryCast(_serviceProvider.GetService(GetType(SVsTaskList)), IVsTaskList2)
                Dim activeProvider As IVsTaskProvider = Nothing
                Dim hr As Integer = taskList.GetActiveProvider(activeProvider)
                Debug.Assert(hr = VSConstants.S_OK, "GetActiveProvider did not return S_OK.")
                Return activeProvider Is Me
            End Get
        End Property

        Private Sub IgnoreSelectedItems(ByVal sender As Object, ByVal e As EventArgs)
            IgnoreSelectedItems(True)
        End Sub

        Private Sub DontIgnoreSelectedItems(ByVal sender As Object, ByVal e As EventArgs)
            IgnoreSelectedItems(False)
        End Sub

        Private Sub IgnoreSelectedItems(ByVal ignore As Boolean)
            Dim hr As Integer = VSConstants.S_OK
            Dim taskList As IVsTaskList2 = TryCast(_serviceProvider.GetService(GetType(SVsTaskList)), IVsTaskList2)

            Dim enumerator As IVsEnumTaskItems = Nothing
            hr = taskList.EnumSelectedItems(enumerator)
            Debug.Assert(hr = VSConstants.S_OK, "EnumSelectedItems did not return S_OK.")

            Dim items() As IVsTaskItem = {Nothing}
            Dim fetched() As UInteger = {0}
            enumerator.Reset()
            Do While ((enumerator.Next(1, items, fetched) = VSConstants.S_OK) And (fetched(0) = 1))
                Dim task As Task = TryCast(items(0), Task)
                If task IsNot Nothing Then
                    task.Ignored = ignore
                End If
            Loop
            Refresh()
        End Sub

        Private Sub ShowIgnoredInstances(ByVal sender As Object, ByVal e As EventArgs)
            _showingIgnoredInstances = Not _showingIgnoredInstances

            Dim mcs As OleMenuCommandService = TryCast(_serviceProvider.GetService(GetType(IMenuCommandService)), OleMenuCommandService)
            mcs.FindCommand(New CommandID(GuidList.guidVSPackageCmdSet, CInt(Fix(PkgCmdIDList.cmdidShowIgnoredInstances)))).Checked = _showingIgnoredInstances

            Refresh()
        End Sub

        Private Sub Refresh()
            Dim taskList As IVsTaskList = TryCast(_serviceProvider.GetService(GetType(SVsTaskList)), IVsTaskList)
            Dim hr As Integer = taskList.RefreshTasks(_cookie)
            Debug.Assert(hr = VSConstants.S_OK, "RefreshTasks did not return S_OK.")
        End Sub

#End Region
    End Class
End Namespace
