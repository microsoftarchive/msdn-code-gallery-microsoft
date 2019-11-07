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
Imports Microsoft.VisualStudio.Shell.Interop
Imports EnvDTE
Imports System.IO
Imports System.Globalization
Imports System.Diagnostics

Namespace Microsoft.Samples.VisualStudio.CodeSweep.VSPackage
    Friend Class NonMSBuildProjectConfigStore
        Implements IProjectConfigurationStore
        Public Sub New(ByVal project As IVsProject, ByVal serviceProvider As IServiceProvider)
            _serviceProvider = serviceProvider
            _project = project

            ReadConfigFromProject()

            AddHandler _termTableFiles.ItemAdded, AddressOf _termTableFiles_ItemAdded
            AddHandler _termTableFiles.ItemsRemoved, AddressOf _termTableFiles_ItemsRemoved
            AddHandler _ignoreInstances.ItemAdded, AddressOf _ignoreInstances_ItemAdded
            AddHandler _ignoreInstances.ItemsRemoved, AddressOf _ignoreInstances_ItemsRemoved
        End Sub

#Region "IProjectConfigurationStore Members"

        Public ReadOnly Property TermTableFiles() As ICollection(Of String) Implements IProjectConfigurationStore.TermTableFiles
            Get
                Return _termTableFiles
            End Get
        End Property

        Public ReadOnly Property IgnoreInstances() As ICollection(Of BuildTask.IIgnoreInstance) Implements IProjectConfigurationStore.IgnoreInstances
            Get
                Return _ignoreInstances
            End Get
        End Property

        Public Property RunWithBuild() As Boolean Implements IProjectConfigurationStore.RunWithBuild
            Get
                Return False
            End Get
            Set(ByVal value As Boolean)
                Throw New InvalidOperationException(Resources.RunWithBuildForNonMSBuild)
            End Set
        End Property

        Public ReadOnly Property HasConfiguration() As Boolean Implements IProjectConfigurationStore.HasConfiguration
            Get
                Dim dteProject As Project = GetDTEProject(_project)
                If dteProject Is Nothing Then
                    Return False
                End If

                Return dteProject.Globals.VariableExists(_termTablesName) OrElse dteProject.Globals.VariableExists(_ignoreInstancesName)
            End Get
        End Property

        Public Sub CreateDefaultConfiguration() Implements IProjectConfigurationStore.CreateDefaultConfiguration
            If HasConfiguration Then
                Throw New InvalidOperationException(Resources.AlreadyHasConfiguration)
            End If

            _termTableFiles.Add(Globals.DefaultTermTablePath)
        End Sub

#End Region ' IProjectConfigurationStore Members

#Region "Private Members"

        Private ReadOnly _serviceProvider As IServiceProvider
        Private ReadOnly _project As IVsProject
        Private ReadOnly _termTableFiles As New CollectionWithEvents(Of String)()
        Private ReadOnly _ignoreInstances As New CollectionWithEvents(Of BuildTask.IIgnoreInstance)()

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

        Private Function GetDTEProject(ByVal project As IVsProject) As Project
            Dim projectPath As String = ProjectUtilities.GetProjectFilePath(project)

            Dim dte As DTE = TryCast(_serviceProvider.GetService(GetType(DTE)), DTE)
            If dte Is Nothing Then
                Debug.Fail("Failed to get DTE service.")
                Return Nothing
            End If

            For Each dteProject As Project In dte.Solution.Projects
                If String.Compare(dteProject.FileName, projectPath, StringComparison.OrdinalIgnoreCase) = 0 Then
                    Return dteProject
                End If
            Next dteProject

            Return Nothing
        End Function

        Private Const _termTablesName As String = "CodeSweep_TermTables"
        Private Const _ignoreInstancesName As String = "CodeSweep_IgnoreInstances"

        Private Sub ReadConfigFromProject()
            Dim projectFolder As String = Path.GetDirectoryName(ProjectUtilities.GetProjectFilePath(_project))
            Dim dteProject As Project = GetDTEProject(_project)
            If dteProject Is Nothing Then
                Return
            End If

            If dteProject.Globals.VariableExists(_termTablesName) Then
                Dim termTables As String = CStr(dteProject.Globals(_termTablesName))

                For Each table As String In termTables.Split(";"c)
                    If table IsNot Nothing AndAlso table.Length > 0 Then
                        If Path.IsPathRooted(table) Then
                            _termTableFiles.Add(table)
                        Else
                            _termTableFiles.Add(Utilities.AbsolutePathFromRelative(table, projectFolder))
                        End If
                    End If
                Next table
            End If

            If dteProject.Globals.VariableExists(_ignoreInstancesName) Then
                Dim ignoreInstances As String = CStr(dteProject.Globals(_ignoreInstancesName))
                _ignoreInstances.AddRange(BuildTask.Factory.DeserializeIgnoreInstances(ignoreInstances, projectFolder))
            End If
        End Sub

        Private Sub PersistTermTables()
            Dim projectFolder As String = Path.GetDirectoryName(ProjectUtilities.GetProjectFilePath(_project))
            Dim relativePaths As List(Of String) = Utilities.RelativizePathsIfPossible(_termTableFiles, projectFolder)

            Dim dteProject As Project = GetDTEProject(_project)
            If dteProject Is Nothing Then
                Return
            End If

            dteProject.Globals(_termTablesName) = Utilities.Concatenate(relativePaths, ";")
            dteProject.Globals.VariablePersists(_termTablesName) = True
        End Sub

        Private Sub PersistIgnoreInstances()
            Dim projectFolder As String = Path.GetDirectoryName(ProjectUtilities.GetProjectFilePath(_project))
            Dim serialization As String = BuildTask.Factory.SerializeIgnoreInstances(_ignoreInstances, projectFolder)

            Dim dteProject As Project = GetDTEProject(_project)
            If dteProject Is Nothing Then
                Return
            End If

            dteProject.Globals(_ignoreInstancesName) = serialization
            dteProject.Globals.VariablePersists(_ignoreInstancesName) = True
        End Sub

#End Region ' Private Members
    End Class
End Namespace
