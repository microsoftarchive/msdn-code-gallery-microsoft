'**************************************************************************

'Copyright (c) Microsoft Corporation. All rights reserved.
'This code is licensed under the Visual Studio SDK license terms.
'THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
'ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
'IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
'PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

'**************************************************************************

' System references.

Imports Microsoft.VisualBasic
Imports System
Imports System.Reflection

' Platform references.
Imports Microsoft.VisualStudio.Shell.Interop

' Unit Test framework.
Imports Microsoft.VsSDK.UnitTestLibrary
Imports Microsoft.VisualStudio.TestTools.UnitTesting

' Assembly to test.
Imports Microsoft.Samples.VisualStudio.ComboBox

Namespace Microsoft.Samples.VisualStudio.ComboBox.UnitTest

	<TestClass()> _
	Public Class ComboBoxPackageTest

		<TestMethod()> _
		Public Sub PackageCreation()
            Dim package As New ComboBoxPackage()
			Assert.IsNotNull(package, "Failed to create package")
		End Sub

		<TestMethod()> _
		Public Sub PackageInitialize()
            Dim packageObject As New ComboBoxPackage()
			Assert.IsNotNull(packageObject, "Failed to create package")
			Dim package As IVsPackage = TryCast(packageObject, IVsPackage)
			Assert.IsNotNull(package, "Can not get the package interface from the package object.")
			Using serviceProvider As OleServiceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices()
				Assert.AreEqual(0, package.SetSite(serviceProvider), "Can not get the package interface from the package object.")
			End Using
		End Sub

		<TestMethod()> _
		Public Sub PackageInitializeWithNull()
            Dim packageObject As New ComboBoxPackage()
			Assert.IsNotNull(packageObject, "Failed to create package")
			Dim package As IVsPackage = TryCast(packageObject, IVsPackage)
			Assert.IsNotNull(package, "Can not get the package interface from the package object.")
			Assert.AreEqual(0, package.SetSite(Nothing), "SetSite returned an unexpected value")
		End Sub

		<TestMethod()> _
		Public Sub PackageResourcesCreation()
            Dim t As System.Type = System.Type.GetType("Resources, Reference.ComboBox")
			Dim resources As Object = System.Activator.CreateInstance(t, True)

			Assert.IsNotNull(resources, "Failed to create resources")

			Dim method_get_Culture As MethodInfo = t.GetMethod("get_Culture", BindingFlags.NonPublic Or BindingFlags.Static)
			Dim result1 As Object = method_get_Culture.Invoke(Nothing, Nothing)

			Dim method_set_Culture As MethodInfo = t.GetMethod("set_Culture", BindingFlags.NonPublic Or BindingFlags.Static)
			Dim result2 As Object = method_set_Culture.Invoke(Nothing, New Object() { System.Globalization.CultureInfo.CurrentCulture })
		End Sub
	End Class
End Namespace
