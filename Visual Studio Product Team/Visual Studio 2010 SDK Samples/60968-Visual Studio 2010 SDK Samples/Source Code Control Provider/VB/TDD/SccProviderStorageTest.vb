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
Imports System.IO
Imports System.Text
Imports System.Collections.Generic
Imports Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider

Namespace Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.UnitTests
	''' <summary>
    ''' This is a test class for Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.SccProviderStorage and is intended
    ''' to contain all Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.SccProviderStorage Unit Tests.
    ''' </summary>
	<TestClass()> _
	Public Class SccProviderStorageTest
		''' <summary>
        ''' A test for SccProviderStorage (string).
        ''' </summary>
		<TestMethod()> _
		Public Sub ConstructorTest()
            Dim target As New SccProviderStorage("Dummy.txt")
			Assert.IsNotNull(target, "SccProviderStorage cannot be created")
		End Sub

		''' <summary>
        ''' A test for AddFilesToStorage (IList&lt;string&gt;).
        ''' </summary>
		<TestMethod()> _
		Public Sub AddFilesToStorageTest()
			Dim projectFile As String = Path.GetTempFileName()
			Dim storageFile As String = projectFile & ".storage"
			If File.Exists(storageFile) Then
				File.Delete(storageFile)
			End If

            Dim target As New SccProviderStorage(projectFile)

            Dim files As New List(Of String)()
			files.Add(projectFile)
			files.Add("dummy.txt")
			target.AddFilesToStorage(files)
            ' Test that project file is now controlled.
			Assert.AreEqual(SourceControlStatus.scsCheckedIn, target.GetFileStatus(projectFile), "Failed to add the project file")
            ' Test the storage file was written.
			Assert.IsTrue(File.Exists(storageFile), "Storage file was not created")

            ' Cleanup the files written by the test.
			File.SetAttributes(projectFile, FileAttributes.Normal)
			File.Delete(projectFile)
			File.Delete(storageFile)
		End Sub

		''' <summary>
        ''' A test for CheckinFile (string).
        ''' </summary>
		<TestMethod()> _
		Public Sub CheckinCheckoutFileTest()
			Dim projectFile As String = Path.GetTempFileName()
            Dim target As New SccProviderStorage(projectFile)

			target.CheckinFile(projectFile)
            ' Test the file is readonly.
			Assert.AreEqual(File.GetAttributes(projectFile) And FileAttributes.ReadOnly, FileAttributes.ReadOnly, "Checkin failed")

            ' Cleanup the files written by the test.
			File.SetAttributes(projectFile, FileAttributes.Normal)
			File.Delete(projectFile)
		End Sub

		''' <summary>
        ''' A test for CheckoutFile (string).
        ''' </summary>
		<TestMethod()> _
		Public Sub CheckoutFileTest()
			Dim projectFile As String = Path.GetTempFileName()
            Dim target As New SccProviderStorage(projectFile)

			target.CheckinFile(projectFile)
            ' Test the file is readonly.
			Assert.AreEqual(File.GetAttributes(projectFile) And FileAttributes.ReadOnly, FileAttributes.ReadOnly, "Checkin failed")

			target.CheckoutFile(projectFile)
            ' Test the file is readwrite.
            Assert.AreEqual(File.GetAttributes(projectFile) And FileAttributes.ReadOnly, CType(CType(0, FileAttributes), Object), "Checkout failed")

            ' Cleanup the files written by the test.
			File.Delete(projectFile)
		End Sub

		''' <summary>
        ''' A test for GetFileStatus (string).
        ''' </summary>
		<TestMethod()> _
		Public Sub GetFileStatusTest()
			Dim projectFile As String = Path.GetTempFileName()
			Dim storageFile As String = projectFile & ".storage"
			If File.Exists(storageFile) Then
				File.Delete(storageFile)
			End If

            Dim target As New SccProviderStorage(projectFile)

            Dim files As New List(Of String)()
			files.Add(projectFile)
			target.AddFilesToStorage(files)
            ' Test that project file is now controlled.
			Assert.AreEqual(SourceControlStatus.scsCheckedIn, target.GetFileStatus(projectFile), "GetFileStatus failed for project file")
            ' Checkout the file and test status again.
			target.CheckoutFile(projectFile)
			Assert.AreEqual(SourceControlStatus.scsCheckedOut, target.GetFileStatus(projectFile), "GetFileStatus failed for project file")
            ' Test that a dummy file is uncontrolled.
			Assert.AreEqual(SourceControlStatus.scsUncontrolled, target.GetFileStatus("Dummy.txt"), "GetFileStatus failed for uncontrolled file")

            ' Cleanup the files written by the test.
			File.Delete(projectFile)
			File.Delete(storageFile)
		End Sub

		''' <summary>
        ''' A test for RenameFileInStorage (string, string).
        ''' </summary>
		<TestMethod()> _
		Public Sub RenameFileInStorageTest()
			Dim projectFile As String = Path.GetTempFileName()
			Dim storageFile As String = projectFile & ".storage"
			Dim newStorageFile As String = "dummy.txt.storage"
			If File.Exists(storageFile) Then
				File.Delete(storageFile)
			End If
			If File.Exists(newStorageFile) Then
				File.Delete(newStorageFile)
			End If

            Dim target As New SccProviderStorage(projectFile)

            Dim files As New List(Of String)()
			files.Add(projectFile)
			target.AddFilesToStorage(files)
			target.RenameFileInStorage(projectFile, "dummy.txt")
            ' Test that project file is now uncontrolled.
			Assert.AreEqual(SourceControlStatus.scsUncontrolled, target.GetFileStatus(projectFile), "GetFileStatus failed for old name")
            ' Test that dummy file is now controlled (and checked in since it's missing from disk).
			Assert.AreEqual(SourceControlStatus.scsCheckedIn, target.GetFileStatus("Dummy.txt"), "GetFileStatus failed for new name")

            ' Cleanup the files written by the test.
			File.SetAttributes(projectFile, FileAttributes.Normal)
			File.Delete(projectFile)
			File.Delete(newStorageFile)
		End Sub

		''' <summary>
        ''' A test for ReadStorageFile ().
        ''' </summary>
		<TestMethod()> _
		Public Sub ReadStorageFileTest()
			Dim projectFile As String = Path.GetTempFileName()
			Dim storageFile As String = projectFile & ".storage"
			If File.Exists(storageFile) Then
				File.Delete(storageFile)
			End If

            Dim objWriter As New StreamWriter(storageFile, False, System.Text.Encoding.Unicode)
			objWriter.Write("dummy.txt" & Constants.vbCrLf)
			objWriter.Close()
			objWriter.Dispose()

            Dim target As New SccProviderStorage(projectFile)
            ' Test that dummy file is now controlled (and checked in since it's missing from disk).
			Assert.AreEqual(SourceControlStatus.scsCheckedIn, target.GetFileStatus("Dummy.txt"), "GetFileStatus failed for new name")

            ' Cleanup the files written by the test.
			File.SetAttributes(projectFile, FileAttributes.Normal)
			File.Delete(projectFile)
			File.Delete(storageFile)
		End Sub
	End Class
End Namespace
