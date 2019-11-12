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
Imports Microsoft.Build.Evaluation
Imports Microsoft.Build.Construction
Imports Microsoft.Build.Execution
Imports Microsoft.VisualStudio.Shell.Interop
Imports Microsoft.VisualStudio
Imports System.IO
Imports System.Reflection
Imports System.Diagnostics
Imports System.Linq
Imports Microsoft.Samples.VisualStudio.CodeSweep.VSPackage.Properties

Namespace Microsoft.Samples.VisualStudio.CodeSweep.VSPackage
    Friend Class HierarchyConstants
        Public Const VSITEMID_NIL As UInteger = &HFFFFFFFFL
        Public Const VSITEMID_ROOT As UInteger = &HFFFFFFFEL
        Public Const VSITEMID_SELECTION As UInteger = &HFFFFFFFDL
    End Class

    Friend Class BuildManager
        Implements IBuildManager
        Public Const RunWithBuildFlag As String = "RunCodeSweepAfterBuild"

        ''' <summary>
        ''' Creates a new build manager object.
        ''' </summary>
        ''' <param name="provider">The service provider that will be used to get VS services.</param>
        ''' <exception cref="System.ArgumentNullException">Thrown if <c>serviceProvider</c> is null.</exception>
        Public Sub New(ByVal provider As IServiceProvider)
            If provider Is Nothing Then
                Throw New ArgumentNullException("provider")
            End If

            _serviceProvider = provider
        End Sub

#Region "IBuildManager Members"

        Public Event BuildStarted As EmptyEvent Implements IBuildManager.BuildStarted
        Public Event BuildStopped As EmptyEvent Implements IBuildManager.BuildStopped

        ''' <summary>
        ''' Gets or sets whether this object is subscribed to the build events fired by the VS IDE.
        ''' </summary>
        ''' <remarks>
        ''' If this object is subscribed to build events, it will clear the CodeSweep task list and
        ''' set the host object for all ScannerTask tasks in all projects when the build begins.
        ''' </remarks>
        Public Property IsListeningToBuildEvents() As Boolean Implements IBuildManager.IsListeningToBuildEvents
            Get
                Return _listening
            End Get
            Set(ByVal value As Boolean)
                If value <> _listening Then
                    If value Then
                        AddHandler GetBuildEvents().OnBuildBegin, AddressOf buildEvents_OnBuildBegin
                        AddHandler GetBuildEvents().OnBuildDone, AddressOf buildEvents_OnBuildDone
                    Else
                        RemoveHandler GetBuildEvents().OnBuildBegin, AddressOf buildEvents_OnBuildBegin
                        RemoveHandler GetBuildEvents().OnBuildDone, AddressOf buildEvents_OnBuildDone
                    End If
                    _listening = value
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets the CodeSweep build task for the given project, optionally creating it if it does not exist.
        ''' </summary>
        ''' <param name="project">The project from which the task will be retrieved.</param>
        ''' <param name="createIfNecessary">If true, the task will be created if it does not exist.</param>
        ''' <returns>The task object retrieved, or null if no task was found and <c>createIfNecessary</c> is false.</returns>
        ''' <exception cref="System.ArgumentNullException">Thrown if <c>project</c> is null.</exception>
        ''' <remarks>
        ''' If the task is created, a <c>UsingTask</c> entry is also inserted into the project to
        ''' specify the location of the dll.
        ''' The task is created in the "AfterBuild" target of the project file.
        ''' The task is created with the following properties:
        '''     Condition = "'$(RunCodeSweepAfterBuild)' == 'true'"
        '''     ContinueOnError = false
        ''' and the following parameters:
        '''     FilesToScan = all item groups in project except "Reference", formatted as "@(group1);@(group2);..."
        '''     TermTables = [user's app data folder]\CodeSweep\sample_term_table.xml
        '''     Project = "$(MSBuildProjectFullPath)"
        ''' </remarks>
        Public Function GetBuildTask(ByVal project As IVsProject, ByVal createIfNecessary As Boolean) As ProjectTaskElement Implements IBuildManager.GetBuildTask
            If project Is Nothing Then
                Throw New ArgumentNullException("project")
            End If

            If createIfNecessary Then
                CreateTaskIfNecessary(project)
            End If

            Dim msbuildProject As Project = MSBuildProjectFromIVsProject(project)
            Return GetNamedTask(msbuildProject.Xml.Targets.FirstOrDefault(Function(element) element.Name = "AfterBuild"), "ScannerTask")
        End Function
        Private NotInheritable Class AnonymousClass10
            Public proj As IVsProject
            Public projectDir As String

            Public Function AnonymousMethod(ByVal id As UInteger) As String
                Dim name As String = Nothing
                proj.GetMkDocument(id, name)
                If name IsNot Nothing AndAlso name.Length > 0 AndAlso (Not Path.IsPathRooted(name)) Then
                    name = Utilities.AbsolutePathFromRelative(name, projectDir)
                End If
                Return name
            End Function
        End Class

        ''' <summary>
        ''' Enumerates all items in the project except those in the "Reference" group.
        ''' </summary>
        ''' <param name="project">The project from which to retrieve the items.</param>
        ''' <returns>A list of item "Include" values.  For items that specify files, these will be the file names.</returns>
        ''' <exception cref="System.ArgumentNullException">Thrown if <c>project</c> is null.</exception>
        Public Function AllItemsInProject(ByVal project As IVsProject) As ICollection(Of String) Implements IBuildManager.AllItemsInProject
            If project Is Nothing Then
                Throw New ArgumentNullException("project")
            End If
            Dim locals As New AnonymousClass10()
            locals.proj = project
            locals.projectDir = Path.GetDirectoryName(ProjectUtilities.GetProjectFilePath(locals.proj))

            Dim hierarchy As IVsHierarchy = TryCast(locals.proj, IVsHierarchy)
            Dim allNames As List(Of String) = ChildrenOf(hierarchy, HierarchyConstants.VSITEMID_ROOT).ConvertAll(Of String)(AddressOf locals.AnonymousMethod)
            allNames.RemoveAll(AddressOf AnonymousMethod1)

            Return allNames
        End Function

        Private Function AnonymousMethod1(ByVal name As String) As Boolean
            Return Not File.Exists(name)
        End Function

        ''' <summary>
        ''' Sets a property in the given project, overriding the existing value if it exists.
        ''' </summary>
        ''' <param name="project">The project in which the property will be set.</param>
        ''' <param name="name">The name of the property.</param>
        ''' <param name="value">The value of the property.</param>
        ''' <exception cref="System.ArgumentNullException">Thrown if <c>project</c>, <c>name</c>, or <c>value</c> is null.</exception>
        ''' <exception cref="System.ArgumentException">Thrown if <c>name</c> is empty or contains invalid characters.</exception>
        Public Sub SetProperty(ByVal project As IVsProject, ByVal name As String, ByVal value As String) Implements IBuildManager.SetProperty
            If project Is Nothing Then
                Throw New ArgumentNullException("project")
            End If
            If name Is Nothing Then
                Throw New ArgumentNullException("name")
            End If
            If value Is Nothing Then
                Throw New ArgumentNullException("value")
            End If
            If name.Length = 0 Then
                Throw New ArgumentException(Resources.EmptyProperty, "name")
            End If

            Dim msbuildProject As Project = MSBuildProjectFromIVsProject(project)

            Dim [property] As ProjectPropertyElement = FindProperty(msbuildProject.Xml, name)
            If [property] Is Nothing Then
                Dim group As ProjectPropertyGroupElement = msbuildProject.Xml.AddPropertyGroup()
                group.AddProperty(name, value)
            Else
                [property].Value = value
            End If
        End Sub

        ''' <summary>
        ''' Gets a property from the given project.
        ''' </summary>
        ''' <param name="project">The project from which the property will be retrieved.</param>
        ''' <param name="name">The name of the property.</param>
        ''' <returns>The value of the specified property, or null if it does not exist.</returns>
        ''' <exception cref="System.ArgumentNullException">Thrown if <c>project</c> or <c>name</c> is null.</exception>
        ''' <exception cref="System.ArgumentException">Thrown if <c>name</c> is empty.</exception>
        Public Function GetProperty(ByVal project As IVsProject, ByVal name As String) As String Implements IBuildManager.GetProperty
            If project Is Nothing Then
                Throw New ArgumentNullException("project")
            End If
            If name Is Nothing Then
                Throw New ArgumentNullException("name")
            End If
            If name.Length = 0 Then
                Throw New ArgumentException(Resources.EmptyProperty, "name")
            End If

            Dim msbuildProject As Project = MSBuildProjectFromIVsProject(project)
            Dim [property] As ProjectPropertyElement = FindProperty(msbuildProject.Xml, name)

            If [property] Is Nothing Then
                Return Nothing
            Else
                Return [property].Value
            End If
        End Function

        ''' <summary>
        ''' Creates the per-user files for the current user if they have not yet been created.
        ''' </summary>
        Public Sub CreatePerUserFilesAsNecessary() Implements IBuildManager.CreatePerUserFilesAsNecessary
            CreateDefaultTermTableIfNecessary()
            CreateInclusionListIfNecessary()
        End Sub

#End Region

#Region "Private Members"

        Private _serviceProvider As IServiceProvider
        Private _listening As Boolean = False
        Private _buildEvents As EnvDTE.BuildEvents

        Private Shared Sub CreateDefaultTermTableIfNecessary()
            If (Not File.Exists(Globals.DefaultTermTablePath)) Then
                Directory.CreateDirectory(Path.GetDirectoryName(Globals.DefaultTermTablePath))
                File.Copy(Globals.DefaultTermTableInstallPath, Globals.DefaultTermTablePath)
            End If
        End Sub

        Private Shared Sub CreateInclusionListIfNecessary()
            If (Not File.Exists(Globals.AllowedExtensionsPath)) Then
                Directory.CreateDirectory(Path.GetDirectoryName(Globals.AllowedExtensionsPath))
                File.Copy(Globals.AllowedExtensionsInstallPath, Globals.AllowedExtensionsPath)
            End If
        End Sub

        Private Sub SetHostObject()
            Dim services As HostServices = New HostServices()
            For Each projectName As String In ProjectNames
                If projectName IsNot Nothing Then
                    services.RegisterHostObject(projectName, "AfterBuild", "ScannerTask", Factory.GetScannerHost())
                End If
            Next projectName

            ProjectCollection.GlobalProjectCollection.HostServices = services
        End Sub

        Private Shared Function MSBuildProjectFromIVsProject(ByVal project As IVsProject) As Project
            Return ProjectCollection.GlobalProjectCollection.GetLoadedProjects(ProjectUtilities.GetProjectFilePath(project)).FirstOrDefault()
        End Function

        Private Shared Function GetNamedTask(ByVal target As ProjectTargetElement, ByVal taskName As String) As ProjectTaskElement
            If target IsNot Nothing Then
                For Each task As ProjectTaskElement In target.Tasks
                    If task.Name = taskName Then
                        Return task
                    End If
                Next task
            End If

            Return Nothing
        End Function

        Private Shared Sub CreateTaskIfNecessary(ByVal project As IVsProject)
            Dim msbuildProject As Project = MSBuildProjectFromIVsProject(project)
            Dim target As ProjectTargetElement = msbuildProject.Xml.Targets.FirstOrDefault(Function(element) element.Name = "AfterBuild")
            If target Is Nothing Then
                target = msbuildProject.Xml.AddTarget("AfterBuild")
            End If

            Dim importFolder As String = Path.GetDirectoryName(GetType(CodeSweep.BuildTask.ScannerTask).Module.FullyQualifiedName)
            Dim importPath As String = Utilities.EncodeProgramFilesVar(importFolder) & "\CodeSweep.targets"
            Dim installedCondition As String = "Exists('" & importPath & "')"

            If GetNamedTask(target, "ScannerTask") Is Nothing Then
                Dim projectFolder As String = Path.GetDirectoryName(ProjectUtilities.GetProjectFilePath(project))
                Dim newTask As ProjectTaskElement = target.AddTask("ScannerTask")
                newTask.Condition = installedCondition & " and '$(" & RunWithBuildFlag & ")' == 'true'"
                newTask.ContinueOnError = "false"
                newTask.SetParameter("FilesToScan", CodeSweep.Utilities.Concatenate(AllItemGroupsInProject(msbuildProject.Xml), ";"))
                newTask.SetParameter("TermTables", Globals.DefaultTermTablePath)
                newTask.SetParameter("Project", "$(MSBuildProjectFullPath)")
            End If

            Dim found As Boolean = False

            For Each import As ProjectImportElement In msbuildProject.Xml.Imports

                If import.Project = importPath Then
                    found = True
                    Exit For
                End If
            Next import

            If (Not found) Then
                Dim importElement As ProjectImportElement = msbuildProject.Xml.AddImport(importPath)
                importElement.Condition = installedCondition
            End If
        End Sub

        Private Shared Function FindProperty(ByVal msbuildProject As ProjectRootElement, ByVal name As String) As ProjectPropertyElement
            For Each group As ProjectPropertyGroupElement In msbuildProject.PropertyGroups
                For Each [property] As ProjectPropertyElement In group.Properties
                    If [property].Name = name Then
                        Return [property]
                    End If
                Next [property]
            Next group
            Return Nothing
        End Function

        Private Shared Function AllItemGroupsInProject(ByVal project As ProjectRootElement) As IEnumerable(Of String)
            Dim names As New List(Of String)()

            For Each group As ProjectItemGroupElement In project.ItemGroups
                For Each item As ProjectItemElement In group.Items
                    If item.ItemType <> "Reference" Then
                        Dim groupName As String = "@(" & item.ItemType & ")"
                        If (Not names.Contains(groupName)) Then
                            names.Add(groupName)
                        End If
                    End If
                Next item
            Next group

            Return names
        End Function

        Private Sub buildEvents_OnBuildBegin(ByVal Scope As EnvDTE.vsBuildScope, ByVal Action As EnvDTE.vsBuildAction)
            If Action = EnvDTE.vsBuildAction.vsBuildActionBuild OrElse Action = EnvDTE.vsBuildAction.vsBuildActionRebuildAll Then
                Factory.GetBackgroundScanner().StopIfRunning(True)
                Factory.GetTaskProvider().Clear()
                Factory.GetTaskProvider().SetAsActiveProvider()
                SetHostObject()

                If BuildStartedEvent IsNot Nothing Then
                    RaiseEvent BuildStarted()
                End If
            End If
        End Sub

        Private Sub buildEvents_OnBuildDone(ByVal Scope As EnvDTE.vsBuildScope, ByVal Action As EnvDTE.vsBuildAction)
            If BuildStoppedEvent IsNot Nothing Then
                RaiseEvent BuildStopped()
            End If
        End Sub

        Private ReadOnly Property ProjectNames() As IEnumerable(Of String)
            Get
                Dim solution As IVsSolution = TryCast(_serviceProvider.GetService(GetType(SVsSolution)), IVsSolution)

                Dim projectCount As UInteger = 0
                Dim hr As Integer = solution.GetProjectFilesInSolution(CUInt(__VSGETPROJFILESFLAGS.GPFF_SKIPUNLOADEDPROJECTS), 0, Nothing, projectCount)
                System.Diagnostics.Debug.Assert(hr = VSConstants.S_OK, "GetProjectFilesInSolution failed.")

                Dim projNames As String() = New String(CInt(projectCount - 1)) {}
                hr = solution.GetProjectFilesInSolution(CUInt(__VSGETPROJFILESFLAGS.GPFF_SKIPUNLOADEDPROJECTS), projectCount, projNames, projectCount)
                System.Diagnostics.Debug.Assert(hr = VSConstants.S_OK, "GetProjectFilesInSolution failed.")

                Return projNames
            End Get
        End Property

        Private Function GetBuildEvents() As EnvDTE.BuildEvents
            If _buildEvents Is Nothing Then
                Dim dte As EnvDTE.DTE = TryCast(_serviceProvider.GetService(GetType(EnvDTE.DTE)), EnvDTE.DTE)

                _buildEvents = dte.Events.BuildEvents
            End If
            Return _buildEvents
        End Function

        Private Function ChildrenOf(ByVal hierarchy As IVsHierarchy, ByVal rootID As UInteger) As List(Of UInteger)
            Dim result As New List(Of UInteger)()

            Dim itemID As UInteger = FirstChild(hierarchy, rootID)
            Do While itemID <> HierarchyConstants.VSITEMID_NIL
                result.Add(itemID)
                result.AddRange(ChildrenOf(hierarchy, itemID))
                itemID = NextSibling(hierarchy, itemID)
            Loop

            Return result
        End Function

        Private Shared Function FirstChild(ByVal hierarchy As IVsHierarchy, ByVal rootID As UInteger) As UInteger
            Dim childIDObj As Object = Nothing
            hierarchy.GetProperty(rootID, CInt(Fix(__VSHPROPID.VSHPROPID_FirstChild)), childIDObj)
            If childIDObj IsNot Nothing Then
                If CType(childIDObj, Integer) < 0 Then
                    Return CUInt(4294967294 - (CType(childIDObj, Integer)))
                End If
                Return CUInt(CInt(Fix(childIDObj)))
            End If

            Return HierarchyConstants.VSITEMID_NIL
        End Function
        Private Shared Function NextSibling(ByVal hierarchy As IVsHierarchy, ByVal firstID As UInteger) As UInteger
            Dim siblingIDObj As Object = Nothing
            hierarchy.GetProperty(firstID, CInt(Fix(__VSHPROPID.VSHPROPID_NextSibling)), siblingIDObj)
            If siblingIDObj IsNot Nothing Then
                If CType(siblingIDObj, Integer) < 0 Then
                    Return CUInt(4294967294 - (CType(siblingIDObj, Integer)))
                End If
                Return CUInt(CInt(Fix(siblingIDObj)))
            End If

            Return HierarchyConstants.VSITEMID_NIL
        End Function
#End Region
    End Class
End Namespace
