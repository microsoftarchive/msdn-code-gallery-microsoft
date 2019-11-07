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
Imports System.Windows.Forms
Imports System.Runtime.InteropServices
Imports Microsoft.VisualStudio.Shell.Interop
Imports Microsoft.VisualStudio.OLE.Interop
Imports Microsoft.VisualStudio.Shell

Imports Constants = Microsoft.VisualStudio.OLE.Interop.Constants
Imports VSConstants = Microsoft.VisualStudio.VSConstants
Imports ErrorHandler = Microsoft.VisualStudio.ErrorHandler

Namespace Microsoft.Samples.VisualStudio.IDE.EditorWithToolbox
	''' <summary>
	''' This control host the editor (an extended RichTextBox) and is responsible for
	''' handling the commands targeted to the editor as well as saving and loading
	''' the document.
	''' </summary>
	''' <remarks>
	''' Uses an entry in the new file dialog.
	''' EditorWithToolbox.vsdir contents description:
	''' tbx.tbx|{74D1A709-1703-4365-9A21-42464AA1D0B1}|#106|80|#109|{74D1A709-1703-4365-9A21-42464AA1D0B1}|401|0|#107
	'''
	''' The fields in order are as follows:-
	'''    - tbx.tbx - our empty tbx file
	'''    - {74D1A709-1703-4365-9A21-42464AA1D0B1} - our Editor package guid
    '''    - #106 - the ID of "Editor with Toolbox - VB Sample" in the resource
	'''    - 80 - the display ordering priority
    '''    - #109 - the ID of "Editor with Toolbox File" in the resource
	'''    - {74D1A709-1703-4365-9A21-42464AA1D0B1} - resource dll string (we don't use this)
	'''    - 401 - the ID of our icon
	'''    - 0 - various flags (we don't use this - see vsshell.idl)
    '''    - #107 - Default file name "TbxFile.tbx"
	''' </remarks>
	Public NotInheritable Class EditorPane
		Inherits WindowPane
		Implements IOleCommandTarget, IVsPersistDocData, IPersistFileFormat, IVsToolboxUser

		Private Const fileFormat As UInteger = 0
		Private Const fileExtension As String = ".tbx"
		Private Const endLine As Char = ChrW(10)

		#Region "Fields"

        Private Shared toolboxData As OleDataObject

		' Full path to the file.
		Private fileName As String
        ''' Determines whether an object has changed since being saved to its current file.

        Private bIsDirty As Boolean
		' Flag true when we are loading the file. It is used to avoid to change the isDirty flag
		' when the changes are related to the load operation.
		Private loading As Boolean
		' This flag is true when we are asking the QueryEditQuerySave service if we can edit the
		' file. It is used to avoid to have more than one request queued.
		Private gettingCheckoutStatus As Boolean
		' Indicate that object is in NoScribble mode or in Normal mode. 
		' Object enter into the NoScribble mode when IPersistFileFormat.Save() call is occurred.
		' This flag using to indicate SaveCompleted state (entering into the Normal mode).
		Private noScribbleMode As Boolean
		' Object that handles the editor window.
		Private editorControl As EditorControl

		#End Region

		#Region "Contructors"
		''' <summary>
		''' Create and initialize EditorPane instance object.
		''' </summary>
		''' <param name="serviceProvider">Service Provider object, previously initialized by services set.</param>
		Public Sub New()
			MyBase.New(Nothing)
			PrivateInit()
		End Sub
		''' <summary>
		''' Initialize GUI context objects.
		''' </summary>
		Private Sub PrivateInit()
			noScribbleMode = False
			loading = False
			gettingCheckoutStatus = False

			' This call is required by the Windows.Forms Form Designer.
			Me.editorControl = New Microsoft.Samples.VisualStudio.IDE.EditorWithToolbox.EditorControl()
			Me.editorControl.AllowDrop = True
			Me.editorControl.HideSelection = False
			Me.editorControl.TabIndex = 0
			Me.editorControl.Text = String.Empty
			Me.editorControl.Name = "EditorPane"

            AddHandler editorControl.DragEnter, AddressOf OnDragEnter
			AddHandler editorControl.DragDrop, AddressOf OnDragDrop
			AddHandler editorControl.TextChanged, AddressOf OnTextChange
		End Sub

		''' <summary>
		''' This method is called when the pane is sited with a non null service provider.
		''' Here is where you can do all the initialization that requare access to
		''' services provided by the shell.
		''' </summary>
        Protected Overrides Sub Initialize()
            ' If toolboxData have initialized, skip creating a new one.
            If toolboxData Is Nothing Then
                ' Create the data object that will store the data for the menu item.
                toolboxData = New OleDataObject()
                toolboxData.SetData(GetType(ToolboxItemData), New ToolboxItemData("Test string"))

                ' Get the toolbox service.
                Dim toolbox As IVsToolbox = CType(GetService(GetType(SVsToolbox)), IVsToolbox)

                ' Create the array of TBXITEMINFO structures to describe the items
                ' we are adding to the toolbox.
                Dim itemInfo As TBXITEMINFO() = New TBXITEMINFO(0) {}
                itemInfo(0).bstrText = "Toolbox Sample Item"
                itemInfo(0).hBmp = IntPtr.Zero
                itemInfo(0).dwFlags = CUInt(__TBXITEMINFOFLAGS.TBXIF_DONTPERSIST)

                ErrorHandler.ThrowOnFailure(toolbox.AddItem(CType(toolboxData, Microsoft.VisualStudio.OLE.Interop.IDataObject), itemInfo, "Toolbox Test"))
            End If
        End Sub
		#End Region

		#Region "Properties"
		''' <summary>
		''' Gets extended rich text box that are hosted.
		''' This is a required override from the Microsoft.VisualStudio.Shell.WindowPane class.
		''' </summary>
		''' <remarks>The resultant handle can be used with Win32 API calls.</remarks>
		Public Overrides ReadOnly Property Window() As IWin32Window
			Get
				Return Me.editorControl
			End Get
		End Property
#End Region

		#Region "Methods"

		#Region "IDisposable Pattern implementation"

		''' <summary> 
		''' Clean up any resources being used.
		''' </summary>
		Protected Overrides Overloads Sub Dispose(ByVal disposing As Boolean)
			Try
				If disposing Then
                    If editorControl IsNot Nothing Then
                        editorControl.Dispose()
                        editorControl = Nothing
                    End If
					GC.SuppressFinalize(Me)
				End If
			Finally
				MyBase.Dispose(disposing)
			End Try
		End Sub

		#End Region

		#Region "IOleCommandTarget Members"

		''' <summary>
		''' The shell call this function to know if a menu item should be visible and
		''' if it should be enabled/disabled.
		''' Note that this function will only be called when an instance of this editor is open.
		''' </summary>
		''' <param name="pguidCmdGroup">Guid describing which set of command the current command(s) belong to.</param>
		''' <param name="cCmds">Number of command which status are being asked for.</param>
		''' <param name="prgCmds">Information for each command.</param>
		''' <param name="pCmdText">Used to dynamically change the command text.</param>
		''' <returns>S_OK if the method succeeds.</returns> 
        Public Function QueryStatus(ByRef pguidCmdGroup As Guid, ByVal cCmds As UInteger, ByVal prgCmds As OLECMD(), ByVal pCmdText As IntPtr) As Integer Implements IOleCommandTarget.QueryStatus
            ' Validate parameters.
            If prgCmds Is Nothing OrElse cCmds <> 1 Then
                Return VSConstants.E_INVALIDARG
            End If

            Dim cmdf As OLECMDF = OLECMDF.OLECMDF_SUPPORTED

            If pguidCmdGroup = VSConstants.GUID_VSStandardCommandSet97 Then
                ' Process standard Commands.
                Select Case prgCmds(0).cmdID
                    Case CUInt(VSConstants.VSStd97CmdID.SelectAll)
                        ' Always enabled.
                        cmdf = OLECMDF.OLECMDF_SUPPORTED Or OLECMDF.OLECMDF_ENABLED
                        Exit Select
                    Case CUInt(VSConstants.VSStd97CmdID.Copy), CUInt(VSConstants.VSStd97CmdID.Cut)
                        ' Enable if something is selected.
                        If editorControl.SelectionLength > 0 Then
                            cmdf = cmdf Or OLECMDF.OLECMDF_ENABLED
                        End If
                        Exit Select
                    Case CUInt(VSConstants.VSStd97CmdID.Paste)
                        ' Enable if clipboard has content we can paste.

                        If editorControl.CanPaste(DataFormats.GetFormat(DataFormats.Text)) Then
                            cmdf = cmdf Or OLECMDF.OLECMDF_ENABLED
                        End If
                        Exit Select
                    Case CUInt(VSConstants.VSStd97CmdID.Redo)
                        ' Enable if actions that have occurred within the RichTextBox 
                        ' can be reapplied.
                        If editorControl.CanRedo Then
                            cmdf = cmdf Or OLECMDF.OLECMDF_ENABLED
                        End If
                        Exit Select
                    Case CUInt(VSConstants.VSStd97CmdID.Undo)
                        If editorControl.CanUndo Then
                            cmdf = cmdf Or OLECMDF.OLECMDF_ENABLED
                        End If
                        Exit Select
                    Case Else
                        Return CInt(Fix(Constants.OLECMDERR_E_NOTSUPPORTED))
                End Select
            ElseIf pguidCmdGroup = GuidList.guidEditorCmdSet Then
                ' Process our Commands.
                Select Case prgCmds(0).cmdID
                    ' If we had commands specific to our editor, they would be processed here.
                    Case Else
                        Return CInt(Fix(Constants.OLECMDERR_E_NOTSUPPORTED))
                End Select
            Else
                Return CInt(Fix(Constants.OLECMDERR_E_NOTSUPPORTED))

            End If

            prgCmds(0).cmdf = CUInt(cmdf)
            Return VSConstants.S_OK
        End Function

        ''' <summary>
        ''' Execute a specified command.
        ''' </summary>
        ''' <param name="pguidCmdGroup">Guid describing which set of command the current command(s) belong to.</param>
        ''' <param name="nCmdID">Command that should be executed.</param>
        ''' <param name="nCmdexecopt">Options for the command.</param>
        ''' <param name="pvaIn">Pointer to input arguments.</param>
        ''' <param name="pvaOut">Pointer to command output.</param>
        ''' <returns>S_OK if the method succeeds or OLECMDERR_E_NOTSUPPORTED on unsupported command.</returns> 
        ''' <remarks>Typically, only the first 2 arguments are used (to identify which command should be run).</remarks>
        Public Function Exec(ByRef pguidCmdGroup As Guid, ByVal nCmdID As UInteger, ByVal nCmdexecopt As UInteger, ByVal pvaIn As IntPtr, ByVal pvaOut As IntPtr) As Integer Implements IOleCommandTarget.Exec
            Trace.WriteLine(String.Format(CultureInfo.CurrentCulture, "Entering Exec() of: {0}", Me.ToString()))

            If pguidCmdGroup = VSConstants.GUID_VSStandardCommandSet97 Then
                ' Process standard Visual Studio Commands.
                Select Case nCmdID
                    Case CUInt(VSConstants.VSStd97CmdID.Copy)
                        editorControl.Copy()
                        Exit Select
                    Case CUInt(VSConstants.VSStd97CmdID.Cut)
                        editorControl.Cut()
                        Exit Select
                    Case CUInt(VSConstants.VSStd97CmdID.Paste)
                        editorControl.Paste()
                        Exit Select
                    Case CUInt(VSConstants.VSStd97CmdID.Redo)
                        editorControl.Redo()
                        Exit Select
                    Case CUInt(VSConstants.VSStd97CmdID.Undo)
                        editorControl.Undo()
                        Exit Select
                    Case CUInt(VSConstants.VSStd97CmdID.SelectAll)
                        editorControl.SelectAll()
                        Exit Select
                    Case Else
                        Return CInt(Fix(Constants.OLECMDERR_E_NOTSUPPORTED))
                End Select
            ElseIf pguidCmdGroup = GuidList.guidEditorCmdSet Then
                Select Case nCmdID
                    ' If we had commands specific to our editor, they would be processed here.
                    Case Else
                        Return CInt(Fix(Constants.OLECMDERR_E_NOTSUPPORTED))
                End Select
            Else
                Return CInt(Fix(Constants.OLECMDERR_E_UNKNOWNGROUP))
            End If

            Return VSConstants.S_OK
        End Function
#End Region

#Region "IPersist"
        ''' <summary>
        ''' Retrieves the class identifier (CLSID) of an object.
        ''' </summary>
        ''' <param name="pClassID">[out] Pointer to the location of the CLSID on return.</param>
        ''' <returns>S_OK if the method succeeds.</returns>
        Private Function IPersist_GetClassID(<System.Runtime.InteropServices.Out()> ByRef pClassID As Guid) As Integer Implements Microsoft.VisualStudio.OLE.Interop.IPersist.GetClassID
            pClassID = GuidList.guidEditorFactory
            Return VSConstants.S_OK
        End Function
#End Region

#Region "IPersistFileFormat Members"

        ''' <summary>
        ''' Returns the class identifier of the editor type.
        ''' </summary>
        ''' <param name="pClassID">pointer to the class identifier.</param>
        ''' <returns>S_OK if the method succeeds.</returns>
        Private Function GetClassID(<System.Runtime.InteropServices.Out()> ByRef pClassID As Guid) As Integer Implements IPersistFileFormat.GetClassID
            CType(Me, Microsoft.VisualStudio.OLE.Interop.IPersist).GetClassID(pClassID)
            Return VSConstants.S_OK
        End Function

        ''' <summary>
        ''' Returns the path to the object's current working file.
        ''' </summary>
        ''' <param name="ppszFilename">Pointer to the file name.</param>
        ''' <param name="pnFormatIndex">Value that indicates the current format of the file as a zero based index
        ''' into the list of formats. Since we support only a single format, we need to return zero. 
        ''' Subsequently, we will return a single element in the format list through a call to GetFormatList.</param>
        ''' <returns>S_OK if the function succeeds.</returns>
        Private Function GetCurFile(<System.Runtime.InteropServices.Out()> ByRef ppszFilename As String, <System.Runtime.InteropServices.Out()> ByRef pnFormatIndex As UInteger) As Integer Implements IPersistFileFormat.GetCurFile
            ' We only support 1 format so return its index.
            pnFormatIndex = fileFormat
            ppszFilename = fileName
            Return VSConstants.S_OK
        End Function

        ''' <summary>
        ''' Provides the caller with the information necessary to open the standard common "Save As" dialog box. 
        ''' This returns an enumeration of supported formats, from which the caller selects the appropriate format. 
        ''' Each string for the format is terminated with a newline (\n) character. 
        ''' The last string in the buffer must be terminated with the newline character as well. 
        ''' The first string in each pair is a display string that describes the filter, such as "Text Only 
        ''' (*.txt)". The second string specifies the filter pattern, such as "*.txt". To specify multiple filter 
        ''' patterns for a single display string, use a semicolon to separate the patterns: "*.htm;*.html;*.asp". 
        ''' A pattern string can be a combination of valid file name characters and the asterisk (*) wildcard character. 
        ''' Do not include spaces in the pattern string. The following string is an example of a file pattern string: 
        ''' "HTML File (*.htm; *.html; *.asp)\n*.htm;*.html;*.asp\nText File (*.txt)\n*.txt\n."
        ''' </summary>
        ''' <param name="ppszFormatList">Pointer to a string that contains pairs of format filter strings.</param>
        ''' <returns>S_OK if the method succeeds.</returns>
        Private Function GetFormatList(<System.Runtime.InteropServices.Out()> ByRef ppszFormatList As String) As Integer Implements IPersistFileFormat.GetFormatList
            Dim formatList As String = String.Format(CultureInfo.CurrentCulture, "Test Editor (*{0}){1}*{0}{1}{1}", fileExtension, endLine)
            ppszFormatList = formatList
            Return VSConstants.S_OK
        End Function

        ''' <summary>
        ''' Notifies the object that it has concluded the Save transaction.
        ''' </summary>
        ''' <param name="pszFilename">Pointer to the file name.</param>
        ''' <returns>S_OK if the function succeeds.</returns>
        Private Function SaveCompleted(ByVal pszFilename As String) As Integer Implements IPersistFileFormat.SaveCompleted
            If noScribbleMode Then
                Return VSConstants.S_FALSE
                ' If NoScribble mode is inactive - Save() operation was completed.
            Else
                Return VSConstants.S_OK
            End If
        End Function

        ''' <summary>
        ''' Initialization for the object.
        ''' </summary>
        ''' <param name="nFormatIndex">Zero based index into the list of formats that indicates the current format
        ''' of the file.</param>
        ''' <returns>S_OK if the method succeeds.</returns>
        Private Function InitNew(ByVal nFormatIndex As UInteger) As Integer Implements IPersistFileFormat.InitNew
            If nFormatIndex <> fileFormat Then
                Throw New ArgumentException(Resources.ExceptionMessageFormat)
            End If
            ' Until someone change the file, we can consider it not dirty as
            ' the user would be annoyed if we prompt him to save an empty file.
            bIsDirty = False
            Return VSConstants.S_OK
        End Function

        ''' <summary>
        ''' Determines whether an object has changed since being saved to its current file.
        ''' </summary>
        ''' <param name="pfIsDirty">true if the document has changed.</param>
        ''' <returns>S_OK if the method succeeds.</returns>
        Private Function IsDirty(<System.Runtime.InteropServices.Out()> ByRef pfIsDirty As Integer) As Integer Implements IPersistFileFormat.IsDirty
            If bIsDirty Then
                pfIsDirty = 1
            Else
                pfIsDirty = 0
            End If
            Return VSConstants.S_OK
        End Function

        ''' <summary>
        ''' Loads the file content into the TextBox.
        ''' </summary>
        ''' <param name="pszFilename">Pointer to the full path name of the file to load.</param>
        ''' <param name="grfMode">file format mode.</param>
        ''' <param name="fReadOnly">determines if the file should be opened as read only.</param>
        ''' <returns>S_OK if the method succeeds.</returns>
        Private Function Load(ByVal pszFilename As String, ByVal grfMode As UInteger, ByVal fReadOnly As Integer) As Integer Implements IPersistFileFormat.Load
            If (pszFilename Is Nothing) AndAlso ((fileName Is Nothing) OrElse (fileName.Length = 0)) Then
                Throw New ArgumentNullException("pszFilename")
            End If

            loading = True
            Dim hr As Integer = VSConstants.S_OK
            Try
                Dim isReload As Boolean = False

                ' If the new file name is null, then this operation is a reload.
                If pszFilename Is Nothing Then
                    isReload = True
                End If

                ' Show the wait cursor while loading the file.
                Dim vsUiShell As IVsUIShell = CType(GetService(GetType(SVsUIShell)), IVsUIShell)
                If vsUiShell IsNot Nothing Then
                    ' Note: we don't want to throw or exit if this call fails, so
                    ' don't check the return code.
                    vsUiShell.SetWaitCursor()
                End If

                ' Set the new file name.
                If (Not isReload) Then
                    ' Unsubscribe from the notification of the changes in the previous file.
                    fileName = pszFilename
                End If
                ' Load the file.
                editorControl.LoadFile(fileName, RichTextBoxStreamType.PlainText)
                bIsDirty = False

                ' Notify the load or reload.
                NotifyDocChanged()
            Finally
                loading = False
            End Try
            Return hr
        End Function

        ''' <summary>
        ''' Save the contents of the TextBox into the specified file. If doing the save on the same file, we need to
        ''' suspend notifications for file changes during the save operation.
        ''' </summary>
        ''' <param name="pszFilename">Pointer to the file name. If the pszFilename parameter is a null reference 
        ''' we need to save using the current file.
        ''' </param>
        ''' <param name="remember">Boolean value that indicates whether the pszFileName parameter is to be used 
        ''' as the current working file.
        ''' If remember != 0, pszFileName needs to be made the current file and the dirty flag needs to be cleared after the save.
        '''                   Also, file notifications need to be enabled for the new file and disabled for the old file 
        ''' If remember == 0, this save operation is a Save a Copy As operation. In this case, 
        '''                   the current file is unchanged and dirty flag is not cleared.
        ''' </param>
        ''' <param name="nFormatIndex">Zero based index into the list of formats that indicates the format in which 
        ''' the file will be saved.</param>
        ''' <returns>S_OK if the method succeeds.</returns>
        Private Function Save(ByVal pszFilename As String, ByVal fRemember As Integer, ByVal nFormatIndex As UInteger) As Integer Implements IPersistFileFormat.Save
            ' Switch into the NoScribble mode.
            noScribbleMode = True
            Try
                ' If file is null or same --> SAVE.
                If pszFilename Is Nothing OrElse pszFilename = fileName Then
                    editorControl.SaveFile(fileName, RichTextBoxStreamType.PlainText)
                    bIsDirty = False
                Else
                    If fRemember <> 0 Then
                        fileName = pszFilename
                        editorControl.SaveFile(fileName, RichTextBoxStreamType.PlainText)
                        bIsDirty = False
                        ' Else, Save a Copy As.
                    Else
                        editorControl.SaveFile(pszFilename, RichTextBoxStreamType.PlainText)
                    End If
                End If
            Catch e1 As Exception
                Throw
            Finally
                ' Switch into the Normal mode.
                noScribbleMode = False
            End Try
            Return VSConstants.S_OK
        End Function

#End Region

#Region "IVsPersistDocData Members"

        ''' <summary>
        ''' Close the IVsPersistDocData object.
        ''' </summary>
        ''' <returns>S_OK if the function succeeds.</returns>
        Private Function Close() As Integer Implements IVsPersistDocData.Close
            If editorControl IsNot Nothing Then
                editorControl.Dispose()
            End If
            Return VSConstants.S_OK
        End Function

        ''' <summary>
        ''' Returns the Guid of the editor factory that created the IVsPersistDocData object.
        ''' </summary>
        ''' <param name="pClassID">Pointer to the class identifier of the editor type.</param>
        ''' <returns>S_OK if the method succeeds.</returns>
        Private Function GetGuidEditorType(<System.Runtime.InteropServices.Out()> ByRef pClassID As Guid) As Integer Implements IVsPersistDocData.GetGuidEditorType
            Return (CType(Me, IPersistFileFormat)).GetClassID(pClassID)
        End Function

        ''' <summary>
        ''' Used to determine if the document data has changed since the last time it was saved.
        ''' </summary>
        ''' <param name="pfDirty">Will be set to 1 if the data has changed.</param>
        ''' <returns>S_OK if the function succeeds.</returns>
        Private Function IsDocDataDirty(<System.Runtime.InteropServices.Out()> ByRef pfDirty As Integer) As Integer Implements IVsPersistDocData.IsDocDataDirty
            Return (CType(Me, IPersistFileFormat)).IsDirty(pfDirty)
        End Function

        ''' <summary>
        ''' Determines if it is possible to reload the document data.
        ''' </summary>
        ''' <param name="pfReloadable">set to 1 if the document can be reloaded.</param>
        ''' <returns>S_OK if the method succeeds.</returns>
        Private Function IsDocDataReloadable(<System.Runtime.InteropServices.Out()> ByRef pfReloadable As Integer) As Integer Implements IVsPersistDocData.IsDocDataReloadable
            ' Allow file to be reloaded.
            pfReloadable = 1
            Return VSConstants.S_OK
        End Function

        ''' <summary>
        ''' Loads the document data from the file specified.
        ''' </summary>
        ''' <param name="pszMkDocument">Path to the document file which needs to be loaded.</param>
        ''' <returns>S_OK if the method succeeds.</returns>
        Private Function LoadDocData(ByVal pszMkDocument As String) As Integer Implements IVsPersistDocData.LoadDocData
            Return (CType(Me, IPersistFileFormat)).Load(pszMkDocument, 0, 0)
        End Function

        ''' <summary>
        ''' Called by the Running Document Table when it registers the document data. 
        ''' </summary>
        ''' <param name="docCookie">Handle for the document to be registered.</param>
        ''' <param name="pHierNew">Pointer to the IVsHierarchy interface.</param>
        ''' <param name="itemidNew">Item identifier of the document to be registered from VSITEM.</param>
        ''' <returns>S_OK if the method succeeds.</returns>
        Private Function OnRegisterDocData(ByVal docCookie As UInteger, ByVal pHierNew As IVsHierarchy, ByVal itemidNew As UInteger) As Integer Implements IVsPersistDocData.OnRegisterDocData
            Return VSConstants.S_OK
        End Function

        ''' <summary>
        ''' Reloads the document data.
        ''' </summary>
        ''' <param name="grfFlags">Flag indicating whether to ignore the next file change when reloading the document data.
        ''' This flag should not be set for us since we implement the "IVsDocDataFileChangeControl" interface in order to 
        ''' indicate ignoring of file changes.
        ''' </param>
        ''' <returns>S_OK if the method succeeds.</returns>
        Private Function ReloadDocData(ByVal grfFlags As UInteger) As Integer Implements IVsPersistDocData.ReloadDocData
            Return (CType(Me, IPersistFileFormat)).Load(Nothing, grfFlags, 0)
        End Function

        ''' <summary>
        ''' Renames the document data.
        ''' </summary>
        ''' <param name="grfAttribs">File attribute of the document data to be renamed. See the data type __VSRDTATTRIB.</param>
        ''' <param name="pHierNew">Pointer to the IVsHierarchy interface of the document being renamed.</param>
        ''' <param name="itemidNew">Item identifier of the document being renamed. See the data type VSITEMID.</param>
        ''' <param name="pszMkDocumentNew">Path to the document being renamed.</param>
        ''' <returns>S_OK if the method succeeds.</returns>
        Private Function RenameDocData(ByVal grfAttribs As UInteger, ByVal pHierNew As IVsHierarchy, ByVal itemidNew As UInteger, ByVal pszMkDocumentNew As String) As Integer Implements IVsPersistDocData.RenameDocData
            Return VSConstants.S_OK
        End Function

        ''' <summary>
        ''' Saves the document data. Before actually saving the file, we first need to indicate to the environment
        ''' that a file is about to be saved. This is done through the "SVsQueryEditQuerySave" service. We call the
        ''' "QuerySaveFile" function on the service instance and then proceed depending on the result returned as follows:
        ''' If result is QSR_SaveOK - We go ahead and save the file and the file is not read only at this point.
        ''' If result is QSR_ForceSaveAs - We invoke the "Save As" functionality which will bring up the Save file name 
        '''                                dialog 
        ''' If result is QSR_NoSave_Cancel - We cancel the save operation and indicate that the document could not be saved
        '''                                by setting the "pfSaveCanceled" flag
        ''' If result is QSR_NoSave_Continue - Nothing to do here as the file need not be saved.
        ''' </summary>
        ''' <param name="dwSave">Flags which specify the file save options:
        ''' VSSAVE_Save        - Saves the current file to itself.
        ''' VSSAVE_SaveAs      - Prompts the User for a filename and saves the file to the file specified.
        ''' VSSAVE_SaveCopyAs  - Prompts the user for a filename and saves a copy of the file with a name specified.
        ''' VSSAVE_SilentSave  - Saves the file without prompting for a name or confirmation.  
        ''' </param>
        ''' <param name="pbstrMkDocumentNew">Pointer to the path to the new document.</param>
        ''' <param name="pfSaveCanceled">value 1 if the document could not be saved.</param>
        ''' <returns>S_OK if the method succeeds.</returns>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters")> _
        Private Function SaveDocData(ByVal dwSave As Microsoft.VisualStudio.Shell.Interop.VSSAVEFLAGS, <System.Runtime.InteropServices.Out()> ByRef pbstrMkDocumentNew As String, <System.Runtime.InteropServices.Out()> ByRef pfSaveCanceled As Integer) As Integer Implements IVsPersistDocData.SaveDocData
            pbstrMkDocumentNew = Nothing
            pfSaveCanceled = 0
            Dim hr As Integer = VSConstants.S_OK

            Select Case dwSave
                Case VSSAVEFLAGS.VSSAVE_Save, VSSAVEFLAGS.VSSAVE_SilentSave
                    Dim queryEditQuerySave As IVsQueryEditQuerySave2 = CType(GetService(GetType(SVsQueryEditQuerySave)), IVsQueryEditQuerySave2)

                    ' Call QueryEditQuerySave.
                    Dim result As UInteger = 0
                    ' result.
                    hr = queryEditQuerySave.QuerySaveFile(fileName, 0, Nothing, result)

                    If ErrorHandler.Failed(hr) Then
                        Return hr
                    End If

                    ' Process according to result from QuerySave.
                    Select Case CType(result, tagVSQuerySaveResult)
                        Case tagVSQuerySaveResult.QSR_NoSave_Cancel
                            ' Note that this is also case tagVSQuerySaveResult.QSR_NoSave_UserCanceled because these
                            ' two tags have the same value.
                            pfSaveCanceled = Not 0

                        Case tagVSQuerySaveResult.QSR_SaveOK
                            ' Call the shell to do the save for us.
                            Dim uiShell As IVsUIShell = CType(GetService(GetType(SVsUIShell)), IVsUIShell)
                            hr = uiShell.SaveDocDataToFile(dwSave, CType(Me, IPersistFileFormat), fileName, pbstrMkDocumentNew, pfSaveCanceled)
                            If ErrorHandler.Failed(hr) Then
                                Return hr
                            End If

                        Case tagVSQuerySaveResult.QSR_ForceSaveAs
                            ' Call the shell to do the SaveAS for us.
                            Dim uiShell As IVsUIShell = CType(GetService(GetType(SVsUIShell)), IVsUIShell)
                            hr = uiShell.SaveDocDataToFile(VSSAVEFLAGS.VSSAVE_SaveAs, CType(Me, IPersistFileFormat), fileName, pbstrMkDocumentNew, pfSaveCanceled)
                            If ErrorHandler.Failed(hr) Then
                                Return hr
                            End If

                        Case tagVSQuerySaveResult.QSR_NoSave_Continue
                            ' In this case there is nothing to do.

                        Case Else
                            Throw New COMException(Resources.ExceptionMessageQEQS)
                    End Select
                    Exit Select
                Case VSSAVEFLAGS.VSSAVE_SaveAs, VSSAVEFLAGS.VSSAVE_SaveCopyAs
                    ' Make sure the file name as the right extension.
                    If String.Compare(fileExtension, System.IO.Path.GetExtension(fileName), True, CultureInfo.CurrentCulture) <> 0 Then
                        fileName &= fileExtension
                    End If
                    ' Call the shell to do the save for us.
                    Dim uiShell As IVsUIShell = CType(GetService(GetType(SVsUIShell)), IVsUIShell)
                    hr = uiShell.SaveDocDataToFile(dwSave, CType(Me, IPersistFileFormat), fileName, pbstrMkDocumentNew, pfSaveCanceled)
                    If ErrorHandler.Failed(hr) Then
                        Return hr
                    End If
                    Exit Select
                Case Else
                    Throw New ArgumentException(Resources.ExceptionMessageSaveFlag)
            End Select

            Return VSConstants.S_OK
        End Function

        ''' <summary>
        ''' Used to set the initial name for unsaved, newly created document data.
        ''' </summary>
        ''' <param name="pszDocDataPath">String containing the path to the document.
        ''' We need to ignore this parameter.
        ''' </param>
        ''' <returns>S_OK if the method succeeds.</returns>
        Private Function SetUntitledDocPath(ByVal pszDocDataPath As String) As Integer Implements IVsPersistDocData.SetUntitledDocPath
            Return (CType(Me, IPersistFileFormat)).InitNew(fileFormat)
        End Function

#End Region

#Region "IVsToolboxUser"
        ''' <summary>
        ''' Determines whether the Toolbox user supports the referenced data object.
        ''' </summary>
        ''' <param name="pDO">Data object to be supported.</param>
        ''' <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code.</returns>
        Private Function IsSupported(ByVal pDO As Microsoft.VisualStudio.OLE.Interop.IDataObject) As Integer Implements IVsToolboxUser.IsSupported
            ' Create a OleDataObject from the input interface.
            Dim oleData As New OleDataObject(pDO)

            ' Check if the data object is of type MyToolboxData.
            If oleData.GetDataPresent(GetType(ToolboxItemData)) Then
                Return VSConstants.S_OK
            End If

            ' In all the other cases return S_FALSE.
            Return VSConstants.S_FALSE
        End Function
        ''' <summary>
        ''' Sends notification that an item in the Toolbox is selected through a click, or by pressing ENTER.
        ''' </summary>
        ''' <param name="pDO">Data object that is selected.</param>
        ''' <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code.</returns>
        Private Function ItemPicked(ByVal pDO As Microsoft.VisualStudio.OLE.Interop.IDataObject) As Integer Implements IVsToolboxUser.ItemPicked
            ' Create a OleDataObject from the input interface.
            Dim oleData As New OleDataObject(pDO)

            ' Check if the picked item is the one we added to the toolbox.
            If oleData.GetDataPresent(GetType(ToolboxItemData)) Then
                System.Diagnostics.Trace.WriteLine("MyToolboxItemData selected from the toolbox")
                Dim myData As ToolboxItemData = CType(oleData.GetData(GetType(ToolboxItemData)), ToolboxItemData)
                editorControl.Text &= myData.Content
            End If
            Return VSConstants.S_OK
        End Function
#End Region

#Region "Event handlers"

        ''' <summary>
        ''' Handles the TextChanged event of contained RichTextBox object. 
        ''' Process changes occurred inside the editor.
        ''' </summary>
        ''' <param name="sender">The reference to contained RichTextBox object.</param>
        ''' <param name="e">The event arguments.</param>
        Private Sub OnTextChange(ByVal sender As Object, ByVal e As System.EventArgs)
            ' During the load operation the text of the control will change, but
            ' this change must not be stored in the status of the document.
            If (Not loading) Then
                ' The only interesting case is when we are changing the document
                ' for the first time.
                If (Not bIsDirty) Then
                    ' Check if the QueryEditQuerySave service allow us to change the file.
                    If (Not CanEditFile()) Then
                        ' We can not change the file (e.g. a checkout operation failed),
                        ' so undo the change and exit.
                        editorControl.Undo()
                        Return
                    End If

                    ' It is possible to change the file, so update the status.
                    bIsDirty = True
                End If
            End If
        End Sub

        ''' <summary>
        ''' Handles the DragEnter event of contained RichTextBox object. 
        ''' Process drag effect for the toolbox item.
        ''' </summary>
        ''' <param name="sender">The reference to contained RichTextBox object.</param>
        ''' <param name="e">The event arguments.</param>
        Private Sub OnDragEnter(ByVal sender As Object, ByVal e As DragEventArgs)
            ' Check if the source of the drag is the toolbox item
            ' created by this sample.
            If e.Data.GetDataPresent(GetType(ToolboxItemData)) Then
                ' Only in this case we will enable the drop.
                e.Effect = DragDropEffects.Copy
            End If
        End Sub

        ''' <summary>
        ''' Handles the DragDrop event of contained RichTextBox object. 
        ''' Process text changes on drop event.
        ''' </summary>
        ''' <param name="sender">The reference to contained RichTextBox object.</param>
        ''' <param name="e">The event arguments.</param>
        Private Sub OnDragDrop(ByVal sender As Object, ByVal e As DragEventArgs)
            ' System.Diagnostics.Debug.Assert(false, "Breakpoint");
            ' The only drop allow is from the toolbox item.
            If e.Data.GetDataPresent(GetType(ToolboxItemData)) Then
                Dim d As Object = e.Data.GetData(GetType(ToolboxItemData))
                Dim data As ToolboxItemData = CType(d, ToolboxItemData)
                editorControl.Text &= data.Content
                e.Effect = DragDropEffects.Copy
            End If
        End Sub

#End Region

#Region "Other methods"
        ''' <summary>
        ''' This function asks to the QueryEditQuerySave service if it is possible to
        ''' edit the file.
        ''' </summary>
        ''' <returns>True if the editing of the file are enabled, otherwise returns false.</returns>
        Private Function CanEditFile() As Boolean
            ' Check the status of the recursion guard.
            If gettingCheckoutStatus Then
                Return False
            End If

            Try
                ' Set the recursion guard.
                gettingCheckoutStatus = True

                ' Get the QueryEditQuerySave service.
                Dim queryEditQuerySave As IVsQueryEditQuerySave2 = CType(GetService(GetType(SVsQueryEditQuerySave)), IVsQueryEditQuerySave2)

                ' Now call the QueryEdit method to find the edit status of this file.
                Dim documents As String() = {Me.fileName}
                Dim result As UInteger
                Dim outFlags As UInteger

                ' Note that this function can pop up a dialog to ask the user to checkout the file.
                ' When this dialog is visible, it is possible to receive other request to change
                ' the file and this is the reason for the recursion guard.
                Dim hr As Integer = queryEditQuerySave.QueryEditFiles(0, 1, documents, Nothing, Nothing, result, outFlags)
                If ErrorHandler.Succeeded(hr) AndAlso (result = CUInt(tagVSQueryEditResult.QER_EditOK)) Then
                    ' In this case (and only in this case) we can return true from this function.
                    Return True
                End If
            Finally
                gettingCheckoutStatus = False
            End Try
            Return False
        End Function

        ''' <summary>
        ''' Gets an instance of the RunningDocumentTable (RDT) service which manages the set of currently open 
        ''' documents in the environment and then notifies the client that an open document has changed.
        ''' </summary>
        Private Sub NotifyDocChanged()
            ' Make sure that we have a file name.
            If fileName.Length = 0 Then
                Return
            End If

            ' Get a reference to the Running Document Table.
            Dim runningDocTable As IVsRunningDocumentTable = CType(GetService(GetType(SVsRunningDocumentTable)), IVsRunningDocumentTable)

            ' Lock the document.
            Dim docCookie As UInteger
            Dim hierarchy As IVsHierarchy = Nothing
            Dim itemID As UInteger
            Dim docData As IntPtr
            Dim hr As Integer = runningDocTable.FindAndLockDocument(CUInt(_VSRDTFLAGS.RDT_ReadLock), fileName, hierarchy, itemID, docData, docCookie)
            ErrorHandler.ThrowOnFailure(hr)

            ' Send the notification.
            hr = runningDocTable.NotifyDocumentChanged(docCookie, CUInt(__VSRDTATTRIB.RDTA_DocDataReloaded))

            ' Unlock the document.
            ' Note that we have to unlock the document even if the previous call failed.
            runningDocTable.UnlockDocument(CUInt(_VSRDTFLAGS.RDT_ReadLock), docCookie)

            ' Check ff the call to NotifyDocChanged failed.
            ErrorHandler.ThrowOnFailure(hr)
        End Sub
#End Region

#End Region
    End Class
End Namespace
