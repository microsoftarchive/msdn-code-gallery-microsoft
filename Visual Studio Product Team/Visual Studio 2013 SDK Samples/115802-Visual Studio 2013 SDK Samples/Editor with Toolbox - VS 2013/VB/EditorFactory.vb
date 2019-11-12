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
Imports System.Diagnostics
Imports System.Globalization
Imports System.Security.Permissions
Imports System.Runtime.InteropServices
Imports Microsoft.VisualStudio
Imports Microsoft.VisualStudio.Shell
Imports Microsoft.VisualStudio.Shell.Interop

Namespace Microsoft.Samples.VisualStudio.IDE.EditorWithToolbox
	''' <summary>
	''' Factory for creating our editors.
	''' </summary>
	<Guid(GuidStrings.GuidEditorFactory)> _
	Public Class EditorFactory
		Implements IVsEditorFactory, IDisposable
		#Region "Fields"
        ' Private instance of the EditorFactory's OleServiceProvider.
		Private vsServiceProvider As ServiceProvider
		#End Region

		#Region "Constructors"
		''' <summary>
		''' Explicitly defined default constructor.
		''' Initialize new instance of the EditorFactory object.
		''' </summary>
		Public Sub New()
			Debug.WriteLine(String.Format(CultureInfo.CurrentCulture, "Entering {0} constructor", GetType(EditorFactory).ToString()))
		End Sub

#End Region

		#Region "Methods"

		#Region "IDisposable Pattern implementation"
		''' <summary>
		''' Clean up any resources being used.
		''' </summary>
		Public Sub Dispose() Implements IDisposable.Dispose
			Dispose(True)
		End Sub

		''' <summary>
		''' Clean up any resources being used.
		''' </summary>
		''' <param name="disposing">This parameter determines whether the method has been called directly or indirectly by a user's code.</param>
		Private Sub Dispose(ByVal disposing As Boolean)
            ' If disposing equals true, dispose all managed and unmanaged resources.
			If disposing Then
				''' Since we create a ServiceProvider which implements IDisposable we
				''' also need to implement IDisposable to make sure that the ServiceProvider's
				''' Dispose method gets called.
                If vsServiceProvider IsNot Nothing Then
                    vsServiceProvider.Dispose()
                    vsServiceProvider = Nothing
                End If
			End If
		End Sub
		#End Region

		#Region "IVsEditorFactory Members"

		''' <summary>
		''' Used for initialization of the editor in the environment.
		''' </summary>  
		''' <param name="psp">Pointer to the service provider. Can be used to obtain instances of other interfaces.</param>
		''' <returns>S_OK if the method succeeds.</returns>
        Public Function SetSite(ByVal psp As Microsoft.VisualStudio.OLE.Interop.IServiceProvider) As Integer Implements IVsEditorFactory.SetSite
            vsServiceProvider = New ServiceProvider(psp)
            Return VSConstants.S_OK
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
        '		HKLM\Software\Microsoft\VisualStudio\10.0\Editors\
		'			{...guidEditor...}\
		'				LogicalViews\
		'					{...LOGVIEWID_TextView...} = s ''
		'					{...LOGVIEWID_Code...} = s ''
		'					{...LOGVIEWID_Debugging...} = s ''
		'					{...LOGVIEWID_Designer...} = s 'Form'
		'
		Public Function MapLogicalView(ByRef rguidLogicalView As Guid, <System.Runtime.InteropServices.Out()> ByRef pbstrPhysicalView As String) As Integer Implements IVsEditorFactory.MapLogicalView
			pbstrPhysicalView = Nothing ' initialize out parameter

            ' We support only a single physical view.
			If VSConstants.LOGVIEWID_Primary = rguidLogicalView Then
                ' Primary view uses NULL as pbstrPhysicalView.
				Return VSConstants.S_OK
			Else
                ' You must return E_NOTIMPL for any unrecognized rguidLogicalView values.
				Return VSConstants.E_NOTIMPL
			End If
		End Function
		''' <summary>
        ''' Releases all cached interface pointers and unregisters any event sinks.
		''' </summary>
		''' <returns>S_OK if the method succeeds.</returns>
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
		''' are valid.
		''' </param>
		''' <param name="pszMkDocument">path to the file to be opened.</param>
		''' <param name="pszPhysicalView">name of the physical view.</param>
		''' <param name="pvHier">pointer to the IVsHierarchy interface.</param>
		''' <param name="itemid">Item identifier of this editor instance.</param>
		''' <param name="punkDocDataExisting">This parameter is used to determine if a document buffer 
		''' (DocData object) has already been created.
		''' </param>
		''' <param name="ppunkDocView">Pointer to the IUnknown interface for the DocView object.</param>
		''' <param name="ppunkDocData">Pointer to the IUnknown interface for the DocData object.</param>
		''' <param name="pbstrEditorCaption">Caption mentioned by the editor for the doc window.</param>
		''' <param name="pguidCmdUI">the Command UI Guid. Any UI element that is visible in the editor has 
		''' to use this GUID.
		''' </param>
		''' <param name="pgrfCDW">Flags for CreateDocumentWindow.</param>
		''' <returns>HRESULT result code. S_OK if the method succeeds.</returns>
		''' <remarks>
		''' Attribute usage according to FxCop rule regarding SecurityAction requirements (LinkDemand).
		''' This method do use SecurityAction.Demand action instead of LinkDemand because it overrides method without LinkDemand
		''' see "Demand vs. LinkDemand" article in MSDN for more details.
		''' </remarks>
		<EnvironmentPermission(SecurityAction.Demand, Unrestricted := True)> _
		Public Function CreateEditorInstance(ByVal grfCreateDoc As UInteger, ByVal pszMkDocument As String, ByVal pszPhysicalView As String, ByVal pvHier As IVsHierarchy, ByVal itemid As UInteger, ByVal punkDocDataExisting As System.IntPtr, <System.Runtime.InteropServices.Out()> ByRef ppunkDocView As System.IntPtr, <System.Runtime.InteropServices.Out()> ByRef ppunkDocData As System.IntPtr, <System.Runtime.InteropServices.Out()> ByRef pbstrEditorCaption As String, <System.Runtime.InteropServices.Out()> ByRef pguidCmdUI As Guid, <System.Runtime.InteropServices.Out()> ByRef pgrfCDW As Integer) As Integer Implements IVsEditorFactory.CreateEditorInstance
			Debug.WriteLine(String.Format(CultureInfo.CurrentCulture, "Entering {0} CreateEditorInstance()", Me.ToString()))

            ' Initialize to null.
			ppunkDocView = IntPtr.Zero
			ppunkDocData = IntPtr.Zero
			pguidCmdUI = GuidList.guidEditorFactory
			pgrfCDW = 0
			pbstrEditorCaption = Nothing

            ' Validate inputs.
			If (grfCreateDoc And (VSConstants.CEF_OPENFILE Or VSConstants.CEF_SILENT)) = 0 Then
				Return VSConstants.E_INVALIDARG
			End If
			If punkDocDataExisting <> IntPtr.Zero Then
				Return VSConstants.VS_E_INCOMPATIBLEDOCDATA
			End If

            ' Create the Document (editor).
            Dim newEditor As New EditorPane()
			ppunkDocView = Marshal.GetIUnknownForObject(newEditor)
			ppunkDocData = Marshal.GetIUnknownForObject(newEditor)
			pbstrEditorCaption = ""

			Return VSConstants.S_OK
		End Function

		#End Region

		#Region "Other methods"

		''' <summary>
		''' Gets the service object of the specified type.
		''' </summary>
		''' <param name="serviceType">An Type that specifies the type of service object to get.</param>
		''' <returns>A service object of type serviceType. -or- 
		''' A a null reference if there is no service object of type serviceType.</returns>
		Public Function GetService(ByVal serviceType As Type) As Object
			Return vsServiceProvider.GetService(serviceType)
		End Function

		#End Region 

		#End Region
	End Class
End Namespace
