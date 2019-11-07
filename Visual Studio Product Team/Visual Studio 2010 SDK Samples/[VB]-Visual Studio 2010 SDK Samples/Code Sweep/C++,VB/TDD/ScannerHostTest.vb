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

Namespace Microsoft.Samples.VisualStudio.CodeSweep.UnitTests
    ''' <summary>
    ''' This is a test class for CodeSweep.VSPackage.ScannerHost and is intended
    ''' to contain all CodeSweep.VSPackage.ScannerHost Unit Tests.
    ''' </summary>
    <TestClass()> _
    Public Class ScannerHostTest


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


        Private NotInheritable Class AnonymousClass8

            Public resultCounts As New List(Of Integer)()
            Public Sub AnonymousMethod(ByVal sender As Object, ByVal args As MockTaskList.RefreshTasksArgs)
                resultCounts.Add(Utilities.TasksOfProvider(args.Provider).Count)
            End Sub

        End Class

        ''' <summary>
        ''' A test for AddResult (IScanResult, string).
        ''' </summary>
        <DeploymentItem("VsPackage.dll"), TestMethod()> _
        Public Sub AddResultTest()
            Dim locals As New AnonymousClass8()
            Dim accessor As New CodeSweep.VSPackage.ScannerHost_Accessor(_serviceProvider)

            Dim table As New MockTermTable("scannedFile")
            Dim term As New MockTerm("term text", 0, "term class", "term comment", "recommended", table)
            Dim hit As New MockScanHit("scannedFile", 5, 6, "line text", term, Nothing)
            Dim scanResult As New MockScanResult("scannedFile", New IScanHit() {hit}, True)


            Dim taskList As MockTaskList = TryCast(_serviceProvider.GetService(GetType(SVsTaskList)), MockTaskList)
            AddHandler taskList.OnRefreshTasks, AddressOf locals.AnonymousMethod


            accessor.AddResult(scanResult, "c:\projFile")

            Assert.AreEqual(1, locals.resultCounts.Count, "Task list was not updated by AddResult.")
            Assert.AreEqual(1, locals.resultCounts(0), "Refresh did not enumerate one task.")
        End Sub

    End Class


End Namespace
