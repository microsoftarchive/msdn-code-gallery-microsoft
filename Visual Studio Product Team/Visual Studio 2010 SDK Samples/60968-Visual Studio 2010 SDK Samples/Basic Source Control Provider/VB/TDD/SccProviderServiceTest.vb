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
Imports System.Collections.Generic
Imports System.Reflection
Imports Microsoft.VisualStudio
Imports Microsoft.VsSDK.UnitTestLibrary
Imports Microsoft.VisualStudio.Shell.Interop
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Microsoft.Samples.VisualStudio.SourceControlIntegration.BasicSccProvider

Namespace Microsoft.Samples.VisualStudio.SourceControlIntegration.BasicSccProvider.UnitTests
	''' <summary>
	''' Wee need this fake implementation of BasicSccProvider in order to override
    ''' the behavior of OnActiveState change.
	''' </summary>
	Friend Class FakeSccProvider
		Inherits BasicSccProvider
		Public Overrides Sub OnActiveStateChange()
			Return
		End Sub
	End Class

	''' <summary>
    ''' This is a test class for Microsoft.Samples.VisualStudio.SourceControlIntegration.BasicSccProvider.SccProviderService and is intended
    ''' to contain all Microsoft.Samples.VisualStudio.SourceControlIntegration.BasicSccProvider.SccProviderService Unit Tests.
    ''' </summary>
	<TestClass()> _
	Public Class SccProviderServiceTest
		''' <summary>
        ''' Creates a SccProviderService object.
        ''' </summary>
		Public ReadOnly Property GetSccProviderServiceInstance() As SccProviderService
			Get
                ' Create a provider package.
                Dim sccProvider As New FakeSccProvider()

				Dim serviceProvider As OleServiceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices()

                ' Need to mock a service implementing IVsRegisterScciProvider, because the scc provider will register with it.
				Dim registerScciProvider As IVsRegisterScciProvider = MockRegisterScciProvider.GetBaseRegisterScciProvider()
				serviceProvider.AddService(GetType(IVsRegisterScciProvider), registerScciProvider, True)

                ' Site the package.
				Dim package As IVsPackage = TryCast(sccProvider, IVsPackage)
				package.SetSite(serviceProvider)

                '  Get the source control provider service object.
				Dim sccServiceMember As FieldInfo = GetType(BasicSccProvider).GetField("sccService", BindingFlags.Instance Or BindingFlags.NonPublic)
				Dim target As SccProviderService = TryCast(sccServiceMember.GetValue(sccProvider), SccProviderService)

				Return target
			End Get
		End Property

		''' <summary>
        ''' A test for SccProviderService creation and interfaces.
        ''' </summary>
		<TestMethod()> _
		Public Sub ConstructorTest()
            Dim sccProvider As New BasicSccProvider()
            Dim target As New SccProviderService(sccProvider)

			Assert.AreNotEqual(Nothing, target, "Could not create provider service")
			Assert.IsNotNull(TryCast(target, IVsSccProvider), "The object does not implement IVsPackage")
		End Sub

		''' <summary>
        ''' A test for Active.
        ''' </summary>
		<TestMethod()> _
		Public Sub ActiveTest()
			Dim target As SccProviderService = GetSccProviderServiceInstance

            ' After the object is created, the provider is inactive.
			Assert.AreEqual(False, target.Active, "Microsoft.Samples.VisualStudio.SourceControlIntegration.BasicSccProvider.SccProviderService.Active was not reported correctly.")

            ' Activate the provider and test the result.
			target.SetActive()
			Assert.AreEqual(True, target.Active, "Microsoft.Samples.VisualStudio.SourceControlIntegration.BasicSccProvider.SccProviderService.Active was not reported correctly.")

            ' Deactivate the provider and test the result.
			target.SetInactive()
			Assert.AreEqual(False, target.Active, "Microsoft.Samples.VisualStudio.SourceControlIntegration.BasicSccProvider.SccProviderService.Active was not reported correctly.")
		End Sub

		''' <summary>
        ''' A test for AnyItemsUnderSourceControl (out int).
        ''' </summary>
		<TestMethod()> _
		Public Sub AnyItemsUnderSourceControlTest()
			Dim target As SccProviderService = GetSccProviderServiceInstance

			Dim pfResult As Integer = 0
			Dim actual As Integer = target.AnyItemsUnderSourceControl(pfResult)

            ' The method is not supposed to fail, and the basic provider cannot control any projects.
			Assert.AreEqual(VSConstants.S_OK, pfResult, "pfResult_AnyItemsUnderSourceControl_expected was not set correctly.")
			Assert.AreEqual(0, actual, "Microsoft.Samples.VisualStudio.SourceControlIntegration.BasicSccProvider.SccProviderService.AnyItemsUnderSourceControl did not return the expected value.")
		End Sub

		''' <summary>
        ''' A test for SetActive ().
        ''' </summary>
		<TestMethod()> _
		Public Sub SetActiveTest()
			Dim target As SccProviderService = GetSccProviderServiceInstance

			Dim actual As Integer = target.SetActive()
			Assert.AreEqual(VSConstants.S_OK, actual, "Microsoft.Samples.VisualStudio.SourceControlIntegration.BasicSccProvider.SccProviderService.SetActive failed.")
		End Sub

		''' <summary>
        ''' A test for SetInactive ().
        ''' </summary>
		<TestMethod()> _
		Public Sub SetInactiveTest()
			Dim target As SccProviderService = GetSccProviderServiceInstance

			Dim actual As Integer = target.SetInactive()
			Assert.AreEqual(VSConstants.S_OK, actual, "Microsoft.Samples.VisualStudio.SourceControlIntegration.BasicSccProvider.SccProviderService.SetInactive failed.")
		End Sub
	End Class
End Namespace
