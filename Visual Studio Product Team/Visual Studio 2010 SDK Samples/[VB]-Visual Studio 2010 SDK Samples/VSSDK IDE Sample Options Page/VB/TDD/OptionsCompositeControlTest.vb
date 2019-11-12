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
Imports System.IO
Imports System.Drawing
Imports System.Runtime.InteropServices
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Microsoft.Samples.VisualStudio.IDE.OptionsPage

Namespace Microsoft.Samples.VisualStudio.IDE.OptionsPage.UnitTests
	'''<summary>
	''' This is a test class for OptionsPage.OptionsCompositeControl and is intended
	''' to contain all OptionsPage.OptionsCompositeControl Unit Tests.
	'''</summary>
	<TestClass()> _
	Public Class OptionsCompositeControlTest
		Implements IDisposable
		' Fields
        #Region "Fields"
		Private ReadOnly testCustomImagePath As String = "SomeImagePath.bmp"
		Private testContextInstance As TestContext
        ' Instance of tested object.
		Private compositeControl As OptionsCompositeControl
        ' Accessor for the private interface of the tested object.
		Private compositeControlAccessor As Microsoft_Samples_VisualStudio_IDE_OptionsPage_OptionsCompositeControlAccessor
		#End Region 

        ' Properties
		#Region "Properties"
		''' <summary>
		''' Gets or sets the test context which provides
		''' information about and functionality for the current test run.
		'''</summary>
		Public Property TestContext() As TestContext
			Get
				Return testContextInstance
			End Get
			Set(ByVal value As TestContext)
				testContextInstance = value
			End Set
		End Property
		#End Region 

		' Initialization && Cleanup
        #Region "Initialization && Cleanup"

		''' <summary>
		''' Runs before the test to allocate and configure resources needed 
		''' by all tests in the test class.
		''' </summary>
		<TestInitialize()> _
		Public Sub TestInitialize()
			compositeControl = New OptionsCompositeControl()
			compositeControlAccessor = New Microsoft_Samples_VisualStudio_IDE_OptionsPage_OptionsCompositeControlAccessor(compositeControl)
			Assert.IsNotNull(compositeControl, "General OptionsCompositeControl instance (compositeControl) was not created successfully.")
		End Sub

		''' <summary>
		''' Runs after the test has run and to free resources obtained 
		''' by all the tests in the test class.
		''' </summary>
		<TestCleanup()> _
		Public Sub TestCleanup()
			compositeControl = Nothing
			compositeControlAccessor = Nothing
		End Sub
		#End Region 

		#Region "IDisposable Pattern implementation"
		''' <summary>
		''' Clean up any resources being used.
		''' </summary>
		Public Sub Dispose() Implements IDisposable.Dispose
			Dispose(True)
		End Sub
		''' <summary> 
		''' Clean up any resources being used.
		''' </summary>
		Protected Overridable Sub Dispose(ByVal disposing As Boolean)
			If disposing Then
                If compositeControl IsNot Nothing Then
                    compositeControl.Dispose()
                    compositeControl = Nothing
                End If
                If compositeControlAccessor IsNot Nothing Then
                    compositeControlAccessor.Dispose(True)
                    compositeControlAccessor = Nothing
                End If
				GC.SuppressFinalize(Me)
			End If
		End Sub
		#End Region

		#Region "Test methods"
		#Region "Methods of testing of the Constructors and Initializers"
		''' <summary>
		''' The test for OptionsCompositeControl().
		'''</summary>
		<TestMethod()> _
		Public Sub ConstructorTest()
            Dim target As New OptionsCompositeControl()
			Assert.IsNotNull(target, "Instance of the OptionsCompositeControl was not created successfully after default constructor call.")
		End Sub
		''' <summary>
		''' The test for InitializeComponent() method.
		'''</summary>
		<TestMethod()> _
		Public Sub InitializeComponentTest()
            Dim target As New OptionsCompositeControl()
			Assert.IsNotNull(target, "Instance of the OptionsCompositeControl object was not created successfully.")
            Dim accessor As New Microsoft_Samples_VisualStudio_IDE_OptionsPage_OptionsCompositeControlAccessor(target)
			Assert.IsNotNull(accessor, "Instance of the OptionsCompositeControl accessor was not created successfully.")

			accessor.InitializeComponent()

			Assert.IsNotNull(target.Controls, "Controls collection was not initialized after InitializeComponent() call.")
			Assert.IsTrue((target.Controls.Count>0), "Controls collection was not populated by controls objects.")
		End Sub
        ' Methods of testing of the Constructors.
		#End Region 

		' Methods of testing of the IDisposable implementation.
        #Region "Methods of testing of the IDisposable implementation"
		''' <summary>
		''' The test for Dispose() method.
		'''</summary>
		<TestMethod()> _
		Public Sub DisposeTest()
			Dim target As OptionsCompositeControl = compositeControl

			Assert.IsTrue((TypeOf target Is IDisposable), "Tested OptionsCompositeControl instance does not implements IDisposable interface.")

			target.Dispose()
			Assert.IsTrue(target.IsDisposed, "Internal state of the OptionsCompositeControl instance is in the NotDosposed state, was expected that IsDisosed is True.")
		End Sub
		#End Region 

		#Region "Methods of testing of the GUI Event Handlers"
		''' <summary>
		''' The test for OnClearImage() event handler.
		'''</summary>
		<TestMethod()> _
		Public Sub OnClearImageTest()
			Dim target As OptionsCompositeControl = compositeControl
			Dim accessor As Microsoft_Samples_VisualStudio_IDE_OptionsPage_OptionsCompositeControlAccessor = compositeControlAccessor
			accessor.customOptionsPage = New OptionsPageCustom()

			accessor.customOptionsPage.CustomBitmap = testCustomImagePath
			Assert.AreEqual(testCustomImagePath, accessor.customOptionsPage.CustomBitmap, "CustomBitmap path was not initialized by expected value.")

			accessor.OnClearImage(Me, EventArgs.Empty)

			Assert.IsNull(accessor.customOptionsPage.CustomBitmap, "CustomBitmap path after Clear command was not cleared.")
		End Sub
		' Methods of testing of the GUI Event Handlers.
        #End Region 

		#Region "Methods of testing of the public properties"
		''' <summary>
		''' The test for OptionsPage public property.
		'''</summary>
		<TestMethod()> _
		Public Sub OptionsPageTest()
			Dim target As OptionsCompositeControl = compositeControl
            Dim expectedValue As New OptionsPageCustom()

			target.OptionsPage = expectedValue
			Assert.AreEqual(expectedValue, target.OptionsPage, "OptionsPage property was initialized by unexpected value.")
		End Sub
		' Methods of testing of the public properties.
        #End Region 

		#Region "Methods of testing of the public methods"
		''' <summary>
		''' The test for RefreshImage() in scenario when internal OptionsPageCustom object is null.
		'''</summary>
		<TestMethod()> _
		Public Sub RefreshImageWithNullableOptionPageTest()
			Dim target As OptionsCompositeControl = compositeControl
			Dim accessor As Microsoft_Samples_VisualStudio_IDE_OptionsPage_OptionsCompositeControlAccessor = compositeControlAccessor

			accessor.customOptionsPage = Nothing
			accessor.RefreshImage()
		End Sub
		''' <summary>
		''' The test for RefreshImage() in scenario when internal OptionsPageCustom object
		''' is properly initialized.
		'''</summary>
		<TestMethod()> _
		Public Sub RefreshImageWithCompleteOptionPageTest()
			Dim target As OptionsCompositeControl = compositeControl
			Dim accessor As Microsoft_Samples_VisualStudio_IDE_OptionsPage_OptionsCompositeControlAccessor = compositeControlAccessor

			Try
				CreateTestBitmapFile()

				accessor.customOptionsPage = New OptionsPageCustom()
				accessor.pictureBox.Image = Nothing
                accessor.customOptionsPage.CustomBitmap = testCustomImagePath
				accessor.RefreshImage()

				Assert.IsNotNull(accessor.pictureBox.Image, "Internal PictureBox Image object was not initialized.")
			Finally
				DestroyTestBitmapFile()
			End Try
		End Sub
		' Methods of testing of the public methods.
        #End Region 
        ' Test methods
		#End Region 

		#Region "Service functions"
		''' <summary>
		''' Create test bitmap image file.
		''' </summary>
		Public Sub CreateTestBitmapFile()
			If File.Exists(testCustomImagePath) Then
				File.Delete(testCustomImagePath)
			End If

            Dim bitmapData As New Bitmap(10, 10)

			Try
				bitmapData.Save(testCustomImagePath)
			Catch e1 As ArgumentNullException
				Assert.Fail("Path to the test bitmap image file was not properly initialized.")
			Catch handledException As ExternalException
				Assert.Fail(handledException.Message)
			End Try
		End Sub
		''' <summary>
		''' Destroy test bitmap image file.
		''' </summary>
		Public Sub DestroyTestBitmapFile()
			If File.Exists(testCustomImagePath) Then
				Try
					File.Delete(testCustomImagePath)
				Catch handleedException As Exception
					Assert.Fail(handleedException.Message)
				End Try
			End If
		End Sub
		' Service functions.
        #End Region 
	End Class
End Namespace
