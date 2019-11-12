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
    ''' This is a test class for CodeSweep.Scanner.Factory and is intended
    ''' to contain all CodeSweep.Scanner.Factory Unit Tests.
    ''' </summary>
    <TestClass()> _
    Public Class Scanner_FactoryTest


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


        ''' <summary>
        ''' A test case for GetScanner ().
        ''' </summary>
        <DeploymentItem("Scanner.dll"), TestMethod()> _
        Public Sub GetScannerTest()
            Assert.IsNotNull(Factory.GetScanner(), "GetScanner returned null.")
        End Sub

        ''' <summary>
        ''' A test case for GetTermTable (string).
        ''' </summary>
        <DeploymentItem("Scanner.dll"), TestMethod()> _
        Public Sub GetTermTableFromEmptyFile()
            Dim filePath As String = Utilities.CreateTempTxtFile("")

            Dim thrown As Boolean = False

            Try
                Factory.GetTermTable(filePath)
            Catch e1 As System.Xml.XmlException
                thrown = True
            End Try

            Assert.IsTrue(thrown, "GetTermTable did not throw XmlException with empty file.")
        End Sub

        <DeploymentItem("Scanner.dll"), TestMethod()> _
        Public Sub GetTermTableWithNullPath()
            Dim thrown As Boolean = False

            Try
                Factory.GetTermTable(Nothing)
            Catch e1 As System.ArgumentNullException
                thrown = True
            End Try

            Assert.IsTrue(thrown, "GetTermTable did not throw ArgumentNullException with null file name.")
        End Sub

        <DeploymentItem("Scanner.dll"), TestMethod()> _
        Public Sub GetTermTableWithInvalidPath()
            Dim thrown As Boolean = False

            Try
                Factory.GetTermTable("z:\somedir\somefile.ext")
            Catch e1 As System.IO.FileNotFoundException
                thrown = True
            Catch e2 As System.IO.DirectoryNotFoundException
                thrown = True
            End Try

            Assert.IsTrue(thrown, "GetTermTable did not throw FileNotFoundException or DirectoryNotFoundException with invalid file name.")
        End Sub

        <DeploymentItem("Scanner.dll"), TestMethod()> _
        Public Sub GetTermTableUsingMinimalSchema()
            Dim fileContent As New StringBuilder()

            fileContent.Append("<?xml version=""1.0""?>" & Microsoft.VisualBasic.Constants.vbLf)
            fileContent.Append("<xmldata>" & Microsoft.VisualBasic.Constants.vbLf)
            fileContent.Append("  <PLCKTT>" & Microsoft.VisualBasic.Constants.vbLf)
            fileContent.Append("    <Lang>" & Microsoft.VisualBasic.Constants.vbLf)
            fileContent.Append("      <Term Term=""countries"" Severity=""2"" TermClass=""Geopolitical"">" & Microsoft.VisualBasic.Constants.vbLf)
            fileContent.Append("        <Comment>For detailed info see - http://relevant-url-here.com</Comment>" & Microsoft.VisualBasic.Constants.vbLf)
            fileContent.Append("      </Term>" & Microsoft.VisualBasic.Constants.vbLf)
            fileContent.Append("    </Lang>" & Microsoft.VisualBasic.Constants.vbLf)
            fileContent.Append("  </PLCKTT>" & Microsoft.VisualBasic.Constants.vbLf)
            fileContent.Append("</xmldata>" & Microsoft.VisualBasic.Constants.vbLf)

            Dim fileName As String = Utilities.CreateTempTxtFile(fileContent.ToString())

            Dim table As ITermTable = Factory.GetTermTable(fileName)

            Assert.AreEqual(fileName, table.SourceFile, "SourceFile property is incorrect.")
            Assert.IsNotNull(table.Terms, "Terms property is null.")

            Dim termList As List(Of ISearchTerm) = Utilities.ListFromEnum(table.Terms)

            Assert.AreEqual(1, termList.Count, "Number of terms is incorrect.")
            Assert.AreEqual("countries", termList(0).Text, "Text property of term 0 is incorrect.")
            Assert.AreEqual("Geopolitical", termList(0).Class, "Class property of term 0 is incorrect.")
            Assert.AreEqual("For detailed info see - http://relevant-url-here.com", termList(0).Comment, "Comment property of term 0 is incorrect.")
            Assert.IsNull(termList(0).RecommendedTerm, "RecommendedTerm property of term 0 is incorrect.")

            Dim term0exclusions As List(Of IExclusion) = Utilities.ListFromEnum(termList(0).Exclusions)

            Assert.AreEqual(0, term0exclusions.Count, "Number of exclusions for term 0 is incorrect.")
        End Sub

        <DeploymentItem("Scanner.dll"), TestMethod()> _
        Public Sub GetTermTableUsingFullSchema()
            Dim fileContent As New StringBuilder()

            fileContent.Append("<?xml version=""1.0""?>" & Microsoft.VisualBasic.Constants.vbLf)
            fileContent.Append("<xmldata>" & Microsoft.VisualBasic.Constants.vbLf)
            fileContent.Append("  <PLCKTT>" & Microsoft.VisualBasic.Constants.vbLf)
            fileContent.Append("    <Lang>" & Microsoft.VisualBasic.Constants.vbLf)
            fileContent.Append("      <Term Term=""countries"" TermID=""5"" Severity=""2"" TermClass=""Geopolitical"" CaseSensitive=""0"" WholeWord=""1"">" & Microsoft.VisualBasic.Constants.vbLf)
            fileContent.Append("        <RecommendedTerm>countries/regions</RecommendedTerm>" & Microsoft.VisualBasic.Constants.vbLf)
            fileContent.Append("        <Comment>For detailed info see - http://relevant-url-here.com</Comment>" & Microsoft.VisualBasic.Constants.vbLf)
            fileContent.Append("        <Exclusion>and for other fareast countries</Exclusion>" & Microsoft.VisualBasic.Constants.vbLf)
            fileContent.Append("        <Exclusion>""countries"",</Exclusion>" & Microsoft.VisualBasic.Constants.vbLf)
            fileContent.Append("        <ExclusionContext>hold true for other countries in the world</ExclusionContext>" & Microsoft.VisualBasic.Constants.vbLf)
            fileContent.Append("        <ExclusionContext>lists the available countries in north america</ExclusionContext>" & Microsoft.VisualBasic.Constants.vbLf)
            fileContent.Append("      </Term>" & Microsoft.VisualBasic.Constants.vbLf)
            fileContent.Append("      <Term Term=""shoot"" TermID=""6"" Severity=""3"" TermClass=""Profanity"" CaseSensitive=""1"" WholeWord=""0"">" & Microsoft.VisualBasic.Constants.vbLf)
            fileContent.Append("        <RecommendedTerm>darn</RecommendedTerm>" & Microsoft.VisualBasic.Constants.vbLf)
            fileContent.Append("        <Comment>Watch that language!</Comment>" & Microsoft.VisualBasic.Constants.vbLf)
            fileContent.Append("        <Exclusion>Did you shoot the sherrif?</Exclusion>" & Microsoft.VisualBasic.Constants.vbLf)
            fileContent.Append("        <Exclusion>I did not shoot the deputy</Exclusion>" & Microsoft.VisualBasic.Constants.vbLf)
            fileContent.Append("        <ExclusionContext>an old-fashioned shoot-em-up</ExclusionContext>" & Microsoft.VisualBasic.Constants.vbLf)
            fileContent.Append("      </Term>" & Microsoft.VisualBasic.Constants.vbLf)
            fileContent.Append("    </Lang>" & Microsoft.VisualBasic.Constants.vbLf)
            fileContent.Append("  </PLCKTT>" & Microsoft.VisualBasic.Constants.vbLf)
            fileContent.Append("</xmldata>" & Microsoft.VisualBasic.Constants.vbLf)

            Dim fileName As String = Utilities.CreateTempTxtFile(fileContent.ToString())

            Dim table As ITermTable = Factory.GetTermTable(fileName)

            Assert.AreEqual(fileName, table.SourceFile, "SourceFile property is incorrect.")
            Assert.IsNotNull(table.Terms, "Terms property is null.")

            Dim termList As List(Of ISearchTerm) = Utilities.ListFromEnum(table.Terms)

            Assert.AreEqual(2, termList.Count, "Number of terms is incorrect.")
            Assert.AreEqual("countries", termList(0).Text, "Text property of term 0 is incorrect.")
            Assert.AreEqual("Geopolitical", termList(0).Class, "Class property of term 0 is incorrect.")
            Assert.AreEqual("For detailed info see - http://relevant-url-here.com", termList(0).Comment, "Comment property of term 0 is incorrect.")
            Assert.AreEqual("countries/regions", termList(0).RecommendedTerm, "RecommendedTerm property of term 0 is incorrect.")

            Dim term0exclusions As List(Of IExclusion) = Utilities.ListFromEnum(termList(0).Exclusions)

            Assert.AreEqual(4, term0exclusions.Count, "Number of exclusions for term 0 is incorrect.")
            Assert.AreEqual("and for other fareast countries", term0exclusions(0).Text, "Exclusion 0 of term 0 is incorrect.")
            Assert.AreEqual("""countries"",", term0exclusions(1).Text, "Exclusion 1 of term 0 is incorrect.")
            Assert.AreEqual("hold true for other countries in the world", term0exclusions(2).Text, "Exclusion 2 of term 0 is incorrect.")
            Assert.AreEqual("lists the available countries in north america", term0exclusions(3).Text, "Exclusion 3 of term 0 is incorrect.")

            Assert.AreEqual("shoot", termList(1).Text, "Text property of term 1 is incorrect.")
            Assert.AreEqual("Profanity", termList(1).Class, "Class property of term 1 is incorrect.")
            Assert.AreEqual("Watch that language!", termList(1).Comment, "Comment property of term 1 is incorrect.")
            Assert.AreEqual("darn", termList(1).RecommendedTerm, "RecommendedTerm property of term 1 is incorrect.")

            Dim term1exclusions As List(Of IExclusion) = Utilities.ListFromEnum(termList(1).Exclusions)

            Assert.AreEqual(3, term1exclusions.Count, "Number of exclusions for term 1 is incorrect.")
            Assert.AreEqual("Did you shoot the sherrif?", term1exclusions(0).Text, "Exclusion 0 of term 1 is incorrect.")
            Assert.AreEqual("I did not shoot the deputy", term1exclusions(1).Text, "Exclusion 1 of term 1 is incorrect.")
            Assert.AreEqual("an old-fashioned shoot-em-up", term1exclusions(2).Text, "Exclusion 2 of term 1 is incorrect.")
        End Sub

    End Class


End Namespace
