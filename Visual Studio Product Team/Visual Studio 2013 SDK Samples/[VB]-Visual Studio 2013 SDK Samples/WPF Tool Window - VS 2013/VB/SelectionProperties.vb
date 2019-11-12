'**************************************************************************
'
'Copyright (c) Microsoft Corporation. All rights reserved.
'THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
'ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
'IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
'PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'**************************************************************************

Imports Microsoft.VisualBasic
Imports System
Imports System.ComponentModel
Imports System.Collections.Generic
Imports System.Text

Namespace Microsoft.Samples.VisualStudio.IDE.ToolWindow
	''' <summary>
	''' This class holds the properties that will be displayed in the Properties
	''' window for the selected object.
	''' 
	''' While all these properties are read-only, defining the set method would
	''' make them writable.
	''' 
	''' We derive from CustomTypeDescriptor, which is an ICustomTypeDescriptor, and
	''' the only part that we overload is the ComponentName.
	''' </summary>
	Friend Class SelectionProperties
		Inherits CustomTypeDescriptor
		Private caption_Renamed As String = String.Empty
		Private persistanceGuid As Guid = Guid.Empty
		Private index_Renamed As Integer = -1

		''' <summary>
		''' This class holds the properties for one item.
		''' The list of properties could be modified to display a different
		''' set of properties.
		''' </summary>
		''' <param name="caption">Window Title</param>
		''' <param name="persistence">Persistence Guid</param>
		Public Sub New(ByVal caption_Renamed As String, ByVal persistence As Guid)
			Me.caption_Renamed = caption_Renamed
			Me.persistanceGuid = persistence
		End Sub

		''' <summary>
		''' Caption property that will be exposed in the Properties window.
		''' </summary>
		Public ReadOnly Property Caption() As String
			Get
				Return caption_Renamed
			End Get
		End Property

		''' <summary>
		''' Guid corresponding to the tool window.
		''' Note that this property uses attributes to provide richer data
		''' to the Properties window.
		''' </summary>
		<System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode"), DisplayName("Persistence GUID"), Description("Guids used to uniquely identify the window type."), Category("Advanced")> _
		Public ReadOnly Property PersistenceGuid() As String
			Get
				Return persistanceGuid.ToString("B")
			End Get
		End Property

		''' <summary>
		''' Index of the window in our list. We use this internally to avoid having to
		''' search the list of windows when the selection is changed from the Property
		''' window.
        ''' This property will not be visible because we are using the Browsable(false) attribute.
		''' </summary>
		<Browsable(False)> _
		Public Property Index() As Integer
			Get
				Return index_Renamed
			End Get
			Set(ByVal value As Integer)
				index_Renamed = value
			End Set
		End Property

		''' <summary>
		''' String that will be displayed in the Properties window combo box.
		''' </summary>
		Protected Overrides ReadOnly Property ComponentName() As String
			Get
				Return Me.Caption
			End Get
		End Property
	End Class
End Namespace
