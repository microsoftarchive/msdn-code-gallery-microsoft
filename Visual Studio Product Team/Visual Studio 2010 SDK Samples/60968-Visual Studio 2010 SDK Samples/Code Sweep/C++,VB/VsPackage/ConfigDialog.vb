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
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Text
Imports System.Windows.Forms
Imports Microsoft.VisualStudio.Shell.Interop
Imports System.IO
Imports Microsoft.Samples.VisualStudio.CodeSweep.VSPackage.Properties
Imports System.Globalization

Namespace Microsoft.Samples.VisualStudio.CodeSweep.VSPackage
    Partial Public Class ConfigDialog
        Inherits Form
        Implements IConfigurationDialog
        ''' <summary>
        ''' Creates a new dialog object.
        ''' </summary>
        Public Sub New()
            InitializeComponent()

            AddHandler _termTableListBox.DrawItem, AddressOf _termTableListBox_DrawItem
        End Sub

        ''' <summary>
        ''' Sets the service provider used by this object to get VS services.
        ''' </summary>
        ''' <exception cref="System.ArgumentNullException">Thrown if <c>provider</c> is null.</exception>
        Public Property ServiceProvider() As IServiceProvider
            Set(ByVal value As IServiceProvider)
                _serviceProvider = value
            End Set

            Get
                Return _serviceProvider
            End Get
        End Property

#Region "IConfigurationDialog Members"

        ''' <summary>
        ''' Shows the dialog using the configuration of the specified project(s).
        ''' </summary>
        ''' <param name="projects">The projects whose configuration will be used to populate the dialog, and which will be modified when the user changes the settings in the dialog.</param>
        ''' <exception cref="System.ArgumentNullException">Thrown if <c>projects</c> is null.</exception>
        ''' <exception cref="System.ArgumentException">Thrown if <c>projects</c> is empty or contains null entries.</exception>
        ''' <exception cref="System.InvalidOperationException">Thrown if <c>SetServiceProvider</c> has not been called.</exception>
        ''' <remarks>
        ''' The dialog is opened in a modal state; this method does not return until the dialog is
        ''' closed.
        ''' If the specified projects do not have identical configurations, a warning dialog will
        ''' be displayed and the dialog will not be shown.
        ''' When the dialog is opened, a new build task will be created in each project if one does
        ''' not already exist.
        ''' The UI behavior is as follows:
        '''     The Add button is always enabled.  Clicking it opens an "Open File" common dialog.
        '''     If the dialog is canceled, there is no change to the state.  If a file is opened,
        '''     the file's full path is shown in the list box and is appended to the "TermTables"
        '''     property of each project's scanner task.  If the full path is too wide to be
        '''     shown in the list box, it will be abbreviated by substitution of ellipses.
        ''' 
        '''     The Remove button is enabled if and only if one or more items are selected in the
        '''     list box.  Clicking it deletes the selected items (and removes them from the
        '''     "TermTables" property of each project's scanner task).
        ''' 
        '''     The Scan Now button is enabled if and only if the list box contains one
        '''     or more valid term tables.  Pressing it closes the dialog and begins a background
        '''     scan using the BackgroundScanner class.
        ''' 
        '''     The checkbox to scan automatically with every build is checked if and only if the
        '''     RunCodeSweepAfterBuild property is set in the project(s).
        ''' 
        '''     The Close button closes the dialog, retaining any changes the user has made.  The
        '''     "X" button in the title bar has the same effect.
        ''' </remarks>
        Public Overloads Sub Invoke(ByVal projects As IList(Of IVsProject)) Implements IConfigurationDialog.Invoke
            If projects Is Nothing Then
                Throw New ArgumentNullException("projects")
            End If

            If projects.Count = 0 Then
                Throw New ArgumentException("The list cannot be empty.", "projects")
            End If

            MyBase.Font = GetVSFont()

            CreateDefaultConfigurationsIfNecessary(projects)

            If (Not IdenticallyConfigured(projects)) Then
                ShowInconsistentConfigurationError()
                Return
            End If

            _projects = projects
            RefreshAllControls()
            ShowDialog()
        End Sub

#End Region

#Region "Private Members"

        Private Class ListEntry
            Private ReadOnly _fullPath As String
            Private ReadOnly _isValid As Boolean

            Public Sub New(ByVal fullPath As String, ByVal isValid As Boolean)
                _fullPath = fullPath
                _isValid = isValid
            End Sub

            Public ReadOnly Property FullPath() As String
                Get
                    Return _fullPath
                End Get
            End Property

            Public ReadOnly Property IsValid() As Boolean
                Get
                    Return _isValid
                End Get
            End Property

            Public Sub DrawAbbreviatedPath(ByVal font As Font, ByVal color As Color, ByVal bounds As Rectangle, ByVal graphics As Graphics)
                Dim format As New StringFormat()
                format.Trimming = StringTrimming.EllipsisPath

                Dim brush As Brush = New SolidBrush(color)
                graphics.DrawString(_fullPath, font, brush, bounds, format)

                brush.Dispose()
                format.Dispose()
            End Sub
        End Class

        Private _projects As IList(Of IVsProject)
        Private _serviceProvider As IServiceProvider

        Private Sub RefreshEnabledStates()
            Dim store As IProjectConfigurationStore = Factory.GetProjectConfigurationStore(_projects(0))

            _removeButton.Enabled = _termTableListBox.SelectedItems.Count > 0
            _autoScanCheckBox.Checked = store.RunWithBuild
            _scanButton.Enabled = ValidTermTableCount > 0

            If (Not AllAreMSBuildProjects(_projects)) Then
                _autoScanCheckBox.Enabled = False
                _toolTip.SetToolTip(_autoScanCheckBox, Resources.NonMSBuildCheckboxTip)
            End If
        End Sub

        Private Shared Function AllAreMSBuildProjects(ByVal projects As IList(Of IVsProject)) As Boolean
            For Each project As IVsProject In projects
                If (Not ProjectUtilities.IsMSBuildProject(project)) Then
                    Return False
                End If
            Next project

            Return True
        End Function

        Private Sub RefreshAllControls()
            Dim store As IProjectConfigurationStore = Factory.GetProjectConfigurationStore(_projects(0))

            _termTableListBox.Items.Clear()
            For Each tableFile As String In store.TermTableFiles
                AddTermTableWithAbsolutePath(tableFile)
            Next tableFile

            RefreshEnabledStates()
        End Sub

        Private ReadOnly Property ValidTermTableCount() As Integer
            Get
                Dim count As Integer = 0

                For Each fileObj As Object In _termTableListBox.Items
                    Dim entry As ListEntry = CType(fileObj, ListEntry)

                    If entry.IsValid Then
                        count += 1
                    End If
                Next fileObj

                Return count
            End Get
        End Property

        Private Sub _autoScanCheckBox_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles _autoScanCheckBox.CheckedChanged
            For Each project As IVsProject In _projects
                Factory.GetProjectConfigurationStore(project).RunWithBuild = _autoScanCheckBox.Checked
            Next project
        End Sub

        Private Sub _closeButton_Click(ByVal sender As Object, ByVal e As EventArgs) Handles _closeButton.Click
            Me.DialogResult = System.Windows.Forms.DialogResult.OK
            Close()
        End Sub
        Private NotInheritable Class AnonymousClass11
            Public dialog As New OpenFileDialog()

            Public Sub AnonymousMethod(ByVal sender2 As Object, ByVal e2 As CancelEventArgs)
                ' Try to instantiate each term table file, to validate it.
                For Each file As String In dialog.FileNames
                    Try
                        CodeSweep.Scanner.Factory.GetTermTable(file)
                    Catch
                        MsgBox(Resources.InvalidTermTableError, MsgBoxStyle.OkOnly Or MsgBoxStyle.Critical, Resources.InvalidTermTableCaption)
                        e2.Cancel = True
                        Exit For
                    End Try
                Next file
            End Sub
        End Class

        Private Sub _addButton_Click(ByVal sender As Object, ByVal e As EventArgs) Handles _addButton.Click
            Dim locals As New AnonymousClass11()
            locals.dialog.Title = Resources.AddTableDialogCaption

            locals.dialog.DefaultExt = ".xml"
            locals.dialog.Multiselect = True
            locals.dialog.RestoreDirectory = True

            locals.dialog.Filter = Resources.FileOpenDlgFilters
            locals.dialog.FilterIndex = 0

            AddHandler locals.dialog.FileOk, AddressOf locals.AnonymousMethod
            If locals.dialog.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
                For Each file As String In locals.dialog.FileNames
                    AddTermTableWithAbsolutePath(file)
                Next file

                RefreshEnabledStates()
            End If
        End Sub

        Private Sub AddTermTableWithAbsolutePath(ByVal file As String)
            For Each project As IVsProject In _projects
                If (Not Factory.GetProjectConfigurationStore(project).TermTableFiles.Contains(file)) Then
                    Factory.GetProjectConfigurationStore(project).TermTableFiles.Add(file)
                End If
            Next project

            Dim isValid As Boolean = True
            Try
                CodeSweep.Scanner.Factory.GetTermTable(file)
            Catch ex As Exception
                If TypeOf ex Is ArgumentException OrElse TypeOf ex Is System.Xml.XmlException Then
                    isValid = False
                Else
                    Throw
                End If
            End Try

            _termTableListBox.Items.Add(New ListEntry(file, isValid))
        End Sub

        Private Sub _removeButton_Click(ByVal sender As Object, ByVal e As EventArgs) Handles _removeButton.Click
            Do While _termTableListBox.SelectedItems.Count > 0
                Dim item As Object = _termTableListBox.SelectedItems(0)

                For Each project As IVsProject In _projects
                    For Each existingTable As String In Factory.GetProjectConfigurationStore(project).TermTableFiles
                        If String.Compare(existingTable, (CType(item, ListEntry)).FullPath, StringComparison.OrdinalIgnoreCase) = 0 Then
                            Factory.GetProjectConfigurationStore(project).TermTableFiles.Remove(existingTable)
                            Exit For
                        End If
                    Next existingTable
                Next project

                _termTableListBox.Items.Remove(item)
            Loop

            RefreshEnabledStates()
        End Sub

        Private Sub _termTableListBox_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles _termTableListBox.SelectedIndexChanged
            _removeButton.Enabled = _termTableListBox.SelectedItems.Count > 0
        End Sub

        Private Sub _termTableListBox_ControlAdded(ByVal sender As Object, ByVal e As ControlEventArgs) Handles _termTableListBox.ControlAdded
            _scanButton.Enabled = ValidTermTableCount > 0
        End Sub

        Private Sub _termTableListBox_ControlRemoved(ByVal sender As Object, ByVal e As ControlEventArgs) Handles _termTableListBox.ControlRemoved
            _scanButton.Enabled = ValidTermTableCount > 0
        End Sub

        Private Sub _scanButton_Click(ByVal sender As Object, ByVal e As EventArgs) Handles _scanButton.Click
            Factory.GetBackgroundScanner().StopIfRunning(True)
            Factory.GetBackgroundScanner().Start(_projects)
            Me.DialogResult = System.Windows.Forms.DialogResult.OK
            Close()
        End Sub

        Private Sub ConfigDialog_KeyPress(ByVal sender As Object, ByVal e As KeyPressEventArgs) Handles MyBase.KeyPress
            If e.KeyChar = ChrW(27) Then
                Me.DialogResult = System.Windows.Forms.DialogResult.OK
                Close()
            End If
        End Sub

        Private Sub _termTableListBox_DrawItem(ByVal sender As Object, ByVal e As DrawItemEventArgs)
            e.DrawBackground()
            If _termTableListBox.Items.Count > 0 AndAlso e.Index >= 0 Then
                Dim entry As ListEntry = CType(_termTableListBox.Items(e.Index), ListEntry)

                If entry IsNot Nothing Then
                    Dim color As Color

                    If entry.IsValid Then
                        color = e.ForeColor
                    Else
                        color = color.FromKnownColor(KnownColor.GrayText)
                    End If
                    entry.DrawAbbreviatedPath(e.Font, color, e.Bounds, e.Graphics)
                End If
            End If
        End Sub

        Private Shared Sub CreateDefaultConfigurationsIfNecessary(ByVal projects As IEnumerable(Of IVsProject))
            For Each project As IVsProject In projects
                Dim store As IProjectConfigurationStore = Factory.GetProjectConfigurationStore(project)

                If (Not store.HasConfiguration) Then
                    store.CreateDefaultConfiguration()
                End If
            Next project
        End Sub

        Private Shared Sub ShowInconsistentConfigurationError()
            MsgBox(Resources.InconsistentConfigurationError, MsgBoxStyle.OkOnly Or MsgBoxStyle.Critical, Resources.InconsistentConfigurationCaption)
        End Sub

        Private Shared Function IdenticallyConfigured(ByVal projects As IList(Of IVsProject)) As Boolean
            If projects Is Nothing Then
                Throw New ArgumentNullException("projects")
            End If

            If projects.Count = 0 Then
                Throw New ArgumentException("The list cannot be empty.", "projects")
            End If

            Dim firstStore As IProjectConfigurationStore = Factory.GetProjectConfigurationStore(projects(0))

            For i As Integer = 1 To projects.Count - 1
                Dim thisStore As IProjectConfigurationStore = Factory.GetProjectConfigurationStore(projects(i))

                If firstStore.RunWithBuild <> thisStore.RunWithBuild OrElse (Not Utilities.UnorderedCollectionsAreEqual(firstStore.TermTableFiles, thisStore.TermTableFiles)) Then
                    Return False
                End If
            Next i

            Return True
        End Function

        Private Function GetVSFont() As Font
            Dim hostLocale As IUIHostLocale = TryCast(_serviceProvider.GetService(GetType(IUIHostLocale)), IUIHostLocale)

            If hostLocale IsNot Nothing Then
                Dim dlgFont() As UIDLGLOGFONT = {New UIDLGLOGFONT()}
                hostLocale.GetDialogFont(dlgFont)
                Return Font.FromLogFont(dlgFont(0))
            Else
                Return MyBase.Font
            End If
        End Function

#End Region
    End Class
End Namespace