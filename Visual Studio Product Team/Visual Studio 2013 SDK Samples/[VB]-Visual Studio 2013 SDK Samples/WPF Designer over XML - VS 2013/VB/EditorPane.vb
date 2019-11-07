'**************************************************************************
'
'Copyright (c) Microsoft Corporation. All rights reserved.
'THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
'ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
'IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
'PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'**************************************************************************

Imports System.Collections
Imports System.ComponentModel
Imports System.ComponentModel.Design
Imports System.IO
Imports System.Globalization
Imports System.Runtime.InteropServices
Imports System.Security.Permissions
Imports Microsoft.VisualStudio
Imports Microsoft.VisualStudio.Shell.Interop
Imports Microsoft.VisualStudio.OLE.Interop
Imports Microsoft.VisualStudio.TextManager.Interop
Imports Microsoft.VisualStudio.Shell
Imports Microsoft.VisualStudio.XmlEditor
Imports EnvDTE
Imports tom


Imports VSStd97CmdID = Microsoft.VisualStudio.VSConstants.VSStd97CmdID

Namespace Microsoft.VsTemplateDesigner
	''' <summary>
	''' This control hosts the editor and is responsible for
	''' handling the commands targeted to the editor 
	''' </summary>

	<ComVisible(True)>
	Public NotInheritable Class EditorPane
		Inherits Microsoft.VisualStudio.Shell.WindowPane
		Implements IOleComponent, IVsDeferredDocView, IVsLinkedUndoClient
		#Region "Fields"
		Private _thisPackage As VsTemplateDesignerPackage
		Private _fileName As String = String.Empty
		Private _vsDesignerControl As VsDesignerControl
		Private _textBuffer As IVsTextLines
		Private _componentId As UInteger
		Private _undoManager As IOleUndoManager
		Private _store As XmlStore
		Private _model As XmlModel
		#End Region

		#region "Window.Pane Overrides"
		''' <summary>
		''' Constructor that calls the Microsoft.VisualStudio.Shell.WindowPane constructor then
		''' our initialization functions.
		''' </summary>
		''' <param name="package">Our Package instance.</param>
		Public Sub New(ByVal package As VsTemplateDesignerPackage, ByVal fileName As String, ByVal textBuffer As IVsTextLines)
			MyBase.New(Nothing)
			_thisPackage = package
			_fileName = fileName
			_textBuffer = textBuffer
		End Sub

		Protected Overrides Sub OnClose()
			' unhook from Undo related services
			If _undoManager IsNot Nothing Then
				Dim linkCapableUndoMgr As IVsLinkCapableUndoManager = CType(_undoManager, IVsLinkCapableUndoManager)
				If linkCapableUndoMgr IsNot Nothing Then
					linkCapableUndoMgr.UnadviseLinkedUndoClient()
				End If

				' Throw away the undo stack etc.
				' It is important to "zombify" the undo manager when the owning object is shutting down.
				' This is done by calling IVsLifetimeControlledObject.SeverReferencesToOwner on the undoManager.
				' This call will clear the undo and redo stacks. This is particularly important to do if
				' your undo units hold references back to your object. It is also important if you use
				' "mdtStrict" linked undo transactions as this sample does (see IVsLinkedUndoTransactionManager). 
				' When one object involved in linked undo transactions clears its undo/redo stacks, then 
				' the stacks of the other documents involved in the linked transaction will also be cleared. 
				Dim lco As IVsLifetimeControlledObject = CType(_undoManager, IVsLifetimeControlledObject)
				lco.SeverReferencesToOwner()
				_undoManager = Nothing
			End If

			Dim mgr As IOleComponentManager = TryCast(GetService(GetType(SOleComponentManager)), IOleComponentManager)
			mgr.FRevokeComponent(_componentId)

			Me.Dispose(True)

			MyBase.OnClose()
		End Sub
		#End Region

		''' <summary>
		''' Called after the WindowPane has been sited with an IServiceProvider from the environment
		''' 
		Protected Overrides Sub Initialize()
			MyBase.Initialize()

			' Create and initialize the editor
'			#Region "Register with IOleComponentManager"
			Dim componentManager As IOleComponentManager = CType(GetService(GetType(SOleComponentManager)), IOleComponentManager)
			If Me._componentId = 0 AndAlso componentManager IsNot Nothing Then
                Dim crinfo(0) As OLECRINFO
                With crinfo(0)
                    .cbSize = CUInt(Marshal.SizeOf(GetType(OLECRINFO)))
                    .grfcrf = CUInt(_OLECRF.olecrfNeedIdleTime) Or CUInt(_OLECRF.olecrfNeedPeriodicIdleTime)
                    .grfcadvf = CUInt(_OLECADVF.olecadvfModal) Or CUInt(_OLECADVF.olecadvfRedrawOff) Or CUInt(_OLECADVF.olecadvfWarningsOff)
                    .uIdleTimeInterval = 100
                End With
                Dim hr = componentManager.FRegisterComponent(Me, crinfo, Me._componentId)
                ErrorHandler.Succeeded(hr)
            End If
'			#End Region

			Dim resources As New System.ComponentModel.ComponentResourceManager(GetType(EditorPane))

'			#Region "Hook Undo Manager"
			' Attach an IOleUndoManager to our WindowFrame. Merely calling QueryService 
			' for the IOleUndoManager on the site of our IVsWindowPane causes an IOleUndoManager
			' to be created and attached to the IVsWindowFrame. The WindowFrame automaticall 
			' manages to route the undo related commands to the IOleUndoManager object.
			' Thus, our only responsibilty after this point is to add IOleUndoUnits to the 
			' IOleUndoManager (aka undo stack).
			_undoManager = CType(GetService(GetType(SOleUndoManager)), IOleUndoManager)

			' In order to use the IVsLinkedUndoTransactionManager, it is required that you
			' advise for IVsLinkedUndoClient notifications. This gives you a callback at 
			' a point when there are intervening undos that are blocking a linked undo.
			' You are expected to activate your document window that has the intervening undos.
			If _undoManager IsNot Nothing Then
				Dim linkCapableUndoMgr As IVsLinkCapableUndoManager = CType(_undoManager, IVsLinkCapableUndoManager)
				If linkCapableUndoMgr IsNot Nothing Then
					linkCapableUndoMgr.AdviseLinkedUndoClient(Me)
				End If
			End If
'			#End Region

			' hook up our 
			Dim es As XmlEditorService = TryCast(GetService(GetType(XmlEditorService)), XmlEditorService)
			_store = es.CreateXmlStore()
			_store.UndoManager = _undoManager

			_model = _store.OpenXmlModel(New Uri(_fileName))

			' This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
			' we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on 
			' the object returned by the Content property.
			_vsDesignerControl = New VsDesignerControl(New ViewModel(_store, _model, Me, _textBuffer))
			MyBase.Content = _vsDesignerControl

			RegisterIndependentView(True)

			Dim mcs As IMenuCommandService = TryCast(GetService(GetType(IMenuCommandService)), IMenuCommandService)
			If Nothing IsNot mcs Then
				' Now create one object derived from MenuCommnad for each command defined in
				' the CTC file and add it to the command service.

				' For each command we have to define its id that is a unique Guid/integer pair, then
				' create the OleMenuCommand object for this command. The EventHandler object is the
				' function that will be called when the user will select the command. Then we add the 
				' OleMenuCommand to the menu service.  The addCommand helper function does all this for us.
                AddCommand(mcs, VSConstants.GUID_VSStandardCommandSet97, CInt(Fix(VSConstants.VSStd97CmdID.NewWindow)),
                           New EventHandler(AddressOf OnNewWindow), New EventHandler(AddressOf OnQueryNewWindow))
                AddCommand(mcs, VSConstants.GUID_VSStandardCommandSet97, CInt(Fix(VSConstants.VSStd97CmdID.ViewCode)),
                           New EventHandler(AddressOf OnViewCode), New EventHandler(AddressOf OnQueryViewCode))
			End If
		End Sub

		''' <summary>
		''' returns the name of the file currently loaded
		''' </summary>
		Public ReadOnly Property FileName() As String
			Get
				Return _fileName
			End Get
		End Property

		''' <summary> 
		''' Clean up any resources being used.
		''' </summary>
		Protected Overrides Overloads Sub Dispose(ByVal disposing As Boolean)
			If disposing Then
				RegisterIndependentView(False)

				Using _model
					_model = Nothing
				End Using
				Using _store
					_store = Nothing
				End Using
			End If
			MyBase.Dispose(disposing)
		End Sub

		''' <summary>
		''' Gets an instance of the RunningDocumentTable (RDT) service which manages the set of currently open 
		''' documents in the environment and then notifies the client that an open document has changed
		''' </summary>
		Private Sub NotifyDocChanged()
			' Make sure that we have a file name
			If _fileName.Length = 0 Then
				Return
			End If

			' Get a reference to the Running Document Table
            Dim runningDocTable As IVsRunningDocumentTable = CType(GetService(GetType(SVsRunningDocumentTable)), 
                IVsRunningDocumentTable)

			' Lock the document
			Dim docCookie As UInteger
            Dim hierarchy As IVsHierarchy = Nothing
			Dim itemID As UInteger
			Dim docData As IntPtr
            Dim hr As Integer = runningDocTable.FindAndLockDocument(CUInt(_VSRDTFLAGS.RDT_ReadLock), _fileName, hierarchy,
                                                                    itemID, docData, docCookie)
			ErrorHandler.ThrowOnFailure(hr)

			' Send the notification
			hr = runningDocTable.NotifyDocumentChanged(docCookie, CUInt(__VSRDTATTRIB.RDTA_DocDataReloaded))

			' Unlock the document.
			' Note that we have to unlock the document even if the previous call failed.
			ErrorHandler.ThrowOnFailure(runningDocTable.UnlockDocument(CUInt(_VSRDTFLAGS.RDT_ReadLock), docCookie))

			' Check ff the call to NotifyDocChanged failed.
			ErrorHandler.ThrowOnFailure(hr)
		End Sub

		''' <summary>
		''' Helper function used to add commands using IMenuCommandService
		''' </summary>
		''' <param name="mcs"> The IMenuCommandService interface.</param>
		''' <param name="menuGroup"> This guid represents the menu group of the command.</param>
		''' <param name="cmdID"> The command ID of the command.</param>
		''' <param name="commandEvent"> An EventHandler which will be called whenever the command is invoked.</param>
		''' <param name="queryEvent"> An EventHandler which will be called whenever we want to query the status of
		''' the command.  If null is passed in here then no EventHandler will be added.</param>
        Private Shared Sub AddCommand(ByVal mcs As IMenuCommandService, ByVal menuGroup As Guid, ByVal cmdID As Integer,
                                      ByVal commandEvent As EventHandler, ByVal queryEvent As EventHandler)
            ' Create the OleMenuCommand from the menu group, command ID, and command event
            Dim menuCommandID As New CommandID(menuGroup, cmdID)
            Dim command As New OleMenuCommand(commandEvent, menuCommandID)

            ' Add an event handler to BeforeQueryStatus if one was passed in
            If Nothing IsNot queryEvent Then
                AddHandler command.BeforeQueryStatus, queryEvent
            End If

            ' Add the command using our IMenuCommandService instance
            mcs.AddCommand(command)
        End Sub

		''' <summary>
		''' Registers an independent view with the IVsTextManager so that it knows
		''' the user is working with a view over the text buffer. This will trigger
		''' the text buffer to prompt the user whether to reload the file if it is
		''' edited outside of the environment.
		''' </summary>
		''' <param name="subscribe">True to subscribe, false to unsubscribe</param>
		Private Sub RegisterIndependentView(ByVal subscribe As Boolean)
			Dim textManager As IVsTextManager = CType(GetService(GetType(SVsTextManager)), IVsTextManager)

			If textManager IsNot Nothing Then
				If subscribe Then
					textManager.RegisterIndependentView(CType(Me, IVsWindowPane), Me._textBuffer)
				Else
					textManager.UnregisterIndependentView(CType(Me, IVsWindowPane), Me._textBuffer)
				End If
			End If
		End Sub

		''' <summary>
		''' This method loads a localized string based on the specified resource.
		''' </summary>
		''' <param name="resourceName">Resource to load</param>
		''' <returns>String loaded for the specified resource</returns>
		Friend Function GetResourceString(ByVal resourceName As String) As String
            Dim resourceValue As String = Nothing
			Dim resourceManager As IVsResourceManager = CType(GetService(GetType(SVsResourceManager)), IVsResourceManager)
			If resourceManager Is Nothing Then
				Throw New InvalidOperationException("Could not get SVsResourceManager service. Make sure the package is Sited before calling this method")
			End If
			Dim packageGuid As Guid = _thisPackage.GetType().GUID
			Dim hr As Integer = resourceManager.LoadResourceString(packageGuid, -1, resourceName, resourceValue)
			Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(hr)
			Return resourceValue
		End Function

		#Region "Commands"

		Private Sub OnQueryNewWindow(ByVal sender As Object, ByVal e As EventArgs)
			Dim command As OleMenuCommand = CType(sender, OleMenuCommand)
			command.Enabled = True
		End Sub

		Private Sub OnNewWindow(ByVal sender As Object, ByVal e As EventArgs)
			NewWindow()
		End Sub

		Private Sub OnQueryViewCode(ByVal sender As Object, ByVal e As EventArgs)
			Dim command As OleMenuCommand = CType(sender, OleMenuCommand)
			command.Enabled = True
		End Sub

		Private Sub OnViewCode(ByVal sender As Object, ByVal e As EventArgs)
			ViewCode()
		End Sub

		Private Sub NewWindow()
			Dim hr As Integer = VSConstants.S_OK

			Dim uishellOpenDocument As IVsUIShellOpenDocument = CType(GetService(GetType(SVsUIShellOpenDocument)), IVsUIShellOpenDocument)
			If uishellOpenDocument IsNot Nothing Then
				Dim windowFrameOrig As IVsWindowFrame = CType(GetService(GetType(SVsWindowFrame)), IVsWindowFrame)
				If windowFrameOrig IsNot Nothing Then
                    Dim windowFrameNew As IVsWindowFrame = Nothing
					Dim LOGVIEWID_Primary As Guid = Guid.Empty
					hr = uishellOpenDocument.OpenCopyOfStandardEditor(windowFrameOrig, LOGVIEWID_Primary, windowFrameNew)
					If windowFrameNew IsNot Nothing Then
						hr = windowFrameNew.Show()
					End If
					ErrorHandler.ThrowOnFailure(hr)
				End If
			End If
		End Sub

		Private Sub ViewCode()
			Dim XmlTextEditorGuid As New Guid("FA3CD31E-987B-443A-9B81-186104E8DAC1")

			' Open the referenced document using our editor.
            Dim frame As IVsWindowFrame = Nothing
            Dim hierarchy As IVsUIHierarchy = Nothing
			Dim itemid As UInteger
			VsShellUtilities.OpenDocumentWithSpecificEditor(Me, _model.Name, XmlTextEditorGuid, VSConstants.LOGVIEWID_Primary, hierarchy, itemid, frame)
			ErrorHandler.ThrowOnFailure(frame.Show())
		End Sub

		#End Region

		#Region "IVsLinkedUndoClient"

		Public Function OnInterveningUnitBlockingLinkedUndo() As Integer Implements IVsLinkedUndoClient.OnInterveningUnitBlockingLinkedUndo
			Return VSConstants.E_FAIL
		End Function

		#End Region

		#Region "IVsDeferredDocView"

		''' <summary>
		''' Assigns out parameter with the Guid of the EditorFactory.
		''' </summary>
		''' <param name="pGuidCmdId">The output parameter that receives a value of the Guid of the EditorFactory.</param>
		''' <returns>S_OK if Marshal operations completed successfully.</returns>
		Private Function get_CmdUIGuid(<System.Runtime.InteropServices.Out()> ByRef pGuidCmdId As Guid) As Integer Implements IVsDeferredDocView.get_CmdUIGuid
			pGuidCmdId = GuidList.guidVsTemplateDesignerEditorFactory
			Return VSConstants.S_OK
		End Function

		''' <summary>
		''' Assigns out parameter with the document view being implemented.
		''' </summary>
		''' <param name="ppUnkDocView">The parameter that receives a reference to current view.</param>
		''' <returns>S_OK if Marshal operations completed successfully.</returns>
		<EnvironmentPermission(SecurityAction.Demand)>
		Private Function get_DocView(<System.Runtime.InteropServices.Out()> ByRef ppUnkDocView As IntPtr) As Integer Implements IVsDeferredDocView.get_DocView
			ppUnkDocView = Marshal.GetIUnknownForObject(Me)
			Return VSConstants.S_OK
		End Function

		#End Region

		#Region "IOleComponent"

		Private Function FContinueMessageLoop(ByVal uReason As UInteger, ByVal pvLoopData As IntPtr, ByVal pMsgPeeked() As MSG) As Integer Implements IOleComponent.FContinueMessageLoop
			Return VSConstants.S_OK
		End Function

		Private Function FDoIdle(ByVal grfidlef As UInteger) As Integer Implements IOleComponent.FDoIdle
			If _vsDesignerControl IsNot Nothing Then
				_vsDesignerControl.DoIdle()
			End If
			Return VSConstants.S_OK
		End Function

		Private Function FPreTranslateMessage(ByVal pMsg() As MSG) As Integer Implements IOleComponent.FPreTranslateMessage
			Return VSConstants.S_OK
		End Function

		Private Function FQueryTerminate(ByVal fPromptUser As Integer) As Integer Implements IOleComponent.FQueryTerminate
			Return 1 'true
		End Function

		Private Function FReserved1(ByVal dwReserved As UInteger, ByVal message As UInteger, ByVal wParam As IntPtr, ByVal lParam As IntPtr) As Integer Implements IOleComponent.FReserved1
			Return VSConstants.S_OK
		End Function

		Private Function HwndGetWindow(ByVal dwWhich As UInteger, ByVal dwReserved As UInteger) As IntPtr Implements IOleComponent.HwndGetWindow
			Return IntPtr.Zero
		End Function

        Private Sub OnActivationChange(ByVal pic As IOleComponent, ByVal fSameComponent As Integer,
                                       ByVal pcrinfo() As OLECRINFO, ByVal fHostIsActivating As Integer,
                                       ByVal pchostinfo() As OLECHOSTINFO, ByVal dwReserved As UInteger) Implements IOleComponent.OnActivationChange
        End Sub
		Private Sub OnAppActivate(ByVal fActive As Integer, ByVal dwOtherThreadID As UInteger) Implements IOleComponent.OnAppActivate
		End Sub
		Private Sub OnEnterState(ByVal uStateID As UInteger, ByVal fEnter As Integer) Implements IOleComponent.OnEnterState
		End Sub
		Private Sub OnLoseActivation() Implements IOleComponent.OnLoseActivation
		End Sub
		Private Sub Terminate() Implements IOleComponent.Terminate
		End Sub

		#End Region
	End Class
End Namespace
