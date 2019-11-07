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
Imports System.Windows.Forms
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Microsoft.VsSDK.UnitTestLibrary
Imports Microsoft.VisualStudio
Imports Microsoft.VisualStudio.Shell.Interop
Imports Microsoft.Samples.VisualStudio.IDE.ToolWindow

Namespace Microsoft.Samples.VisualStudio.IDE.ToolWindow.UnitTests
	<TestClass()> _
	Public Class DynamicWindowTest
		<TestMethod()> _
		Public Sub ShowDynamicWindow0()
            ' Create the package.
            Dim package As New PackageToolWindow()
            ' Create a basic service provider.
			Dim serviceProvider As OleServiceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices()

            ' Add site support to create and enumerate tool windows.
			Dim uiShell As BaseMock = MockUiShellProvider.GetWindowEnumerator0()
			serviceProvider.AddService(GetType(SVsUIShell), uiShell, False)

            ' Site the package.
			Assert.AreEqual(0, (CType(package, IVsPackage)).SetSite(serviceProvider), "SetSite did not return S_OK")

			Dim method As MethodInfo = GetType(PackageToolWindow).GetMethod("ShowDynamicWindow", BindingFlags.NonPublic Or BindingFlags.Instance)

			Dim result As Object = method.Invoke(package, New Object() { Nothing, Nothing })
		End Sub
	End Class
End Namespace
