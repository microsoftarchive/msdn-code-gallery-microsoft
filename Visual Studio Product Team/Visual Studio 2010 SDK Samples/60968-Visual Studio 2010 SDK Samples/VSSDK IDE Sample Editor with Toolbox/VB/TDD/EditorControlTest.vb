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
Imports Microsoft.Samples.VisualStudio.IDE.EditorWithToolbox

Namespace Microsoft.Samples.VisualStudio.IDE.EditorWithToolbox.UnitTests
	'''<summary>
	'''This is a test class for EditorWithToolbox.EditorControl and is intended
	'''to contain all EditorWithToolbox.EditorControl Unit Tests.
	'''</summary>
	<TestClass()> _
	Public Class EditorControlTest
		Implements IDisposable
		#Region "Fields"

		Private testContextInstance As TestContext
		Private editorControl As EditorControl
		Private testString As String = String.Empty

		#End Region

		#Region "Initialization && Cleanup"
		''' <summary>
		''' Runs before the test to allocate and configure resources needed 
		''' by all tests in the test class.
		''' </summary>
		<TestInitialize()> _
		Public Sub EditorControlTestInitialize()
			testString = "This is a test string"

			editorControl = New EditorControl()
			editorControl.Text = testString
		End Sub
		''' <summary>
		''' Runs after the test has run and to free resources obtained 
		''' by all the tests in the test class.
		''' </summary>
		<TestCleanup()> _
		Public Sub EditorControlTestCleanup()
			editorControl = Nothing
		End Sub

#End Region

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
                If editorControl IsNot Nothing Then
                    editorControl.Dispose()
                    editorControl = Nothing
                End If
				GC.SuppressFinalize(Me)
			End If
		End Sub
		#End Region

		#Region "Constructors tests"
		''' <summary>
		''' The test for EditorControl default constructor.
        ''' </summary>
		<TestMethod()> _
		Public Sub ConstructorTest()
            Dim target As New EditorControl()

			Assert.IsNotNull(target, "EditorControl object was not created successfully")
		End Sub

		''' <summary>
		''' The test for InitializeComponent() method.
        ''' </summary>
		<TestMethod()> _
		Public Sub InitializeComponentTest()
            Dim target As New EditorControl()

            Dim accessor As New Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorControlAccessor(target)

			accessor.InitializeComponent()

			Assert.IsFalse(target.WordWrap, "WordWrap property is not switched in to False state")
		End Sub
#End Region
	End Class
End Namespace
