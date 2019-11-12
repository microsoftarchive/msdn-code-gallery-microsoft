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
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Microsoft.Samples.VisualStudio.ComboBox
Imports System.Reflection
Imports Microsoft.VisualStudio.Shell
Imports System.Runtime.InteropServices
Imports Microsoft.VsSDK.UnitTestLibrary
Imports Microsoft.VisualStudio.Shell.Interop
Imports System.Globalization

Namespace ComboBox.UnitTest
	''' <summary>
    ''' Summary description for TestMRUComboCommands.
	''' </summary>
	<TestClass> _
	Public Class TestMRUComboCommands
		Public Sub New()
		End Sub

		#Region "Additional test attributes"
		'
		' You can use the following additional attributes as you write your tests:
		'
		' Use ClassInitialize to run code before running the first test in the class
		' [ClassInitialize()]
		' public static void MyClassInitialize(TestContext testContext) { }
		'
		' Use ClassCleanup to run code after all tests in a class have run
		' [ClassCleanup()]
		' public static void MyClassCleanup() { }
		'
		' Use TestInitialize to run code before running each test 
		' [TestInitialize()]
		' public void MyTestInitialize() { }
		'
		' Use TestCleanup to run code after each test has run
		' [TestCleanup()]
		' public void MyTestCleanup() { }
		'
		#End Region
        Private NotInheritable Class AnonymousClass1
            Public packageObject As New ComboBoxPackage()
            Public method As MethodInfo = GetType(ComboBoxPackage).GetMethod("OnMenuMyMRUCombo", BindingFlags.NonPublic Or BindingFlags.Instance)
            Public Sub AnonymousMethod()
                method.Invoke(packageObject, New Object() {Nothing, Nothing})
            End Sub
        End Class
        <TestMethod()> _
        Public Sub TestMRUComboNoParams()
            Dim locals As New AnonymousClass1()
            Assert.IsNotNull(locals.packageObject, "Failed to create package")
            Assert.IsTrue(Utilities.HasFunctionThrown(Of ArgumentException)(AddressOf locals.AnonymousMethod))
        End Sub
        Private NotInheritable Class AnonymousClass2
            Public packageObject As New ComboBoxPackage()
            Public method As MethodInfo = GetType(ComboBoxPackage).GetMethod("OnMenuMyMRUCombo", BindingFlags.NonPublic Or BindingFlags.Instance)
            Public inParam As Object = "200"
            ' 64 == size of a variant + a little extra padding;
            Public outParam As IntPtr = Marshal.AllocCoTaskMem(64)
            Public eventArgs As New OleMenuCmdEventArgs(inParam, outParam)
            Public Sub AnonymousMethod()
                method.Invoke(packageObject, New Object() {Nothing, eventArgs})
            End Sub

        End Class
		<TestMethod> _
		Public Sub TestMRUComboBothInOutParamsGiven()

            Dim locals As New AnonymousClass2()
            Assert.IsNotNull(locals.packageObject, "Failed to create package")
            Try
                Assert.IsTrue(Utilities.HasFunctionThrown(Of ArgumentException)(AddressOf locals.AnonymousMethod))
            Finally
                If locals.outParam <> IntPtr.Zero Then
                    Marshal.FreeCoTaskMem(locals.outParam)
                End If
            End Try
		End Sub
        Private NotInheritable Class AnonymousClass3
            Public packageObject As New ComboBoxPackage()
            Public method As MethodInfo = GetType(ComboBoxPackage).GetMethod("OnMenuMyMRUCombo", BindingFlags.NonPublic Or BindingFlags.Instance)
            Public inParam As Object = String.Empty
            Public outParam As IntPtr = IntPtr.Zero
            Public eventArgs As New OleMenuCmdEventArgs(inParam, outParam)
            Public Sub AnonymousMethod()
                method.Invoke(packageObject, New Object() {Nothing, eventArgs})
            End Sub
        End Class
		<TestMethod> _
		Public Sub TestMRUComboInvalidInParamEmptyString()
            ' NOTE: invalid input is ignored and treated as a NOP.
            Dim locals As New AnonymousClass3()
            Assert.IsNotNull(locals.packageObject, "Failed to create package")
            Assert.IsTrue(Utilities.HasFunctionThrown(Of ArgumentException)(AddressOf locals.AnonymousMethod))
		End Sub

		<TestMethod> _
		Public Sub TestMRUComboInParamNumber()
            Dim packageObject As New ComboBoxPackage()
			Assert.IsNotNull(packageObject, "Failed to create package")
			Dim serviceProvider As OleServiceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices()

            ' Add site support to create and enumerate tool windows.
            Dim mockFactory As New GenericMockFactory("MockUIShell", New Type() {GetType(IVsUIShell)})
			Dim uiShell As BaseMock = mockFactory.GetInstance()
			serviceProvider.AddService(GetType(SVsUIShell), uiShell, False)

            ' Site the package.
			Assert.AreEqual(0, (CType(packageObject, IVsPackage)).SetSite(serviceProvider), "SetSite did not return S_OK")

			Dim method As MethodInfo = GetType(ComboBoxPackage).GetMethod("OnMenuMyMRUCombo", BindingFlags.NonPublic Or BindingFlags.Instance)

			Dim inParam1 As Object = 42
			Dim outParam1 As IntPtr = IntPtr.Zero
            Dim inParam2 As Object = Nothing
            ' 64 == size of a variant + a little extra padding
            Dim outParam2 As IntPtr = Marshal.AllocCoTaskMem(64)
			Try
                Dim eventArgs As New OleMenuCmdEventArgs(inParam1, outParam1)
				Dim result As Object = method.Invoke(packageObject, New Object() { Nothing, eventArgs })

                ' Retrieve current value of Zoom and verify.
                Dim eventArgs2 As New OleMenuCmdEventArgs(inParam2, outParam2)
				result = method.Invoke(packageObject, New Object() { Nothing, eventArgs2 })

				Dim retrieved As String = CStr(Marshal.GetObjectForNativeVariant(outParam2))
				Assert.AreEqual(Of String)(inParam1.ToString(), retrieved)
				Assert.AreEqual(1, uiShell.FunctionCalls(String.Format("{0}.{1}", GetType(IVsUIShell).FullName, "ShowMessageBox")), "IVsUIShell.ShowMessageBox was not called")
			Finally
			End Try
		End Sub
        Private NotInheritable Class AnonymousClass4
            Public packageObject As New ComboBoxPackage()
            Public method As MethodInfo = GetType(ComboBoxPackage).GetMethod("OnMenuMyMRUCombo", BindingFlags.NonPublic Or BindingFlags.Instance)
            Public inParam As Object = Nothing
            Public outParam As IntPtr = IntPtr.Zero
            Public eventArgs As New OleMenuCmdEventArgs(inParam, outParam)
            Public Sub AnonymousMethod()
                method.Invoke(packageObject, New Object() {Nothing, eventArgs})
            End Sub
        End Class
		<TestMethod> _
		Public Sub TestMRUComboNoInOutParams()
            Dim locals As New AnonymousClass4()
            Assert.IsNotNull(locals.packageObject, "Failed to create package")
            Assert.IsTrue(Utilities.HasFunctionThrown(Of ArgumentException)(AddressOf locals.AnonymousMethod))
		End Sub
        Private NotInheritable Class AnonymousClass5
            Public packageObject As New ComboBoxPackage()
            Public method As MethodInfo = GetType(ComboBoxPackage).GetMethod("OnMenuMyMRUCombo", BindingFlags.NonPublic Or BindingFlags.Instance)
            Public Sub AnonymousMethod()
                method.Invoke(packageObject, New Object() {Nothing, EventArgs.Empty})
            End Sub
        End Class

        <TestMethod()> _
        Public Sub TestMRUComboEmptyEventArgs()
            Dim locals As New AnonymousClass5()
            Assert.IsNotNull(locals.packageObject, "Failed to create package")

            Assert.IsTrue(Utilities.HasFunctionThrown(Of ArgumentException)(AddressOf locals.AnonymousMethod))
        End Sub

        <TestMethod()> _
        Public Sub TestMRUComboGetCurVal()
            Dim packageObject As New ComboBoxPackage()
            Assert.IsNotNull(packageObject, "Failed to create package")
            Dim method As MethodInfo = GetType(ComboBoxPackage).GetMethod("OnMenuMyMRUCombo", BindingFlags.NonPublic Or BindingFlags.Instance)

            Dim inParam As Object = Nothing
            ' 64 == size of a variant + a little extra padding
            Dim outParam As IntPtr = Marshal.AllocCoTaskMem(64)
            Try
                Dim eventArgs As New OleMenuCmdEventArgs(inParam, outParam)
                Dim result As Object = method.Invoke(packageObject, New Object() {Nothing, eventArgs})

                Dim retrieved As String = CStr(Marshal.GetObjectForNativeVariant(outParam))
            Finally
                If outParam <> IntPtr.Zero Then
                    Marshal.FreeCoTaskMem(outParam)
                End If
            End Try
        End Sub

        <TestMethod()> _
        Public Sub TestMRUComboSetCurValWithString()
            Dim packageObject As New ComboBoxPackage()
            Assert.IsNotNull(packageObject, "Failed to create package")
            Dim serviceProvider As OleServiceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices()

            ' Add site support to create and enumerate tool windows.
            Dim mockFactory As New GenericMockFactory("MockUIShell", New Type() {GetType(IVsUIShell)})
            Dim uiShell As BaseMock = mockFactory.GetInstance()
            serviceProvider.AddService(GetType(SVsUIShell), uiShell, False)

            ' Site the package.
            Assert.AreEqual(0, (CType(packageObject, IVsPackage)).SetSite(serviceProvider), "SetSite did not return S_OK")

            Dim method As MethodInfo = GetType(ComboBoxPackage).GetMethod("OnMenuMyMRUCombo", BindingFlags.NonPublic Or BindingFlags.Instance)

            Dim inParam1 As Object = DateTime.Now.ToString()
            Dim outParam1 As IntPtr = IntPtr.Zero
            Dim inParam2 As Object = Nothing
            ' 64 == size of a variant + a little extra padding
            Dim outParam2 As IntPtr = Marshal.AllocCoTaskMem(64)
            Try
                ' Set MRUCombo to 2nd choice in list.
                Dim eventArgs1 As New OleMenuCmdEventArgs(inParam1, outParam1)
                Dim result As Object = method.Invoke(packageObject, New Object() {Nothing, eventArgs1})

                ' Retrieve current value of Zoom and verify.
                Dim eventArgs2 As New OleMenuCmdEventArgs(inParam2, outParam2)
                result = method.Invoke(packageObject, New Object() {Nothing, eventArgs2})

                Dim retrieved As String = CStr(Marshal.GetObjectForNativeVariant(outParam2))
                Assert.AreEqual(Of String)(inParam1.ToString(), retrieved)

                Assert.AreEqual(1, uiShell.FunctionCalls(String.Format("{0}.{1}", GetType(IVsUIShell).FullName, "ShowMessageBox")), "IVsUIShell.ShowMessageBox was not called")
            Finally
                If outParam2 <> IntPtr.Zero Then
                    Marshal.FreeCoTaskMem(outParam2)
                End If
            End Try
        End Sub

        <TestMethod()> _
        Public Sub TestMRUComboSetCurValWithInt()
            Dim packageObject As New ComboBoxPackage()
            Assert.IsNotNull(packageObject, "Failed to create package")
            Dim serviceProvider As OleServiceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices()

            ' Add site support to create and enumerate tool windows.
            Dim mockFactory As New GenericMockFactory("MockUIShell", New Type() {GetType(IVsUIShell)})
            Dim uiShell As BaseMock = mockFactory.GetInstance()
            serviceProvider.AddService(GetType(SVsUIShell), uiShell, False)

            ' Site the package.
            Assert.AreEqual(0, (CType(packageObject, IVsPackage)).SetSite(serviceProvider), "SetSite did not return S_OK")

            Dim method As MethodInfo = GetType(ComboBoxPackage).GetMethod("OnMenuMyMRUCombo", BindingFlags.NonPublic Or BindingFlags.Instance)

            Dim inChoice As Integer = 17
            Dim inParam1 As Object = inChoice
            Dim outParam1 As IntPtr = IntPtr.Zero
            Dim inParam2 As Object = Nothing
            ' 64 == size of a variant + a little extra padding
            Dim outParam2 As IntPtr = Marshal.AllocCoTaskMem(64)
            Try
                ' Set MRUCombo to 17 %.
                Dim eventArgs1 As New OleMenuCmdEventArgs(inParam1, outParam1)
                Dim result As Object = method.Invoke(packageObject, New Object() {Nothing, eventArgs1})

                ' Retrieve current value of ZoomLevel and verify it is "17 %".
                Dim eventArgs2 As New OleMenuCmdEventArgs(inParam2, outParam2)
                result = method.Invoke(packageObject, New Object() {Nothing, eventArgs2})

                Dim retrieved As String = CStr(Marshal.GetObjectForNativeVariant(outParam2))
                Assert.AreEqual(Of String)(String.Format("{0}", inChoice), retrieved)

                Assert.AreEqual(1, uiShell.FunctionCalls(String.Format("{0}.{1}", GetType(IVsUIShell).FullName, "ShowMessageBox")), "IVsUIShell.ShowMessageBox was not called")
            Finally
                If outParam2 <> IntPtr.Zero Then
                    Marshal.FreeCoTaskMem(outParam2)
                End If
            End Try
        End Sub

        <TestMethod()> _
        Public Sub TestMRUComboSetCurValWithDouble()
            Dim packageObject As New ComboBoxPackage()
            Assert.IsNotNull(packageObject, "Failed to create package")
            Dim serviceProvider As OleServiceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices()

            ' Add site support to create and enumerate tool windows.
            Dim mockFactory As New GenericMockFactory("MockUIShell", New Type() {GetType(IVsUIShell)})
            Dim uiShell As BaseMock = mockFactory.GetInstance()
            serviceProvider.AddService(GetType(SVsUIShell), uiShell, False)

            ' Site the package.
            Assert.AreEqual(0, (CType(packageObject, IVsPackage)).SetSite(serviceProvider), "SetSite did not return S_OK")

            Dim method As MethodInfo = GetType(ComboBoxPackage).GetMethod("OnMenuMyMRUCombo", BindingFlags.NonPublic Or BindingFlags.Instance)

            Dim inChoice As Double = 82.756
            Dim inParam1 As Object = inChoice
            Dim outParam1 As IntPtr = IntPtr.Zero
            Dim inParam2 As Object = Nothing
            ' 64 == size of a variant + a little extra padding.
            Dim outParam2 As IntPtr = Marshal.AllocCoTaskMem(64)
            Try
                ' Set MRUCombo to 82.756.
                Dim eventArgs1 As New OleMenuCmdEventArgs(inParam1, outParam1)
                Dim result As Object = method.Invoke(packageObject, New Object() {Nothing, eventArgs1})

                ' Retrieve current value of Zoom Level and verify it is "83".
                Dim eventArgs2 As New OleMenuCmdEventArgs(inParam2, outParam2)
                result = method.Invoke(packageObject, New Object() {Nothing, eventArgs2})

                Dim retrieved As String = CStr(Marshal.GetObjectForNativeVariant(outParam2))
                Assert.AreEqual(Of String)(String.Format("{0}", inChoice), retrieved)

                Assert.AreEqual(1, uiShell.FunctionCalls(String.Format("{0}.{1}", GetType(IVsUIShell).FullName, "ShowMessageBox")), "IVsUIShell.ShowMessageBox was not called")
            Finally
                If outParam2 <> IntPtr.Zero Then
                    Marshal.FreeCoTaskMem(outParam2)
                End If
            End Try
        End Sub

        <TestMethod()> _
        Public Sub TestMRUComboSetCurValWithNegativeInt()
            Dim packageObject As New ComboBoxPackage()
            Assert.IsNotNull(packageObject, "Failed to create package")
            Dim serviceProvider As OleServiceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices()

            ' Add site support to create and enumerate tool windows.
            Dim mockFactory As New GenericMockFactory("MockUIShell", New Type() {GetType(IVsUIShell)})
            Dim uiShell As BaseMock = mockFactory.GetInstance()
            serviceProvider.AddService(GetType(SVsUIShell), uiShell, False)

            ' Site the package.
            Assert.AreEqual(0, (CType(packageObject, IVsPackage)).SetSite(serviceProvider), "SetSite did not return S_OK")

            Dim method As MethodInfo = GetType(ComboBoxPackage).GetMethod("OnMenuMyMRUCombo", BindingFlags.NonPublic Or BindingFlags.Instance)

            Dim inChoice As Integer = -1
            Dim inParam1 As Object = inChoice
            Dim outParam1 As IntPtr = IntPtr.Zero
            Dim inParam2 As Object = Nothing
            ' 64 == size of a variant + a little extra padding
            Dim outParam2 As IntPtr = Marshal.AllocCoTaskMem(64)
            Try
                ' Set MRUCombo to -1.
                Dim eventArgs1 As New OleMenuCmdEventArgs(inParam1, outParam1)
                Dim result As Object = method.Invoke(packageObject, New Object() {Nothing, eventArgs1})

                Dim eventArgs2 As New OleMenuCmdEventArgs(inParam2, outParam2)
                result = method.Invoke(packageObject, New Object() {Nothing, eventArgs2})

                Dim retrieved As String = CStr(Marshal.GetObjectForNativeVariant(outParam2))
                Assert.AreEqual(Of String)(String.Format("{0}", inChoice), retrieved)

                Assert.AreEqual(1, uiShell.FunctionCalls(String.Format("{0}.{1}", GetType(IVsUIShell).FullName, "ShowMessageBox")), "IVsUIShell.ShowMessageBox was not called")
            Finally
                If outParam2 <> IntPtr.Zero Then
                    Marshal.FreeCoTaskMem(outParam2)
                End If
            End Try
        End Sub

        <TestMethod()> _
        Public Sub TestMRUComboSetCurValWithOverflowInt()
            Dim packageObject As New ComboBoxPackage()
            Assert.IsNotNull(packageObject, "Failed to create package")
            Dim serviceProvider As OleServiceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices()

            ' Add site support to create and enumerate tool windows.
            Dim mockFactory As New GenericMockFactory("MockUIShell", New Type() {GetType(IVsUIShell)})
            Dim uiShell As BaseMock = mockFactory.GetInstance()
            serviceProvider.AddService(GetType(SVsUIShell), uiShell, False)

            ' Site the package.
            Assert.AreEqual(0, (CType(packageObject, IVsPackage)).SetSite(serviceProvider), "SetSite did not return S_OK")

            Dim method As MethodInfo = GetType(ComboBoxPackage).GetMethod("OnMenuMyMRUCombo", BindingFlags.NonPublic Or BindingFlags.Instance)

            Dim inChoice As Int64 = Int64.MaxValue
            Dim inParam1 As Object = inChoice
            Dim outParam1 As IntPtr = IntPtr.Zero
            Dim inParam2 As Object = Nothing
            ' 64 == size of a variant + a little extra padding
            Dim outParam2 As IntPtr = Marshal.AllocCoTaskMem(64)
            Try
                ' Set MRUCombo to overflow value.
                Dim eventArgs1 As New OleMenuCmdEventArgs(inParam1, outParam1)
                Dim result As Object = method.Invoke(packageObject, New Object() {Nothing, eventArgs1})

                ' Retrieve current value of Zoom and verify.
                Dim eventArgs2 As New OleMenuCmdEventArgs(inParam2, outParam2)
                result = method.Invoke(packageObject, New Object() {Nothing, eventArgs2})

                Dim retrieved As String = CStr(Marshal.GetObjectForNativeVariant(outParam2))
                Assert.AreEqual(Of String)(inParam1.ToString(), retrieved)
                Assert.AreEqual(1, uiShell.FunctionCalls(String.Format("{0}.{1}", GetType(IVsUIShell).FullName, "ShowMessageBox")), "IVsUIShell.ShowMessageBox was not called")
            Finally
                If outParam2 <> IntPtr.Zero Then
                    Marshal.FreeCoTaskMem(outParam2)
                End If
            End Try
        End Sub
    End Class
End Namespace
