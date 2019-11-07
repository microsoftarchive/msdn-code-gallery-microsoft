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
Imports Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider

Namespace Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.UnitTests
	<TestClass()> _
	Public Class PackageTest
		<TestMethod()> _
		Public Sub CreateInstance()
            Dim package As New SccProvider()
		End Sub

		<TestMethod()> _
		Public Sub IsIVsPackage()
            Dim package As New SccProvider()
			Assert.IsNotNull(TryCast(package, IVsPackage), "The object does not implement IVsPackage")
		End Sub

		<TestMethod()> _
		Public Sub SetSite()
            ' Create the package.
            Dim package As IVsPackage = TryCast(New SccProvider(), IVsPackage)
			Assert.IsNotNull(package, "The object does not implement IVsPackage")

            ' Create a basic service provider.
			Dim serviceProvider As OleServiceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices()

            ' Need to mock a service implementing IVsRegisterScciProvider, because the scc provider will register with it.
			Dim registerScciProvider As IVsRegisterScciProvider = MockRegisterScciProvider.GetBaseRegisterScciProvider()
			serviceProvider.AddService(GetType(IVsRegisterScciProvider), registerScciProvider, True)

            ' Register solution events because the provider will try to subscribe to them.
            Dim solution As New MockSolution()
			serviceProvider.AddService(GetType(SVsSolution), TryCast(solution, IVsSolution), True)

            ' Register TPD service because the provider will try to subscribe to TPD.
			Dim tpd As IVsTrackProjectDocuments2 = TryCast(MockTrackProjectDocumentsProvider.GetTrackProjectDocuments(), IVsTrackProjectDocuments2)
			serviceProvider.AddService(GetType(SVsTrackProjectDocuments), tpd, True)

            ' Site the package.
			Assert.AreEqual(0, package.SetSite(serviceProvider), "SetSite did not return S_OK")

            ' Unsite the package.
			Assert.AreEqual(0, package.SetSite(Nothing), "SetSite(null) did not return S_OK")
		End Sub
	End Class
End Namespace
