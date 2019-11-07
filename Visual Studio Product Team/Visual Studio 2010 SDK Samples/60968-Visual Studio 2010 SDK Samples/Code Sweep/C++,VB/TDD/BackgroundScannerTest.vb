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
Imports Microsoft.Samples.VisualStudio.CodeSweep.Scanner
Imports Microsoft.VisualStudio.Shell.Interop
Imports Microsoft.Build.Evaluation
Imports System.IO
Imports System.Reflection
Imports Microsoft.VisualStudio

Namespace Microsoft.Samples.VisualStudio.CodeSweep.UnitTests
    ''' <summary>
    ''' This is a test class for CodeSweep.VSPackage.BackgroundScanner and is intended
    ''' to contain all CodeSweep.VSPackage.BackgroundScanner Unit Tests.
    ''' </summary>
    <TestClass()> _
    Public Class BackgroundScannerTest

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

            CodeSweep.VSPackage.Factory_Accessor.GetBuildManager().CreatePerUserFilesAsNecessary()
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
            ' If the test left the scanner running, stop it now.
            If _scanner IsNot Nothing Then
                _scanner.StopIfRunning(True)
                _scanner = Nothing
            End If

            Utilities.CleanUpTempFiles()
            Utilities.RemoveCommandHandlers(_serviceProvider)
        End Sub
        '
#End Region

        Private _scanner As CodeSweep.VSPackage.BackgroundScanner_Accessor

        Private Function GetScanner() As CodeSweep.VSPackage.BackgroundScanner_Accessor
            _scanner = New CodeSweep.VSPackage.BackgroundScanner_Accessor(_serviceProvider)
            Return _scanner
        End Function

        <DeploymentItem("VsPackage.dll"), TestMethod(), ExpectedException(GetType(ArgumentNullException))> _
        Public Sub CreateWithNullArg()
            Dim backgroundScanner As CodeSweep.VSPackage.BackgroundScanner_Accessor = New CodeSweep.VSPackage.BackgroundScanner_Accessor(serviceProvider:=Nothing)
        End Sub

        <DeploymentItem("VsPackage.dll"), TestMethod(), ExpectedException(GetType(ArgumentNullException))> _
        Public Sub StartWithNullArg()
            Dim accessor As CodeSweep.VSPackage.BackgroundScanner_Accessor = GetScanner()
            accessor.Start(Nothing)
        End Sub

        Private Function CreateMinimalTermTableFile() As String
            Return Utilities.CreateTermTable(New String() {"foo"})
        End Function

        ' TODO: This test is subject to timing issues where the build finishes before checking
        ' the properties of the object under test. Need to fix this issue to enable this unit test.
        <Ignore(), DeploymentItem("VsPackage.dll"), TestMethod()> _
        Public Sub StartWhenAlreadyRunning()
            Dim accessor As CodeSweep.VSPackage.BackgroundScanner_Accessor = GetScanner()

            Dim project As Project = Utilities.SetupMSBuildProject(New String() {Utilities.CreateBigFile()}, New String() {CreateMinimalTermTableFile()})
            Dim vsProject As MockIVsProject = Utilities.RegisterProjectWithMocks(project, _serviceProvider)

            Dim thrown As Boolean = False

            accessor.Start(New IVsProject() {vsProject})

            Try
                accessor.Start(New IVsProject() {New MockIVsProject(project.FullPath)})
            Catch ex As TargetInvocationException
                If TypeOf ex.InnerException Is InvalidOperationException Then
                    thrown = True
                End If
            End Try

            Utilities.WaitForStop(accessor)

            Assert.IsTrue(thrown, "Start did not throw InvalidOperationException with scan already running.")
        End Sub

        <DeploymentItem("VsPackage.dll"), TestMethod()> _
        Public Sub StartOnProjectWithoutScannerTask()
            Dim accessor As CodeSweep.VSPackage.BackgroundScanner_Accessor = GetScanner()

            Dim project As Project = Utilities.SetupMSBuildProject()
            Dim vsProject As MockIVsProject = Utilities.RegisterProjectWithMocks(project, _serviceProvider)

            ' TODO: if we could, it would be good to make sure the Started event isn't fired --
            ' but apparently the VS test framework doesn't support events.

            accessor.Start(New IVsProject() {vsProject})

            Assert.IsFalse(accessor.IsRunning, "IsRunning returned true after Start() called with project that does not contain a scanner config.")
        End Sub
        Private NotInheritable Class AnonymousClass1
            Public activeProvider As Guid = Guid.Empty
            Public cmdPosted As Boolean = False

            Public Sub AnonymousMethod1(ByVal sender As Object, ByVal args As MockTaskList.SetActiveProviderArgs)
                activeProvider = args.ProviderGuid
            End Sub

            Public Sub AnonymousMethod2(ByVal sender As Object, ByVal args As MockShell.PostExecCommandArgs)
                If args.Group = VSConstants.GUID_VSStandardCommandSet97 AndAlso args.ID = CUInt(VSConstants.VSStd97CmdID.TaskListWindow) Then
                    cmdPosted = True
                End If
            End Sub
        End Class
        <DeploymentItem("VsPackage.dll"), TestMethod()> _
        Public Sub StartUpdatesTaskList()
            Dim locals As New AnonymousClass1()
            Dim accessor As CodeSweep.VSPackage.BackgroundScanner_Accessor = GetScanner()

            ' Set up events so we know when the task list is called.

            Dim taskList As MockTaskList = TryCast(_serviceProvider.GetService(GetType(SVsTaskList)), MockTaskList)
            AddHandler taskList.OnSetActiveProvider, AddressOf locals.AnonymousMethod1


            Dim shell As MockShell = TryCast(_serviceProvider.GetService(GetType(SVsUIShell)), MockShell)
            AddHandler shell.OnPostExecCommand, AddressOf locals.AnonymousMethod2


            Dim project As Project = Utilities.SetupMSBuildProject(New String() {Utilities.CreateBigFile()}, New String() {CreateMinimalTermTableFile()})
            Dim vsProject As MockIVsProject = Utilities.RegisterProjectWithMocks(project, _serviceProvider)

            accessor.Start(New IVsProject() {vsProject})

            Assert.IsTrue(locals.cmdPosted, "Start did not activate the task list.")
            Assert.AreEqual(New Guid("{9ACC41B7-15B4-4dd7-A0F3-0A935D5647F3}"), locals.activeProvider, "Start did not select the correct task bucket.")

            Utilities.WaitForStop(accessor)
        End Sub
        Private NotInheritable Class AnonymousClass2

            Public inProgress As Boolean = False
            Public label As String = Nothing
            Public complete As UInteger = 0
            Public total As UInteger = 0

            Public Sub AnonymousMethod(ByVal sender As Object, ByVal args As MockStatusBar.ProgressArgs)
                If (args.InProgress = 0) Then
                    inProgress = False
                Else
                    inProgress = True
                End If
                label = args.Label
                complete = args.Complete
                total = args.Total
            End Sub

        End Class
        ' TODO: This test is subject to timing issues where the build finishes before checking
        ' the properties of the object under test. Need to fix this issue to enable this unit test.
        <Ignore(), DeploymentItem("VsPackage.dll"), TestMethod()> _
        Public Sub ProgressShowsInStatusBar()
            Dim accessor As CodeSweep.VSPackage.BackgroundScanner_Accessor = GetScanner()
            Dim locals As New AnonymousClass2()

            Dim statusBar As MockStatusBar = TryCast(_serviceProvider.GetService(GetType(SVsStatusbar)), MockStatusBar)
            AddHandler statusBar.OnProgress, AddressOf locals.AnonymousMethod


            Dim project As Project = Utilities.SetupMSBuildProject(New String() {Utilities.CreateBigFile()}, New String() {CreateMinimalTermTableFile()})
            Dim vsProject As MockIVsProject = Utilities.RegisterProjectWithMocks(project, _serviceProvider)

            accessor.Start(New IVsProject() {vsProject})

            Assert.IsTrue(locals.inProgress, "Start did not show progress in status bar.")
            Assert.AreEqual(CUInt(0), locals.complete, "Scan did not start with zero complete.")

            Utilities.WaitForStop(accessor)

            Assert.IsFalse(locals.inProgress, "Progress bar not cleared when scan ends.")
        End Sub
        Private NotInheritable Class AnonymousClass3
            Public resultCounts As New List(Of Integer)()

            Public Sub AnonymousMethod(ByVal sender As Object, ByVal args As MockTaskList.RefreshTasksArgs)
                resultCounts.Add(Utilities.TasksOfProvider(args.Provider).Count)
            End Sub
        End Class
        <DeploymentItem("VsPackage.dll"), TestMethod()> _
        Public Sub TaskListIsUpdated()
            Dim accessor As CodeSweep.VSPackage.BackgroundScanner_Accessor = GetScanner()
            Dim locals As New AnonymousClass3()

            Dim taskList As MockTaskList = TryCast(_serviceProvider.GetService(GetType(SVsTaskList)), MockTaskList)
            AddHandler taskList.OnRefreshTasks, AddressOf locals.AnonymousMethod

            Dim firstFile As String = Utilities.CreateTempTxtFile("foo abc foo def foo")
            Dim secondFile As String = Utilities.CreateTempTxtFile("bar bar bar floop doop bar")
            Dim termTable1 As String = Utilities.CreateTermTable(New String() {"foo", "bar"})
            Dim termTable2 As String = Utilities.CreateTermTable(New String() {"floop"})
            Dim project1 As Project = Utilities.SetupMSBuildProject(New String() {firstFile, secondFile}, New String() {termTable1, termTable2})
            Dim vsProject1 As MockIVsProject = Utilities.RegisterProjectWithMocks(project1, _serviceProvider)

            Dim thirdFile As String = Utilities.CreateTempTxtFile("blarg")
            Dim termTable3 As String = Utilities.CreateTermTable(New String() {"blarg"})
            Dim project2 As Project = Utilities.SetupMSBuildProject(New String() {thirdFile}, New String() {termTable3})
            Dim vsProject2 As MockIVsProject = Utilities.RegisterProjectWithMocks(project2, _serviceProvider)

            accessor.Start(New IVsProject() {vsProject1, vsProject2})

            Utilities.WaitForStop(accessor)

            Assert.AreEqual(4, locals.resultCounts.Count, "Task list did not recieve correct number of updates.")
            Assert.AreEqual(0, locals.resultCounts(0), "Number of hits in first update is wrong.")
            Assert.AreEqual(3, locals.resultCounts(1), "Number of hits in second update is wrong.")
            Assert.AreEqual(3 + 5, locals.resultCounts(2), "Number of hits in third update is wrong.")
            Assert.AreEqual(3 + 5 + 1, locals.resultCounts(3), "Number of hits in fourth update is wrong.")
        End Sub
        Private NotInheritable Class AnonymousClass4
            Public resultCounts As New List(Of Integer)()

            Public Sub AnonymousMethod(ByVal sender As Object, ByVal args As MockTaskList.RefreshTasksArgs)
                resultCounts.Add(Utilities.TasksOfProvider(args.Provider).Count)
            End Sub
        End Class
        <DeploymentItem("VsPackage.dll"), TestMethod()> _
        Public Sub RepeatLastProducesSameResultsAsPreviousScan()
            Dim accessor As CodeSweep.VSPackage.BackgroundScanner_Accessor = GetScanner()
            Dim locals As New AnonymousClass4()
            Dim firstFile As String = Utilities.CreateTempTxtFile("foo abc foo def foo")
            Dim secondFile As String = Utilities.CreateTempTxtFile("bar bar bar floop doop bar")
            Dim termTable1 As String = Utilities.CreateTermTable(New String() {"foo", "bar"})
            Dim termTable2 As String = Utilities.CreateTermTable(New String() {"floop"})
            Dim project1 As Project = Utilities.SetupMSBuildProject(New String() {firstFile, secondFile}, New String() {termTable1, termTable2})
            Dim vsProject1 As MockIVsProject = Utilities.RegisterProjectWithMocks(project1, _serviceProvider)

            Dim thirdFile As String = Utilities.CreateTempTxtFile("blarg")
            Dim termTable3 As String = Utilities.CreateTermTable(New String() {"blarg"})
            Dim project2 As Project = Utilities.SetupMSBuildProject(New String() {thirdFile}, New String() {termTable3})
            Dim vsProject2 As MockIVsProject = Utilities.RegisterProjectWithMocks(project2, _serviceProvider)

            accessor.Start(New IVsProject() {vsProject1, vsProject2})

            Utilities.WaitForStop(accessor)


            Dim taskList As MockTaskList = TryCast(_serviceProvider.GetService(GetType(SVsTaskList)), MockTaskList)
            AddHandler taskList.OnRefreshTasks, AddressOf locals.AnonymousMethod


            accessor.RepeatLast()

            Utilities.WaitForStop(accessor)

            Assert.AreEqual(4, locals.resultCounts.Count, "Task list did not recieve correct number of updates.")
            Assert.AreEqual(0, locals.resultCounts(0), "Number of hits in first update is wrong.")
            Assert.AreEqual(3, locals.resultCounts(1), "Number of hits in second update is wrong.")
            Assert.AreEqual(3 + 5, locals.resultCounts(2), "Number of hits in third update is wrong.")
            Assert.AreEqual(3 + 5 + 1, locals.resultCounts(3), "Number of hits in fourth update is wrong.")
        End Sub

        <DeploymentItem("VsPackage.dll"), TestMethod(), ExpectedException(GetType(InvalidOperationException))> _
        Public Sub RepeatLastWithNoPreviousScan()
            Dim accessor As CodeSweep.VSPackage.BackgroundScanner_Accessor = GetScanner()
            accessor.RepeatLast()
        End Sub
        Friend NotInheritable Class AnonymousClass
            Public s As BackgroundScannerTest
            Public accessor As CodeSweep.VSPackage.BackgroundScanner_Accessor = s.GetScanner()
            Public Sub AnonymousMethod()
                accessor.RepeatLast()
            End Sub
        End Class

        ' TODO: This test is subject to timing issues where the build finishes before checking
        ' the properties of the object under test. Need to fix this issue to enable this unit test.
        <Ignore(), DeploymentItem("VsPackage.dll"), TestMethod()> _
        Public Sub RepeatLastWhenAlreadyRunning()
            Dim locals As New AnonymousClass()

            Dim project As Project = Utilities.SetupMSBuildProject(New String() {Utilities.CreateBigFile()}, New String() {CreateMinimalTermTableFile()})
            Dim vsProject As MockIVsProject = Utilities.RegisterProjectWithMocks(project, _serviceProvider)

            locals.accessor.Start(New IVsProject() {vsProject})
            Dim thrown As Boolean = Utilities.HasFunctionThrown(Of InvalidOperationException)(AddressOf locals.AnonymousMethod)
            Assert.IsTrue(thrown, "RepeatLast did not throw InvalidOperationException with scan already running.")
        End Sub

        Private NotInheritable Class AnonymousClass5
            Public refreshes As Integer = 0

            Public Sub AnonymousMethod(ByVal sender As Object, ByVal args As MockTaskList.RefreshTasksArgs)
                refreshes += 1
            End Sub
        End Class
        ' TODO: This test is subject to timing issues where the build finishes before checking
        ' the properties of the object under test. Need to fix this issue to enable this unit test.
        <Ignore(), DeploymentItem("VsPackage.dll"), TestMethod()> _
        Public Sub StopStopsScanBeforeNextFile()
            Dim accessor As CodeSweep.VSPackage.BackgroundScanner_Accessor = GetScanner()
            Dim locals As New AnonymousClass5()

            Dim taskList As MockTaskList = TryCast(_serviceProvider.GetService(GetType(SVsTaskList)), MockTaskList)
            AddHandler taskList.OnRefreshTasks, AddressOf locals.AnonymousMethod

            Dim firstFile As String = Utilities.CreateBigFile()
            Dim secondFile As String = Utilities.CreateTempTxtFile("bar bar bar floop doop bar")
            Dim termTable As String = Utilities.CreateTermTable(New String() {"foo", "bar"})
            Dim project As Project = Utilities.SetupMSBuildProject(New String() {firstFile, secondFile}, New String() {termTable})
            Dim vsProject As MockIVsProject = Utilities.RegisterProjectWithMocks(project, _serviceProvider)

            accessor.Start(New IVsProject() {vsProject})
            accessor.StopIfRunning(True)

            ' There should be one update, when the task list was initially cleared.
            Assert.AreEqual(1, locals.refreshes, "Stop did not stop scan before next file.")
        End Sub
        Private NotInheritable Class AnonymousClass6
            Public refreshes As Integer = 0

            Public Sub AnonymousMethod(ByVal sender As Object, ByVal args As MockTaskList.RefreshTasksArgs)
                refreshes += 1
            End Sub
        End Class
        ' TODO: This test is subject to timing issues where the build finishes before checking
        ' the properties of the object under test. Need to fix this issue to enable this unit test.
        <DeploymentItem("VsPackage.dll"), TestMethod(), Ignore()> _
        Public Sub IsRunning()
            Dim accessor As CodeSweep.VSPackage.BackgroundScanner_Accessor = GetScanner()
            Dim locals As New AnonymousClass6()
            Dim taskList As MockTaskList = TryCast(_serviceProvider.GetService(GetType(SVsTaskList)), MockTaskList)
            AddHandler taskList.OnRefreshTasks, AddressOf locals.AnonymousMethod
            Dim firstFile As String = Utilities.CreateBigFile()
            Dim secondFile As String = Utilities.CreateTempTxtFile("bar bar bar floop doop bar")
            Dim termTable As String = Utilities.CreateTermTable(New String() {"foo", "bar"})
            Dim project As Project = Utilities.SetupMSBuildProject(New String() {firstFile, secondFile}, New String() {termTable})
            Dim vsProject As MockIVsProject = Utilities.RegisterProjectWithMocks(project, _serviceProvider)

            accessor.Start(New IVsProject() {vsProject})

            Assert.IsTrue(accessor.IsRunning, "IsRunning was not true after Start.")

            accessor.StopIfRunning(False)

            Assert.IsTrue(accessor.IsRunning, "IsRunning was not true after Stop called while scan is still running.")
        End Sub

    End Class


End Namespace
