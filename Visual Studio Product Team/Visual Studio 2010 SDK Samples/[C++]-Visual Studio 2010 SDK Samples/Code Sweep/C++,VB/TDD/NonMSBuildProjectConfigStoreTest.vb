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

Namespace Microsoft.Samples.VisualStudio.CodeSweep.UnitTests
    ''' <summary>
    ''' This is a test class for CodeSweep.VSPackage.NonMSBuildProjectConfigStore and is intended
    ''' to contain all CodeSweep.VSPackage.NonMSBuildProjectConfigStore Unit Tests.
    ''' </summary>
    <TestClass()> _
    Public Class NonMSBuildProjectConfigStoreTest


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

            Dim dte As MockDTE = TryCast(_serviceProvider.GetService(GetType(DTE)), MockDTE)

            For Each project As EnvDTE.Project In dte.Solution.Projects
                Dim globals As MockDTEGlobals = TryCast(project.Globals, MockDTEGlobals)
                globals.ClearAll()
            Next project
        End Sub
        '
#End Region

        <DeploymentItem("VsPackage.dll"), TestMethod()> _
        Public Sub HasConfiguration()
            Dim solution As MockSolution = TryCast(_serviceProvider.GetService(GetType(SVsSolution)), MockSolution)
            Dim vsProject As New MockIVsProject("c:\temp.proj")
            solution.AddProject(vsProject)

            Dim accessor As New CodeSweep.VSPackage.NonMSBuildProjectConfigStore_Accessor(vsProject, _serviceProvider)

            Assert.IsFalse(accessor.HasConfiguration, "HasConfiguration was true for a new project.")

            accessor.CreateDefaultConfiguration()

            Assert.IsTrue(accessor.HasConfiguration, "HasConfiguration was false after CreateDefaultConfiguration.")
        End Sub

        <DeploymentItem("VsPackage.dll"), TestMethod()> _
        Public Sub TermTables()
            Dim solution As MockSolution = TryCast(_serviceProvider.GetService(GetType(SVsSolution)), MockSolution)
            Dim vsProject As New MockIVsProject("c:\temp.proj")
            solution.AddProject(vsProject)

            Dim accessor As New CodeSweep.VSPackage.NonMSBuildProjectConfigStore_Accessor(vsProject, _serviceProvider)

            Assert.AreEqual(0, accessor.TermTableFiles.Count, "TermTableFiles not initially empty.")

            accessor.CreateDefaultConfiguration()

            Assert.AreEqual(1, accessor.TermTableFiles.Count, "TermTableFiles wrong size after CreateDefaultConfiguration.")

            Dim dte As MockDTE = TryCast(_serviceProvider.GetService(GetType(DTE)), MockDTE)
            Dim globals As MockDTEGlobals = TryCast(dte.Solution.Projects.Item(0).Globals, MockDTEGlobals)

            globals.ClearNonPersistedVariables()

            ' Create a new proj config store to see if the change was persisted.
            Dim accessor2 As New CodeSweep.VSPackage.NonMSBuildProjectConfigStore_Accessor(vsProject, _serviceProvider)

            Assert.AreEqual(1, accessor2.TermTableFiles.Count, "CreateDefaultConfiguration changes did not persist.")

            accessor.TermTableFiles.Remove(Utilities.ListFromEnum(accessor.TermTableFiles)(0))
            globals.ClearNonPersistedVariables()

            ' Create a new proj config store to see if the change was persisted.
            Dim accessor3 As New CodeSweep.VSPackage.NonMSBuildProjectConfigStore_Accessor(vsProject, _serviceProvider)

            Assert.AreEqual(0, accessor3.TermTableFiles.Count, "Deletion did not persist.")

            accessor.TermTableFiles.Add("c:\foo")
            accessor.TermTableFiles.Add("c:\bar")
            globals.ClearNonPersistedVariables()

            ' Create a new proj config store to see if the change was persisted.
            Dim accessor4 As New CodeSweep.VSPackage.NonMSBuildProjectConfigStore_Accessor(vsProject, _serviceProvider)

            Assert.AreEqual(2, accessor4.TermTableFiles.Count, "Additions did not persist.")
        End Sub

        ' Note: the IgnoreInstances property can't be accessed from these tests because the test
        ' framework doesn't support generic types in that capacity.
    End Class


End Namespace
