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
Imports Microsoft.Samples.VisualStudio.MenuCommands

Namespace Microsoft.Samples.VisualStudio.MenuCommands.UnitTest

    <TestClass()> _
    Public Class MenuAndCommandPackageTest

        <TestMethod()> _
        Public Sub PackageCreation()
            Dim package As New MenuCommandsPackage()
            Assert.IsNotNull(package, "Failed to create package")
        End Sub

        <TestMethod()> _
        Public Sub PackageInitialize()
            Dim packageObject As New MenuCommandsPackage()
            Assert.IsNotNull(packageObject, "Failed to create package")
            Dim package As IVsPackage = TryCast(packageObject, IVsPackage)
            Assert.IsNotNull(package, "Can not get the package interface from the package object.")
            Using serviceProvider As OleServiceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices()
                Assert.AreEqual(0, package.SetSite(serviceProvider), "Can not get the package interface from the package object.")
            End Using
        End Sub

        <TestMethod()> _
        Public Sub PackageInitializeWithNull()
            Dim packageObject As New MenuCommandsPackage()
            Assert.IsNotNull(packageObject, "Failed to create package")
            Dim package As IVsPackage = TryCast(packageObject, IVsPackage)
            Assert.IsNotNull(package, "Can not get the package interface from the package object.")
            Assert.AreEqual(0, package.SetSite(Nothing), "SetSite returned an unexpected value")
        End Sub
    End Class

End Namespace
