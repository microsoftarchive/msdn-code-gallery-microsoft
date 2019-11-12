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

Namespace ComboBox.UnitTest
	''' <summary>
    ''' Summary description for TestDropDownComboCommands.
	''' </summary>
	<TestClass> _
	Public Class TestDropDownComboCommands
		Private expectedDropDownComboChoices As String() = { "Apples", "Oranges", "Pears", "Bananas" }
		Private expectedInitialDropDownComboChoice As String = "Apples"

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
            Public method As MethodInfo = GetType(ComboBoxPackage).GetMethod("OnMenuMyDropDownCombo", BindingFlags.NonPublic Or BindingFlags.Instance)
            Public Sub AnonymousMethod()
                method.Invoke(packageObject, New Object() {Nothing, Nothing})
            End Sub
        End Class
		<TestMethod> _
		Public Sub TestDropDownComboNoParams()
            Dim locals As New AnonymousClass1()
            Assert.IsNotNull(locals.packageObject, "Failed to create package")
            Assert.IsTrue(Utilities.HasFunctionThrown(Of ArgumentException)(AddressOf locals.AnonymousMethod))
        End Sub
        Private NotInheritable Class AnonymousClass2
            Public packageObject As New ComboBoxPackage()
            Public method As MethodInfo = GetType(ComboBoxPackage).GetMethod("OnMenuMyDropDownCombo", BindingFlags.NonPublic Or BindingFlags.Instance)
            Public inParam As Object = "Non-valid string"
            Public outParam As IntPtr = IntPtr.Zero
            Public eventArgs As New OleMenuCmdEventArgs(inParam, outParam)
            Public Sub AnonymousMethod()
                method.Invoke(packageObject, New Object() {Nothing, EventArgs})
            End Sub
        End Class

		<TestMethod> _
		Public Sub TestDropDownComboInvalidInParamValue()
            Dim locals As New AnonymousClass2()
            Assert.IsNotNull(locals.packageObject, "Failed to create package")
            
            Assert.IsTrue(Utilities.HasFunctionThrown(Of ArgumentException)(AddressOf locals.AnonymousMethod))

		End Sub
        Private NotInheritable Class AnonymousClass3
            Public packageObject As New ComboBoxPackage()
            Public method As MethodInfo = GetType(ComboBoxPackage).GetMethod("OnMenuMyDropDownCombo", BindingFlags.NonPublic Or BindingFlags.Instance)
            Public inParam As Object = "Oranges"
            ' 64 == size of a variant + a little extra padding;
            Public outParam As IntPtr = Marshal.AllocCoTaskMem(64)
            Public eventArgs As New OleMenuCmdEventArgs(inParam, outParam)
            Public Sub AnonymousMethod()
                method.Invoke(packageObject, New Object() {Nothing, EventArgs})
            End Sub
        End Class
		<TestMethod> _
		Public Sub TestDropDownComboBothInOutParamsGiven()
            Dim locals As New AnonymousClass3
            Assert.IsNotNull(locals.packageObject, "Failed to create package")
            Try
                Dim hasThrown As Boolean = Utilities.HasFunctionThrown(Of ArgumentException)(AddressOf locals.AnonymousMethod)
                Assert.IsTrue(hasThrown)
            Finally
                If locals.outParam <> IntPtr.Zero Then
                    Marshal.FreeCoTaskMem(locals.outParam)
                End If
            End Try
		End Sub
        Private NotInheritable Class AnonymousClass4
            Public packageObject As New ComboBoxPackage()
            Public method As MethodInfo = GetType(ComboBoxPackage).GetMethod("OnMenuMyDropDownCombo", BindingFlags.NonPublic Or BindingFlags.Instance)
            Public inParam As Object = String.Empty
            Public outParam As IntPtr = IntPtr.Zero
            Public eventArgs As New OleMenuCmdEventArgs(inParam, outParam)
            Public Sub AnonymousMethod()
                method.Invoke(packageObject, New Object() {Nothing, EventArgs})
            End Sub
        End Class
		<TestMethod> _
		Public Sub TestDropDownComboInvalidInParamEmptyString()
            Dim locals As New AnonymousClass4
            Assert.IsNotNull(locals.packageObject, "Failed to create package")
            Dim hasThrown As Boolean = Utilities.HasFunctionThrown(Of ArgumentException)(AddressOf locals.AnonymousMethod)
			Assert.IsTrue(hasThrown)
		End Sub
        Private NotInheritable Class AnonymousClass5
            Public packageObject As New ComboBoxPackage()
            Public method As MethodInfo = GetType(ComboBoxPackage).GetMethod("OnMenuMyDropDownCombo", BindingFlags.NonPublic Or BindingFlags.Instance)
            Public inParam As Object = 42
            Public outParam As IntPtr = IntPtr.Zero
            Public eventArgs As New OleMenuCmdEventArgs(inParam, outParam)
            Public Sub AnonymousMethod()
                method.Invoke(packageObject, New Object() {Nothing, EventArgs})
            End Sub
        End Class
		<TestMethod> _
		Public Sub TestDropDownComboInvalidInParamNumber()
            Dim locals As New AnonymousClass5
            Assert.IsNotNull(locals.packageObject, "Failed to create package")
            
            Dim hasThrown As Boolean = Utilities.HasFunctionThrown(Of ArgumentException)(AddressOf locals.AnonymousMethod)
			Assert.IsTrue(hasThrown)
		End Sub
        Private NotInheritable Class AnonymousClass6
            Public packageObject As New ComboBoxPackage()
            Public method As MethodInfo = GetType(ComboBoxPackage).GetMethod("OnMenuMyDropDownCombo", BindingFlags.NonPublic Or BindingFlags.Instance)
            Public inParam As New Object()
            Public outParam As IntPtr = IntPtr.Zero
            Public eventArgs As New OleMenuCmdEventArgs(inParam, outParam)
            Public Sub AnonymousMethod()
                method.Invoke(packageObject, New Object() {Nothing, EventArgs})
            End Sub
        End Class
		<TestMethod> _
		Public Sub TestDropDownComboInvalidInParamObject()
            Dim locals As New AnonymousClass6
            Assert.IsNotNull(locals.packageObject, "Failed to create package")
            Dim hasThrown As Boolean = Utilities.HasFunctionThrown(Of ArgumentException)(AddressOf locals.AnonymousMethod)
			Assert.IsTrue(hasThrown)
		End Sub
        Private NotInheritable Class AnonymousClass7
            Public packageObject As New ComboBoxPackage()
            Public method As MethodInfo = GetType(ComboBoxPackage).GetMethod("OnMenuMyDropDownCombo", BindingFlags.NonPublic Or BindingFlags.Instance)
            Public inParam As Object = Nothing
            Public outParam As IntPtr = IntPtr.Zero
            Public eventArgs As New OleMenuCmdEventArgs(inParam, outParam)
            Public Sub AnonymousMethod()
                method.Invoke(packageObject, New Object() {Nothing, EventArgs})
            End Sub
        End Class
		<TestMethod> _
		Public Sub TestDropDownComboNoInOutParams()
            Dim locals As New AnonymousClass7()
            Assert.IsNotNull(locals.packageObject, "Failed to create package")
            
            Dim hasThrown As Boolean = Utilities.HasFunctionThrown(Of ArgumentException)(AddressOf locals.AnonymousMethod)
			Assert.IsTrue(hasThrown)
		End Sub
        Private NotInheritable Class AnonymousClass8
            Public packageObject As New ComboBoxPackage()
            Public method As MethodInfo = GetType(ComboBoxPackage).GetMethod("OnMenuMyDropDownCombo", BindingFlags.NonPublic Or BindingFlags.Instance)
            Public args As New EventArgs()
            Public Sub AnonymousMethod1()
                method.Invoke(packageObject, New Object() {Nothing, EventArgs.Empty})
            End Sub
            Public Sub AnonymousMethod2()
                method.Invoke(packageObject, New Object() {Nothing, Nothing})
            End Sub
            Public Sub AnonymousMethod3()
                method.Invoke(packageObject, New Object() {Nothing, args})
            End Sub
        End Class

        <TestMethod()> _
        Public Sub TestDropDownComboEmptyEventArgs()
            Dim locals As New AnonymousClass8()

            Assert.IsNotNull(locals.packageObject, "Failed to create package")
            Dim hasThrown As Boolean = Utilities.HasFunctionThrown(Of ArgumentException)(AddressOf locals.AnonymousMethod1)
            Assert.IsTrue(hasThrown)

            hasThrown = Utilities.HasFunctionThrown(Of ArgumentException)(AddressOf locals.AnonymousMethod2)
            Assert.IsTrue(hasThrown)

            hasThrown = Utilities.HasFunctionThrown(Of ArgumentException)(AddressOf locals.AnonymousMethod3)
            Assert.IsTrue(hasThrown)
        End Sub

        <TestMethod()> _
        Public Sub TestDropDownComboGetCurVal()
            Dim packageObject As New ComboBoxPackage()
            Assert.IsNotNull(packageObject, "Failed to create package")
            Dim method As MethodInfo = GetType(ComboBoxPackage).GetMethod("OnMenuMyDropDownCombo", BindingFlags.NonPublic Or BindingFlags.Instance)

            Dim inParam As Object = Nothing
            Dim outParam As IntPtr = Marshal.AllocCoTaskMem(64) '64 == size of a variant + a little extra padding
            Try
                Dim eventArgs As New OleMenuCmdEventArgs(inParam, outParam)
                Dim result As Object = method.Invoke(packageObject, New Object() {Nothing, eventArgs})

                Dim retrieved As String = CStr(Marshal.GetObjectForNativeVariant(outParam))
                Assert.AreEqual(Of String)(expectedInitialDropDownComboChoice, retrieved)
            Finally
                If outParam <> IntPtr.Zero Then
                    Marshal.FreeCoTaskMem(outParam)
                End If
            End Try
        End Sub

        <TestMethod()> _
        Public Sub TestDropDownComboSetCurVal()
            Dim packageObject As New ComboBoxPackage()
            Assert.IsNotNull(packageObject, "Failed to create package")
            Dim serviceProvider As OleServiceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices()

            ' Add site support to create and enumerate tool windows.
            Dim mockFactory As New GenericMockFactory("MockUIShell", New Type() {GetType(IVsUIShell)})
            Dim uiShell As BaseMock = mockFactory.GetInstance()
            serviceProvider.AddService(GetType(SVsUIShell), uiShell, False)

            ' Site the package.
            Assert.AreEqual(0, (CType(packageObject, IVsPackage)).SetSite(serviceProvider), "SetSite did not return S_OK")

            Dim method As MethodInfo = GetType(ComboBoxPackage).GetMethod("OnMenuMyDropDownCombo", BindingFlags.NonPublic Or BindingFlags.Instance)

            Dim inParam1 As Object = "Oranges"
            Dim outParam1 As IntPtr = IntPtr.Zero
            Dim inParam2 As Object = Nothing
            ' 64 == size of a variant + a little extra padding
            Dim outParam2 As IntPtr = Marshal.AllocCoTaskMem(64)
            Try
                ' Set DropDownCombo to "Oranges".
                Dim eventArgs1 As New OleMenuCmdEventArgs(inParam1, outParam1)
                Dim result As Object = method.Invoke(packageObject, New Object() {Nothing, eventArgs1})

                ' Retrieve current value of DropDown and verify it is "Oranges".
                Dim eventArgs2 As New OleMenuCmdEventArgs(inParam2, outParam2)
                result = method.Invoke(packageObject, New Object() {Nothing, eventArgs2})

                Dim retrieved As String = CStr(Marshal.GetObjectForNativeVariant(outParam2))
                Assert.AreEqual(Of String)(CStr(inParam1), retrieved)

                Assert.AreEqual(1, uiShell.FunctionCalls(String.Format("{0}.{1}", GetType(IVsUIShell).FullName, "ShowMessageBox")), "IVsUIShell.ShowMessageBox was not called")
            Finally
                If outParam2 <> IntPtr.Zero Then
                    Marshal.FreeCoTaskMem(outParam2)
                End If
            End Try
        End Sub
        Private NotInheritable Class AnonymousClass9
            Public packageObject As New ComboBoxPackage()
            Public method As MethodInfo = GetType(ComboBoxPackage).GetMethod("OnMenuMyDropDownComboGetList", BindingFlags.NonPublic Or BindingFlags.Instance)
            Public inParam As Object = Nothing
            Public outParam As IntPtr = IntPtr.Zero
            Public eventArgs As New OleMenuCmdEventArgs(inParam, outParam)
            Public Sub AnonymousMethod()
                method.Invoke(packageObject, New Object() {Nothing, eventArgs})
            End Sub
        End Class
        <TestMethod()> _
        Public Sub TestDropDownComboGetListNoInOutParams()
            Dim locals As New AnonymousClass9()
            Assert.IsNotNull(locals.packageObject, "Failed to create package")
            Assert.IsTrue(Utilities.HasFunctionThrown(Of ArgumentException)(AddressOf locals.AnonymousMethod))
        End Sub
        Private NotInheritable Class AnonymousClass10
            Public packageObject As New ComboBoxPackage()
            Public method As MethodInfo = GetType(ComboBoxPackage).GetMethod("OnMenuMyDropDownComboGetList", BindingFlags.NonPublic Or BindingFlags.Instance)
            Public inParam As Object = "Oranges"
            ' 64 == size of a variant + a little extra padding;
            Public outParam As IntPtr = Marshal.AllocCoTaskMem(64)
            Public eventArgs As New OleMenuCmdEventArgs(inParam, outParam)
            Public Sub AnonymousMethod()
                method.Invoke(packageObject, New Object() {Nothing, eventArgs})
            End Sub
        End Class
        <TestMethod()> _
        Public Sub TestDropDownComboGetListInParamGiven()
            Dim locals As New AnonymousClass10()
            Assert.IsNotNull(locals.packageObject, "Failed to create package")
            Try
                Assert.IsTrue(Utilities.HasFunctionThrown(Of ArgumentException)(AddressOf locals.AnonymousMethod))
            Finally
                If locals.outParam <> IntPtr.Zero Then
                    Marshal.FreeCoTaskMem(locals.outParam)
                End If
            End Try
        End Sub
        Private NotInheritable Class AnonymousClass11
            Public packageObject As New ComboBoxPackage()
            Public method As MethodInfo = GetType(ComboBoxPackage).GetMethod("OnMenuMyDropDownComboGetList", BindingFlags.NonPublic Or BindingFlags.Instance)
            Public Sub AnonymousMethod1()
                method.Invoke(packageObject, New Object() {Nothing, EventArgs.Empty})
            End Sub
            Public Sub AnonymousMethod2()
                method.Invoke(packageObject, New Object() {Nothing, Nothing})
            End Sub
        End Class
        <TestMethod()> _
        Public Sub TestDropDownComboGetListEmptyEventArgs()
            Dim locals As New AnonymousClass11()
            Assert.IsNotNull(locals.packageObject, "Failed to create package")

            Dim hasThrown As Boolean = Utilities.HasFunctionThrown(Of ArgumentException)(AddressOf locals.AnonymousMethod1)
            Assert.IsTrue(hasThrown)
            hasThrown = Utilities.HasFunctionThrown(Of ArgumentException)(AddressOf locals.AnonymousMethod2)
            Assert.IsTrue(hasThrown)
        End Sub

        <TestMethod()> _
        Public Sub TestDropDownComboGetList()
            Dim packageObject As New ComboBoxPackage()
            Assert.IsNotNull(packageObject, "Failed to create package")
            Dim method As MethodInfo = GetType(ComboBoxPackage).GetMethod("OnMenuMyDropDownComboGetList", BindingFlags.NonPublic Or BindingFlags.Instance)

            Dim inParam As Object = Nothing
            ' 64 == size of a variant + a little extra padding
            Dim outParam As IntPtr = Marshal.AllocCoTaskMem(64)
            Try
                Dim eventArgs As New OleMenuCmdEventArgs(inParam, outParam)
                Dim result As Object = method.Invoke(packageObject, New Object() {Nothing, eventArgs})

                Dim retrieved As String() = CType(Marshal.GetObjectForNativeVariant(outParam), String())
                Utilities.SameArray(Of String)(expectedDropDownComboChoices, retrieved)
            Finally
                If outParam <> IntPtr.Zero Then
                    Marshal.FreeCoTaskMem(outParam)
                End If
            End Try
        End Sub
    End Class
End Namespace
