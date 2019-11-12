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
Imports System.ComponentModel
Imports System.Drawing
Imports System.Data
Imports System.Windows.Forms
Imports System.Globalization
Imports Microsoft.VisualStudio
Imports Microsoft.VisualStudio.Shell
Imports Microsoft.VisualStudio.Shell.Interop
Imports MsVsShell = Microsoft.VisualStudio.Shell
Imports EnvDTE

Namespace MyCompany.RdtEventExplorer
	''' <summary>
	''' The RDT event explorer user control.
	''' Displays a log of events in a data grid.
	''' When an event is selected, its details appear in the Properties window.
	''' </summary>
	Public Partial Class RdtEventControl
		Inherits UserControl
		Implements IDisposable, IVsRunningDocTableEvents, IVsRunningDocTableEvents2, IVsRunningDocTableEvents3, IVsRunningDocTableEvents4
		' RDT
		Private rdtCookie As UInteger
		Private rdt As RunningDocumentTable

        ' Selection container.
		Private selectionContainer As MsVsShell.SelectionContainer

		' A reference to the single copy of the options.
		Private options As Options

		#Region "Constructor"
		''' <summary>
		''' The event explorer user control constructor.
		''' </summary>
		Public Sub New()
			InitializeComponent()

			' Create a selection container for tracking selected RDT events.
			selectionContainer = New MsVsShell.SelectionContainer()

			' Advise the RDT of this event sink.
			Dim sp As Microsoft.VisualStudio.OLE.Interop.IServiceProvider = TryCast(Package.GetGlobalService(GetType(Microsoft.VisualStudio.OLE.Interop.IServiceProvider)), Microsoft.VisualStudio.OLE.Interop.IServiceProvider)
			If sp Is Nothing Then
			Return
			End If

			rdt = New RunningDocumentTable(New ServiceProvider(sp))
			If rdt Is Nothing Then
			Return
			End If

			rdtCookie = rdt.Advise(Me)

			' Obtain the single instance of the options via automation. 
			Try
				Dim dte As DTE = CType(Package.GetGlobalService(GetType(DTE)), DTE)

				Dim props As EnvDTE.Properties = dte.Properties("RDT Event Explorer", "Explorer Options")

				Dim o As IOptions = TryCast(props.Item("ContainedOptions").Object, IOptions)
				options = CType(o, Options)
			Catch
				Dim log As IVsActivityLog = TryCast(Package.GetGlobalService(GetType(SVsActivityLog)), IVsActivityLog)
                If log IsNot Nothing Then
                    log.LogEntry(CUInt(__ACTIVITYLOG_ENTRYTYPE.ALE_INFORMATION), Me.ToString(), String.Format(CultureInfo.CurrentCulture, "RdtEventExplorer could not obtain properties via automation: {0}", Me.ToString()))
                End If
				options = New Options()
			End Try
			' Prepare the event grid.
			eventGrid.AutoGenerateColumns = False
			eventGrid.AllowUserToAddRows = False
			eventGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect

			eventGrid.Columns.Add("Event", Resources.EventHeader)
			eventGrid.Columns.Add("Moniker", Resources.MonikerHeader)
			eventGrid.Columns("Event").ReadOnly = True
			eventGrid.Columns("Moniker").ReadOnly = True

			eventGrid.AllowUserToResizeRows = False
			eventGrid.AllowUserToResizeColumns = True
			eventGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill

			Dim x As Integer = Screen.PrimaryScreen.Bounds.Size.Width
			Dim y As Integer = Screen.PrimaryScreen.Bounds.Size.Height
			Size = New Size(x \ 3, y \ 3)
		End Sub
		#End Region
		#Region "IDisposable Members"
		Private Sub IDisposable_Dispose() Implements IDisposable.Dispose
			Try
				If rdtCookie <> 0 Then
				rdt.Unadvise(rdtCookie)
				End If
			Finally
				MyBase.Dispose()
			End Try
		End Sub
		#End Region
		#Region "ProcessDialogChar"
		''' <summary> 
		''' Let this control process the mnemonics.
		''' </summary>
		Protected Overrides Function ProcessDialogChar(ByVal charCode As Char) As Boolean
            ' If we're the top-level form or control, we need to do the mnemonic handling.
            If charCode <> " " AndAlso ProcessMnemonic(charCode) Then
                Return True
            End If
			Return MyBase.ProcessDialogChar(charCode)
		End Function
		#End Region

		#Region "Add Event to Grid"
		''' <summary>
		''' Adds an RDT event wrapper to the grid.
		''' </summary>
		''' <param name="ev"></param>
		Public Sub AddEventToGrid(ByVal ev As GenericEvent)
			If ev Is Nothing Then
			Return
			End If

			Dim n As Integer = eventGrid.Rows.Add()
			Dim row As DataGridViewRow = eventGrid.Rows(n)
			row.Cells("Event").Value = ev.EventName
			row.Cells("Moniker").Value = ev.DocumentName
			row.Tag = ev
		End Sub
		#End Region
		#Region "IVsRunningDocTableEvents Members"
		Public Function OnAfterAttributeChange(ByVal docCookie As UInteger, ByVal grfAttribs As UInteger) As Integer Implements IVsRunningDocTableEvents.OnAfterAttributeChange, IVsRunningDocTableEvents2.OnAfterAttributeChange, IVsRunningDocTableEvents3.OnAfterAttributeChange
			If options.OptAfterAttributeChange Then
				AddEventToGrid(New AttributeEvent(rdt, "OnAfterAttributeChange", docCookie, grfAttribs))
			End If
			Return VSConstants.S_OK
		End Function
		Public Function OnAfterDocumentWindowHide(ByVal docCookie As UInteger, ByVal pFrame As IVsWindowFrame) As Integer Implements IVsRunningDocTableEvents.OnAfterDocumentWindowHide, IVsRunningDocTableEvents2.OnAfterDocumentWindowHide, IVsRunningDocTableEvents3.OnAfterDocumentWindowHide
			If options.OptAfterDocumentWindowHide Then
				AddEventToGrid(New WindowFrameEvent(rdt, "OnAfterDocumentWindowHide", docCookie, pFrame))
			End If
			Return VSConstants.S_OK
		End Function
		Public Function OnAfterFirstDocumentLock(ByVal docCookie As UInteger, ByVal dwRDTLockType As UInteger, ByVal dwReadLocksRemaining As UInteger, ByVal dwEditLocksRemaining As UInteger) As Integer Implements IVsRunningDocTableEvents.OnAfterFirstDocumentLock, IVsRunningDocTableEvents2.OnAfterFirstDocumentLock, IVsRunningDocTableEvents3.OnAfterFirstDocumentLock
			If options.OptAfterFirstDocumentLock Then
				AddEventToGrid(New LockEvent(rdt, "OnAfterFirstDocumentLock", docCookie, dwRDTLockType))
			End If
			Return VSConstants.S_OK
		End Function
		Public Function OnAfterSave(ByVal docCookie As UInteger) As Integer Implements IVsRunningDocTableEvents.OnAfterSave, IVsRunningDocTableEvents2.OnAfterSave, IVsRunningDocTableEvents3.OnAfterSave
			If options.OptAfterSave Then
				AddEventToGrid(New GenericEvent(rdt, "OnAfterSave", docCookie))
			End If
			Return VSConstants.S_OK
		End Function
		Public Function OnBeforeDocumentWindowShow(ByVal docCookie As UInteger, ByVal fFirstShow As Integer, ByVal pFrame As IVsWindowFrame) As Integer Implements IVsRunningDocTableEvents.OnBeforeDocumentWindowShow, IVsRunningDocTableEvents2.OnBeforeDocumentWindowShow, IVsRunningDocTableEvents3.OnBeforeDocumentWindowShow
			If options.OptBeforeDocumentWindowShow Then
				AddEventToGrid(New ShowEvent(rdt, "OnBeforeDocumentWindowShow", docCookie, fFirstShow, pFrame))
			End If
			Return VSConstants.S_OK
		End Function
		Public Function OnBeforeLastDocumentUnlock(ByVal docCookie As UInteger, ByVal dwRDTLockType As UInteger, ByVal dwReadLocksRemaining As UInteger, ByVal dwEditLocksRemaining As UInteger) As Integer Implements IVsRunningDocTableEvents.OnBeforeLastDocumentUnlock, IVsRunningDocTableEvents2.OnBeforeLastDocumentUnlock, IVsRunningDocTableEvents3.OnBeforeLastDocumentUnlock
			If options.OptBeforeLastDocumentUnlock Then
				AddEventToGrid(New LockEvent(rdt, "OnBeforeLastDocumentUnlock", docCookie, dwRDTLockType))
			End If
			Return VSConstants.S_OK
		End Function
		#End Region
		#Region "IVsRunningDocTableEvents2 Members"
		Public Function OnAfterAttributeChangeEx(ByVal docCookie As UInteger, ByVal grfAttribs As UInteger, ByVal pHierOld As IVsHierarchy, ByVal itemidOld As UInteger, ByVal pszMkDocumentOld As String, ByVal pHierNew As IVsHierarchy, ByVal itemidNew As UInteger, ByVal pszMkDocumentNew As String) As Integer Implements IVsRunningDocTableEvents2.OnAfterAttributeChangeEx, IVsRunningDocTableEvents3.OnAfterAttributeChangeEx
			If options.OptAfterAttributeChangeEx Then
				AddEventToGrid(New AttributeEventEx(rdt, "OnAfterAttributeChangeEx", docCookie, grfAttribs, pHierOld, itemidOld, pszMkDocumentOld, pHierNew, itemidNew, pszMkDocumentNew))
			End If
			Return VSConstants.S_OK
		End Function
		#End Region
		#Region "IVsRunningDocTableEvents3 Members"
		Public Function OnBeforeSave(ByVal docCookie As UInteger) As Integer Implements IVsRunningDocTableEvents3.OnBeforeSave
			If options.OptBeforeSave Then
				AddEventToGrid(New GenericEvent(rdt, "OnBeforeSave", docCookie))
			End If
			Return VSConstants.S_OK
		End Function
		#End Region
		#Region "IVsRunningDocTableEvents4 Members"
		Public Function OnAfterLastDocumentUnlock(ByVal pHier As IVsHierarchy, ByVal itemid As UInteger, ByVal pszMkDocument As String, ByVal fClosedWithoutSaving As Integer) As Integer Implements IVsRunningDocTableEvents4.OnAfterLastDocumentUnlock
			If options.OptAfterLastDocumentUnlock Then
				AddEventToGrid(New UnlockEventEx("OnAfterLastDocumentUnlock", pHier, itemid, pszMkDocument, fClosedWithoutSaving))
			End If
			Return VSConstants.S_OK
		End Function
		Public Function OnAfterSaveAll() As Integer Implements IVsRunningDocTableEvents4.OnAfterSaveAll
			If options.OptAfterSaveAll Then
				AddEventToGrid(New GenericEvent(Nothing, "OnAfterSaveAll", 0))
			End If
			Return VSConstants.S_OK
		End Function
		Public Function OnBeforeFirstDocumentLock(ByVal pHier As IVsHierarchy, ByVal itemid As UInteger, ByVal pszMkDocument As String) As Integer Implements IVsRunningDocTableEvents4.OnBeforeFirstDocumentLock
			If options.OptBeforeFirstDocumentLock Then
				AddEventToGrid(New LockEventEx("OnBeforeFirstDocumentLock", pHier, itemid, pszMkDocument))
			End If
			Return VSConstants.S_OK
		End Function
		#End Region

		#Region "Selection tracking"
        ' Cached Selection tracking service used to expose properties.
        Private fldTrackSelection As ITrackSelection

		''' <summary>
		''' Track selection service for the tool window.
		''' This should be set by the tool window pane as soon as the tool
		''' window is created.
		''' </summary>
		Friend Property TrackSelection() As ITrackSelection
			Get
                Return fldTrackSelection
			End Get
			Set(ByVal value As ITrackSelection)
				If value Is Nothing Then
					Throw New ArgumentNullException("TrackSelection")
				End If
                fldTrackSelection = value
				' Inititalize with an empty selection
				' Failure to do this would result in our later calls to 
				' OnSelectChange to be ignored (unless focus is lost
				' and regained).
				selectionContainer.SelectableObjects = Nothing
				selectionContainer.SelectedObjects = Nothing
                fldTrackSelection.OnSelectChange(selectionContainer)
			End Set
		End Property
		''' <summary>
		' Update the selection in the Properties window.
		''' </summary>
		Public Sub UpdateSelection()
			Dim track As ITrackSelection = TrackSelection
            If track IsNot Nothing Then
                track.OnSelectChange(CType(selectionContainer, ISelectionContainer))
            End If
		End Sub
		''' <summary>
		' Update the selection container.
		''' </summary>
		''' <param name="list">list of objects to be selected and selectable</param>
		Public Sub SelectList(ByVal list As ArrayList)
			selectionContainer = New MsVsShell.SelectionContainer(True, False)
			selectionContainer.SelectableObjects = list
			selectionContainer.SelectedObjects = list
			UpdateSelection()
		End Sub
		#End Region        
		#Region "Control events"
		''' <summary>
		''' Clear event lines from grid and refresh display to show empty grid.
		''' </summary>
		Public Sub ClearGrid()
			eventGrid.Rows.Clear()
			eventGrid.Refresh()
		End Sub
		''' <summary>
		''' Refresh the grid display.  May not be needed.
		''' </summary>
		''' <param name="sender"></param>
		''' <param name="e"></param>
		Public Sub RefreshGrid()
			eventGrid.Refresh()
		End Sub
		''' <summary>
		''' Track the event associated with selected grid row in the Properties window.
		''' </summary>
		''' <param name="sender"></param>
		''' <param name="e"></param>
		Private Sub eventGrid_CellClick(ByVal sender As Object, ByVal e As DataGridViewCellEventArgs) Handles eventGrid.CellClick
			' Ignore click on header row.
			If e.RowIndex < 0 Then
			Return
			End If

			' Find the selected row.
			Dim row As DataGridViewRow = eventGrid.Rows(e.RowIndex)
			' Recover the associated event object.
			Dim ev As GenericEvent = CType(row.Tag, GenericEvent)

			' Create an array of one event object and track it in the Properties window.
			Dim listObjects As ArrayList = New ArrayList()
			listObjects.Add(ev)
			SelectList(listObjects)
		End Sub
		#End Region
	End Class
End Namespace
