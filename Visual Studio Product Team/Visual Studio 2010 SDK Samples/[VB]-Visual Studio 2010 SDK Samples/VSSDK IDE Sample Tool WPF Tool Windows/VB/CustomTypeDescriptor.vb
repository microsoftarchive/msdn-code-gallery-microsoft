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
Imports System.ComponentModel
Imports System.Collections.Generic
Imports System.Text

Namespace Microsoft.Samples.VisualStudio.IDE.ToolWindow
	''' <summary>
	''' This base class forwards all the ICustomTypeDescriptor calls
	''' to the default TypeDescriptor, except for GetComponentName.
	''' This allows for a class to specify the name that will be
	''' displayed in the combo box of the Properties window.
	''' </summary>
	Friend MustInherit Class CustomTypeDescriptor
		Implements ICustomTypeDescriptor
		''' <summary>
		''' Name of the component.
		''' When this class is used to expose property in the Properties
		''' window, this should be the name associated with this instance.
		''' </summary>
		Protected MustOverride ReadOnly Property ComponentName() As String

		#Region "ICustomTypeDescriptor Members"

		Private Function GetAttributes() As AttributeCollection Implements ICustomTypeDescriptor.GetAttributes
			Return TypeDescriptor.GetAttributes(Me.GetType())
		End Function

		Private Function GetClassName() As String Implements ICustomTypeDescriptor.GetClassName
			Return TypeDescriptor.GetClassName(Me.GetType())
		End Function

		Private Function GetComponentName() As String Implements ICustomTypeDescriptor.GetComponentName
			Return ComponentName
		End Function

		Private Function GetConverter() As TypeConverter Implements ICustomTypeDescriptor.GetConverter
			Return TypeDescriptor.GetConverter(Me.GetType())
		End Function

		Private Function GetDefaultEvent() As EventDescriptor Implements ICustomTypeDescriptor.GetDefaultEvent
			Return TypeDescriptor.GetDefaultEvent(Me.GetType())
		End Function

		Private Function GetDefaultProperty() As PropertyDescriptor Implements ICustomTypeDescriptor.GetDefaultProperty
			Return TypeDescriptor.GetDefaultProperty(Me.GetType())
		End Function

		Private Function GetEditor(ByVal editorBaseType As Type) As Object Implements ICustomTypeDescriptor.GetEditor
			Return TypeDescriptor.GetEditor(Me.GetType(), editorBaseType)
		End Function

		Private Function GetEvents(ByVal attributes As Attribute()) As EventDescriptorCollection Implements ICustomTypeDescriptor.GetEvents
			Return TypeDescriptor.GetEvents(Me.GetType(), attributes)
		End Function

		Private Function GetEvents() As EventDescriptorCollection Implements ICustomTypeDescriptor.GetEvents
			Return TypeDescriptor.GetEvents(Me.GetType())
		End Function

		Private Function GetProperties(ByVal attributes As Attribute()) As PropertyDescriptorCollection Implements ICustomTypeDescriptor.GetProperties
			Return TypeDescriptor.GetProperties(Me.GetType(), attributes)
		End Function

		Private Function GetProperties() As PropertyDescriptorCollection Implements ICustomTypeDescriptor.GetProperties
			Return TypeDescriptor.GetProperties(Me.GetType())
		End Function

		Private Function GetPropertyOwner(ByVal pd As PropertyDescriptor) As Object Implements ICustomTypeDescriptor.GetPropertyOwner
			Return Me
		End Function

		#End Region
	End Class
End Namespace
