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
Imports System.Reflection

Namespace Microsoft.Samples.VisualStudio.CodeSweep.UnitTests
    ''' <summary>
    ''' This is a test class for CodeSweep.VSPackage.IgnoreInstance and is intended
    ''' to contain all CodeSweep.VSPackage.IgnoreInstance Unit Tests.
    ''' </summary>
    <TestClass()> _
    Public Class IgnoreInstanceTest


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

        Public Sub AnonymousMethod1()
            Dim ignoreInstance As New CodeSweep.BuildTask.IgnoreInstance_Accessor(Nothing, "line text", "text", 0)
        End Sub
        Public Sub AnonymousMethod2()
            Dim ignoreInstance As New CodeSweep.BuildTask.IgnoreInstance_Accessor("c:\file.ext", Nothing, "text", 0)
        End Sub
        Public Sub AnonymousMethod3()
            Dim ignoreInstance As New CodeSweep.BuildTask.IgnoreInstance_Accessor("c:\file.ext", "line text", Nothing, 0)
        End Sub
        Public Sub AnonymousMethod4()
            Dim ignoreInstance As New CodeSweep.BuildTask.IgnoreInstance_Accessor("", "line text", "text", 0)
        End Sub
        Public Sub AnonymousMethod5()
            Dim ignoreInstance As New CodeSweep.BuildTask.IgnoreInstance_Accessor("c:\file.ext", "", "text", 0)
        End Sub
        Public Sub AnonymousMethod6()
            Dim ignoreInstance As New CodeSweep.BuildTask.IgnoreInstance_Accessor("c:\file.ext", "line text", "", 0)
        End Sub
        Public Sub AnonymousMethod7()
            Dim ignoreInstance As New CodeSweep.BuildTask.IgnoreInstance_Accessor("c:\file.ext", "line text", "text", -1)
        End Sub
        Public Sub AnonymousMethod8()
            Dim ignoreInstance As New CodeSweep.BuildTask.IgnoreInstance_Accessor("c:\file.ext", "line text", "text", 9)
        End Sub
        <DeploymentItem("BuildTask.dll"), TestMethod()> _
        Public Sub ConstructWithInvalidArgs()
            Dim [function] As Action = AddressOf AnonymousMethod1
            Assert.IsTrue(Utilities.HasFunctionThrown(Of ArgumentNullException)([function]), "Constructor did not throw ArgumentNullException for null file name.")
            [function] = AddressOf AnonymousMethod2
            Assert.IsTrue(Utilities.HasFunctionThrown(Of ArgumentNullException)([function]), "Constructor did not throw ArgumentNullException for null line text.")
            [function] = AddressOf AnonymousMethod3
            Assert.IsTrue(Utilities.HasFunctionThrown(Of ArgumentNullException)([function]), "Constructor did not throw ArgumentNullException for null term.")
            [function] = AddressOf AnonymousMethod4
            Assert.IsTrue(Utilities.HasFunctionThrown(Of ArgumentException)([function]), "Constructor did not throw ArgumentException for empty file name.")
            [function] = AddressOf AnonymousMethod5
            Assert.IsTrue(Utilities.HasFunctionThrown(Of ArgumentException)([function]), "Constructor did not throw ArgumentException for empty line text.")
            [function] = AddressOf AnonymousMethod6
            Assert.IsTrue(Utilities.HasFunctionThrown(Of ArgumentException)([function]), "Constructor did not throw ArgumentException for empty term.")
            [function] = AddressOf AnonymousMethod7
            Assert.IsTrue(Utilities.HasFunctionThrown(Of ArgumentException)([function]), "Constructor did not throw ArgumentException for negative column.")
            [function] = AddressOf AnonymousMethod8
            Assert.IsTrue(Utilities.HasFunctionThrown(Of ArgumentException)([function]), "Constructor did not throw ArgumentException for column too high.")
        End Sub

        <DeploymentItem("BuildTask.dll"), TestMethod()> _
        Public Sub ConstructAndTestProperties()
            Dim accessor As New CodeSweep.BuildTask.IgnoreInstance_Accessor("c:\file.ext", "line text", "text", 5)

            Assert.AreEqual("c:\file.ext", accessor.FilePath, "FilePath property incorrect.")
            Assert.AreEqual("line text", accessor.IgnoredLine, "IgnoredLine property incorrect.")
            Assert.AreEqual("text", accessor.IgnoredTerm, "IgnoredTerm property incorrect.")
            Assert.AreEqual(5, accessor.PositionOfIgnoredTerm, "PositionOfIgnoredTerm property incorrect.")
        End Sub
        Public Sub AnonymousMethod9()
            Dim ignoreInstance As New CodeSweep.BuildTask.IgnoreInstance_Accessor(Nothing, "c:\")
        End Sub
        Public Sub AnonymousMethod10()
            Dim ignoreInstance As New CodeSweep.BuildTask.IgnoreInstance_Accessor("filename,term,0,linetext", Nothing)
        End Sub
        Public Sub AnonymousMethod11()
            Dim ignoreInstance As New CodeSweep.BuildTask.IgnoreInstance_Accessor("filename,term,0,linetext", "")
        End Sub
        Public Sub AnonymousMethod12()
            Dim ignoreInstance As New CodeSweep.BuildTask.IgnoreInstance_Accessor("filename,term,0,linetext", "..\")
        End Sub
        Public Sub AnonymousMethod13()
            Dim ignoreInstance As New CodeSweep.BuildTask.IgnoreInstance_Accessor("filename,term,0,linetext,foo", "c:\")
        End Sub
        Public Sub AnonymousMethod14()
            Dim ignoreInstance As New CodeSweep.BuildTask.IgnoreInstance_Accessor("filename,term,0", "c:\")
        End Sub
        Public Sub AnonymousMethod15()
            Dim ignoreInstance As New CodeSweep.BuildTask.IgnoreInstance_Accessor(",term,0,linetext", "c:\")
        End Sub
        Public Sub AnonymousMethod16()
            Dim ignoreInstance As New CodeSweep.BuildTask.IgnoreInstance_Accessor("filename,,0,linetext", "c:\")
        End Sub
        Public Sub AnonymousMethod17()
            Dim ignoreInstance As New CodeSweep.BuildTask.IgnoreInstance_Accessor("filename,term,,linetext", "c:\")
        End Sub
        Public Sub AnonymousMethod18()
            Dim ignoreInstance As New CodeSweep.BuildTask.IgnoreInstance_Accessor("filename,term,0,", "c:\")
        End Sub
        Public Sub AnonymousMethod19()
            Dim ignoreInstance As New CodeSweep.BuildTask.IgnoreInstance_Accessor("filename,term,abc,linetext", "c:\")
        End Sub
        Public Sub AnonymousMethod20()
            Dim ignoreInstance As New CodeSweep.BuildTask.IgnoreInstance_Accessor("filename,term,-1,linetext", "c:\")
        End Sub
        Public Sub AnonymousMethod21()
            Dim ignoreInstance As New CodeSweep.BuildTask.IgnoreInstance_Accessor("filename,term,8,linetext", "c:\")
        End Sub
        <DeploymentItem("BuildTask.dll"), TestMethod()> _
        Public Sub DeserializeWithInvalidArgs()
            ' Null serialization text.
            Dim [function] As Action = AddressOf AnonymousMethod9
            Assert.IsTrue(Utilities.HasFunctionThrown(Of ArgumentNullException)([function]), "Constructor did not throw ArgumentNullException for null serialization string.")

            ' Null project path.
            [function] = AddressOf AnonymousMethod10
            Assert.IsTrue(Utilities.HasFunctionThrown(Of ArgumentNullException)([function]), "Constructor did not throw ArgumentNullException for null project path.")

            ' Empty project path.
            [function] = AddressOf AnonymousMethod11
            Assert.IsTrue(Utilities.HasFunctionThrown(Of ArgumentException)([function]), "Constructor did not throw ArgumentException for empty project path.")

            ' Non-rooted project path.
            [function] = AddressOf AnonymousMethod12
            Assert.IsTrue(Utilities.HasFunctionThrown(Of ArgumentException)([function]), "Constructor did not throw ArgumentException for non-rooted project path.")

            ' Too many fields.
            [function] = AddressOf AnonymousMethod13
            Assert.IsTrue(Utilities.HasFunctionThrown(Of ArgumentException)([function]), "Constructor did not throw ArgumentException with too many fields.")

            ' Too few fields.
            [function] = AddressOf AnonymousMethod14
            Assert.IsTrue(Utilities.HasFunctionThrown(Of ArgumentException)([function]), "Constructor did not throw ArgumentException with too few fields.")

            ' File field is empty.
            [function] = AddressOf AnonymousMethod15
            Assert.IsTrue(Utilities.HasFunctionThrown(Of ArgumentException)([function]), "Constructor did not throw ArgumentException with empty file field.")

            ' Term field is empty.
            [function] = AddressOf AnonymousMethod16
            Assert.IsTrue(Utilities.HasFunctionThrown(Of ArgumentException)([function]), "Constructor did not throw ArgumentException with empty term field.")

            ' Column field is empty.
            [function] = AddressOf AnonymousMethod17
            Assert.IsTrue(Utilities.HasFunctionThrown(Of ArgumentException)([function]), "Constructor did not throw ArgumentException with empty column field.")

            ' Line text field is empty.
            [function] = AddressOf AnonymousMethod18
            Assert.IsTrue(Utilities.HasFunctionThrown(Of ArgumentException)([function]), "Constructor did not throw ArgumentException with empty line text field.")

            ' Column field cannot be parsed.
            [function] = AddressOf AnonymousMethod19
            Assert.IsTrue(Utilities.HasFunctionThrown(Of ArgumentException)([function]), "Constructor did not throw ArgumentException with unparsable column field.")

            ' Column field is too low.
            [function] = AddressOf AnonymousMethod20
            Assert.IsTrue(Utilities.HasFunctionThrown(Of ArgumentException)([function]), "Constructor did not throw ArgumentException with negative column field.")

            ' Column field is too high.
            [function] = AddressOf AnonymousMethod21
            Assert.IsTrue(Utilities.HasFunctionThrown(Of ArgumentException)([function]), "Constructor did not throw ArgumentException with column field too high.")
        End Sub

        <DeploymentItem("BuildTask.dll"), TestMethod()> _
        Public Sub DeserializeAndTestProperties()
            Dim accessor As New CodeSweep.BuildTask.IgnoreInstance_Accessor("subdir\\file.ext,term,5,line\,text with commas\,", "c:\")

            Assert.AreEqual("c:\subdir\file.ext", accessor.FilePath, "FilePath property incorrect.")
            Assert.AreEqual("line,text with commas,", accessor.IgnoredLine, "IgnoredLine property incorrect.")
            Assert.AreEqual("term", accessor.IgnoredTerm, "IgnoredTerm property incorrect.")
            Assert.AreEqual(5, accessor.PositionOfIgnoredTerm, "PositionOfIgnoredTerm property incorrect.")
        End Sub

        <DeploymentItem("BuildTask.dll"), TestMethod()> _
        Public Sub Serialize()
            Dim accessor As New CodeSweep.BuildTask.IgnoreInstance_Accessor("c:\subdir\file.ext", "line text, with commas", "text", 5)

            Dim serialization As String = accessor.Serialize("c:\")
            Assert.AreEqual("subdir\\file.ext,text,5,line text\, with commas", serialization, "Serialize did not return expected string.")

            Dim accessor2 As New CodeSweep.BuildTask.IgnoreInstance_Accessor(serialization, "c:\")

            Assert.AreEqual("c:\subdir\file.ext", accessor.FilePath, "FilePath property incorrect.")
            Assert.AreEqual("line text, with commas", accessor.IgnoredLine, "IgnoredLine property incorrect.")
            Assert.AreEqual("text", accessor.IgnoredTerm, "IgnoredTerm property incorrect.")
            Assert.AreEqual(5, accessor.PositionOfIgnoredTerm, "PositionOfIgnoredTerm property incorrect.")
        End Sub

        <DeploymentItem("BuildTask.dll"), TestMethod()> _
        Public Sub Compare()
            Dim accessor As New CodeSweep.BuildTask.IgnoreInstance_Accessor("file", "line", "term", 0)

            Dim accessor2 As New CodeSweep.BuildTask.IgnoreInstance_Accessor("file", "line", "term", 0)

            Assert.AreEqual(0, accessor.CompareTo(TryCast(accessor2.Target, CodeSweep.BuildTask.IIgnoreInstance)), "Comparison to equal instance returned wrong value.")

            Dim accessor3 As New CodeSweep.BuildTask.IgnoreInstance_Accessor("file1", "line", "term", 0)

            Assert.AreNotEqual(0, accessor.CompareTo(TryCast(accessor3.Target, CodeSweep.BuildTask.IIgnoreInstance)), "Comparison with different file returned wrong value.")

            Dim accessor4 As New CodeSweep.BuildTask.IgnoreInstance_Accessor("file", "line1", "term", 0)

            Assert.AreNotEqual(0, accessor.CompareTo(TryCast(accessor4.Target, CodeSweep.BuildTask.IIgnoreInstance)), "Comparison with different line text returned wrong value.")

            Dim accessor5 As New CodeSweep.BuildTask.IgnoreInstance_Accessor("file", "line", "term1", 0)

            Assert.AreNotEqual(0, accessor.CompareTo(TryCast(accessor5.Target, CodeSweep.BuildTask.IIgnoreInstance)), "Comparison with different term returned wrong value.")

            Dim accessor6 As New CodeSweep.BuildTask.IgnoreInstance_Accessor("file", "line", "term", 1)

            Assert.AreNotEqual(0, accessor.CompareTo(TryCast(accessor6.Target, CodeSweep.BuildTask.IIgnoreInstance)), "Comparison with different column returned wrong value.")
        End Sub

    End Class


End Namespace
