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
Imports System.ComponentModel.Design
Imports Microsoft.VsSDK.UnitTestLibrary
Imports Microsoft.VisualStudio.Shell.Interop
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Microsoft.Samples.VisualStudio.SourceControlIntegration.BasicSccProvider

Imports MsVsShell = Microsoft.VisualStudio.Shell

Namespace Microsoft.Samples.VisualStudio.SourceControlIntegration.BasicSccProvider.UnitTests
	Public Class TestSccCommand
		Public Sub OnSccCommand(ByVal sender As Object, ByVal e As EventArgs)
		End Sub
	End Class

	<TestClass()> _
	Public Class PackageTest
		<TestMethod()> _
		Public Sub CreateInstance()
            Dim package As New BasicSccProvider()
		End Sub

		<TestMethod()> _
		Public Sub IsIVsPackage()
            Dim package As New BasicSccProvider()
			Assert.IsNotNull(TryCast(package, IVsPackage), "The object does not implement IVsPackage")
		End Sub

		<TestMethod()> _
		Public Sub SetSite()
            ' Create the package.
            Dim package As IVsPackage = TryCast(New BasicSccProvider(), IVsPackage)
			Assert.IsNotNull(package, "The object does not implement IVsPackage")

            ' Create a basic service provider.
			Dim serviceProvider As OleServiceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices()

            ' Need to mock a service implementing IVsRegisterScciProvider, because the scc provider will register with it.
			Dim registerScciProvider As IVsRegisterScciProvider = MockRegisterScciProvider.GetBaseRegisterScciProvider()
			serviceProvider.AddService(GetType(IVsRegisterScciProvider), registerScciProvider, True)

            ' Site the package.
			Assert.AreEqual(0, package.SetSite(serviceProvider), "SetSite did not return S_OK")

            ' Unsite the package.
			Assert.AreEqual(0, package.SetSite(Nothing), "SetSite(null) did not return S_OK")

            ' Remove the mock service.
			serviceProvider.RemoveService(GetType(IVsRegisterScciProvider))
		End Sub


		<TestMethod()> _
		Public Sub TestSccComand()
            ' Create the package.
            Dim package As New BasicSccProvider()
            ' Create a basic service provider.
			Dim serviceProvider As OleServiceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices()

            ' Need to mock a service implementing IVsRegisterScciProvider, because the scc provider will register with it.
			Dim registerScciProvider As IVsRegisterScciProvider = MockRegisterScciProvider.GetBaseRegisterScciProvider()
			serviceProvider.AddService(GetType(IVsRegisterScciProvider), registerScciProvider, True)

            ' Site the package.
			Assert.AreEqual(0, (CType(package, IVsPackage)).SetSite(serviceProvider), "SetSite did not return S_OK")

            ' Test the scc command by toggleing it twice.
			Dim method As MethodInfo = GetType(BasicSccProvider).GetMethod("OnSccCommand", BindingFlags.NonPublic Or BindingFlags.Instance)

            Dim commandWell As New TestSccCommand()
            Dim cmdID As New CommandID(New System.Guid(), 0)
            Dim command As New MenuCommand(New EventHandler(AddressOf commandWell.OnSccCommand), cmdID)
			Dim result As Object = method.Invoke(package, New Object() { command, Nothing })
			Assert.AreEqual(True, command.Checked, "OnSccCommand did not execute correctly")

			result = method.Invoke(package, New Object() { command, Nothing })
			Assert.AreEqual(False, command.Checked, "OnSccCommand did not execute correctly")
		End Sub
	End Class
End Namespace
