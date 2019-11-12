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
Imports System.Collections
Imports System.Text
Imports System.Reflection
Imports Microsoft.VsSDK.UnitTestLibrary
Imports Microsoft.VisualStudio.Shell.Interop
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Microsoft.Samples.VisualStudio.SourceControlIntegration.BasicSccProvider

Namespace Microsoft.Samples.VisualStudio.SourceControlIntegration.BasicSccProvider.UnitTests
	<TestClass()> _
	Public Class SccProviderToolWindowTest
		<TestMethod()> _
		Public Sub UseToolWindow()
            ' Create the package.
            Dim package As New BasicSccProvider()
            ' Create a basic service provider.
			Dim serviceProvider As OleServiceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices()

            ' Need to mock a service implementing IVsRegisterScciProvider, because the scc provider will register with it.
			Dim registerScciProvider As IVsRegisterScciProvider = MockRegisterScciProvider.GetBaseRegisterScciProvider()
			serviceProvider.AddService(GetType(IVsRegisterScciProvider), registerScciProvider, True)

            ' Add site support to create and enumerate tool windows.
			Dim uiShell As BaseMock = MockUiShellProvider.GetWindowEnumerator0()
			serviceProvider.AddService(GetType(SVsUIShell), uiShell, False)

            ' Site the package.
			Assert.AreEqual(0, (CType(package, IVsPackage)).SetSite(serviceProvider), "SetSite did not return S_OK")

            ' Test that toolwindow can be created.
			Dim method As MethodInfo = GetType(BasicSccProvider).GetMethod("ViewToolWindow", BindingFlags.NonPublic Or BindingFlags.Instance)
			Dim result As Object = method.Invoke(package, New Object() { Nothing, Nothing })

            ' Test that toolwindow toolbar's command can be executed.
			method = GetType(BasicSccProvider).GetMethod("ToolWindowToolbarCommand", BindingFlags.NonPublic Or BindingFlags.Instance)
			result = method.Invoke(package, New Object() { Nothing, Nothing })

            ' Toggle the toolwindow color back.
			method = GetType(BasicSccProvider).GetMethod("ToolWindowToolbarCommand", BindingFlags.NonPublic Or BindingFlags.Instance)
			result = method.Invoke(package, New Object() { Nothing, Nothing })

            ' Get the window and test the dispose function.
			Dim window As SccProviderToolWindow = CType(package.FindToolWindow(GetType(SccProviderToolWindow), 0, True), SccProviderToolWindow)
			method = GetType(SccProviderToolWindow).GetMethod("Dispose", BindingFlags.NonPublic Or BindingFlags.Instance)
			result = method.Invoke(window, New Object() { True })
		End Sub
	End Class
End Namespace
