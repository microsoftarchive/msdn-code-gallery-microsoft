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
    ''' This is a test class for Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.DlgQueryEditCheckedInFile and is intended
    ''' to contain all Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.DlgQueryEditCheckedInFile Unit Tests.
    ''' </summary>
	<TestClass()> _
	Public Class DlgQueryEditCheckedInFileTest
		''' <summary>
        ''' A test for DlgQueryEditCheckedInFile (string).
        ''' </summary>
		<TestMethod()> _
		Public Sub ConstructorTest()
            Dim target As New DlgQueryEditCheckedInFile("Dummy.txt")
			Assert.IsNotNull(target, "DlgQueryEditCheckedInFile cannot be created")
		End Sub

		''' <summary>
        ''' A test for btnCancel_Click (object, EventArgs).
        ''' </summary>
		<TestMethod()> _
		Public Sub btnCancel_ClickTest()
            Dim target As New DlgQueryEditCheckedInFile("Dummy.txt")
			Dim method As MethodInfo = GetType(DlgQueryEditCheckedInFile).GetMethod("btnCancel_Click", BindingFlags.NonPublic Or BindingFlags.Instance)
			method.Invoke(target, New Object() { Nothing, Nothing })
			Assert.AreEqual(target.Answer, DlgQueryEditCheckedInFile.qecifCancelEdit)
		End Sub

		''' <summary>
        ''' A test for btnCheckout_Click (object, EventArgs).
        ''' </summary>
		<TestMethod()> _
		Public Sub btnCheckout_ClickTest()
            Dim target As New DlgQueryEditCheckedInFile("Dummy.txt")
			Dim method As MethodInfo = GetType(DlgQueryEditCheckedInFile).GetMethod("btnCheckout_Click", BindingFlags.NonPublic Or BindingFlags.Instance)
			method.Invoke(target, New Object() { Nothing, Nothing })
			Assert.AreEqual(target.Answer, DlgQueryEditCheckedInFile.qecifCheckout)
		End Sub

		''' <summary>
        ''' A test for btnEdit_Click (object, EventArgs).
        ''' </summary>
		<TestMethod()> _
		Public Sub btnEdit_ClickTest()
            Dim target As New DlgQueryEditCheckedInFile("Dummy.txt")
			Dim method As MethodInfo = GetType(DlgQueryEditCheckedInFile).GetMethod("btnEdit_Click", BindingFlags.NonPublic Or BindingFlags.Instance)
			method.Invoke(target, New Object() {Nothing, Nothing})
			Assert.AreEqual(target.Answer, DlgQueryEditCheckedInFile.qecifEditInMemory)
		End Sub
	End Class
End Namespace
