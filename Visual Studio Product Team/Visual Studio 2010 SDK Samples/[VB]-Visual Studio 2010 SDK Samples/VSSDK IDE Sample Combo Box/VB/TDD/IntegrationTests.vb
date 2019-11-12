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
Imports EnvDTE
Imports Microsoft.VSSDK.Tools.VsIdeTesting
Imports Microsoft.VisualStudio.CommandBars

Namespace Microsoft.Samples.VisualStudio.ComboBox.IntegrationTest
	''' <summary>
    ''' Summary description for UnitTest1.
	''' </summary>
	<TestClass> _
	Public Class ComboBoxIntegrationTest
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

		Private Delegate Sub ThreadInvoker()

		''' <summary>
		''' Test calling the Tools.DropDownCombo command.
		''' </summary>
		<TestMethod, HostType("VS IDE")> _
		Public Sub TestDropDownComboBox()
            UIThreadInvoker.Invoke(CType(AddressOf AnonymousMethod1, ThreadInvoker))
		End Sub
		Private Sub AnonymousMethod1()
			Dim sp As IServiceProvider = VsIdeTestHostContext.ServiceProvider
			Dim dte As DTE = CType(sp.GetService(GetType(DTE)), DTE)
            Dim expectedDialogBoxText As String = String.Format(System.Globalization.CultureInfo.CurrentCulture, "{0}" & Microsoft.VisualBasic.Constants.vbLf + Microsoft.VisualBasic.Constants.vbLf & "{1}", "My DropDown Combo", "Bananas")
            Dim purger As New DialogBoxPurger(NativeMethods.IDOK, expectedDialogBoxText)
			Try
			purger.Start()
			dte.ExecuteCommand("Tools.DropDownCombo", "Bananas")
			Finally
			Assert.IsTrue(purger.WaitForDialogThreadToTerminate(), "The Drop Down Combo dialog box has not shown")
			End Try
		End Sub

		''' <summary>
		''' Test calling the Tools.IndexCombo command.
		''' </summary>
		<TestMethod, HostType("VS IDE")> _
		Public Sub TestIndexComboBox()
            UIThreadInvoker.Invoke(CType(AddressOf AnonymousMethod2, ThreadInvoker))
		End Sub
		Private Sub AnonymousMethod2()
			Dim sp As IServiceProvider = VsIdeTestHostContext.ServiceProvider
			Dim dte As DTE = CType(sp.GetService(GetType(DTE)), DTE)
			Dim commandBars As CommandBars = CType(dte.CommandBars, CommandBars)
			commandBars("ComboBoxSample").Visible = True
            Dim expectedDialogBoxText As String = String.Format(System.Globalization.CultureInfo.CurrentCulture, "{0}" & Microsoft.VisualBasic.Constants.vbLf + Microsoft.VisualBasic.Constants.vbLf & "{1}", "My Index Combo", "1")
            Dim purger As New DialogBoxPurger(NativeMethods.IDOK, expectedDialogBoxText)
			Try
			purger.Start()
			dte.ExecuteCommand("Tools.IndexCombo", "Tigers")
			Finally
			Assert.IsTrue(purger.WaitForDialogThreadToTerminate(), "The Index Combo dialog box has not shown")
			End Try
		End Sub

		''' <summary>
		''' Test calling the Tools.MRUCombo command.
		''' </summary>
		<TestMethod, HostType("VS IDE")> _
		Public Sub TestMRUComboBox()
            UIThreadInvoker.Invoke(CType(AddressOf AnonymousMethod3, ThreadInvoker))
		End Sub
		Private Sub AnonymousMethod3()
			Dim sp As IServiceProvider = VsIdeTestHostContext.ServiceProvider
			Dim dte As DTE = CType(sp.GetService(GetType(DTE)), DTE)
			Dim commandBars As CommandBars = CType(dte.CommandBars, CommandBars)
			commandBars("ComboBoxSample").Visible = True
            Dim expectedDialogBoxText As String = String.Format(System.Globalization.CultureInfo.CurrentCulture, "{0}" & Microsoft.VisualBasic.Constants.vbLf + Microsoft.VisualBasic.Constants.vbLf & "{1}", "My MRU Combo", "Hello World!")
            Dim purger As New DialogBoxPurger(NativeMethods.IDOK, expectedDialogBoxText)
			Try
			purger.Start()
			dte.ExecuteCommand("Tools.MRUCombo", "Hello World!")
			Finally
			Assert.IsTrue(purger.WaitForDialogThreadToTerminate(), "The MRU Combo dialog box has not shown")
			End Try
		End Sub

		''' <summary>
		''' Test calling the Tools.DynamicCombo command.
		''' </summary>
		<TestMethod, HostType("VS IDE")> _
		Public Sub TestDynamicComboBox()
            UIThreadInvoker.Invoke(CType(AddressOf AnonymousMethod4, ThreadInvoker))
		End Sub
		Private Sub AnonymousMethod4()
			Dim sp As IServiceProvider = VsIdeTestHostContext.ServiceProvider
			Dim dte As DTE = CType(sp.GetService(GetType(DTE)), DTE)
			Dim commandBars As CommandBars = CType(dte.CommandBars, CommandBars)
			commandBars("ComboBoxSample").Visible = True
            Dim expectedDialogBoxText As String = String.Format(System.Globalization.CultureInfo.CurrentCulture, "{0}" & Microsoft.VisualBasic.Constants.vbLf + Microsoft.VisualBasic.Constants.vbLf & "{1}", "My Dynamic Combo", "72")
            Dim purger As New DialogBoxPurger(NativeMethods.IDOK, expectedDialogBoxText)
			Try
			purger.Start()
			dte.ExecuteCommand("Tools.DynamicCombo", "72")
			Finally
			Assert.IsTrue(purger.WaitForDialogThreadToTerminate(), "The Dynamic Combo dialog box has not shown")
			End Try
		End Sub
	End Class
End Namespace
