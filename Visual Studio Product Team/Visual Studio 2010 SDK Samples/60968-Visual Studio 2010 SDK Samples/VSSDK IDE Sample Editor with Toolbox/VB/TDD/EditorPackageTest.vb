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
Imports Microsoft.VisualStudio
Imports System.Collections.Generic
Imports OleInterop = Microsoft.VisualStudio.OLE.Interop
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Microsoft.VisualStudio.Shell.Interop
Imports Microsoft.VsSDK.UnitTestLibrary

Namespace Microsoft.Samples.VisualStudio.IDE.EditorWithToolbox.UnitTests
	''' <summary>
    ''' This is a test class for Microsoft.Samples.VisualStudio.IDE.EditorWithToolbox.EditorPackage and is intended
    ''' to contain all Microsoft.Samples.VisualStudio.IDE.EditorWithToolbox.EditorPackage Unit Tests.
    ''' </summary>
	<TestClass()> _
	Public Class EditorPackageTest
		Implements IDisposable
		#Region "Fields"

		Private testContextInstance As TestContext
		Private editorPackage As EditorPackage

		#End Region
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

		#Region "Initialization && Cleanup"
		''' <summary>
		''' Runs before the test to allocate and configure resources needed 
		''' by all tests in the test class.
		''' </summary>
		<TestInitialize()> _
		Public Sub EditorPackageTestInitialize()
			editorPackage = New EditorPackage()
		End Sub
		''' <summary>
		''' Runs after the test has run and to free resources obtained 
		''' by all the tests in the test class.
		''' </summary>
		<TestCleanup()> _
		Public Sub EditorPackageTestCleanup()
			editorPackage = Nothing
		End Sub

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
                If editorPackage IsNot Nothing Then
                    editorPackage.Dispose()
                    editorPackage = Nothing
                End If
				GC.SuppressFinalize(Me)
			End If
		End Sub
		#End Region

		#Region "IDisposable tests"
		''' <summary>
        ''' A test for Dispose (bool).
        ''' </summary>
		<TestMethod()> _
		Public Sub DisposeTest()
			Dim target As EditorPackage = editorPackage
            Dim accessor As New Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPackageAccessor(target)
			accessor.editorFactory = New EditorFactory()
			target.Dispose()
			Assert.IsNull(accessor.editorFactory, "Internal Editor Factory object was not disposed properly.")
		End Sub
#End Region

		<TestMethod()> _
		Public Sub SetSite()
            ' Create the package.
            Dim package As IVsPackage = TryCast(New EditorPackage(), IVsPackage)
			Assert.IsNotNull(package, "The object does not implement IVsPackage")

            ' Create a basic service provider.
			Dim serviceProvider As OleServiceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices()

            ' Add site support to register editor factory.
			Dim registerEditor As BaseMock = MockRegisterEditors.GetRegisterEditorsInstance()
			serviceProvider.AddService(GetType(SVsRegisterEditors), registerEditor, False)

            ' Site the package.
			Assert.AreEqual(0, package.SetSite(serviceProvider), "SetSite did not return S_OK")
		End Sub
	End Class
End Namespace
