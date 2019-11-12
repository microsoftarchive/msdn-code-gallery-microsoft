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
Imports Microsoft.Samples.VisualStudio.CodeSweep.BuildTask
Imports Microsoft.Build.Framework
Imports Microsoft.Build.Construction
Imports Microsoft.Build.Evaluation

Namespace Microsoft.Samples.VisualStudio.CodeSweep.UnitTests
    ''' <summary>
    ''' This is a test class for CodeSweep.BuildTask.ScannerTask and is intended
    ''' to contain all CodeSweep.BuildTask.ScannerTask Unit Tests.
    ''' </summary>
    <TestClass()> _
    Public Class ScannerTaskTest


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
        End Sub
        '
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


        Private Function CreateTermTableXml(ByVal term As String, ByVal severity As Integer, ByVal termClass As String, ByVal comment As String) As String
            Dim fileContent As New StringBuilder()

            fileContent.Append("<?xml version=""1.0""?>" & Constants.vbLf)
            fileContent.Append("<xmldata>" & Constants.vbLf)
            fileContent.Append("  <PLCKTT>" & Constants.vbLf)
            fileContent.Append("    <Lang>" & Constants.vbLf)
            fileContent.Append("      <Term Term=""" & term & """ Severity=""" & severity.ToString() & """ TermClass=""" & termClass & """>" & Constants.vbLf)
            fileContent.Append("        <Comment>" & comment & "</Comment>" & Constants.vbLf)
            fileContent.Append("      </Term>" & Constants.vbLf)
            fileContent.Append("    </Lang>" & Constants.vbLf)
            fileContent.Append("  </PLCKTT>" & Constants.vbLf)
            fileContent.Append("</xmldata>" & Constants.vbLf)

            Return fileContent.ToString()
        End Function
        Private NotInheritable Class AnonymousClass9
            Public errors As Integer = 0
            Public warnings As Integer = 0
            Public messages As Integer = 0
            Public Sub AnonymousMethod1(ByVal sender As Object, ByVal args As BuildErrorEventArgs)
                errors += 1
            End Sub
            Public Sub AnonymousMethod2(ByVal sender As Object, ByVal args As BuildWarningEventArgs)
                warnings += 1
            End Sub
            Public Sub AnonymousMethod3(ByVal sender As Object, ByVal args As BuildMessageEventArgs)
                messages += 1
            End Sub
        End Class

        ''' <summary>
        ''' A test case for Execute ().
        ''' </summary>
        <DeploymentItem("BuildTask.dll"), TestMethod()> _
        Public Sub ExecuteWithoutHost()
            Dim target As New ScannerTask()
            Dim locals As New AnonymousClass9()

            ' Create the term tables and target files.
            Dim termFile1 As String = Utilities.CreateTempFile(CreateTermTableXml("countries", 2, "Geopolitical", "comment"))
            Dim termFile2 As String = Utilities.CreateTempFile(CreateTermTableXml("shoot", 3, "Profanity", "comment"))

            Dim scanFile1 As String = Utilities.CreateTempTxtFile("the word 'countries' should produce a hit")
            Dim scanFile2 As String = Utilities.CreateTempTxtFile("the word 'shoot' should produce a hit")

            ' Create the project that will execute the task.
            Dim project As Microsoft.Build.Evaluation.Project = Utilities.SetupMSBuildProject(New String() {scanFile1, scanFile2}, New String() {termFile1, termFile2})

            ' Set up a custom logger to capture the output.
            Dim logger As New MockLogger()
            project.ProjectCollection.RegisterLogger(logger)

            AddHandler logger.OnError, AddressOf locals.AnonymousMethod1

            AddHandler logger.OnWarning, AddressOf locals.AnonymousMethod2

            AddHandler logger.OnMessage, AddressOf locals.AnonymousMethod3


            project.Build("AfterBuild")

            Assert.AreEqual(0, locals.errors, "Build did not log expected number of errors.")
            Assert.AreEqual(2, locals.warnings, "Build did not log expected number of warnings.")
            Assert.AreEqual(2, locals.messages, "Build did not log expected number of messages.")
        End Sub


        Private NotInheritable Class AnonymousClass10
            Public termFile1 As String
            Public termFile2 As String
            Public scanFile1 As String
            Public scanFile2 As String
            ' Create the project that will execute the task.
            Public project As Microsoft.Build.Evaluation.Project = Utilities.SetupMSBuildProject(New String() {scanFile1, scanFile2}, New String() {termFile1, termFile2})
            Public errors As Integer = 0
            Public warnings As Integer = 0
            Public messages As Integer = 0

            ' Set the host object for the task.

            Public hostUpdates As Integer = 0
            Public host As New MockHostObject()


            Public Sub AnonymousMethod1(ByVal sender As Object, ByVal args As BuildErrorEventArgs)
                errors += 1
            End Sub
            Public Sub AnonymousMethod2(ByVal sender As Object, ByVal args As BuildWarningEventArgs)
                warnings += 1
            End Sub
            Public Sub AnonymousMethod3(ByVal sender As Object, ByVal args As BuildMessageEventArgs)
                messages += 1
            End Sub
            Public Sub AnonymousMethod4(ByVal sender As Object, ByVal args As MockHostObject.AddResultArgs)
                hostUpdates += 1
            End Sub
            Public Sub AnonymousMethod5(ByVal sender As Object, ByVal args As BuildStartedEventArgs)
                ProjectCollection.GlobalProjectCollection.HostServices.RegisterHostObject(project.FullPath, "AfterBuild", "ScannerTask", host)
            End Sub

        End Class
        ' Note: it would be nice to test task execution with the host object set, but I can't get it to "stick" -- the host is always back to null when the task is executed.

        ' [DeploymentItem("BuildTask.dll")]
        ' [TestMethod()]
        Public Sub ExecuteWithHost()
            Dim target As New ScannerTask()
            Dim locals As New AnonymousClass10()

            ' Create the term tables and target files.
            locals.termFile1 = Utilities.CreateTempFile(CreateTermTableXml("countries", 2, "Geopolitical", "comment"))
            locals.termFile2 = Utilities.CreateTempFile(CreateTermTableXml("shoot", 3, "Profanity", "comment"))

            locals.scanFile1 = Utilities.CreateTempTxtFile("the word 'countries' should produce a hit")
            locals.scanFile2 = Utilities.CreateTempTxtFile("the word 'shoot' should produce a hit")


            ' Set up a custom logger to capture the output.
            Dim logger As New MockLogger()
            locals.project.ProjectCollection.RegisterLogger(logger)

            AddHandler logger.OnError, AddressOf locals.AnonymousMethod1

            AddHandler logger.OnWarning, AddressOf locals.AnonymousMethod2

            AddHandler logger.OnMessage, AddressOf locals.AnonymousMethod3


            Dim task As ProjectTaskElement = Utilities.GetScannerTask(locals.project)

            AddHandler locals.host.OnAddResult, AddressOf locals.AnonymousMethod4

            AddHandler logger.OnBuildStart, AddressOf locals.AnonymousMethod5


            locals.project.Build("AfterBuild")

            Assert.AreEqual(0, locals.errors, "Build did not log expected number of errors.")
            Assert.AreEqual(0, locals.warnings, "Build did not log expected number of warnings.")
            Assert.AreEqual(3, locals.messages, "Build did not log expected number of messages.")
            Assert.AreEqual(2, locals.hostUpdates, "Build did not send expected number of results to host.")
        End Sub

        ''' <summary>
        ''' A test case for TermTables.
        ''' </summary>
        <DeploymentItem("BuildTask.dll"), TestMethod()> _
        Public Sub TermTablesTest()
            Dim target As New ScannerTask()

            SetAndGetTermTables(target, Nothing)
            SetAndGetTermTables(target, "")
            SetAndGetTermTables(target, "z:" & Constants.vbFormFeed & "ileName.ext")
            SetAndGetTermTables(target, "first;second;third")
        End Sub

        Private Sub SetAndGetTermTables(ByVal target As ScannerTask, ByVal val As String)
            target.TermTables = val
            Assert.AreEqual(val, target.TermTables, "TermTables did not return expected value (" & val & ").")
        End Sub

        Private Class MockTaskItem
            Implements ITaskItem
            Private ReadOnly _file As String

            Public Sub New(ByVal file As String)
                _file = file
            End Sub

#Region "ITaskItem Members"

            Public Function CloneCustomMetadata() As System.Collections.IDictionary Implements ITaskItem.CloneCustomMetadata
                Throw New System.Exception("The method or operation is not implemented.")
            End Function

            Public Sub CopyMetadataTo(ByVal destinationItem As ITaskItem) Implements ITaskItem.CopyMetadataTo
                Throw New System.Exception("The method or operation is not implemented.")
            End Sub

            Public Function GetMetadata(ByVal metadataName As String) As String Implements ITaskItem.GetMetadata
                Throw New System.Exception("The method or operation is not implemented.")
            End Function

            Public Property ItemSpec() As String Implements ITaskItem.ItemSpec
                Get
                    Return _file
                End Get
                Set(ByVal value As String)
                    Throw New System.Exception("The method or operation is not implemented.")
                End Set
            End Property

            Public ReadOnly Property MetadataCount() As Integer Implements ITaskItem.MetadataCount
                Get
                    Throw New System.Exception("The method or operation is not implemented.")
                End Get
            End Property

            Public ReadOnly Property MetadataNames() As System.Collections.ICollection Implements ITaskItem.MetadataNames
                Get
                    Throw New System.Exception("The method or operation is not implemented.")
                End Get
            End Property

            Public Sub RemoveMetadata(ByVal metadataName As String) Implements ITaskItem.RemoveMetadata
                Throw New System.Exception("The method or operation is not implemented.")
            End Sub

            Public Sub SetMetadata(ByVal metadataName As String, ByVal metadataValue As String) Implements ITaskItem.SetMetadata
                Throw New System.Exception("The method or operation is not implemented.")
            End Sub

#End Region
        End Class

        ''' <summary>
        ''' A test case for FilesToScan.
        ''' </summary>
        <DeploymentItem("BuildTask.dll"), TestMethod()> _
        Public Sub FilesToScanTest()
            Dim target As New ScannerTask()

            SetAndGetFilesToScan(target, Nothing)
            SetAndGetFilesToScan(target, New ITaskItem() {})
            SetAndGetFilesToScan(target, New ITaskItem() {New MockTaskItem("foo"), New MockTaskItem("bar")})
        End Sub

        Private Sub SetAndGetFilesToScan(ByVal target As ScannerTask, ByVal val As ITaskItem())
            target.FilesToScan = val
            If val Is Nothing Then
                Assert.IsNull(target.FilesToScan, "FilesToScan did not return expected value (null).")
            Else
                Assert.AreEqual(val.Length, target.FilesToScan.Length, "FilesToScan returned array of wrong length.")
                For i As Integer = 0 To val.Length - 1
                    Assert.AreEqual(val(i), target.FilesToScan(i), "Item " & i.ToString() & " of FilesToScan is incorrect.")
                Next i
            End If
        End Sub

    End Class


End Namespace
