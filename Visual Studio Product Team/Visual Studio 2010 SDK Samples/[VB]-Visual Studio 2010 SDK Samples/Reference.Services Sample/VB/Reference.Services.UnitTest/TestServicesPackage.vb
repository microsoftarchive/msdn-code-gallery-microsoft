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
Imports System.Runtime.InteropServices
Imports Microsoft.VisualStudio.Shell.Interop
Imports Microsoft.Samples.VisualStudio.Services
Imports Microsoft.Samples.VisualStudio.Services.Interfaces
Imports Microsoft.VsSDK.UnitTestLibrary
Imports Microsoft.VisualStudio.TestTools.UnitTesting

Namespace Reference.Services.UnitTest
	<TestClass()> _
	Public Class TestServicesPackage
		<TestMethod()> _
		Public Sub SetSiteSimple()
            Dim packageObject As New ServicesPackage()
			Dim package As IVsPackage = CType(packageObject, IVsPackage)
			Using provider As OleServiceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices()
				Dim result As Integer = package.SetSite(provider)
				Assert.IsTrue(Microsoft.VisualStudio.ErrorHandler.Succeeded(result), "SetSite failed.")
			End Using
			package.SetSite(Nothing)
			package.Close()
		End Sub

		<TestMethod()> _
		Public Sub GetGlobalServiceSimple()
            Dim packageObject As New ServicesPackage()
			Dim package As IVsPackage = CType(packageObject, IVsPackage)
			Using provider As OleServiceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices()
				Dim result As Integer = package.SetSite(provider)
				Assert.IsTrue(Microsoft.VisualStudio.ErrorHandler.Succeeded(result), "SetSite failed.")
				Dim serviceProvider As IServiceProvider = TryCast(package, IServiceProvider)
				Dim o As Object = serviceProvider.GetService(GetType(SMyGlobalService))
				Assert.IsNotNull(o, "GetService returned null for the global service.")
				Dim service As IMyGlobalService = TryCast(o, IMyGlobalService)
				Assert.IsNotNull(service, "The service SMyGlobalService does not implements IMyGlobalService.")
				service.GlobalServiceFunction()
			End Using
			package.SetSite(Nothing)
			package.Close()
		End Sub

		<TestMethod()> _
		Public Sub GetGlobalServiceFromNativeProvider()
            Dim packageObject As New ServicesPackage()
			Dim package As IVsPackage = CType(packageObject, IVsPackage)
			Using provider As OleServiceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices()
				Dim result As Integer = package.SetSite(provider)
				Assert.IsTrue(Microsoft.VisualStudio.ErrorHandler.Succeeded(result), "SetSite failed.")
				Dim sp As Microsoft.VisualStudio.OLE.Interop.IServiceProvider = TryCast(package, Microsoft.VisualStudio.OLE.Interop.IServiceProvider)
				Assert.IsNotNull(sp, "The pacckage does not implements the native IServiceProvider")
				Dim guidService As Guid = GetType(SMyGlobalService).GUID
				Dim guidInterface As Guid = GetType(IMyGlobalService).GUID
				Dim ppvObj As IntPtr = IntPtr.Zero
				Dim hr As Integer = sp.QueryService(guidService, guidInterface, ppvObj)
				Assert.IsTrue(Microsoft.VisualStudio.ErrorHandler.Succeeded(hr), "QueryService failed for the global service.")
				Assert.IsTrue(ppvObj <> IntPtr.Zero, "QueryService returned a NULL pointer for the global service.")
				Marshal.Release(ppvObj)
			End Using
			package.SetSite(Nothing)
			package.Close()
		End Sub

		<TestMethod()> _
		Public Sub GetLocalServiceSimple()
            Dim packageObject As New ServicesPackage()
			Dim package As IVsPackage = CType(packageObject, IVsPackage)
			Using provider As OleServiceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices()
				Dim result As Integer = package.SetSite(provider)
				Assert.IsTrue(Microsoft.VisualStudio.ErrorHandler.Succeeded(result), "SetSite failed.")
				Dim serviceProvider As IServiceProvider = TryCast(package, IServiceProvider)
				Dim o As Object = serviceProvider.GetService(GetType(SMyLocalService))
				Assert.IsNotNull(o, "GetService returned null for the local service.")
				Dim service As IMyLocalService = TryCast(o, IMyLocalService)
				Assert.IsNotNull(service, "The service SMyLocalService does not implements IMyLocalService.")
				service.LocalServiceFunction()
			End Using
			package.SetSite(Nothing)
			package.Close()
		End Sub

		<TestMethod()> _
		Public Sub GetLocalServiceFromNativeProvider()
            Dim packageObject As New ServicesPackage()
			Dim package As IVsPackage = CType(packageObject, IVsPackage)
			Using provider As OleServiceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices()
				Dim result As Integer = package.SetSite(provider)
				Assert.IsTrue(Microsoft.VisualStudio.ErrorHandler.Succeeded(result), "SetSite failed.")
				Dim sp As Microsoft.VisualStudio.OLE.Interop.IServiceProvider = TryCast(package, Microsoft.VisualStudio.OLE.Interop.IServiceProvider)
				Assert.IsNotNull(sp, "The package does not implements the native IServiceProvider")
				Dim guidService As Guid = GetType(SMyLocalService).GUID
				Dim guidInterface As Guid = GetType(IMyLocalService).GUID
				Dim ppvObj As IntPtr = IntPtr.Zero
				Dim hr As Integer = sp.QueryService(guidService, guidInterface, ppvObj)
				Assert.IsTrue(Microsoft.VisualStudio.ErrorHandler.Succeeded(hr), "QueryService failed for the local service.")
				Assert.IsTrue(ppvObj <> IntPtr.Zero, "QueryService returned a NULL pointer for the local service.")
				Marshal.Release(ppvObj)
			End Using
			package.SetSite(Nothing)
			package.Close()
		End Sub
	End Class
End Namespace
