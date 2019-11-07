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
Imports Microsoft.Samples.VisualStudio.IDE.OptionsPage
Imports Microsoft.VisualStudio.Shell.Interop
Imports Microsoft.VisualStudio
Imports Microsoft.VsSDK.UnitTestLibrary

Namespace Microsoft.Samples.VisualStudio.IDE.OptionsPage.UnitTests
	''' <summary>
    ''' This is a test class for Microsoft.Samples.VisualStudio.IDE.OptionsPage.OptionsPagePackage and is intended
    ''' to contain all Microsoft.Samples.VisualStudio.IDE.OptionsPage.OptionsPagePackage Unit Tests.
    ''' </summary>
	<TestClass()> _
	Public Class OptionsPagePackageTest
		Implements IDisposable
        ' Fields
		#Region "Fields"
		Private testContextInstance As TestContext
        Private testPackage As OptionsPagePackageVB
        Private serviceProvider As OleServiceProvider
#End Region

        ' Properties
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

        ' Initialization && Cleanup
#Region "Initialization && Cleanup"
        ''' <summary>
        ''' Runs before the test to allocate and configure resources needed 
        ''' by all tests in the test class.
        ''' </summary>
        <TestInitialize()> _
              Public Sub OptionsPagePackageTestInitialize()
            testPackage = New OptionsPagePackageVB()

            ' Create a basic service provider.
            serviceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices()
            AddBasicSiteSupport(serviceProvider)

            CType(testPackage, IVsPackage).SetSite(serviceProvider)
        End Sub
        ''' <summary>
        ''' Runs after the test has run and to free resources obtained 
        ''' by all the tests in the test class.
        ''' </summary>
        <TestCleanup()> _
              Public Sub OptionsPagePackageTestCleanup()
            testPackage = Nothing
            serviceProvider = Nothing
            Dispose()
        End Sub
#End Region

#Region "Test methods"
        ''' <summary>
        ''' The test for OptionsPagePackage() default constructor.
        '''</summary>
        <TestMethod()> _
              Public Sub OptionsPagePackageConstructorTest()
            Dim target As New OptionsPagePackageVB()
            Assert.IsNotNull(target, "Instance of the OptionsPagePackage object was not created successfully.")
        End Sub

        ''' <summary>
        ''' The test for Initialize() method.
        '''</summary>
        <TestMethod()> _
              Public Sub InitializeTest()
            Dim target As OptionsPagePackageVB = testPackage
            Dim accessor As New Microsoft_Samples_VisualStudio_IDE_OptionsPage_OptionsPagePackageAccessor(target)
            accessor.Initialize()
            Assert.IsNotNull(target, "Instance of the OptionsPagePackage object was not initialized successfully.")
        End Sub
        ''' <summary>
        ''' The test for ExportSettings() method.
        ''' </summary>
        <TestMethod()> _
              Public Sub ExportSettingsTest()
            Dim target As OptionsPagePackageVB = testPackage
            Dim pszCategoryGUID As String = Nothing
            Dim pSettings As IVsSettingsWriter = Nothing
            Dim actual As Integer = target.ExportSettings(pszCategoryGUID, pSettings)

            Assert.AreEqual(VSConstants.S_OK, actual, "ExportSettings() method was returned unexpected value. Expected S_OK.")
        End Sub
        ''' <summary>
        ''' The test for ImportSettings() method.
        ''' </summary>
        <TestMethod()> _
              Public Sub ImportSettingsTest()
            Dim target As OptionsPagePackageVB = testPackage
            Dim pszCategoryGUID As String = Nothing
            Dim pSettings As IVsSettingsReader = Nothing
            Dim flags As UInteger = 0
            Dim pfRestartRequired As Integer = 0

            Dim actual As Integer = target.ImportSettings(pszCategoryGUID, pSettings, flags, pfRestartRequired)

            Assert.AreEqual(VSConstants.S_OK, actual, "ExportSettings() method was returned unexpected value. Expected S_OK.")
        End Sub

        ' Test methods
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
                If testPackage IsNot Nothing Then
                    testPackage = Nothing
                End If
                If serviceProvider IsNot Nothing Then
                    serviceProvider = Nothing
                End If
				GC.SuppressFinalize(Me)
			End If
		End Sub
        ' IDisposable Pattern implementation.
		#End Region 

		#Region "Service functions"

		Public Shared Sub AddBasicSiteSupport(ByVal serviceProvider As OleServiceProvider)
			If serviceProvider Is Nothing Then
				Throw New ArgumentException("serviceProvider")
			End If

			' Add solution Support.
			Dim solution As BaseMock = MockServiceProvider.GetUserSettingsFactoryInstance()
			serviceProvider.AddService(GetType(IVsUserSettings), solution, False)
		End Sub
        ' Service functions.
		#End Region 
	End Class
End Namespace
