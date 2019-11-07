'***************************************************************************
' Copyright © 2010 Microsoft Corporation.  All Rights Reserved.
' This code released under the terms of the 
' Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
'***************************************************************************
Imports System.Globalization
Imports System.Runtime.InteropServices
Imports System.Security.Permissions
Imports Microsoft.VisualStudio
Imports Microsoft.VisualStudio.Shell.Interop
Imports Microsoft.VisualStudio.Shell
Imports Microsoft.VisualStudio.TextManager.Interop
Imports Microsoft.VisualStudio.OLE.Interop



Namespace Microsoft.VsTemplateDesigner
	''' <summary>
	''' Factory for creating our editor object. Extends from the IVsEditoryFactory interface
	''' </summary>
	<Guid(GuidList.guidVsTemplateDesignerEditorFactoryString)>
	Public NotInheritable Class EditorFactory
		Implements IVsEditorFactory, IDisposable
		Public Const Extension As String = ".vstemplate"

		Private editorPackage As VsTemplateDesignerPackage
		Private vsServiceProvider As ServiceProvider


		Public Sub New(ByVal package As VsTemplateDesignerPackage)
			Me.editorPackage = package
		End Sub

		#Region "IVsEditorFactory Members"

		''' <summary>
		''' Used for initialization of the editor in the environment
		''' </summary>
		''' <param name="psp">pointer to the service provider. Can be used to obtain instances of other interfaces
		''' </param>
		''' <returns></returns>
		Public Function SetSite(ByVal psp As Microsoft.VisualStudio.OLE.Interop.IServiceProvider) As Integer Implements IVsEditorFactory.SetSite
			vsServiceProvider = New ServiceProvider(psp)
			Return VSConstants.S_OK
		End Function

		Public Function GetService(ByVal serviceType As Type) As Object
			Return vsServiceProvider.GetService(serviceType)
		End Function

		' This method is called by the Environment (inside IVsUIShellOpenDocument::
		' OpenStandardEditor and OpenSpecificEditor) to map a LOGICAL view to a 
		' PHYSICAL view. A LOGICAL view identifies the purpose of the view that is
		' desired (e.g. a view appropriate for Debugging [LOGVIEWID_Debugging], or a 
		' view appropriate for text view manipulation as by navigating to a find
		' result [LOGVIEWID_TextView]). A PHYSICAL view identifies an actual type 
		' of view implementation that an IVsEditorFactory can create. 
		'
		' NOTE: Physical views are identified by a string of your choice with the 
		' one constraint that the default/primary physical view for an editor  
		' *MUST* use a NULL string as its physical view name (*pbstrPhysicalView = NULL).
		'
		' NOTE: It is essential that the implementation of MapLogicalView properly
		' validates that the LogicalView desired is actually supported by the editor.
		' If an unsupported LogicalView is requested then E_NOTIMPL must be returned.
		'
		' NOTE: The special Logical Views supported by an Editor Factory must also 
		' be registered in the local registry hive. LOGVIEWID_Primary is implicitly 
		' supported by all editor types and does not need to be registered.
		' For example, an editor that supports a ViewCode/ViewDesigner scenario
		' might register something like the following:
		'        HKLM\Software\Microsoft\VisualStudio\<version>\Editors\
		'            {...guidEditor...}\
		'                LogicalViews\
		'                    {...LOGVIEWID_TextView...} = s ''
		'                    {...LOGVIEWID_Code...} = s ''
		'                    {...LOGVIEWID_Debugging...} = s ''
		'                    {...LOGVIEWID_Designer...} = s 'Form'
		'
		Public Function MapLogicalView(ByRef rguidLogicalView As Guid, <System.Runtime.InteropServices.Out()> ByRef pbstrPhysicalView As String) As Integer Implements IVsEditorFactory.MapLogicalView
			pbstrPhysicalView = Nothing ' initialize out parameter

			' we support only a single physical view
			If VSConstants.LOGVIEWID_Primary = rguidLogicalView Then
				Return VSConstants.S_OK ' primary view uses NULL as pbstrPhysicalView
			Else
				Return VSConstants.E_NOTIMPL ' you must return E_NOTIMPL for any unrecognized rguidLogicalView values
			End If
		End Function

		Public Function Close() As Integer Implements IVsEditorFactory.Close
			Return VSConstants.S_OK
		End Function

		''' <summary>
		''' Used by the editor factory to create an editor instance. the environment first determines the 
		''' editor factory with the highest priority for opening the file and then calls 
		''' IVsEditorFactory.CreateEditorInstance. If the environment is unable to instantiate the document data 
		''' in that editor, it will find the editor with the next highest priority and attempt to so that same 
		''' thing. 
		''' NOTE: The priority of our editor is 32 as mentioned in the attributes on the package class.
		''' 
		''' Since our editor supports opening only a single view for an instance of the document data, if we 
		''' are requested to open document data that is already instantiated in another editor, or even our 
		''' editor, we return a value VS_E_INCOMPATIBLEDOCDATA.
		''' </summary>
		''' <param name="grfCreateDoc">Flags determining when to create the editor. Only open and silent flags 
		''' are valid
		''' </param>
		''' <param name="pszMkDocument">path to the file to be opened</param>
		''' <param name="pszPhysicalView">name of the physical view</param>
		''' <param name="pvHier">pointer to the IVsHierarchy interface</param>
		''' <param name="itemid">Item identifier of this editor instance</param>
		''' <param name="punkDocDataExisting">This parameter is used to determine if a document buffer 
		''' (DocData object) has already been created
		''' </param>
		''' <param name="ppunkDocView">Pointer to the IUnknown interface for the DocView object</param>
		''' <param name="ppunkDocData">Pointer to the IUnknown interface for the DocData object</param>
		''' <param name="pbstrEditorCaption">Caption mentioned by the editor for the doc window</param>
		''' <param name="pguidCmdUI">the Command UI Guid. Any UI element that is visible in the editor has 
		''' to use this GUID. This is specified in the .vsct file
		''' </param>
		''' <param name="pgrfCDW">Flags for CreateDocumentWindow</param>
		''' <returns></returns>
		<SecurityPermission(SecurityAction.Demand, Flags := SecurityPermissionFlag.UnmanagedCode)>
		Public Function CreateEditorInstance(ByVal grfCreateDoc As UInteger, ByVal pszMkDocument As String, ByVal pszPhysicalView As String, ByVal pvHier As IVsHierarchy, ByVal itemid As UInteger, ByVal punkDocDataExisting As IntPtr, <System.Runtime.InteropServices.Out()> ByRef ppunkDocView As IntPtr, <System.Runtime.InteropServices.Out()> ByRef ppunkDocData As IntPtr, <System.Runtime.InteropServices.Out()> ByRef pbstrEditorCaption As String, <System.Runtime.InteropServices.Out()> ByRef pguidCmdUI As Guid, <System.Runtime.InteropServices.Out()> ByRef pgrfCDW As Integer) As Integer Implements IVsEditorFactory.CreateEditorInstance
			' Initialize to null
			ppunkDocView = IntPtr.Zero
			ppunkDocData = IntPtr.Zero
			pguidCmdUI = GuidList.guidVsTemplateDesignerEditorFactory
			pgrfCDW = 0
			pbstrEditorCaption = Nothing

			' Validate inputs
			If (grfCreateDoc And (VSConstants.CEF_OPENFILE Or VSConstants.CEF_SILENT)) = 0 Then
				Return VSConstants.E_INVALIDARG
			End If

			Dim textBuffer As IVsTextLines = Nothing

			If punkDocDataExisting = IntPtr.Zero Then
				' punkDocDataExisting is null which means the file is not yet open.
				' We need to create a new text buffer object 

				' get the ILocalRegistry interface so we can use it to
				' create the text buffer from the shell's local registry
				Try
					Dim localRegistry As ILocalRegistry = CType(GetService(GetType(SLocalRegistry)), ILocalRegistry)
					If localRegistry IsNot Nothing Then
						Dim ptr As IntPtr
						Dim iid As Guid = GetType(IVsTextLines).GUID
						Dim CLSID_VsTextBuffer As Guid = GetType(VsTextBufferClass).GUID
						localRegistry.CreateInstance(CLSID_VsTextBuffer, Nothing, iid, 1, ptr) 'CLSCTX_INPROC_SERVER
						Try
							textBuffer = TryCast(Marshal.GetObjectForIUnknown(ptr), IVsTextLines)
						Finally
							Marshal.Release(ptr) ' Release RefCount from CreateInstance call
						End Try

						' It is important to site the TextBuffer object
						Dim objWSite As IObjectWithSite = CType(textBuffer, IObjectWithSite)
						If objWSite IsNot Nothing Then
							Dim oleServiceProvider As Microsoft.VisualStudio.OLE.Interop.IServiceProvider = CType(GetService(GetType(Microsoft.VisualStudio.OLE.Interop.IServiceProvider)), Microsoft.VisualStudio.OLE.Interop.IServiceProvider)
							objWSite.SetSite(oleServiceProvider)
						End If
					End If
				Catch ex As Exception
					Debug.WriteLine("Can not get IVsCfgProviderEventsHelper" & ex.Message)
					Throw
				End Try
			Else
				' punkDocDataExisting is *not* null which means the file *is* already open. 
				' We need to verify that the open document is in fact a TextBuffer. If not
				' then we need to return the special error code VS_E_INCOMPATIBLEDOCDATA which
				' causes the user to be prompted to close the open file. If the user closes the
				' file then we will be called again with punkDocDataExisting as null

				' QI existing buffer for text lines
				textBuffer = TryCast(Marshal.GetObjectForIUnknown(punkDocDataExisting), IVsTextLines)
				If textBuffer Is Nothing Then
					Return VSConstants.VS_E_INCOMPATIBLEDOCDATA
				End If
			End If

			' Create the Document (editor)
			Dim NewEditor As New EditorPane(editorPackage, pszMkDocument, textBuffer)
			ppunkDocView = Marshal.GetIUnknownForObject(NewEditor)
			ppunkDocData = Marshal.GetIUnknownForObject(textBuffer)
			pbstrEditorCaption = ""
			Return VSConstants.S_OK
		End Function

		#End Region

		#Region "IDisposable Members"

		Public Sub Dispose() Implements IDisposable.Dispose
			Dispose(True)
		End Sub

		''' <summary>
		''' This method performs instance resources clean up
		''' </summary>
		''' <param name="disposing">This parameter determines whether the method has been called directly or indirectly by a user's code</param>
		Private Sub Dispose(ByVal disposing As Boolean)
			SyncLock Me
				' If disposing equals true, dispose all managed 
				' and unmanaged resources
				If disposing Then
					If vsServiceProvider IsNot Nothing Then
						vsServiceProvider.Dispose()
						vsServiceProvider = Nothing
					End If
				End If
			End SyncLock
		End Sub
		#End Region
	End Class
End Namespace
