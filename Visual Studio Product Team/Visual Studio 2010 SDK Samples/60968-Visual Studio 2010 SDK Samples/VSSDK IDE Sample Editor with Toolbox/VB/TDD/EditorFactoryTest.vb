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
Imports Microsoft.VisualStudio.Shell.Interop
Imports OleInterop = Microsoft.VisualStudio.OLE.Interop
Imports Microsoft.VsSDK.UnitTestLibrary
Imports Microsoft.VisualStudio

Namespace Microsoft.Samples.VisualStudio.IDE.EditorWithToolbox.UnitTests
	''' <summary>
    ''' This is a test class for EditorWithToolbox.EditorFactory and is intended
    ''' to contain all EditorWithToolbox.EditorFactory Unit Tests.
    ''' </summary>
	<TestClass()> _
	Public Class EditorFactoryTest
		Implements IDisposable
		#Region "Fields"

		Private testContextInstance As TestContext
		Private editorPackage As EditorPackage
		Private editorFactory As EditorFactory

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

		#Region "Initialization && Cleanup"
		''' <summary>
		''' Runs before the test to allocate and configure resources needed 
		''' by all tests in the test class.
		''' </summary>
		<TestInitialize()> _
		Public Sub EditorFactoryTestInitialize()
			editorPackage = New EditorPackage()
			editorFactory = New EditorFactory()
        End Sub

		''' <summary>
		''' Runs after the test has run and to free resources obtained 
		''' by all the tests in the test class.
		''' </summary>
		<TestCleanup()> _
		Public Sub EditorFactoryTestCleanup()
			editorPackage.Dispose()
            editorPackage = Nothing

			editorFactory.Dispose()
			editorFactory = Nothing
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
                If editorPackage IsNot Nothing Then
                    editorPackage.Dispose()
                    editorPackage = Nothing
                End If
                If editorFactory IsNot Nothing Then
                    editorFactory.Dispose()
                    editorFactory = Nothing
                End If
				GC.SuppressFinalize(Me)
			End If
		End Sub
		#End Region

		#Region "Test methods"
		#Region "Constructors tests"
		''' <summary>
        ''' A test for EditorFactory (EditorPackage).
        ''' </summary>
		<TestMethod()> _
		Public Sub ConstructorTest()
			editorFactory = Nothing
			editorFactory = New EditorFactory()
			Assert.IsNotNull(editorFactory, "EditorFactory instance was not created successfully")
		End Sub
#End Region

		#Region "IDisposable tests"
		''' <summary>
        ''' Verifies that the object implement IDisposable interface.
		''' </summary>
		<TestMethod()> _
		Public Sub IsIDisposableTest()
			Dim target As EditorFactory= editorFactory
			Assert.IsNotNull(TryCast(target, IDisposable), "The object does not implement IDisposable interface")
		End Sub

		''' <summary>
        ''' Object is destroyed deterministically by Dispose() method call test.
		''' </summary>
		<TestMethod()> _
		Public Sub DisposeTest()
			Dim target As EditorFactory = editorFactory
            Dim accessor As New Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorFactoryAccessor(target)

			Assert.IsNull(accessor.vsServiceProvider, "Internal service provider object was not disposed properly.")
		End Sub
#End Region

		#Region "IVsEditorFactory tests"
		<TestMethod()> _
		Public Sub IsIVsEditorFactory()
            Dim editorFactory As New EditorFactory()
			Assert.IsNotNull(TryCast(editorFactory, IVsEditorFactory), "The object does not implement IVsEditorFactory")
		End Sub

		<TestMethod()> _
		Public Sub CreateEditorInstanceTest()
            'Create the editor factory.
			Dim targetFactory As EditorFactory = editorFactory

            ' Create a basic service provider.
			Dim serviceProvider As OleServiceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices()
			AddBasicSiteSupport(serviceProvider)

            ' Site the editor factory.
			Assert.AreEqual(0, targetFactory.SetSite(serviceProvider), "SetSite did not return S_OK")

			Dim ppunkDocView As IntPtr
			Dim ppunkDocData As IntPtr
			Dim pbstrEditorCaption As String = String.Empty
			Dim pguidCmdUI As Guid = Guid.Empty
			Dim pgrfCDW As Integer = 0
			Dim cef_option As UInteger = VSConstants.CEF_OPENFILE

			Dim actual_result As Integer = targetFactory.CreateEditorInstance(cef_option, Nothing, Nothing, Nothing, 0, IntPtr.Zero, ppunkDocView, ppunkDocData, pbstrEditorCaption, pguidCmdUI, pgrfCDW)

			Assert.AreEqual(VSConstants.S_OK, actual_result)
		End Sub

		<TestMethod()> _
		Public Sub CreateEditorInstanceWithBadInputsTest()
            'Create the editor factory.
			Dim targetFactory As EditorFactory = editorFactory

            ' Create a basic service provider.
			Dim serviceProvider As OleServiceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices()

            ' Site the editor factory.
			Assert.AreEqual(0, targetFactory.SetSite(serviceProvider), "SetSite did not return S_OK")

			Dim ppunkDocView As IntPtr
			Dim ppunkDocData As IntPtr
			Dim pbstrEditorCaption As String = String.Empty
			Dim pguidCmdUI As Guid = Guid.Empty
			Dim pgrfCDW As Integer = 0
			Dim cef_option As UInteger = 0

            ' Test scenario with invalid CEF_* option.
			Dim punkDocDataExisting As IntPtr = IntPtr.Zero

			Dim actual_result As Integer = targetFactory.CreateEditorInstance(cef_option, Nothing, Nothing, Nothing, 0, punkDocDataExisting, ppunkDocView, ppunkDocData, pbstrEditorCaption, pguidCmdUI, pgrfCDW)
			Assert.AreEqual(VSConstants.E_INVALIDARG, actual_result, "CreateEditorInstance() can not process invalid CEF_* arguments")

			ppunkDocView = IntPtr.Zero
			ppunkDocData = IntPtr.Zero
			pbstrEditorCaption = String.Empty
			pguidCmdUI = Guid.Empty
			pgrfCDW = 0
			cef_option = VSConstants.CEF_OPENFILE

            ' Test scenario with not-null punkDocDataExisting parameter value.
			punkDocDataExisting = New IntPtr(Int32.MaxValue)

			actual_result = targetFactory.CreateEditorInstance(cef_option, Nothing, Nothing, Nothing, 0, punkDocDataExisting, ppunkDocView, ppunkDocData, pbstrEditorCaption, pguidCmdUI, pgrfCDW)
			Assert.AreEqual(VSConstants.VS_E_INCOMPATIBLEDOCDATA, actual_result, "CreateEditorInstance() can not process incompatible document data argument")
		End Sub
		<TestMethod()> _
		Public Sub SetSite()
            'Create the editor factory.
            Dim editorFactory As New EditorFactory()

            ' Create a basic service provider.
			Dim serviceProvider As OleServiceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices()

            ' Site the editor factory.
			Assert.AreEqual(0, editorFactory.SetSite(serviceProvider), "SetSite did not return S_OK")
		End Sub

		''' <summary>
        ''' A test for Close ().
        ''' </summary>
		<TestMethod()> _
		Public Sub CloseTest()
			Dim actual_result As Integer = editorFactory.Close()
			Assert.AreEqual(VSConstants.S_OK, actual_result, "Close(0 method does not return expected S_OK value")
		End Sub

		''' <summary>
        ''' A test for MapLogicalView in case of not supported view.
        ''' </summary>
		<TestMethod()> _
		Public Sub MapLogicalViewNotSupportedIdTest()
			Dim target As EditorFactory = editorFactory

			Dim pbstrPhysicalView As String = String.Empty

            ' Specify a not supported view ID.
			Dim rguidLogicalView As Guid = VSConstants.LOGVIEWID_TextView
			Dim actual_result As Integer = target.MapLogicalView(rguidLogicalView, pbstrPhysicalView)
			Assert.IsNull(pbstrPhysicalView, "pbstrPhysicalView out parameter not initialized by null")
			Assert.AreEqual(VSConstants.E_NOTIMPL, actual_result, "In case of supported view ID was expected E_NOTIMPL result")
		End Sub
		''' <summary>
        ''' A test for MapLogicalView in case of single physical view.
        ''' </summary>
		<TestMethod()> _
		Public Sub MapLogicalViewSupportedIdTest()
			Dim target As EditorFactory = editorFactory

			Dim pbstrPhysicalView As String = String.Empty

            ' Specify a primary physical view.
			Dim rguidLogicalView As Guid = VSConstants.LOGVIEWID_Primary
			Dim actual_result As Integer = target.MapLogicalView(rguidLogicalView, pbstrPhysicalView)
			Assert.IsNull(pbstrPhysicalView, "pbstrPhysicalView out parameter not initialized by null")
			Assert.AreEqual(VSConstants.S_OK, actual_result, "In case of supported view ID was expected S_OK result")
		End Sub
#End Region
#End Region

		#Region "Service functions"
		''' <summary>
		''' Add some basic service mock objects to the service provider.
		''' </summary>
		''' <param name="serviceProvider"></param>
		Public Shared Sub AddBasicSiteSupport(ByVal serviceProvider As OleServiceProvider)
			If serviceProvider Is Nothing Then
				Throw New ArgumentNullException("serviceProvider")
			End If

            ' Add site support for UI Shell.
			Dim uiShell As BaseMock = MockServicesProvider.GetUiShellInstance0()
			serviceProvider.AddService(GetType(SVsUIShell), uiShell, False)
			serviceProvider.AddService(GetType(SVsUIShellOpenDocument), CType(uiShell, IVsUIShellOpenDocument), False)

            ' Add site support for Running Document Table.
			Dim runningDoc As BaseMock = MockServicesProvider.GetRunningDocTableInstance()
			serviceProvider.AddService(GetType(SVsRunningDocumentTable), runningDoc, False)

            ' Add site support for IVsTextManager.
			Dim queryEditQuerySave As BaseMock = MockServicesProvider.GetQueryEditQuerySaveInstance()
			serviceProvider.AddService(GetType(SVsQueryEditQuerySave), queryEditQuerySave, False)

			Dim toolbox As BaseMock = MockIVsToolbox.GetIVsToolboxInstance()
			serviceProvider.AddService(GetType(SVsToolbox), toolbox, False)
		End Sub
#End Region
	End Class
End Namespace
