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
Imports Microsoft.Samples.VisualStudio.IDE.EditorWithToolbox

Namespace Microsoft.Samples.VisualStudio.IDE.EditorWithToolbox.UnitTests
	''' <summary>
    ''' This is a test class for EditorWithToolbox.ToolboxItemData and is intended
    ''' to contain all IDE.EditorWithToolbox.ToolboxItemData Unit Tests.
    ''' </summary>
	<TestClass()> _
	Public Class ToolboxItemDataTest
		#Region "Test methods"
		''' <summary>
		''' The test for ToolboxItemData default constructor.
        ''' </summary>
		<TestMethod()> _
		Public Sub ConstructorTest()
			Dim sentence As String = "This is MyToolbox test sentence"
            Dim target As New ToolboxItemData(sentence)
            Dim accessor As New Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_ToolboxItemDataAccessor(target)

			Assert.AreEqual(sentence, accessor.content, "ToolBox content was not properly initialized.")
		End Sub

		''' <summary>
		''' The test for the Content() method.
        ''' </summary>
		<TestMethod()> _
		Public Sub ContentTest()
			Dim sentence As String = "This is MyToolbox test sentence"
            Dim target As New ToolboxItemData(sentence)
			Assert.AreEqual(sentence, target.Content, "ToolBox content was not properly initialized.")
		End Sub
#End Region
	End Class
End Namespace
