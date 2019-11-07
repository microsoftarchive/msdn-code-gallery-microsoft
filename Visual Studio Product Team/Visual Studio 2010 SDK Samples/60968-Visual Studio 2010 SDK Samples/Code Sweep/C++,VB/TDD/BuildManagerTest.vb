'**************************************************************************

'Copyright (c) Microsoft Corporation. All rights reserved.
'This code is licensed under the Visual Studio SDK license terms.
'THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
'ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
'IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
'PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

'**************************************************************************


Imports Microsoft.VisualBasic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports System
Imports System.Text
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.Shell.Interop
Imports EnvDTE
Imports System.IO
Imports System.Reflection
Imports Microsoft.Build.Construction
Imports System.Linq

Namespace Microsoft.Samples.VisualStudio.CodeSweep.UnitTests
    ''' <summary>
    ''' This is a test class for CodeSweep.VSPackage.BuildManager and is intended
    ''' to contain all CodeSweep.VSPackage.BuildManager Unit Tests.
    ''' </summary>
    <TestClass()> _
    Public Class BuildManagerTest


        Private testContextInstance As TestContext

        ''' <summary>
        ''' Gets or sets the test context which provides
        ''' information about and functionality for the current test run.
        ''' </summary>
        Public Property TestContext() As TestContext
            Get
                Return testContextInstance
            End Get
            Set(ByVal value As TestContext)
                testContextInstance = value
            End Set
        End Property
#Region "Additional test attributes"
        ' 
        'You can use the following additional attributes as you write your tests:
        '
        'Use ClassInitialize to run code before running the first test in the class
        '
        Private Shared _serviceProvider As MockServiceProvider
        <ClassInitialize()> _
        Public Shared Sub MyClassInitialize(ByVal testContext As TestContext)
            If CodeSweep.VSPackage.Factory_Accessor.ServiceProvider Is Nothing Then
                _serviceProvider = New MockServiceProvider()
                CodeSweep.VSPackage.Factory_Accessor.ServiceProvider = _serviceProvider
            Else
                _serviceProvider = TryCast(CodeSweep.VSPackage.Factory_Accessor.ServiceProvider, MockServiceProvider)
            End If

            Utilities.CopyTargetsFileToBinDir()
        End Sub

        'Use ClassCleanup to run code after all tests in a class have run
        '
        '[ClassCleanup()]
        'public static void MyClassCleanup()
        '{
        '}
        '
        'Use TestInitialize to run code before running each test
        '
        '[TestInitialize()]
        'public void MyTestInitialize()
        '{
        '}
        '
        'Use TestCleanup to run code after each test has run
        '
        <TestCleanup()> _
        Public Sub MyTestCleanup()
            Utilities.CleanUpTempFiles()
            Utilities.RemoveCommandHandlers(_serviceProvider)
        End Sub
        '
#End Region

        <DeploymentItem("VsPackage.dll"), TestMethod(), ExpectedException(GetType(ArgumentNullException))> _
        Public Sub ConstructWithNullArg()
            Dim accessor As New CodeSweep.VSPackage.BuildManager_Accessor(provider:=Nothing)
        End Sub

        <DeploymentItem("VsPackage.dll"), TestMethod()> _
        Public Sub IsListeningToBuildEventsTest()
            Dim accessor As New CodeSweep.VSPackage.BuildManager_Accessor(_serviceProvider)

            Dim dte As MockDTE = TryCast(_serviceProvider.GetService(GetType(EnvDTE.DTE)), MockDTE)
            Dim buildEvents As MockBuildEvents = TryCast(dte.Events.BuildEvents, MockBuildEvents)

            Assert.IsFalse(accessor.IsListeningToBuildEvents, "IsListeningToBuildEvents should be false by default.")
            Dim expectedSubscriberCount As Int32 = buildEvents.OnBuildBeginSubscriberCount + 1
            accessor.IsListeningToBuildEvents = True
            Assert.IsTrue(accessor.IsListeningToBuildEvents, "IsListeningToBuildEvents could not be set to true.")
            Assert.AreEqual(expectedSubscriberCount, buildEvents.OnBuildBeginSubscriberCount, "Build manager did not subscribe to OnBuildBegin when IsListeningToBuildEvents set to true.")

            accessor.IsListeningToBuildEvents = False
            expectedSubscriberCount -= 1
            Assert.IsFalse(accessor.IsListeningToBuildEvents, "IsListeningToBuildEvents could not be set to false.")
            Assert.AreEqual(expectedSubscriberCount, buildEvents.OnBuildBeginSubscriberCount, "Build manager did not unsubscribe from OnBuildBegin when IsListeningToBuildEvents set to false.")
        End Sub

        Private Function GetImport(ByVal project As Microsoft.Build.Evaluation.Project, ByVal importPath As String) As ProjectImportElement
            For Each import As ProjectImportElement In project.Xml.Imports
                If import.Project = importPath Then
                    Return import
                End If
            Next import

            Return Nothing
        End Function
        Private NotInheritable Class AnonymousClass7
            Public resultCounts As New List(Of Integer)()
            Public Sub AnonymousMethod(ByVal sender As Object, ByVal args As MockTaskList.RefreshTasksArgs)
                resultCounts.Add(Utilities.TasksOfProvider(args.Provider).Count)
            End Sub


        End Class

        <DeploymentItem("VsPackage.dll"), TestMethod()> _
        Public Sub TestBuildBegin()
            Dim locals As New AnonymousClass7()
            Dim accessor As New CodeSweep.VSPackage.BuildManager_Accessor(_serviceProvider)

            accessor.IsListeningToBuildEvents = True

            ' Listen for task list refresh events.

            Dim taskList As MockTaskList = TryCast(_serviceProvider.GetService(GetType(SVsTaskList)), MockTaskList)
            AddHandler taskList.OnRefreshTasks, AddressOf locals.AnonymousMethod

            ' Create multiple projects with ScannerTask tasks.
            Dim scanFile As String = Utilities.CreateTempTxtFile("foo abc foo def foo")
            Dim termTable As String = Utilities.CreateTermTable(New String() {"foo", "bar"})
            Dim project1 As Microsoft.Build.Evaluation.Project = Utilities.SetupMSBuildProject(New String() {scanFile}, New String() {termTable})
            Dim project2 As Microsoft.Build.Evaluation.Project = Utilities.SetupMSBuildProject(New String() {scanFile}, New String() {termTable})

            Utilities.RegisterProjectWithMocks(project1, _serviceProvider)
            Utilities.RegisterProjectWithMocks(project2, _serviceProvider)

            ' Fire the build begin event.
            Dim dte As MockDTE = TryCast(_serviceProvider.GetService(GetType(EnvDTE.DTE)), MockDTE)
            Dim buildEvents As MockBuildEvents = TryCast(dte.Events.BuildEvents, MockBuildEvents)
            buildEvents.FireOnBuildBegin(vsBuildScope.vsBuildScopeProject, vsBuildAction.vsBuildActionBuild)

            Try
                Assert.IsNotNull(Build.Evaluation.ProjectCollection.GlobalProjectCollection.HostServices.GetHostObject(project1.FullPath, "AfterBuild", "ScannerTask"), "Host object for task in first project not set.")
                Assert.IsNotNull(Build.Evaluation.ProjectCollection.GlobalProjectCollection.HostServices.GetHostObject(project2.FullPath, "AfterBuild", "ScannerTask"), "Host object for task in second project not set.")

                Assert.AreEqual(1, locals.resultCounts.Count, "Task list recieved wrong number of refresh requests.")
                Assert.AreEqual(0, locals.resultCounts(0), "Task list was not cleared.")
            Finally
                buildEvents.FireOnBuildDone(vsBuildScope.vsBuildScopeProject, vsBuildAction.vsBuildActionBuild)
            End Try
        End Sub

        <DeploymentItem("VsPackage.dll"), TestMethod(), ExpectedException(GetType(ArgumentNullException))> _
        Public Sub GetBuildTaskWithNullArg()
            Dim accessor As New CodeSweep.VSPackage.BuildManager_Accessor(_serviceProvider)

            accessor.GetBuildTask(Nothing, False)
        End Sub

        <DeploymentItem("VsPackage.dll"), TestMethod()> _
        Public Sub GetBuildTaskWithNoCreation()
            Dim accessor As New CodeSweep.VSPackage.BuildManager_Accessor(_serviceProvider)

            ' Create a project without a build task.
            Dim project1 As Microsoft.Build.Evaluation.Project = Utilities.SetupMSBuildProject()
            Dim vsProject As MockIVsProject = Utilities.RegisterProjectWithMocks(project1, _serviceProvider)

            Assert.IsNull(accessor.GetBuildTask(vsProject, False), "GetBuildTask did not return null for project without a ScannerTask.")

            ' Create a project with a build task.
            Dim scanFile As String = Utilities.CreateTempTxtFile("foo abc foo def foo")
            Dim termTable As String = Utilities.CreateTermTable(New String() {"foo", "bar"})
            Dim project2 As Build.Evaluation.Project = Utilities.SetupMSBuildProject(New String() {scanFile}, New String() {termTable})

            Dim existingTask As ProjectTaskElement = Utilities.GetScannerTask(project2)

            vsProject = Utilities.RegisterProjectWithMocks(project2, _serviceProvider)

            Assert.AreEqual(existingTask, accessor.GetBuildTask(vsProject, False), "GetBuildTask did not return expected task object.")
            Assert.IsNull(GetImport(project2, Utilities.GetTargetsPath()), "GetBuildTask created Import unexpected.")
        End Sub

        <DeploymentItem("VsPackage.dll"), TestMethod()> _
        Public Sub GetBuildTaskWithCreation()
            Dim accessor As New CodeSweep.VSPackage.BuildManager_Accessor(_serviceProvider)

            ' Create a project without a build task.
            Dim project As Microsoft.Build.Evaluation.Project = Utilities.SetupMSBuildProject()

            project.Xml.AddItem("foo", "blah.txt")
            project.Xml.AddItem("bar", "blah2.cs")
            project.Xml.AddItem("Reference", "My.Namespace.Etc")

            Dim vsProject As MockIVsProject = Utilities.RegisterProjectWithMocks(project, _serviceProvider)

            Dim task As ProjectTaskElement = accessor.GetBuildTask(vsProject, True)

            Assert.IsNotNull(task, "GetBuildTask did not create task.")
            Assert.AreEqual("false", task.ContinueOnError, "ContinueOnError is wrong.")
            Assert.AreEqual("Exists('" & Utilities.GetTargetsPath() & "') and '$(RunCodeSweepAfterBuild)' == 'true'", task.Condition, "Condition is wrong.")
            Assert.AreEqual("@(foo);@(bar)", task.GetParameter("FilesToScan"), "FilesToScan property is wrong.")
            Assert.AreEqual("$(MSBuildProjectFullPath)", task.GetParameter("Project"), "Project property is wrong.")

            Dim projectFolder As String = Path.GetDirectoryName(project.FullPath)
            Dim expectedTermTable As String = CodeSweep.Utilities.RelativePathFromAbsolute(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & "\Microsoft\CodeSweep\sample_term_table.xml", projectFolder)
            Assert.AreEqual(Path.GetFileName(expectedTermTable), Path.GetFileName(task.GetParameter("TermTables")), "TermTables property is wrong.")

            ' Ensure the task is in the AfterBuild target.
            Dim found As Boolean = False
            For Each thisTask As ProjectTaskElement In project.Xml.Targets.FirstOrDefault(Function(element) element.Name = "AfterBuild").Tasks
                If thisTask Is task Then
                    found = True
                    Exit For
                End If
            Next thisTask
            Assert.IsTrue(found, "The task was not in the AfterBuild target.")

            Dim import As ProjectImportElement = GetImport(project, Utilities.GetTargetsPath())
            Assert.IsNotNull(import, "GetBuildTask did not create Import.")
        End Sub

        <DeploymentItem("VsPackage.dll"), TestMethod(), ExpectedException(GetType(ArgumentNullException))> _
        Public Sub AllItemsInProjectWithNullArg()
            Dim accessor As New CodeSweep.VSPackage.BuildManager_Accessor(_serviceProvider)

            accessor.AllItemsInProject(Nothing)
        End Sub

        <DeploymentItem("VsPackage.dll"), TestMethod()> _
        Public Sub AllItemsInProjectTest()
            Dim accessor As New CodeSweep.VSPackage.BuildManager_Accessor(_serviceProvider)

            Dim project As Microsoft.Build.Evaluation.Project = Utilities.SetupMSBuildProject()
            Dim projectFolder As String = Path.GetDirectoryName(project.FullPath)

            project.Xml.AddItem("foo", "blah.txt")
            project.Xml.AddItem("bar", "blah2.cs")
            project.Xml.AddItem("bar", "My.Namespace.Etc")
            project.Xml.AddItem("bar", "blah3.vsmdi")

            ' Create the files on disk, otherwise AllItemsInProject will exclude them.
            File.WriteAllText(projectFolder & "\blah.txt", "")
            File.WriteAllText(projectFolder & "\blah2.cs", "")
            File.WriteAllText(projectFolder & "\blah3.vsmdi", "")

            Dim vsProject As MockIVsProject = Utilities.RegisterProjectWithMocks(project, _serviceProvider)

            Dim projectDir As String = Path.GetDirectoryName(project.FullPath)

            Dim items As List(Of String) = Utilities.ListFromEnum(accessor.AllItemsInProject(vsProject))

            Assert.AreEqual(3, items.Count, "AllItemsInProject returned wrong number of items.")
            CollectionAssert.Contains(items, projectDir & "\blah.txt", "AllItemsInProject did not return blah.txt.")
            CollectionAssert.Contains(items, projectDir & "\blah2.cs", "AllItemsInProject did not return blah2.cs.")
            CollectionAssert.Contains(items, projectDir & "\blah3.vsmdi", "AllItemsInProject did not return blah3.vsmdi.")
            CollectionAssert.DoesNotContain(items, "My.Namespace.Etc", "AllItemsInProject returned Reference entry.")
        End Sub
        Private NotInheritable Class AnonymousClass1
            Public accessor As New CodeSweep.VSPackage.BuildManager_Accessor(_serviceProvider)
            Public project As Microsoft.Build.Evaluation.Project = Utilities.SetupMSBuildProject()
            Public vsProject As MockIVsProject = Utilities.RegisterProjectWithMocks(project, _serviceProvider)

            Public Sub AnonymousMethod1()
                accessor.GetProperty(Nothing, "foo")
            End Sub
            Public Sub AnonymousMethod2()
                accessor.GetProperty(vsProject, Nothing)
            End Sub
            Public Sub AnonymousMethod3()
                accessor.GetProperty(vsProject, "")
            End Sub
        End Class
        <DeploymentItem("VsPackage.dll"), TestMethod()> _
        Public Sub GetPropertyWithInvalidArgs()
            Dim locals As New AnonymousClass1()
            Dim thrown As Boolean = Utilities.HasFunctionThrown(Of ArgumentNullException)(AddressOf locals.AnonymousMethod1)
            Assert.IsTrue(thrown, "GetProperty did not throw ArgumentNullException with null project arg.")
            thrown = Utilities.HasFunctionThrown(Of ArgumentNullException)(AddressOf locals.AnonymousMethod2)
            Assert.IsTrue(thrown, "GetProperty did not throw ArgumentNullException with null name arg.")
            thrown = Utilities.HasFunctionThrown(Of ArgumentException)(AddressOf locals.AnonymousMethod3)
            Assert.IsTrue(thrown, "GetProperty did not throw ArgumentException with empty name arg.")
        End Sub

        <DeploymentItem("VsPackage.dll"), TestMethod()> _
        Public Sub GetPropertyTest()
            Dim accessor As New CodeSweep.VSPackage.BuildManager_Accessor(_serviceProvider)

            Dim project As Microsoft.Build.Evaluation.Project = Utilities.SetupMSBuildProject()

            Dim group As ProjectPropertyGroupElement = project.Xml.AddPropertyGroup()
            group.AddProperty("foo", "bar")

            Dim vsProject As MockIVsProject = Utilities.RegisterProjectWithMocks(project, _serviceProvider)

            Assert.IsNull(accessor.GetProperty(vsProject, "blah"), "GetProperty did not return null with invalid name.")
            Assert.AreEqual("bar", accessor.GetProperty(vsProject, "foo"), "GetProperty did not return correct value.")
        End Sub
        Private NotInheritable Class AnonymousClass2
            Public accessor As New CodeSweep.VSPackage.BuildManager_Accessor(_serviceProvider)
            Public project As Microsoft.Build.Evaluation.Project = Utilities.SetupMSBuildProject()
            Public vsProject As MockIVsProject = Utilities.RegisterProjectWithMocks(project, _serviceProvider)
            Public Sub AnonymousMethod1()
                accessor.SetProperty(Nothing, "foo", "bar")
            End Sub
            Public Sub AnonymousMethod2()
                accessor.SetProperty(vsProject, Nothing, "bar")
            End Sub
            Public Sub AnonymousMethod3()
                accessor.SetProperty(vsProject, "foo", Nothing)
            End Sub
            Public Sub AnonymousMethod4()
                accessor.SetProperty(vsProject, "", "bar")
            End Sub
        End Class

        <DeploymentItem("VsPackage.dll"), TestMethod()> _
        Public Sub SetPropertyWithInvalidArgs()
            Dim locals As New AnonymousClass2()
            Dim thrown As Boolean = Utilities.HasFunctionThrown(Of ArgumentNullException)(AddressOf locals.AnonymousMethod1)
            Assert.IsTrue(thrown, "SetProperty did not throw ArgumentNullException with null project arg.")
            thrown = Utilities.HasFunctionThrown(Of ArgumentNullException)(AddressOf locals.AnonymousMethod2)
            Assert.IsTrue(thrown, "SetProperty did not throw ArgumentNullException with null name arg.")


            thrown = Utilities.HasFunctionThrown(Of ArgumentNullException)(AddressOf locals.AnonymousMethod3)
            Assert.IsTrue(thrown, "SetProperty did not throw ArgumentNullException with null value arg.")

            thrown = Utilities.HasFunctionThrown(Of ArgumentException)(AddressOf locals.AnonymousMethod4)
            Assert.IsTrue(thrown, "SetProperty did not throw ArgumentException with empty name arg.")
        End Sub

        <DeploymentItem("VsPackage.dll"), TestMethod()> _
        Public Sub SetPropertyTest()
            Dim accessor As New CodeSweep.VSPackage.BuildManager_Accessor(_serviceProvider)

            Dim project As Microsoft.Build.Evaluation.Project = Utilities.SetupMSBuildProject()
            Dim vsProject As MockIVsProject = Utilities.RegisterProjectWithMocks(project, _serviceProvider)

            accessor.SetProperty(vsProject, "foo", "bar")
            Assert.AreEqual("bar", accessor.GetProperty(vsProject, "foo"), "SetProperty did not set value correctly.")

            accessor.SetProperty(vsProject, "foo", "")
            Assert.AreEqual("", accessor.GetProperty(vsProject, "foo"), "SetProperty did not set value correctly (2).")
        End Sub

        <DeploymentItem("VsPackage.dll"), TestMethod()> _
        Public Sub ScanStopsWhenBuildBegins()
            Dim accessor As New CodeSweep.VSPackage.BuildManager_Accessor(_serviceProvider)

            accessor.IsListeningToBuildEvents = True

            Dim firstFile As String = Utilities.CreateBigFile()
            Dim secondFile As String = Utilities.CreateTempTxtFile("bar bar bar floop doop bar")
            Dim termTable As String = Utilities.CreateTermTable(New String() {"foo", "bar"})
            Dim project As Microsoft.Build.Evaluation.Project = Utilities.SetupMSBuildProject(New String() {firstFile, secondFile}, New String() {termTable})
            Dim vsProject As MockIVsProject = Utilities.RegisterProjectWithMocks(project, _serviceProvider)

            ' Start a background scan.
            Dim scannerAccessor As CodeSweep.VSPackage.IBackgroundScanner_Accessor = CodeSweep.VSPackage.Factory_Accessor.GetBackgroundScanner()

            scannerAccessor.Start(New IVsProject() {vsProject})

            ' Fire the build begin event.
            Dim dte As MockDTE = TryCast(_serviceProvider.GetService(GetType(EnvDTE.DTE)), MockDTE)
            Dim buildEvents As MockBuildEvents = TryCast(dte.Events.BuildEvents, MockBuildEvents)
            buildEvents.FireOnBuildBegin(vsBuildScope.vsBuildScopeProject, vsBuildAction.vsBuildActionBuild)

            Try
                Assert.IsFalse(scannerAccessor.IsRunning, "Background scan did not stop when build began.")
            Finally
                buildEvents.FireOnBuildDone(vsBuildScope.vsBuildScopeProject, vsBuildAction.vsBuildActionBuild)
                accessor.IsListeningToBuildEvents = False
            End Try
        End Sub
    End Class


End Namespace
