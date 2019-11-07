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
Imports System.Collections.Generic
Imports System.Reflection
Imports System.Text
Imports Microsoft.VsSDK.UnitTestLibrary
Imports Microsoft.VisualStudio
Imports Microsoft.VisualStudio.Shell.Interop
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Microsoft.Samples.VisualStudio.IDE.ToolWindow

Namespace Microsoft.Samples.VisualStudio.IDE.ToolWindow.UnitTests
	<TestClass()> _
	Public Class WindowListTest
		<TestMethod()> _
		Public Sub WindowsProperties()
			Dim targetAssembly As System.Reflection.Assembly = GetType(PackageToolWindow).Assembly
			Dim typeName As String = String.Format("{0}.{1}", GetType(PackageToolWindow).Namespace, "WindowList")
			Dim targetType As Type = targetAssembly.GetType(typeName)
			Dim instance As Object = Activator.CreateInstance(targetType)

            ' Provide our own list of mock frame.
			Dim frameList As IList(Of IVsWindowFrame) = New List(Of IVsWindowFrame)()
			Dim properties As Dictionary(Of Integer, Object) = GetProperties(0)
			frameList.Add(MockWindowFrameProvider.GetFrameWithProperties(properties))
			properties = GetProperties(1)
			frameList.Add(MockWindowFrameProvider.GetFrameWithProperties(properties))
            ' Get our instance to use that list.
			Dim field As FieldInfo = targetType.GetField("framesList", BindingFlags.NonPublic Or BindingFlags.Instance)
			field.SetValue(instance, frameList)

            ' We are ready to run our test so get the property.
			Dim windowsProperties As PropertyInfo = targetType.GetProperty("WindowsProperties", BindingFlags.NonPublic Or BindingFlags.Instance)
			Dim result As Object = windowsProperties.GetValue(instance, Nothing)

			Assert.IsNotNull(result, "WindowsProperties returned null")
			Assert.AreEqual(GetType(ArrayList), result.GetType(), "Incorrect Type returned")
			Dim array As ArrayList = CType(result, ArrayList)
			Assert.AreEqual(2, array.Count, "Number of windows is incorrect")
		End Sub

		Private Shared Function GetProperties(ByVal index As Integer) As Dictionary(Of Integer, Object)
            Dim properties As New Dictionary(Of Integer, Object)()
			properties.Add(CInt(Fix(__VSFPROPID.VSFPROPID_Caption)), "Tool Window " & index.ToString())
			properties.Add(CInt(Fix(__VSFPROPID.VSFPROPID_GuidPersistenceSlot)), Guid.NewGuid())
			Return properties
		End Function
	End Class
End Namespace
