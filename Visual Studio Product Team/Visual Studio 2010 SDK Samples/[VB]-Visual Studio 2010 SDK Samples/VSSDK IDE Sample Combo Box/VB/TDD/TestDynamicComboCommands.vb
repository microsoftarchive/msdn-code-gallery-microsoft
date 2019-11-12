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
'using System.Text;
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
    ''' Summary description for TestDynamicComboCommands.
	''' </summary>
	<TestClass> _
	Public Class TestDynamicComboCommands
		Private numericZoomLevels As Double() = { 4.0, 3.0, 2.0, 1.5, 1.25, 1.0,.75,.66,.50,.33,.25,.10 }
		Private zoomToFit As String = "ZoomToFit"
		Private zoom_to_Fit As String = "Zoom to Fit"
		Private zoomLevels As String() = Nothing
        Private expectedInitialZoomFactor As String = "100 %"

		Public Sub New()
			Dim numberFormat As NumberFormatInfo = CType(CultureInfo.CurrentUICulture.NumberFormat.Clone(), NumberFormatInfo)
			zoomLevels = New String(numericZoomLevels.Length){}
			For i As Integer = 0 To numericZoomLevels.Length - 1
                zoomLevels(i) = numericZoomLevels(i).ToString("P0", numberFormat)
			Next i

			zoomLevels(zoomLevels.Length - 1) = zoom_to_Fit
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
            Public method As MethodInfo = GetType(ComboBoxPackage).GetMethod("OnMenuMyDynamicCombo", BindingFlags.NonPublic Or BindingFlags.Instance)
            Public args As New EventArgs()
            Public Sub AnonymousMethod1()
                method.Invoke(packageObject, New Object() {Nothing, Nothing})
            End Sub
            Public Sub AnonymousMethod2()
                method.Invoke(packageObject, New Object() {Nothing, args})
            End Sub
        End Class

        <TestMethod()> _
        Public Sub TestDynamicComboNoParams()
            Dim locals As New AnonymousClass1()
            Assert.IsNotNull(locals.packageObject, "Failed to create package")
            Dim hasThrown As Boolean = Utilities.HasFunctionThrown(Of ArgumentException)(AddressOf locals.AnonymousMethod1)
            Assert.IsTrue(hasThrown)
            hasThrown = Utilities.HasFunctionThrown(Of ArgumentException)(AddressOf locals.AnonymousMethod2)
            Assert.IsTrue(hasThrown)
        End Sub

        <TestMethod()> _
        Public Sub TestDynamicComboInvalidInParamValue()
            ' NOTE: invalid input is ignored and treated as a NOP.
            Dim packageObject As New ComboBoxPackage()
            Assert.IsNotNull(packageObject, "Failed to create package")

            Dim method As MethodInfo = GetType(ComboBoxPackage).GetMethod("OnMenuMyDynamicCombo", BindingFlags.NonPublic Or BindingFlags.Instance)

            Dim inParam As Object = "Non-valid string"
            Dim outParam As IntPtr = IntPtr.Zero

            Dim eventArgs As New OleMenuCmdEventArgs(inParam, outParam)
            Dim result As Object = method.Invoke(packageObject, New Object() {Nothing, eventArgs})
        End Sub
        Private NotInheritable Class AnonymousClass2
            Public packageObject As New ComboBoxPackage()
            Public method As MethodInfo = GetType(ComboBoxPackage).GetMethod("OnMenuMyDynamicCombo", BindingFlags.NonPublic Or BindingFlags.Instance)
            Public inParam As Object = "200"
            ' 64 == size of a variant + a little extra padding;
            Public outParam As IntPtr = Marshal.AllocCoTaskMem(64)
            Public eventArgs As New OleMenuCmdEventArgs(inParam, outParam)
            Public Sub AnonymousMethod()
                method.Invoke(packageObject, New Object() {Nothing, eventArgs})
            End Sub
        End Class
        <TestMethod()> _
        Public Sub TestDynamicComboBothInOutParamsGiven()
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

        <TestMethod()> _
        Public Sub TestDynamicComboInvalidInParamEmptyString()
            ' NOTE: invalid input is ignored and treated as a NOP.
            Dim packageObject As New ComboBoxPackage()
            Assert.IsNotNull(packageObject, "Failed to create package")

            Dim method As MethodInfo = GetType(ComboBoxPackage).GetMethod("OnMenuMyDynamicCombo", BindingFlags.NonPublic Or BindingFlags.Instance)

            Dim inParam As Object = String.Empty
            Dim outParam As IntPtr = IntPtr.Zero

            Dim eventArgs As New OleMenuCmdEventArgs(inParam, outParam)
            Dim result As Object = method.Invoke(packageObject, New Object() {Nothing, eventArgs})
        End Sub

        <TestMethod()> _
        Public Sub TestDynamicComboInvalidInParamObject()
            ' NOTE: invalid input is ignored and treated as a NOP.
            Dim packageObject As New ComboBoxPackage()
            Assert.IsNotNull(packageObject, "Failed to create package")

            Dim method As MethodInfo = GetType(ComboBoxPackage).GetMethod("OnMenuMyDynamicCombo", BindingFlags.NonPublic Or BindingFlags.Instance)

            Dim inParam As New Object()
            Dim outParam As IntPtr = IntPtr.Zero

            Dim eventArgs As New OleMenuCmdEventArgs(inParam, outParam)
            Dim result As Object = method.Invoke(packageObject, New Object() {Nothing, eventArgs})
        End Sub
        Private NotInheritable Class AnonymousClass3
            Public packageObject As New ComboBoxPackage()
            Public method As MethodInfo = GetType(ComboBoxPackage).GetMethod("OnMenuMyDynamicCombo", BindingFlags.NonPublic Or BindingFlags.Instance)
            Public inParam As Object = Nothing
            Public outParam As IntPtr = IntPtr.Zero
            Public eventArgs As New OleMenuCmdEventArgs(inParam, outParam)
            Public Sub AnonymousMethod()
                method.Invoke(packageObject, New Object() {Nothing, eventArgs})
            End Sub
        End Class
        <TestMethod()> _
        Public Sub TestDynamicComboNoInOutParams()
            Dim locals As New AnonymousClass3()
            Assert.IsNotNull(locals.packageObject, "Failed to create package")
            Assert.IsTrue(Utilities.HasFunctionThrown(Of ArgumentException)(AddressOf locals.AnonymousMethod))
        End Sub
        Private NotInheritable Class AnonymousClass4
            Public packageObject As New ComboBoxPackage()
            Public method As MethodInfo = GetType(ComboBoxPackage).GetMethod("OnMenuMyDynamicCombo", BindingFlags.NonPublic Or BindingFlags.Instance)
            Public Sub AnonymousMethod()
                method.Invoke(packageObject, New Object() {Nothing, EventArgs.Empty})
            End Sub
        End Class
        <TestMethod()> _
        Public Sub TestDynamicComboEmptyEventArgs()
            Dim locals As New AnonymousClass4()
            Assert.IsNotNull(locals.packageObject, "Failed to create package")
            Assert.IsTrue(Utilities.HasFunctionThrown(Of ArgumentException)(AddressOf locals.AnonymousMethod))
        End Sub

        <TestMethod()> _
        Public Sub TestDynamicComboGetCurVal()
            Dim packageObject As New ComboBoxPackage()
            Assert.IsNotNull(packageObject, "Failed to create package")
            Dim method As MethodInfo = GetType(ComboBoxPackage).GetMethod("OnMenuMyDynamicCombo", BindingFlags.NonPublic Or BindingFlags.Instance)

            Dim inParam As Object = Nothing
            Dim outParam As IntPtr = Marshal.AllocCoTaskMem(64) '64 == size of a variant + a little extra padding
            Try
                Dim eventArgs As New OleMenuCmdEventArgs(inParam, outParam)
                Dim result As Object = method.Invoke(packageObject, New Object() {Nothing, eventArgs})

                Dim retrieved As String = CStr(Marshal.GetObjectForNativeVariant(outParam))
                Assert.AreEqual(Of String)(expectedInitialZoomFactor, retrieved)
            Finally
                If outParam <> IntPtr.Zero Then
                    Marshal.FreeCoTaskMem(outParam)
                End If
            End Try
        End Sub

        <TestMethod()> _
        Public Sub TestDynamicComboSetCurValWithString()
            Dim packageObject As New ComboBoxPackage()
            Assert.IsNotNull(packageObject, "Failed to create package")
            Dim serviceProvider As OleServiceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices()

            ' Add site support to create and enumerate tool windows.
            Dim mockFactory As New GenericMockFactory("MockUIShell", New Type() {GetType(IVsUIShell)})
            Dim uiShell As BaseMock = mockFactory.GetInstance()
            serviceProvider.AddService(GetType(SVsUIShell), uiShell, False)

            ' Site the package.
            Assert.AreEqual(0, (CType(packageObject, IVsPackage)).SetSite(serviceProvider), "SetSite did not return S_OK")

            Dim method As MethodInfo = GetType(ComboBoxPackage).GetMethod("OnMenuMyDynamicCombo", BindingFlags.NonPublic Or BindingFlags.Instance)

            Dim inParam1 As Object = zoomLevels(1)
            Dim outParam1 As IntPtr = IntPtr.Zero
            Dim inParam2 As Object = Nothing
            ' 64 == size of a variant + a little extra padding
            Dim outParam2 As IntPtr = Marshal.AllocCoTaskMem(64)
            Try
                ' Set DynamicCombo to 2nd choice in list.
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
        Public Sub TestDynamicComboSetCurValWithInt()
            Dim packageObject As New ComboBoxPackage()
            Assert.IsNotNull(packageObject, "Failed to create package")
            Dim serviceProvider As OleServiceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices()

            ' Add site support to create and enumerate tool windows.
            Dim mockFactory As New GenericMockFactory("MockUIShell", New Type() {GetType(IVsUIShell)})
            Dim uiShell As BaseMock = mockFactory.GetInstance()
            serviceProvider.AddService(GetType(SVsUIShell), uiShell, False)

            ' Site the package.
            Assert.AreEqual(0, (CType(packageObject, IVsPackage)).SetSite(serviceProvider), "SetSite did not return S_OK")

            Dim method As MethodInfo = GetType(ComboBoxPackage).GetMethod("OnMenuMyDynamicCombo", BindingFlags.NonPublic Or BindingFlags.Instance)

            Dim inChoice As Integer = 17
            Dim inParam1 As Object = inChoice
            Dim outParam1 As IntPtr = IntPtr.Zero
            Dim inParam2 As Object = Nothing
            Dim outParam2 As IntPtr = Marshal.AllocCoTaskMem(64) '64 == size of a variant + a little extra padding
            Try
                ' Set DynamicCombo to 17 %.
                Dim eventArgs1 As New OleMenuCmdEventArgs(inParam1, outParam1)
                Dim result As Object = method.Invoke(packageObject, New Object() {Nothing, eventArgs1})

                ' Retrieve current value of ZoomLevel and verify it is "17 %".
                Dim eventArgs2 As New OleMenuCmdEventArgs(inParam2, outParam2)
                result = method.Invoke(packageObject, New Object() {Nothing, eventArgs2})

                Dim retrieved As String = CStr(Marshal.GetObjectForNativeVariant(outParam2))
                Assert.AreEqual(Of String)(String.Format("{0} %", inChoice), retrieved)

                Assert.AreEqual(1, uiShell.FunctionCalls(String.Format("{0}.{1}", GetType(IVsUIShell).FullName, "ShowMessageBox")), "IVsUIShell.ShowMessageBox was not called")
            Finally
                If outParam2 <> IntPtr.Zero Then
                    Marshal.FreeCoTaskMem(outParam2)
                End If
            End Try
        End Sub

        <TestMethod()> _
        Public Sub TestDynamicComboSetCurValWithDouble()
            Dim packageObject As New ComboBoxPackage()
            Assert.IsNotNull(packageObject, "Failed to create package")
            Dim serviceProvider As OleServiceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices()

            ' Add site support to create and enumerate tool windows.
            Dim mockFactory As New GenericMockFactory("MockUIShell", New Type() {GetType(IVsUIShell)})
            Dim uiShell As BaseMock = mockFactory.GetInstance()
            serviceProvider.AddService(GetType(SVsUIShell), uiShell, False)

            ' Site the package.
            Assert.AreEqual(0, (CType(packageObject, IVsPackage)).SetSite(serviceProvider), "SetSite did not return S_OK")

            Dim method As MethodInfo = GetType(ComboBoxPackage).GetMethod("OnMenuMyDynamicCombo", BindingFlags.NonPublic Or BindingFlags.Instance)

            Dim inChoice As Double = 82.756
            Dim inParam1 As Object = inChoice.ToString()
            Dim outParam1 As IntPtr = IntPtr.Zero
            Dim inParam2 As Object = Nothing
            ' 64 == size of a variant + a little extra padding.
            Dim outParam2 As IntPtr = Marshal.AllocCoTaskMem(64)
            Try
                ' Set DynamicCombo to 82.756.
                Dim eventArgs1 As New OleMenuCmdEventArgs(inParam1, outParam1)
                Dim result As Object = method.Invoke(packageObject, New Object() {Nothing, eventArgs1})

                ' Retrieve current value of Zoom Level and verify it is "83 %".
                Dim eventArgs2 As New OleMenuCmdEventArgs(inParam2, outParam2)
                result = method.Invoke(packageObject, New Object() {Nothing, eventArgs2})

                Dim retrieved As String = CStr(Marshal.GetObjectForNativeVariant(outParam2))
                Assert.AreEqual(Of String)(String.Format("{0} %", CInt(Fix(Math.Round(inChoice)))), retrieved)

                Assert.AreEqual(1, uiShell.FunctionCalls(String.Format("{0}.{1}", GetType(IVsUIShell).FullName, "ShowMessageBox")), "IVsUIShell.ShowMessageBox was not called")
            Finally
                If outParam2 <> IntPtr.Zero Then
                    Marshal.FreeCoTaskMem(outParam2)
                End If
            End Try
        End Sub

        <TestMethod()> _
        Public Sub TestDynamicComboSetCurValWithZoomToFit()
            Dim packageObject As New ComboBoxPackage()
            Assert.IsNotNull(packageObject, "Failed to create package")
            Dim serviceProvider As OleServiceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices()

            ' Add site support to create and enumerate tool windows.
            Dim mockFactory As New GenericMockFactory("MockUIShell", New Type() {GetType(IVsUIShell)})
            Dim uiShell As BaseMock = mockFactory.GetInstance()
            serviceProvider.AddService(GetType(SVsUIShell), uiShell, False)

            ' Site the package.
            Assert.AreEqual(0, (CType(packageObject, IVsPackage)).SetSite(serviceProvider), "SetSite did not return S_OK")

            Dim method As MethodInfo = GetType(ComboBoxPackage).GetMethod("OnMenuMyDynamicCombo", BindingFlags.NonPublic Or BindingFlags.Instance)

            Dim inParam1 As Object = zoomToFit
            Dim outParam1 As IntPtr = IntPtr.Zero
            Dim inParam2 As Object = Nothing
            ' 64 == size of a variant + a little extra padding
            Dim outParam2 As IntPtr = Marshal.AllocCoTaskMem(64)
            Try
                ' Set DynamicCombo to "Zoom to Fit".
                Dim eventArgs1 As New OleMenuCmdEventArgs(inParam1, outParam1)
                Dim result As Object = method.Invoke(packageObject, New Object() {Nothing, eventArgs1})

                ' Retrieve current value of Zoom Level and verify it is "Zoom to Fit".
                Dim eventArgs2 As New OleMenuCmdEventArgs(inParam2, outParam2)
                result = method.Invoke(packageObject, New Object() {Nothing, eventArgs2})

                Dim retrieved As String = CStr(Marshal.GetObjectForNativeVariant(outParam2))
                Assert.AreEqual(Of String)(zoom_to_Fit, retrieved)

                Assert.AreEqual(1, uiShell.FunctionCalls(String.Format("{0}.{1}", GetType(IVsUIShell).FullName, "ShowMessageBox")), "IVsUIShell.ShowMessageBox was not called")
            Finally
                If outParam2 <> IntPtr.Zero Then
                    Marshal.FreeCoTaskMem(outParam2)
                End If
            End Try
        End Sub

        <TestMethod()> _
        Public Sub TestDynamicComboSetCurValWithZoom_To_Fit()
            Dim packageObject As New ComboBoxPackage()
            Assert.IsNotNull(packageObject, "Failed to create package")
            Dim serviceProvider As OleServiceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices()

            ' Add site support to create and enumerate tool windows.
            Dim mockFactory As New GenericMockFactory("MockUIShell", New Type() {GetType(IVsUIShell)})
            Dim uiShell As BaseMock = mockFactory.GetInstance()
            serviceProvider.AddService(GetType(SVsUIShell), uiShell, False)

            ' Site the package.
            Assert.AreEqual(0, (CType(packageObject, IVsPackage)).SetSite(serviceProvider), "SetSite did not return S_OK")

            Dim method As MethodInfo = GetType(ComboBoxPackage).GetMethod("OnMenuMyDynamicCombo", BindingFlags.NonPublic Or BindingFlags.Instance)

            Dim inParam1 As Object = zoom_to_Fit
            Dim outParam1 As IntPtr = IntPtr.Zero
            Dim inParam2 As Object = Nothing
            ' 64 == size of a variant + a little extra padding
            Dim outParam2 As IntPtr = Marshal.AllocCoTaskMem(64)
            Try
                ' Set DynamicCombo to "Zoom to Fit".
                Dim eventArgs1 As New OleMenuCmdEventArgs(inParam1, outParam1)
                Dim result As Object = method.Invoke(packageObject, New Object() {Nothing, eventArgs1})

                ' Retrieve current value of Zoom Level and verify it is "Zoom to Fit".
                Dim eventArgs2 As New OleMenuCmdEventArgs(inParam2, outParam2)
                result = method.Invoke(packageObject, New Object() {Nothing, eventArgs2})

                Dim retrieved As String = CStr(Marshal.GetObjectForNativeVariant(outParam2))
                Assert.AreEqual(Of String)(zoom_to_Fit, retrieved)

                Assert.AreEqual(1, uiShell.FunctionCalls(String.Format("{0}.{1}", GetType(IVsUIShell).FullName, "ShowMessageBox")), "IVsUIShell.ShowMessageBox was not called")
            Finally
                If outParam2 <> IntPtr.Zero Then
                    Marshal.FreeCoTaskMem(outParam2)
                End If
            End Try
        End Sub
        Private NotInheritable Class AnonymousClass5
            Public packageObject As New ComboBoxPackage()
            Public method As MethodInfo = GetType(ComboBoxPackage).GetMethod("OnMenuMyDynamicCombo", BindingFlags.NonPublic Or BindingFlags.Instance)
            Public inChoice As Integer = -1
            Public inParam1 As Object = inChoice.ToString()
            Public outParam1 As IntPtr = IntPtr.Zero
            ' Set DynamicCombo to -1.
            Public eventArgs1 As New OleMenuCmdEventArgs(inParam1, outParam1)
            Public Sub AnonymousMethod()
                method.Invoke(packageObject, New Object() {Nothing, eventArgs1})
            End Sub
        End Class

        <TestMethod()> _
        Public Sub TestDynamicComboSetCurValWithNegativeInt()
            Dim locals As New AnonymousClass5()
            Assert.IsNotNull(locals.packageObject, "Failed to create package")
            Dim serviceProvider As OleServiceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices()

            ' Add site support to create and enumerate tool windows.
            Dim mockFactory As New GenericMockFactory("MockUIShell", New Type() {GetType(IVsUIShell)})
            Dim uiShell As BaseMock = mockFactory.GetInstance()
            serviceProvider.AddService(GetType(SVsUIShell), uiShell, False)

            ' Site the package.
            Assert.AreEqual(0, (CType(locals.packageObject, IVsPackage)).SetSite(serviceProvider), "SetSite did not return S_OK")

            Dim inParam2 As Object = Nothing
            '64 == size of a variant + a little extra padding.
            Dim outParam2 As IntPtr = Marshal.AllocCoTaskMem(64)
            Try
                Assert.IsTrue(Utilities.HasFunctionThrown(Of ArgumentException)(AddressOf locals.AnonymousMethod))

                Dim eventArgs2 As New OleMenuCmdEventArgs(inParam2, outParam2)
                Dim result As Object = locals.method.Invoke(locals.packageObject, New Object() {Nothing, eventArgs2})
            Finally
                If outParam2 <> IntPtr.Zero Then
                    Marshal.FreeCoTaskMem(outParam2)
                End If
            End Try
        End Sub

        <TestMethod()> _
        Public Sub TestDynamicComboSetCurValWithOverflowInt()
            ' NOTE: invalid input is ignored and treated as a NOP.
            Dim packageObject As New ComboBoxPackage()
            Assert.IsNotNull(packageObject, "Failed to create package")
            Dim serviceProvider As OleServiceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices()

            ' Add site support to create and enumerate tool windows.
            Dim mockFactory As New GenericMockFactory("MockUIShell", New Type() {GetType(IVsUIShell)})
            Dim uiShell As BaseMock = mockFactory.GetInstance()
            serviceProvider.AddService(GetType(SVsUIShell), uiShell, False)

            ' Site the package.
            Assert.AreEqual(0, (CType(packageObject, IVsPackage)).SetSite(serviceProvider), "SetSite did not return S_OK")

            Dim method As MethodInfo = GetType(ComboBoxPackage).GetMethod("OnMenuMyDynamicCombo", BindingFlags.NonPublic Or BindingFlags.Instance)

            Dim inChoice As Int64 = Int64.MaxValue
            Dim inParam1 As Object = inChoice.ToString() & inChoice.ToString() & inChoice.ToString()
            Dim outParam1 As IntPtr = IntPtr.Zero
            Dim inParam2 As Object = Nothing
            ' 64 == size of a variant + a little extra padding
            Dim outParam2 As IntPtr = Marshal.AllocCoTaskMem(64)
            Try
                ' Set DynamicCombo to overflow value.
                Dim eventArgs1 As New OleMenuCmdEventArgs(inParam1, outParam1)
                Dim result As Object = method.Invoke(packageObject, New Object() {Nothing, eventArgs1})

                ' Retrieve current value of Zoom and verify.
                Dim eventArgs2 As New OleMenuCmdEventArgs(inParam2, outParam2)
                result = method.Invoke(packageObject, New Object() {Nothing, eventArgs2})

                Dim retrieved As String = CStr(Marshal.GetObjectForNativeVariant(outParam2))
            Finally
                If outParam2 <> IntPtr.Zero Then
                    Marshal.FreeCoTaskMem(outParam2)
                End If
            End Try
        End Sub
        Private NotInheritable Class AnonymousClass6
            Public packageObject As New ComboBoxPackage()
            Public method As MethodInfo = GetType(ComboBoxPackage).GetMethod("OnMenuMyDynamicComboGetList", BindingFlags.NonPublic Or BindingFlags.Instance)
            Public inParam As Object = Nothing
            Public outParam As IntPtr = IntPtr.Zero
            Public eventArgs As New OleMenuCmdEventArgs(inParam, outParam)
            Public Sub AnonymousMethod()
                method.Invoke(packageObject, New Object() {Nothing, eventArgs})
            End Sub
        End Class
        <TestMethod()> _
        Public Sub TestDynamicComboGetListNoInOutParams()
            Dim locals As New AnonymousClass6()
            Assert.IsNotNull(locals.packageObject, "Failed to create package")
            Assert.IsTrue(Utilities.HasFunctionThrown(Of ArgumentException)(AddressOf locals.AnonymousMethod))
        End Sub
        Private NotInheritable Class AnonymousClass7
            Public packageObject As New ComboBoxPackage()
            Public method As MethodInfo = GetType(ComboBoxPackage).GetMethod("OnMenuMyDynamicComboGetList", BindingFlags.NonPublic Or BindingFlags.Instance)
            Public inParam As Object = "73.2"
            ' 64 == size of a variant + a little extra padding;
            Public outParam As IntPtr = Marshal.AllocCoTaskMem(64)
            Public eventArgs As New OleMenuCmdEventArgs(inParam, outParam)
            Public Sub AnonymousMethod()
                method.Invoke(packageObject, New Object() {Nothing, eventArgs})
            End Sub
        End Class
        <TestMethod()> _
        Public Sub TestDynamicComboGetListInParamGiven()
            Dim locals As New AnonymousClass7()
            Assert.IsNotNull(locals.packageObject, "Failed to create package")
            Try
                Assert.IsTrue(Utilities.HasFunctionThrown(Of ArgumentException)(AddressOf locals.AnonymousMethod))
            Finally
                If locals.outParam <> IntPtr.Zero Then
                    Marshal.FreeCoTaskMem(locals.outParam)
                End If
            End Try
        End Sub
        Private NotInheritable Class AnonymousClass8
            Public packageObject As New ComboBoxPackage()
            Public method As MethodInfo = GetType(ComboBoxPackage).GetMethod("OnMenuMyDynamicComboGetList", BindingFlags.NonPublic Or BindingFlags.Instance)
            Public Sub AnonymousMethod1()
                method.Invoke(packageObject, New Object() {Nothing, EventArgs.Empty})
            End Sub
            Public Sub AnonymousMethod2()
                method.Invoke(packageObject, New Object() {Nothing, Nothing})
            End Sub

        End Class
        <TestMethod()> _
        Public Sub TestDynamicComboGetListEmptyEventArgs()
            Dim locals As New AnonymousClass8()
            Assert.IsNotNull(locals.packageObject, "Failed to create package")
            Assert.IsTrue(Utilities.HasFunctionThrown(Of ArgumentNullException)(AddressOf locals.AnonymousMethod1))
            Assert.IsTrue(Utilities.HasFunctionThrown(Of ArgumentNullException)(AddressOf locals.AnonymousMethod2))
        End Sub
    End Class
End Namespace
