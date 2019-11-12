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
Imports System.Text
Imports System.Reflection
Imports System.Collections.Generic
Imports Microsoft.VsSDK.UnitTestLibrary
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider

Namespace Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.UnitTests
	''' <summary>
    ''' This is a test class for Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.DlgQuerySaveCheckedInFile and is intended
    ''' to contain all Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.DlgQuerySaveCheckedInFile Unit Tests.
    ''' </summary>
	<TestClass()> _
	Public Class DlgQuerySaveCheckedInFileTest
		''' <summary>
        ''' A test for DlgQuerySaveCheckedInFile (string).
        ''' </summary>
		<TestMethod()> _
		Public Sub ConstructorTest()
            Dim target As New DlgQuerySaveCheckedInFile("Dummy.txt")
			Assert.IsNotNull(target, "DlgQuerySaveCheckedInFile cannot be created")
		End Sub

		''' <summary>
        ''' A test for btnCancel_Click (object, EventArgs).
        ''' </summary>
		<TestMethod()> _
		Public Sub btnCancel_ClickTest()
            Dim target As New DlgQuerySaveCheckedInFile("Dummy.txt")
			Dim method As MethodInfo = GetType(DlgQuerySaveCheckedInFile).GetMethod("btnCancel_Click", BindingFlags.NonPublic Or BindingFlags.Instance)
			method.Invoke(target, New Object() { Nothing, Nothing })
			Assert.AreEqual(target.Answer, DlgQuerySaveCheckedInFile.qscifCancel)
		End Sub

		''' <summary>
        ''' A test for btnCheckout_Click (object, EventArgs).
        ''' </summary>
		<TestMethod()> _
		Public Sub btnCheckout_ClickTest()
            Dim target As New DlgQuerySaveCheckedInFile("Dummy.txt")
			Dim method As MethodInfo = GetType(DlgQuerySaveCheckedInFile).GetMethod("btnCheckout_Click", BindingFlags.NonPublic Or BindingFlags.Instance)
			method.Invoke(target, New Object() { Nothing, Nothing })
			Assert.AreEqual(target.Answer, DlgQuerySaveCheckedInFile.qscifCheckout)
		End Sub

		''' <summary>
        ''' A test for btnSaveAs_Click (object, EventArgs).
        ''' </summary>
		<TestMethod()> _
		Public Sub btnSaveAs_ClickTest()
            Dim target As New DlgQuerySaveCheckedInFile("Dummy.txt")
			Dim method As MethodInfo = GetType(DlgQuerySaveCheckedInFile).GetMethod("btnSaveAs_Click", BindingFlags.NonPublic Or BindingFlags.Instance)
			method.Invoke(target, New Object() { Nothing, Nothing })
			Assert.AreEqual(target.Answer, DlgQuerySaveCheckedInFile.qscifForceSaveAs)
		End Sub

		''' <summary>
        ''' A test for btnSkipSave_Click (object, EventArgs).
        ''' </summary>
		<TestMethod()> _
		Public Sub btnSkipSave_ClickTest()
            Dim target As New DlgQuerySaveCheckedInFile("Dummy.txt")
			Dim method As MethodInfo = GetType(DlgQuerySaveCheckedInFile).GetMethod("btnSkipSave_Click", BindingFlags.NonPublic Or BindingFlags.Instance)
			method.Invoke(target, New Object() { Nothing, Nothing })
			Assert.AreEqual(target.Answer, DlgQuerySaveCheckedInFile.qscifSkipSave)
		End Sub
	End Class
End Namespace
