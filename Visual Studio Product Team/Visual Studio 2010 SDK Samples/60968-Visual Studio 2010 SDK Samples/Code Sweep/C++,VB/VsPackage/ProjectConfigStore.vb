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
Imports System.IO
Imports Microsoft.Samples.VisualStudio.CodeSweep.VSPackage.Properties
Imports Microsoft.Build.Construction

Namespace Microsoft.Samples.VisualStudio.CodeSweep.VSPackage
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' If the project referenced by this object is unloaded, this object will NOT detect the
    ''' condition and update itself.  Therefore, you must create a new ProjectConfigStore in that
    ''' situation.  Factory.GetProjectConfigurationStore takes care of this; it will return a new
    ''' store object if the project has been unloaded and reloaded.
    ''' </remarks>
    Friend Class ProjectConfigStore
        Implements IProjectConfigurationStore
        ''' <summary>
        ''' Creates a new project configuration store object.
        ''' </summary>
        ''' <param name="project">The project whose state this store will reflect.</param>
        ''' <exception cref="System.ArgumentNullException">Thrown if <c>project</c> is null.</exception>
        Public Sub New(ByVal project As IVsProject)
            _project = project

            _buildTask = Factory.GetBuildManager().GetBuildTask(project, False)
            If _buildTask IsNot Nothing Then
                ReadConfigFromBuildTask()
            End If

            AddHandler _termTableFiles.ItemAdded, AddressOf _termTableFiles_ItemAdded
            AddHandler _termTableFiles.ItemsRemoved, AddressOf _termTableFiles_ItemsRemoved
            AddHandler _ignoreInstances.ItemAdded, AddressOf _ignoreInstances_ItemAdded
            AddHandler _ignoreInstances.ItemsRemoved, AddressOf _ignoreInstances_ItemsRemoved
        End Sub

#Region "IProjectConfigurationStore Members"

        ''' <summary>
        ''' Gets the (read-write) collection of term table files for this project.
        ''' </summary>
        Public ReadOnly Property TermTableFiles() As ICollection(Of String) Implements IProjectConfigurationStore.TermTableFiles
            Get
                Return _termTableFiles
            End Get
        End Property

        ''' <summary>
        ''' Gets the (read-write) collection of ignore instances for this project.
        ''' </summary>
        Public ReadOnly Property IgnoreInstances() As ICollection(Of BuildTask.IIgnoreInstance) Implements IProjectConfigurationStore.IgnoreInstances
            Get
                Return _ignoreInstances
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the boolean value controlling whether the scan will be run as part of the
        ''' build process.
        ''' </summary>
        Public Property RunWithBuild() As Boolean Implements IProjectConfigurationStore.RunWithBuild
            Get
                Dim propVal As String = Factory.GetBuildManager().GetProperty(_project, BuildManager.RunWithBuildFlag)
                Return propVal IsNot Nothing AndAlso propVal = "true"
            End Get
            Set(ByVal value As Boolean)
                Dim propVal As String
                If value Then
                    propVal = "true"
                Else
                    propVal = "false"
                End If
                Factory.GetBuildManager().SetProperty(_project, BuildManager.RunWithBuildFlag, propVal)
            End Set
        End Property

        ''' <summary>
        ''' Gets a boolean value indicating whether the current project contains a CodeSweep configuration.
        ''' </summary>
        Public ReadOnly Property HasConfiguration() As Boolean Implements IProjectConfigurationStore.HasConfiguration
            Get
                Return _buildTask IsNot Nothing
            End Get
        End Property

        ''' <summary>
        ''' Creates the default configuration for a project which does not have one.
        ''' </summary>
        ''' <exception cref="System.InvalidOperationException">Thrown if the project already has a configuration.</exception>
        Public Sub CreateDefaultConfiguration() Implements IProjectConfigurationStore.CreateDefaultConfiguration
            If _buildTask IsNot Nothing Then
                Throw New InvalidOperationException(Resources.AlreadyHasConfiguration)
            End If
            CreateBuildTaskIfNecessary()
        End Sub

#End Region

#Region "Private Members"

        Private ReadOnly _project As IVsProject
        Private _buildTask As ProjectTaskElement
        Private ReadOnly _termTableFiles As New CollectionWithEvents(Of String)()
        Private ReadOnly _ignoreInstances As New CollectionWithEvents(Of BuildTask.IIgnoreInstance)()

        Private Sub PersistIgnoreInstances()
            Dim projectFolder As String = Path.GetDirectoryName(ProjectUtilities.GetProjectFilePath(_project))
            Dim serialization As String = BuildTask.Factory.SerializeIgnoreInstances(_ignoreInstances, projectFolder)

            CreateBuildTaskIfNecessary()
            _buildTask.SetParameter("IgnoreInstances", serialization)
        End Sub

        Private Sub PersistTermTables()
            CreateBuildTaskIfNecessary()

            Dim projectFolder As String = Path.GetDirectoryName(ProjectUtilities.GetProjectFilePath(_project))
            Dim relativePaths As List(Of String) = Utilities.RelativizePathsIfPossible(_termTableFiles, projectFolder)

            _buildTask.SetParameter("TermTables", CodeSweep.Utilities.Concatenate(relativePaths, ";"))
        End Sub

        Private Sub _ignoreInstances_ItemsRemoved(ByVal sender As Object, ByVal e As EventArgs)
            PersistIgnoreInstances()
        End Sub

        Private Sub _ignoreInstances_ItemAdded(ByVal sender As Object, ByVal e As ItemEventArgs(Of BuildTask.IIgnoreInstance))
            PersistIgnoreInstances()
        End Sub

        Private Sub _termTableFiles_ItemsRemoved(ByVal sender As Object, ByVal e As EventArgs)
            PersistTermTables()
        End Sub

        Private Sub _termTableFiles_ItemAdded(ByVal sender As Object, ByVal e As ItemEventArgs(Of String))
            PersistTermTables()
        End Sub

        Private Sub CreateBuildTaskIfNecessary()
            If _buildTask Is Nothing Then
                _buildTask = Factory.GetBuildManager().GetBuildTask(_project, True)
                ReadConfigFromBuildTask()
            End If
        End Sub

        Private Sub ReadConfigFromBuildTask()
            Dim projectFolder As String = Path.GetDirectoryName(ProjectUtilities.GetProjectFilePath(_project))

            _ignoreInstances.AddRange(BuildTask.Factory.DeserializeIgnoreInstances(_buildTask.GetParameter("IgnoreInstances"), projectFolder))

            For Each termTable As String In _buildTask.GetParameter("TermTables").Split(";"c)
                If termTable IsNot Nothing AndAlso termTable.Length > 0 Then
                    If Path.IsPathRooted(termTable) Then
                        _termTableFiles.Add(termTable)
                    Else
                        _termTableFiles.Add(Utilities.AbsolutePathFromRelative(termTable, projectFolder))
                    End If
                End If
            Next termTable
        End Sub

#End Region
    End Class
End Namespace
