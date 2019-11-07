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
Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.VsSDK.UnitTestLibrary
Imports Microsoft.VisualStudio
Imports Microsoft.VisualStudio.Shell.Interop
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Microsoft.Samples.VisualStudio.IDE.ToolWindow

Namespace Microsoft.Samples.VisualStudio.IDE.ToolWindow.UnitTests
	<TestClass()> _
	Public Class WindowStatusTest
		Private count As Integer

		<TestMethod()> _
		Public Sub OnShow()
			Dim windowStatus As WindowStatus = GetWindowStatusInstance()

			Dim show As Integer = 1
			Dim hr As Integer = windowStatus.OnShow(show)
			Assert.AreEqual(0, hr, "Method call failed")
			Assert.AreEqual(0, count, "Incorrect number of events generated")
		End Sub

		<TestMethod()> _
		Public Sub OnClose()
			Dim windowStatus As WindowStatus = GetWindowStatusInstance()

			Dim saveOption As UInteger = &H100
			Dim hr As Integer = windowStatus.OnClose(saveOption)
			Assert.AreEqual(0, hr, "Method call failed")
			Assert.AreEqual(CUInt(&H100), saveOption, "Save Option changed")
			Assert.AreEqual(0, count, "Incorrect number of events generated")
		End Sub

		<TestMethod()> _
		Public Sub OnMove()
			Dim windowStatus As WindowStatus = GetWindowStatusInstance()

			Dim x As Integer = 1, y As Integer = 2, w As Integer = 3, h As Integer = 4
			Dim hr As Integer = windowStatus.OnMove(x, y, w, h)
			Assert.AreEqual(0, hr, "Method call failed")
			Assert.AreEqual(x, windowStatus.X, "Failed to set X")
			Assert.AreEqual(y, windowStatus.Y, "Failed to set Y")
			Assert.AreEqual(w, windowStatus.Width, "Failed to set W")
			Assert.AreEqual(h, windowStatus.Height, "Failed to set H")
			Assert.AreEqual(1, count, "Incorrect number of events generated")
		End Sub

		<TestMethod()> _
		Public Sub OnSize()
			Dim windowStatus As WindowStatus = GetWindowStatusInstance()

			Dim x As Integer = 7, y As Integer = 8, w As Integer = 9, h As Integer = 10
			Dim hr As Integer = windowStatus.OnSize(x, y, w, h)
			Assert.AreEqual(0, hr, "Method call failed")
			Assert.AreEqual(x, windowStatus.X, "Failed to set X")
			Assert.AreEqual(y, windowStatus.Y, "Failed to set Y")
			Assert.AreEqual(w, windowStatus.Width, "Failed to set W")
			Assert.AreEqual(h, windowStatus.Height, "Failed to set H")
			Assert.AreEqual(1, count, "Incorrect number of events generated")
		End Sub

		<TestMethod()> _
		Public Sub OnDockableChange()
			Dim windowStatus As WindowStatus = GetWindowStatusInstance()

			Dim x As Integer = 71, y As Integer = 82, w As Integer = 93, h As Integer = 104, docked As Integer=1
			Dim hr As Integer = windowStatus.OnDockableChange(docked, x, y, w, h)
			Assert.AreEqual(0, hr, "Method call failed")
			Assert.AreEqual(x, windowStatus.X, "Failed to set X")
			Assert.AreEqual(y, windowStatus.Y, "Failed to set Y")
			Assert.AreEqual(w, windowStatus.Width, "Failed to set W")
			Assert.AreEqual(h, windowStatus.Height, "Failed to set H")
			Assert.AreEqual(True, windowStatus.IsDockable, "Failed to set Docked")
			Assert.AreEqual(1, count, "Incorrect number of events generated")
		End Sub

		Private Function GetWindowStatusInstance() As WindowStatus
            Dim windowStatus As New WindowStatus(Nothing,Nothing)
			Me.count = 0
            AddHandler windowStatus.StatusChange, AddressOf Me.StatusChanged

			Return windowStatus
		End Function

		Private Sub StatusChanged(ByVal sender As Object, ByVal arguments As EventArgs)
			count += 1
		End Sub
	End Class
End Namespace
