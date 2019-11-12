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
Imports Microsoft.VisualStudio.Shell.Interop
Imports Microsoft.Samples.VisualStudio.Services
Imports Microsoft.VsSDK.UnitTestLibrary
Imports Microsoft.VisualStudio.TestTools.UnitTesting

Namespace Reference.Services.UnitTest
	<TestClass()> _
	Public Class TestMyGlobalService
		Private mockOutputWindowFactory As GenericMockFactory

		'
        '* We can not run this test against a debug version of the platform because it will
        '* assert and this will let the test fail.
        '[Test()]
        'public void TestNoInitialize()
        '{
        '// Check that the service can handle the fact that the package is not initialize.
        'MyGlobalService service = new MyGlobalService();
        'service.GlobalServiceFunction();
        '}
        '

		<TestMethod()> _
		Public Sub TestOutputNoPane()
			' Create an instance of the package and initialize it so that the GetService
			' will succeed, but the GetPane will fail.

			' As first create a service provider.
			Using serviceProvider As OleServiceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices()
				' Now create the mock object for the output window.
                If mockOutputWindowFactory Is Nothing Then
                    mockOutputWindowFactory = New GenericMockFactory("MockOutputWindow", New Type() {GetType(IVsOutputWindow)})
                End If
				Dim mockBase As BaseMock = TryCast(mockOutputWindowFactory.GetInstance(), BaseMock)
				mockBase.AddMethodReturnValues(String.Format("{0}.{1}", GetType(IVsOutputWindow).FullName, "GetPane"), New Object() { -1, Guid.Empty, Nothing })
				' Add the output window to the services provided by the service provider.
				serviceProvider.AddService(GetType(SVsOutputWindow), mockBase, False)

				' Create an instance of the package and initialize it calling SetSite.
                Dim package As New ServicesPackage()
				Dim result As Integer = (CType(package, IVsPackage)).SetSite(serviceProvider)
				Assert.IsTrue(Microsoft.VisualStudio.ErrorHandler.Succeeded(result), "SetSite failed.")

                ' Now we can create an instance of the service.
                Dim service As New MyGlobalService(package)
				service.GlobalServiceFunction()
				CType(package, IVsPackage).SetSite(Nothing)
				CType(package, IVsPackage).Close()
			End Using
		End Sub

		Private callbackExecuted As Boolean
		Private Sub OutputWindowPaneCallback(ByVal sender As Object, ByVal args As CallbackArgs)
			callbackExecuted = True
            Dim expectedText As String = " ======================================" & Microsoft.VisualBasic.Constants.vbLf & Microsoft.VisualBasic.Constants.vbTab & "GlobalServiceFunction called." & Microsoft.VisualBasic.Constants.vbLf & " ======================================" & Microsoft.VisualBasic.Constants.vbLf
			Dim inputText As String = CStr(args.GetParameter(0))
			Assert.AreEqual(expectedText, inputText, "OutputString called with wrong text.")
			args.ReturnValue = 0
		End Sub
		<TestMethod()> _
		Public Sub TestOutput()
			callbackExecuted = False
			' As first create a service provider.
			Using serviceProvider As OleServiceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices()
				' Create a mock object for the output window pane.
                Dim mockWindowPaneFactory As New GenericMockFactory("MockOutputWindowPane", New Type() {GetType(IVsOutputWindowPane)})
				Dim mockWindowPane As BaseMock = mockWindowPaneFactory.GetInstance()
				mockWindowPane.AddMethodCallback(String.Format("{0}.{1}", GetType(IVsOutputWindowPane).FullName, "OutputString"), New EventHandler(Of CallbackArgs)(AddressOf OutputWindowPaneCallback))

				' Now create the mock object for the output window.
                If mockOutputWindowFactory Is Nothing Then
                    mockOutputWindowFactory = New GenericMockFactory("MockOutputWindow1", New Type() {GetType(IVsOutputWindow)})
                End If
				Dim mockOutputWindow As BaseMock = mockOutputWindowFactory.GetInstance()
				mockOutputWindow.AddMethodReturnValues(String.Format("{0}.{1}", GetType(IVsOutputWindow).FullName, "GetPane"), New Object() { 0, Guid.Empty, CType(mockWindowPane, IVsOutputWindowPane) })

				' Add the output window to the services provided by the service provider.
				serviceProvider.AddService(GetType(SVsOutputWindow), mockOutputWindow, False)

				' Create an instance of the package and initialize it calling SetSite.
                Dim package As New ServicesPackage()
				Dim result As Integer = (CType(package, IVsPackage)).SetSite(serviceProvider)
				Assert.IsTrue(Microsoft.VisualStudio.ErrorHandler.Succeeded(result), "SetSite failed.")

                ' Now we can create an instance of the service.
                Dim service As New MyGlobalService(package)
				service.GlobalServiceFunction()
				Assert.IsTrue(callbackExecuted, "OutputText not called.")
				CType(package, IVsPackage).SetSite(Nothing)
				CType(package, IVsPackage).Close()
			End Using
		End Sub
	End Class
End Namespace
