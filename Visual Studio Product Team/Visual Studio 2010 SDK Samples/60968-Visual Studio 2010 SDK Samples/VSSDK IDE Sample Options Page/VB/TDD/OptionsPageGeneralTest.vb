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
Imports Microsoft.Samples.VisualStudio.IDE.OptionsPage
Imports System.Drawing
Imports System.ComponentModel
Imports System.IO
Imports System.Windows.Forms

Namespace Microsoft.Samples.VisualStudio.IDE.OptionsPage.UnitTests
	''' <summary>
	''' This is a test class for OptionsPage.OptionsPageGeneral and is intended
	''' to contain all OptionsPage.OptionsPageGeneral Unit Tests?
	''' </summary>
	<TestClass()> _
	Public Class OptionsPageGeneralTest
		Implements IDisposable
        ' Fields
		#Region "Fields"
		Private testContextInstance As TestContext
        ' Instance of tested object.
		Private optionsPageGeneral As OptionsPageGeneral
		Private testString As String
		Private tmpImgFilePath As String = "OptionsPageTest.jpg"
		Private optionsPageGeneralAccessor As Microsoft_Samples_VisualStudio_IDE_OptionsPage_OptionsPageGeneralAccessor
		#End Region 

        ' Properties
		#Region "Properties"
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
		#End Region 

		#Region "Initialization && CLeanup"
		<TestInitialize()> _
		Public Sub OptionsPageGeneralTestInitialize()
			testString = "This is the test string."
            tmpImgFilePath = AppDomain.CurrentDomain.BaseDirectory & Path.DirectorySeparatorChar & tmpImgFilePath
			optionsPageGeneral = New OptionsPageGeneral()
			optionsPageGeneralAccessor = New Microsoft_Samples_VisualStudio_IDE_OptionsPage_OptionsPageGeneralAccessor(optionsPageGeneral)
			' To forbid display of MessageBoxes.
			WinFormsHelper.AllowMessageBox = False
		End Sub

		<TestCleanup()> _
		Public Sub OptionsPageGeneralTestCleanup()
			optionsPageGeneral = Nothing
			Dispose()
		End Sub
        ' Initialization && CLeanup.
		#End Region 

		#Region "IDisposable Pattern implementation"
		''' <summary>
		''' Implement IDisposable.
		''' </summary>
		Public Sub Dispose() Implements IDisposable.Dispose
			Dispose(True)
		End Sub
		''' <summary> 
		''' Clean up any resources being used.
		''' </summary>
		Protected Overridable Sub Dispose(ByVal disposing As Boolean)
			If disposing Then
                If optionsPageGeneral IsNot Nothing Then
                    optionsPageGeneral = Nothing
                End If
				GC.SuppressFinalize(Me)
			End If
		End Sub
        ' IDisposable Pattern implementation.
		#End Region 

		#Region "Test methods"

		#Region "Properties tests"
		''' <summary>
		''' The test for CustomSize property.
		'''</summary>
		<TestMethod()> _
		Public Sub CustomSizeTest()
			Dim target As OptionsPageGeneral = optionsPageGeneral
            Dim expectedSizeValue As New Size(100, 100)
			target.CustomSize = expectedSizeValue

			Assert.AreEqual(expectedSizeValue, target.CustomSize, "CustomSize property was not returned expected value.")
		End Sub
		''' <summary>
		''' The test for OptionInteger property.
		'''</summary>
		<TestMethod()> _
		Public Sub OptionIntegerTest()
			Dim target As OptionsPageGeneral = optionsPageGeneral
			Dim expectedIntegerValue As Integer = Integer.MaxValue
			target.OptionInteger = expectedIntegerValue

			Assert.AreEqual(expectedIntegerValue, target.OptionInteger, "OptionInteger property was not returned expected value.")
		End Sub

		''' <summary>
		''' The test for OptionString property.
		'''</summary>
		<TestMethod()> _
		Public Sub OptionStringTest()
			Dim target As OptionsPageGeneral = optionsPageGeneral
			Dim expectedStringValue As String = testString
			target.OptionString = expectedStringValue

			Assert.AreEqual(expectedStringValue, target.OptionString, "OptionString property was not returned expected value.")
		End Sub

        ' Properties tests
		#End Region 

		#Region "Event handlers tests"
		''' <summary>
		''' The test for OnActivate() event handler.
		'''</summary>
		<TestMethod()> _
		Public Sub OnActivateTest()
			Dim target As OptionsPageGeneral = optionsPageGeneral
			Dim accessor As Microsoft_Samples_VisualStudio_IDE_OptionsPage_OptionsPageGeneralAccessor = optionsPageGeneralAccessor
            Dim cancelEventArgs As New CancelEventArgs(False)

			' We simulate Cancel button choice, in this case we expect that 
			' cancelEventArgs.Cancel was switched to the true state.
			WinFormsHelper.FakeDialogResult = DialogResult.Cancel
			accessor.OnActivate(cancelEventArgs)

			Assert.IsTrue(cancelEventArgs.Cancel, "CancelEventArgs Cancel property was initialized by not expected value in case when simulated Cancel button choice.")
		End Sub

		''' <summary>
		''' The test for OnClosed() event handler.
		'''</summary>
		'''<remarks>Tested event handling function does not performs any actions.</remarks>
		<TestMethod()> _
		Public Sub OnClosedTest()
            Dim target As New OptionsPageGeneral()
			Dim accessor As Microsoft_Samples_VisualStudio_IDE_OptionsPage_OptionsPageGeneralAccessor = optionsPageGeneralAccessor
			Dim e As EventArgs = Nothing

			accessor.OnClosed(e)
		End Sub

		''' <summary>
		''' The test for OnDeactivate event handler.
		'''</summary>
		<TestMethod()> _
		Public Sub OnDeactivateTest()
            Dim target As New OptionsPageGeneral()
			Dim accessor As Microsoft_Samples_VisualStudio_IDE_OptionsPage_OptionsPageGeneralAccessor = optionsPageGeneralAccessor

            Dim cancelEventArgs As New CancelEventArgs(False)

			' We simulate Cancel button choice, in this case we expect that 
			' cancelEventArgs.Cancel was switched to the true state.
			WinFormsHelper.FakeDialogResult = DialogResult.Cancel
			accessor.OnDeactivate(cancelEventArgs)

			Assert.IsTrue(cancelEventArgs.Cancel, "CancelEventArgs Cancel property was initialized by not expected value in case when simulated Cancel button choice.")
		End Sub
        ' Event handlers tests
		#End Region 

        ' Test methods
		#End Region 

        ' Service functions
		#Region "Service functions"
		Public Sub CreateTmpImgFile()
			If File.Exists(tmpImgFilePath) Then
				File.Delete(tmpImgFilePath)
			End If
			Dim sw As StreamWriter = File.AppendText(tmpImgFilePath)
			sw.Write(tmpImgFilePath)
			sw.Close()

		End Sub
		Public Sub DestroyTmpImgFile()
			If File.Exists(tmpImgFilePath) Then
				File.Delete(tmpImgFilePath)
			End If
		End Sub
		#End Region 
	End Class
End Namespace
