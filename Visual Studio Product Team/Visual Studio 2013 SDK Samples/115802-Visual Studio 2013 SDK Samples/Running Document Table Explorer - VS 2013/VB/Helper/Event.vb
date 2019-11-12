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
Imports Microsoft.VisualStudio
Imports Microsoft.VisualStudio.Shell
Imports Microsoft.VisualStudio.Shell.Interop
Imports System.ComponentModel
Imports System.Globalization

Namespace MyCompany.RdtEventExplorer
	Public Class GenericEvent
		#Region "Member variables"
		Private rdi As RunningDocumentInfo
		Private message As String
		Private docNameShort As String
		Protected docNameLong As String

		' IVsRunningDocTableEvents
		Protected grfAttribs As UInteger
		Protected pFrame As IVsWindowFrame
		Protected fFirstShow As Integer
		Protected fClosedWithoutSaving As Integer
		Protected dwRDTLockType As UInteger

		' IVsRunningDocTableEvents2
        Private fldOldHierarchy As IVsHierarchy
        Private fldNewHierarchy As IVsHierarchy
		Protected itemidOld As UInteger
		Protected itemidNew As UInteger
		Public pszMkDocumentOld As String
		Public pszMkDocumentNew As String
		#End Region

		#Region "Constructors"
		''' <summary>
		''' Base class for all other RDT event wrappers.  Each event wrapper 
		''' stores event-specific information and formats it for display
		''' in the Properties window.
		''' </summary>
		''' <param name="rdt">Running Document Table instance</param>
		''' <param name="message">Message to be displayed in the grid</param>
		''' <param name="cookie">Cookie to unadvise RDT events</param>
		Public Sub New(ByVal rdt As RunningDocumentTable, ByVal message As String, ByVal cookie As UInteger)
			Me.message = message
			If rdt Is Nothing OrElse cookie = 0 Then
			Return
			End If

			rdi = rdt.GetDocumentInfo(cookie)
			docNameShort = GetShortDocName(rdi.Moniker)
		End Sub
		#End Region
		#Region "Public properties"
		<DisplayName("Event Name"), Description("The name of the event."), Category("Basic")> _
		Public ReadOnly Property EventName() As String
			Get
				Return message
			End Get
		End Property
		Private Shared slashDelim As Char() = { "\"c }
		<DisplayName("Doc Name, Short"), Description("The short name of the document."), Category("Basic")> _
		Public ReadOnly Property DocumentName() As String
			Get
				Return docNameShort
			End Get
		End Property
		<DisplayName("Doc Name, Long"), Description("The long name of the document."), Category("Basic")> _
		Public ReadOnly Property DocumentMoniker() As String
			Get
				Return rdi.Moniker
			End Get
		End Property
		<DisplayName("Locks, Read"), Description("The number of read locks."), Category("Basic")> _
		Public ReadOnly Property RLock() As String
			Get
				Return rdi.ReadLocks.ToString("d")
			End Get
		End Property
		<DisplayName("Locks, Edit"), Description("The number of edit locks."), Category("Basic")> _
		Public ReadOnly Property ELock() As String
			Get
				Return rdi.EditLocks.ToString("d", CultureInfo.CurrentCulture)
			End Get
		End Property
		Protected Property OldHierarchy() As IVsHierarchy
			Get
                Return fldOldHierarchy
			End Get
			Set(ByVal value As IVsHierarchy)
                fldOldHierarchy = value
			End Set
		End Property
		Protected Property NewHierarchy() As IVsHierarchy
			Get
                Return fldNewHierarchy
			End Get
			Set(ByVal value As IVsHierarchy)
                fldNewHierarchy = value
			End Set
		End Property
		#End Region

		#Region "Get and interpret"
		''' <summary>
		''' Formats the file monikor for shortened display.  
		''' Guids are shortened for display, as are full paths.
		''' </summary>
		''' <param name="moniker">the file moniker associated with an event</param>
		''' <returns></returns>
		Protected Shared Function GetShortDocName(ByVal moniker As String) As String
			If moniker Is Nothing Then
			Return String.Empty
			End If
			' Handle GUID form.
			Dim name As String = moniker
			Dim n As Integer = name.IndexOf("::{")
			If n > -1 Then
                Return name.Substring(0, n + "::{".Length + 8) & "...}"
			End If
			' Shorten name.
			Dim parts As String() = name.Split(slashDelim)
			If parts.Length < 1 Then
			Return name
			End If
			Return parts(parts.Length - 1)
		End Function
		''' <summary>
		'''  Formats hierarchy item ID for display.
		''' </summary>
		''' <returns>Returns item ID formatted in hex.</returns>
		Public Function GetItemidOld() As String
			Return String.Format(CultureInfo.CurrentCulture, "0x{0:X}", itemidOld)
		End Function
		''' <summary>
		'''  Formats hierarchy item ID for display.
		''' </summary>
		''' <returns>Returns item ID formatted in hex.</returns>
		Public Function GetItemidNew() As String
			Return String.Format(CultureInfo.CurrentCulture, "0x{0:X}", itemidNew)
		End Function
		''' <summary>
		'''  Formats lock type for display.  Parses bits and identifies them.
		''' </summary>
		''' <returns>Returns the formatted lock type.</returns>
		Public Function GetLockType() As String
			Dim s As String = ""
			Dim mask As _VSRDTFLAGS = CType(dwRDTLockType, _VSRDTFLAGS)

			If (mask And _VSRDTFLAGS.RDT_DontSave) <> 0 AndAlso (mask And _VSRDTFLAGS.RDT_DontSaveAs) <> 0 Then
			s &= "CantSave "
			Else
				If (mask And _VSRDTFLAGS.RDT_DontSave) <> 0 Then
				s &= "DontSave "
				End If
				If (mask And _VSRDTFLAGS.RDT_DontSaveAs) <> 0 Then
				s &= "DontSaveAs "
				End If
			End If
			If (mask And _VSRDTFLAGS.RDT_ReadLock) <> 0 Then
			s &= "ReadLock "
			End If
			If (mask And _VSRDTFLAGS.RDT_EditLock) <> 0 Then
			s &= "EditLock "
			End If

			If (mask And _VSRDTFLAGS.RDT_RequestUnlock) <> 0 Then
			s &= "RequestUnlock "
			End If
			If (mask And _VSRDTFLAGS.RDT_NonCreatable) <> 0 Then
			s &= "NonCreatable "
			End If
			If (mask And _VSRDTFLAGS.RDT_DontAutoOpen) <> 0 Then
			s &= "DontAutoOpen "
			End If
			If (mask And _VSRDTFLAGS.RDT_CaseSensitive) <> 0 Then
			s &= "CaseSensitive "
			End If

			If (mask And _VSRDTFLAGS.RDT_Unlock_NoSave) <> 0 Then
			s &= "Unlock_NoSave "
			End If
			If (mask And _VSRDTFLAGS.RDT_Unlock_SaveIfDirty) <> 0 Then
			s &= "Unlock_SaveIfDirty "
			End If
			If (mask And _VSRDTFLAGS.RDT_Unlock_PromptSave) <> 0 Then
			s &= "Unlock_PromptSave "
			End If

			If (mask And _VSRDTFLAGS.RDT_VirtualDocument) <> 0 Then
			s &= "VirtualDocument "
			End If
			If (mask And _VSRDTFLAGS.RDT_ProjSlnDocument) <> 0 Then
			s &= "ProjSlnDocument "
			End If
			If (mask And _VSRDTFLAGS.RDT_PlaceHolderDoc) <> 0 Then
			s &= "PlaceHolderDoc "
			End If
			If (mask And _VSRDTFLAGS.RDT_CanBuildFromMemory) <> 0 Then
			s &= "CanBuildFromMemory "
			End If
			If (mask And _VSRDTFLAGS.RDT_DontAddToMRU) <> 0 Then
			s &= "DontAddToMRU "
			End If

			Return String.Format(CultureInfo.CurrentCulture, "0x{0:X} ", dwRDTLockType) & s
		End Function
		''' <summary>
		'''  Formats attributes for display.  Parses bits and identifies them.
		''' </summary>
		''' <returns>Returns the formatted attributes.</returns>
		Public Function GetAttribs() As String
			Dim s As String = ""
			Dim mask As __VSRDTATTRIB = CType(grfAttribs, __VSRDTATTRIB)
			If (mask And __VSRDTATTRIB.RDTA_Hierarchy) <> 0 Then
			s &= "Hierarchy "
			End If
			If (mask And __VSRDTATTRIB.RDTA_ItemID) <> 0 Then
			s &= "ItemID "
			End If
			If (mask And __VSRDTATTRIB.RDTA_MkDocument) <> 0 Then
			s &= "FullPath "
			End If
			If (mask And __VSRDTATTRIB.RDTA_DocDataIsDirty) <> 0 Then
			s &= "DataIsDirty "
			End If
			If (mask And __VSRDTATTRIB.RDTA_DocDataIsNotDirty) <> 0 Then
			s &= "DataIsNotDirty "
			End If
			If (mask And __VSRDTATTRIB.RDTA_DocDataReloaded) <> 0 Then
			s &= "DocDataReloaded "
			End If
			If (mask And __VSRDTATTRIB.RDTA_AltHierarchyItemID) <> 0 Then
			s &= "AltHierarchyItemID "
			End If

			Return String.Format(CultureInfo.CurrentCulture, "0x{0:X} ", grfAttribs) & s
		End Function
		#End Region
	End Class
	Friend Class AttributeEvent
		Inherits GenericEvent
		<DisplayName("Attributes"), Description("The attribute flags."), Category("Extended")> _
		Public ReadOnly Property Attribute() As String
			Get
				Return GetAttribs()
			End Get
		End Property

		Public Sub New(ByVal rdt As RunningDocumentTable, ByVal message As String, ByVal cookie As UInteger, ByVal grfAttribs As UInteger)
			MyBase.New(rdt, message, cookie)
			Me.grfAttribs = grfAttribs
		End Sub
	End Class
	Friend Class WindowFrameEvent
		Inherits GenericEvent
		Public Sub New(ByVal rdt As RunningDocumentTable, ByVal message As String, ByVal cookie As UInteger, ByVal pFrame As IVsWindowFrame)
			MyBase.New(rdt, message, cookie)
			Me.pFrame = pFrame
		End Sub
	End Class
	Friend Class LockEvent
		Inherits GenericEvent
		<DisplayName("Lock Type"), Description("The lock flags."), Category("Extended")> _
		Public ReadOnly Property LockType() As String
			Get
				Return GetLockType()
			End Get
		End Property

		Public Sub New(ByVal rdt As RunningDocumentTable, ByVal message As String, ByVal cookie As UInteger, ByVal dwRDTLockType As UInteger)
			MyBase.New(rdt, message, cookie)
			Me.dwRDTLockType = dwRDTLockType
		End Sub
	End Class
	Friend Class ShowEvent
		Inherits GenericEvent
		<DisplayName("First Show"), Description("True if this is the first time the document is shown."), Category("Extended")> _
		Public ReadOnly Property IsFirstShow() As Integer
			Get
				Return fFirstShow
			End Get
		End Property

		Public Sub New(ByVal rdt As RunningDocumentTable, ByVal message As String, ByVal cookie As UInteger, ByVal fFirstShow As Integer, ByVal pFrame As IVsWindowFrame)
			MyBase.New(rdt, message, cookie)
			Me.fFirstShow = fFirstShow
			Me.pFrame = pFrame
		End Sub
	End Class
	' Extended events.
	Friend Class EventEx
		Inherits GenericEvent
		<DisplayName("Hierarchy"), Description("The caption of the old hierarchy root."), Category("Old")> _
		Public ReadOnly Property OldHierarchyRoot() As String
			Get
				If OldHierarchy Is Nothing Then
				Return "null"
				End If
				Dim o As Object = String.Empty
				Try
					OldHierarchy.GetProperty(VSConstants.VSITEMID_ROOT, CInt(Fix(__VSHPROPID.VSHPROPID_Caption)), o)
				Finally
				End Try
				Return CStr(o)
			End Get
		End Property
		<DisplayName("Document"), Description("The long name of the old document."), Category("Old")> _
		Public ReadOnly Property OldDocMoniker() As String
			Get
				Return pszMkDocumentOld
			End Get
		End Property
		<DisplayName("Item ID"), Description("The old item ID."), Category("Old")> _
		Public ReadOnly Property OldItemId() As String
			Get
				Return GetItemidOld()
			End Get
		End Property

		Public Sub New(ByVal rdt As RunningDocumentTable, ByVal message As String, ByVal cookie As UInteger, ByVal oldHierarchy As IVsHierarchy, ByVal itemidOld As UInteger, ByVal pszMkDocumentOld As String)
			MyBase.New(rdt, message, cookie)
			Me.OldHierarchy = oldHierarchy
			Me.itemidOld = itemidOld
			Me.pszMkDocumentOld = pszMkDocumentOld
		End Sub
	End Class
	Friend Class AttributeEventEx
		Inherits EventEx
		<DisplayName("Attributes"), Description("The attribute flags."), Category("Extended")> _
		Public ReadOnly Property Attribute() As String
			Get
				Return GetAttribs()
			End Get
		End Property

		<DisplayName("Hierarchy"), Description("The caption of the new hierarchy root."), Category("New")> _
		Public ReadOnly Property NewHierarchyRoot() As String
			Get
				If NewHierarchy Is Nothing Then
				Return "null"
				End If
				Dim o As Object = String.Empty
				Try
					NewHierarchy.GetProperty(VSConstants.VSITEMID_ROOT, CInt(Fix(__VSHPROPID.VSHPROPID_Caption)), o)
				Finally
				End Try
				Return CStr(o)
			End Get
		End Property
		<DisplayName("Document"), Description("The long name of the new document."), Category("New")> _
		Public ReadOnly Property NewDocMoniker() As String
			Get
				Return pszMkDocumentNew
			End Get
		End Property
		<DisplayName("Item ID"), Description("The new item ID."), Category("New")> _
		Public ReadOnly Property NewItemId() As String
			Get
				Return GetItemidNew()
			End Get
		End Property

		Public Sub New(ByVal rdt As RunningDocumentTable, ByVal message As String, ByVal cookie As UInteger, ByVal grfAttribs As UInteger, ByVal oldHierarchy As IVsHierarchy, ByVal itemidOld As UInteger, ByVal pszMkDocumentOld As String, ByVal newHierarchy As IVsHierarchy, ByVal itemidNew As UInteger, ByVal pszMkDocumentNew As String)
			MyBase.New(rdt, message, cookie, oldHierarchy, itemidOld, pszMkDocumentOld)
			Me.grfAttribs = grfAttribs
			Me.NewHierarchy = newHierarchy
			Me.itemidNew = itemidNew
			Me.pszMkDocumentNew = pszMkDocumentNew
		End Sub
	End Class
	Friend Class UnlockEventEx
		Inherits EventEx
		<DisplayName("Close, No Save"), Description("True if the document is closed without saving."), Category("Extended")> _
		Public ReadOnly Property IsCloseNoSave() As Integer
			Get
				Return fClosedWithoutSaving
			End Get
		End Property

		Public Sub New(ByVal message As String, ByVal pHier As IVsHierarchy, ByVal itemid As UInteger, ByVal pszMkDocument As String, ByVal fClosedWithoutSaving As Integer)
			MyBase.New(Nothing, message, 0, pHier, itemid, pszMkDocument)
			Me.fClosedWithoutSaving = fClosedWithoutSaving
		End Sub
	End Class
	Friend Class LockEventEx
		Inherits EventEx
		Public Sub New(ByVal message As String, ByVal pHier As IVsHierarchy, ByVal itemid As UInteger, ByVal pszMkDocument As String)
			MyBase.New(Nothing, message, 0, pHier, itemid, pszMkDocument)
			Me.OldHierarchy = pHier
			Me.itemidOld = itemid
			Me.pszMkDocumentOld = pszMkDocument
		End Sub
	End Class
End Namespace
