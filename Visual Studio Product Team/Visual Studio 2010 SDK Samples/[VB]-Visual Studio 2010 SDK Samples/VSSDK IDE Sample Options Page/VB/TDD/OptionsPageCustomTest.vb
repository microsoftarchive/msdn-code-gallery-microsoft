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

Namespace Microsoft.Samples.VisualStudio.IDE.OptionsPage.UnitTests
	'''<summary>
	''' This is a test class for OptionsPage.OptionsPageCustom and is intended
	''' to contain all OptionsPage.OptionsPageCustom Unit Tests.
	'''</summary>
	<TestClass()> _
	Public Class OptionsPageCustomTest
		Implements IDisposable
        ' Fields
		#Region "Fields"
		Private testContextInstance As TestContext
        ' Instance of tested object.
		Private optionsPageCustom As OptionsPageCustom
		#End Region 

        ' Proeprties
		#Region "Proeprties"
		'''<summary>
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
		Public Sub OptionsPageCustomTestInitialize()
			optionsPageCustom = New OptionsPageCustom()
		End Sub
		''' <summary>
		''' Runs after the test has run and to free resources obtained 
		''' by all the tests in the test class.
		''' </summary>
		<TestCleanup()> _
		Public Sub OptionsPageCustomTestCleanup()
			optionsPageCustom = Nothing
		End Sub

		#End Region 

		#Region "Test Methods"

		#Region "Methods of testing of the Properties"
		''' <summary>
		''' The test for CustomBitmap property.
		'''</summary>
		<TestMethod()> _
		Public Sub CustomBitmapTest()
			Dim target As OptionsPageCustom = optionsPageCustom
			Dim expectedPathValue As String = AppDomain.CurrentDomain.BaseDirectory & "SomeBitmap.Bmp"
			target.CustomBitmap = expectedPathValue

			Assert.AreEqual(expectedPathValue, target.CustomBitmap, "CustomBitmap property value was initialized by unexpected value.")
		End Sub

		''' <summary>
		''' The test for Window property.
		'''</summary>
		<TestMethod()> _
		Public Sub WindowTest()
			Dim target As OptionsPageCustom = optionsPageCustom
            Dim accessor As New Microsoft_Samples_VisualStudio_IDE_OptionsPage_OptionsPageCustomAccessor(target)
            Dim optionsControl As New OptionsCompositeControl()

			optionsControl = TryCast(accessor.Window, OptionsCompositeControl)
			Assert.IsNotNull(optionsControl, "Internal Window property was not initialized by expected value.")
			Assert.AreEqual(optionsControl.OptionsPage, target, "Internal CompositeCOntrol options page property was initialized by unexpected value.")
		End Sub
        ' Methods of testing of the Properties.
		#End Region 

        ' Test Methods.
		#End Region 

        ' IDisposable Pattern implementation.
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
                If optionsPageCustom IsNot Nothing Then
                    optionsPageCustom = Nothing
                End If
				GC.SuppressFinalize(Me)
			End If
		End Sub
		#End Region 
	End Class
End Namespace
