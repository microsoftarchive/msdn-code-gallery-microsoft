'**************************************************************************
'
'Copyright (c) Microsoft Corporation. All rights reserved.
'THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
'ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
'IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
'PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'**************************************************************************

' SccProviderService.vb : Implementation of Sample Source Control Provider Service
'


Imports Microsoft.VisualBasic
Imports System
Imports System.IO
Imports System.Diagnostics
Imports System.Collections
Imports System.Collections.Generic
Imports System.Globalization
Imports System.Runtime.InteropServices
Imports Microsoft.VisualStudio.OLE.Interop
Imports Microsoft.VisualStudio.Shell.Interop
Imports Microsoft.VisualStudio
Imports System.Windows.Forms

Namespace Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider
	<Guid(GuidStrings.GuidSccProviderService)> _
	Public Class SccProviderService
		Implements IVsSccProvider, IVsSccManager2, IVsSccManagerTooltip, IVsSolutionEvents, IVsSolutionEvents2, IVsQueryEditQuerySave2, IVsTrackProjectDocumentsEvents2, IDisposable
        ' Whether the provider is active or not.
		Private _active As Boolean = False
        ' The service and source control provider.
		Private _sccProvider As SccProvider = Nothing
        ' The cookie for solution events.
		Private _vsSolutionEventsCookie As UInteger
        ' The cookie for project document events.
		Private _tpdTrackProjectDocumentsCookie As UInteger
        ' The list of controlled projects hierarchies.
        Private _controlledProjects As New Hashtable()
        ' The list of controlled and offline projects hierarchies.
        Private _offlineProjects As New Hashtable()
        ' Variable tracking whether the currently loading solution is controlled (during solution load or merge).
		Private _loadingControlledSolutionLocation As String = ""
        ' The location of the currently controlled solution.
		Private _solutionLocation As String
        ' The list of files approved for in-memory edit.
        Private _approvedForInMemoryEdit As New Hashtable()

		#Region "SccProvider Service initialization/unitialization"

		Public Sub New(ByVal sccProvider As SccProvider)
			Debug.Assert(Not Nothing Is sccProvider)
			_sccProvider = sccProvider

            ' Subscribe to solution events.
			Dim sol As IVsSolution = CType(_sccProvider.GetService(GetType(SVsSolution)), IVsSolution)
			sol.AdviseSolutionEvents(Me, _vsSolutionEventsCookie)
			Debug.Assert(VSConstants.VSCOOKIE_NIL <> _vsSolutionEventsCookie)

            ' Subscribe to project documents.
			Dim tpdService As IVsTrackProjectDocuments2 = CType(_sccProvider.GetService(GetType(SVsTrackProjectDocuments)), IVsTrackProjectDocuments2)
			tpdService.AdviseTrackProjectDocumentsEvents(Me, _tpdTrackProjectDocumentsCookie)
			Debug.Assert(VSConstants.VSCOOKIE_NIL <> _tpdTrackProjectDocumentsCookie)
		End Sub

		Public Sub Dispose() Implements IDisposable.Dispose
            ' Unregister from receiving solution events.
			If VSConstants.VSCOOKIE_NIL <> _vsSolutionEventsCookie Then
				Dim sol As IVsSolution = CType(_sccProvider.GetService(GetType(SVsSolution)), IVsSolution)
				sol.UnadviseSolutionEvents(_vsSolutionEventsCookie)
				_vsSolutionEventsCookie = VSConstants.VSCOOKIE_NIL
			End If

            ' Unregister from receiving project documents.
			If VSConstants.VSCOOKIE_NIL <> _tpdTrackProjectDocumentsCookie Then
				Dim tpdService As IVsTrackProjectDocuments2 = CType(_sccProvider.GetService(GetType(SVsTrackProjectDocuments)), IVsTrackProjectDocuments2)
				tpdService.UnadviseTrackProjectDocumentsEvents(_tpdTrackProjectDocumentsCookie)
				_tpdTrackProjectDocumentsCookie = VSConstants.VSCOOKIE_NIL
			End If
		End Sub

		#End Region

		'--------------------------------------------------------------------------------
		' IVsSccProvider specific functions
		'--------------------------------------------------------------------------------
		#Region "IVsSccProvider interface functions"

		' Called by the scc manager when the provider is activated. 
        ' Make visible and enable if necessary scc related menu commands.
		Public Function SetActive() As Integer Implements IVsSccProvider.SetActive
            Debug.WriteLine(String.Format(CultureInfo.CurrentUICulture, "The source control provider is now active"))

			_active = True
			_sccProvider.OnActiveStateChange()

			Return VSConstants.S_OK
		End Function

		' Called by the scc manager when the provider is deactivated. 
        ' Hides and disable scc related menu commands.
		Public Function SetInactive() As Integer Implements IVsSccProvider.SetInactive
            Debug.WriteLine(String.Format(CultureInfo.CurrentUICulture, "The source control provider is now inactive"))

			_active = False
			_sccProvider.OnActiveStateChange()

			Return VSConstants.S_OK
		End Function

		Public Function AnyItemsUnderSourceControl(<System.Runtime.InteropServices.Out()> ByRef pfResult As Integer) As Integer Implements IVsSccProvider.AnyItemsUnderSourceControl
			If (Not _active) Then
				pfResult = 0
			Else
                ' Although the parameter is an int, it's in reality a BOOL value, so let's return 0/1 values.
				If (_controlledProjects.Count <> 0) Then
					pfResult = 1
				Else
					pfResult = 0
				End If
			End If

			Return VSConstants.S_OK
		End Function

		#End Region

		'--------------------------------------------------------------------------------
		' IVsSccManager2 specific functions
		'--------------------------------------------------------------------------------
		#Region "IVsSccManager2 interface functions"

		Public Function BrowseForProject(<System.Runtime.InteropServices.Out()> ByRef pbstrDirectory As String, <System.Runtime.InteropServices.Out()> ByRef pfOK As Integer) As Integer Implements IVsSccManager2.BrowseForProject
            ' Obsolete method.
			pbstrDirectory = Nothing
			pfOK = 0
			Return VSConstants.E_NOTIMPL
		End Function

		Public Function CancelAfterBrowseForProject() As Integer Implements IVsSccManager2.CancelAfterBrowseForProject
            ' Obsolete method.
			Return VSConstants.E_NOTIMPL
		End Function

		''' <summary>
        ''' Returns whether the source control provider is fully installed.
		''' </summary>
		Public Function IsInstalled(<System.Runtime.InteropServices.Out()> ByRef pbInstalled As Integer) As Integer Implements IVsSccManager2.IsInstalled
            ' All source control packages should always return S_OK and set pbInstalled to nonzero.
			pbInstalled = 1
			Return VSConstants.S_OK
		End Function

		''' <summary>
        ''' Provide source control icons for the specified files and returns scc status of files.
		''' </summary>
		''' <returns>The method returns S_OK if at least one of the files is controlled, S_FALSE if none of them are</returns>
		Public Function GetSccGlyph(<InAttribute> ByVal cFiles As Integer, <InAttribute> ByVal rgpszFullPaths As String(), <OutAttribute> ByVal rgsiGlyphs As VsStateIcon(), <OutAttribute> ByVal rgdwSccStatus As UInteger()) As Integer Implements IVsSccManager2.GetSccGlyph
			Debug.Assert(cFiles = 1, "Only getting one file icon at a time is supported")

			' Return the icons and the status. While the status is a combination a flags, we'll return just values 
            ' with one bit set, to make life easier for GetSccGlyphsFromStatus.
			Dim status As SourceControlStatus = GetFileStatus(rgpszFullPaths(0))
			Select Case status
				Case SourceControlStatus.scsCheckedIn
					rgsiGlyphs(0) = VsStateIcon.STATEICON_CHECKEDIN
                    If rgdwSccStatus IsNot Nothing Then
                        rgdwSccStatus(0) = CUInt(__SccStatus.SCC_STATUS_CONTROLLED)
                    End If
				Case SourceControlStatus.scsCheckedOut
					rgsiGlyphs(0) = VsStateIcon.STATEICON_CHECKEDOUT
                    If rgdwSccStatus IsNot Nothing Then
                        rgdwSccStatus(0) = CUInt(__SccStatus.SCC_STATUS_CHECKEDOUT)
                    End If
				Case Else
					Dim nodes As IList(Of VSITEMSELECTION) = GetControlledProjectsContainingFile(rgpszFullPaths(0))
					If nodes.Count > 0 Then
                        ' If the file is not controlled, but is member of a controlled project, report the item as checked out (same as source control in VS2003 did).
                        ' If the provider wants to have special icons for "pending add" files, the IVsSccGlyphs interface needs to be supported.
						rgsiGlyphs(0) = VsStateIcon.STATEICON_CHECKEDOUT
                        If rgdwSccStatus IsNot Nothing Then
                            rgdwSccStatus(0) = CUInt(__SccStatus.SCC_STATUS_CHECKEDOUT)
                        End If
					Else
                        ' This is an uncontrolled file, return a blank scc glyph for it.
						rgsiGlyphs(0) = VsStateIcon.STATEICON_BLANK
                        If rgdwSccStatus IsNot Nothing Then
                            rgdwSccStatus(0) = CUInt(__SccStatus.SCC_STATUS_NOTCONTROLLED)
                        End If
					End If
			End Select

			Return VSConstants.S_OK
		End Function

		''' <summary>
        ''' Determines the corresponding scc status glyph to display, given a combination of scc status flags.
		''' </summary>
		Public Function GetSccGlyphFromStatus(<InAttribute> ByVal dwSccStatus As UInteger, <OutAttribute> ByVal psiGlyph As VsStateIcon()) As Integer Implements IVsSccManager2.GetSccGlyphFromStatus
			Select Case dwSccStatus
				Case CUInt(__SccStatus.SCC_STATUS_CHECKEDOUT)
					psiGlyph(0) = VsStateIcon.STATEICON_CHECKEDOUT
				Case CUInt(__SccStatus.SCC_STATUS_CONTROLLED)
					psiGlyph(0) = VsStateIcon.STATEICON_CHECKEDIN
				Case Else
					psiGlyph(0) = VsStateIcon.STATEICON_BLANK
			End Select
			Return VSConstants.S_OK
		End Function

		''' <summary>
        ''' One of the most important methods in a source control provider, is called by projects that are under source control when they are first opened to register project settings.
		''' </summary>
		Public Function RegisterSccProject(<InAttribute> ByVal pscp2Project As IVsSccProject2, <InAttribute> ByVal pszSccProjectName As String, <InAttribute> ByVal pszSccAuxPath As String, <InAttribute> ByVal pszSccLocalPath As String, <InAttribute> ByVal pszProvider As String) As Integer Implements IVsSccManager2.RegisterSccProject
			If pszProvider.CompareTo(_sccProvider.ProviderName)<>0 Then
				' If the provider name controlling this project is not our provider, the user may be adding to a 
				' solution controlled by this provider an existing project controlled by some other provider.
				' We'll deny the registration with scc in such case.
				Return VSConstants.E_FAIL
			End If

			If pscp2Project Is Nothing Then
                ' Manual registration with source control of the solution, from OnAfterOpenSolution.
                Debug.WriteLine(String.Format(CultureInfo.CurrentUICulture, "Solution {0} is registering with source control - {1}, {2}, {3}, {4}", _sccProvider.GetSolutionFileName(), pszSccProjectName, pszSccAuxPath, pszSccLocalPath, pszProvider))

				Dim solHier As IVsHierarchy = CType(_sccProvider.GetService(GetType(SVsSolution)), IVsHierarchy)
				Dim solutionFile As String = _sccProvider.GetSolutionFileName()
                Dim storage As New SccProviderStorage(solutionFile)
				_controlledProjects(solHier) = storage
			Else
                Debug.WriteLine(String.Format(CultureInfo.CurrentUICulture, "Project {0} is registering with source control - {1}, {2}, {3}, {4}", _sccProvider.GetProjectFileName(pscp2Project), pszSccProjectName, pszSccAuxPath, pszSccLocalPath, pszProvider))

                ' Add the project to the list of controlled projects.
				Dim hierProject As IVsHierarchy = CType(pscp2Project, IVsHierarchy)
                Dim storage As New SccProviderStorage(_sccProvider.GetProjectFileName(pscp2Project))
				_controlledProjects(hierProject) = storage
			End If

			Return VSConstants.S_OK
		End Function

		''' <summary>
		''' Called by projects registered with the source control portion of the environment before they are closed. 
		''' </summary>
		Public Function UnregisterSccProject(<InAttribute> ByVal pscp2Project As IVsSccProject2) As Integer Implements IVsSccManager2.UnregisterSccProject
            ' Get the project's hierarchy.
			Dim hierProject As IVsHierarchy = Nothing
			If pscp2Project Is Nothing Then
                ' If the project's pointer is null, it must be the solution calling to unregister, from OnBeforeCloseSolution.
                Debug.WriteLine(String.Format(CultureInfo.CurrentUICulture, "Solution {0} is unregistering with source control.", _sccProvider.GetSolutionFileName()))
				hierProject = CType(_sccProvider.GetService(GetType(SVsSolution)), IVsHierarchy)
			Else
                Debug.WriteLine(String.Format(CultureInfo.CurrentUICulture, "Project {0} is unregistering with source control.", _sccProvider.GetProjectFileName(pscp2Project)))
				hierProject = CType(pscp2Project, IVsHierarchy)
			End If

            ' Remove the project from the list of controlled projects.
			If _controlledProjects.ContainsKey(hierProject) Then
				_controlledProjects.Remove(hierProject)
				Return VSConstants.S_OK
			Else
				Return VSConstants.S_FALSE
			End If
		End Function

		#End Region

		'--------------------------------------------------------------------------------
		' IVsSccManagerTooltip specific functions
		'--------------------------------------------------------------------------------
		#Region "IVsSccManagerTooltip interface functions"

		''' <summary>
		''' Called by solution explorer to provide tooltips for items. Returns a text describing the source control status of the item.
		''' </summary>
		Public Function GetGlyphTipText(<InAttribute> ByVal phierHierarchy As IVsHierarchy, <InAttribute> ByVal itemidNode As UInteger, <System.Runtime.InteropServices.Out()> ByRef pbstrTooltipText As String) As Integer Implements IVsSccManagerTooltip.GetGlyphTipText
            ' Initialize output parameters.
			pbstrTooltipText = ""

			Dim files As IList(Of String) = _sccProvider.GetNodeFiles(phierHierarchy, itemidNode)
			If files.Count = 0 Then
				Return VSConstants.S_OK
			End If

            ' Return the glyph text based on the first file of node (the master file).
			Dim status As SourceControlStatus = GetFileStatus(files(0))
			Select Case status
				Case SourceControlStatus.scsCheckedIn
					pbstrTooltipText = Resources.ResourceManager.GetString("Status_CheckedIn")
				Case SourceControlStatus.scsCheckedOut
					pbstrTooltipText = Resources.ResourceManager.GetString("Status_CheckedOut")
				Case Else
                    ' If the file is not controlled, but is member of a controlled project, report the item as checked out (same as source control in VS2003 did).
                    ' If the provider wants to have special icons for "pending add" files, the IVsSccGlyphs interface needs to be supported.
					Dim nodes As IList(Of VSITEMSELECTION) = GetControlledProjectsContainingFile(files(0))
					If nodes.Count > 0 Then
						pbstrTooltipText = Resources.ResourceManager.GetString("Status_PendingAdd")
					End If
			End Select

			Return VSConstants.S_OK
		End Function

		#End Region

		'--------------------------------------------------------------------------------
		' IVsSolutionEvents and IVsSolutionEvents2 specific functions
		'--------------------------------------------------------------------------------
		#Region "IVsSolutionEvents interface functions"

		Public Function OnAfterCloseSolution(<InAttribute> ByVal pUnkReserved As Object) As Integer Implements IVsSolutionEvents.OnAfterCloseSolution, IVsSolutionEvents2.OnAfterCloseSolution
            ' Reset all source-control-related data now that solution is closed.
			_controlledProjects.Clear()
			_offlineProjects.Clear()
			_sccProvider.SolutionHasDirtyProps = False
			_loadingControlledSolutionLocation = ""
			_solutionLocation = ""
			_approvedForInMemoryEdit.Clear()

			Return VSConstants.S_OK
		End Function

		Public Function OnAfterLoadProject(<InAttribute> ByVal pStubHierarchy As IVsHierarchy, <InAttribute> ByVal pRealHierarchy As IVsHierarchy) As Integer Implements IVsSolutionEvents.OnAfterLoadProject, IVsSolutionEvents2.OnAfterLoadProject
			Return VSConstants.S_OK
		End Function

		Public Function OnAfterOpenProject(<InAttribute> ByVal pHierarchy As IVsHierarchy, <InAttribute> ByVal fAdded As Integer) As Integer Implements IVsSolutionEvents.OnAfterOpenProject, IVsSolutionEvents2.OnAfterOpenProject
            ' If a solution folder is added to the solution after the solution is added to scc, we need to controll that folder.
			If _sccProvider.IsSolutionFolderProject(pHierarchy) AndAlso (fAdded = 1) Then
				Dim solHier As IVsHierarchy = CType(_sccProvider.GetService(GetType(SVsSolution)), IVsHierarchy)
				If IsProjectControlled(solHier) Then
                    ' Register this solution folder using the same location as the solution.
					Dim pSccProject As IVsSccProject2 = CType(pHierarchy, IVsSccProject2)
					RegisterSccProject(pSccProject, _solutionLocation, "", "", _sccProvider.ProviderName)

                    ' We'll also need to refresh the solution folders glyphs to reflect the controlled state.
                    Dim nodes As New List(Of VSITEMSELECTION)()

					Dim vsItem As VSITEMSELECTION
					vsItem.itemid = VSConstants.VSITEMID_ROOT
					vsItem.pHier = pHierarchy
					nodes.Add(vsItem)

					_sccProvider.RefreshNodesGlyphs(nodes)
				End If
			End If

			Return VSConstants.S_OK
		End Function

		Public Function OnAfterOpenSolution(<InAttribute> ByVal pUnkReserved As Object, <InAttribute> ByVal fNewSolution As Integer) As Integer Implements IVsSolutionEvents.OnAfterOpenSolution, IVsSolutionEvents2.OnAfterOpenSolution
			' This event is fired last by the shell when opening a solution.
			' By this time, we have already loaded the solution persistence data from the PreLoad section
            ' the controlled projects should be opened and registered with source control.
			If _loadingControlledSolutionLocation.Length > 0 Then
                ' We'll also need to refresh the solution glyphs to reflect the controlled state.
				Dim nodes As IList(Of VSITEMSELECTION) = New List(Of VSITEMSELECTION)()

				' If the solution was controlled, now it is time to register the solution hierarchy with souce control, too.
                ' Note that solution is not calling RegisterSccProject(), the scc package will do this call as it knows the source control location.
				RegisterSccProject(Nothing, _loadingControlledSolutionLocation, "", "", _sccProvider.ProviderName)

				Dim vsItem As VSITEMSELECTION
				vsItem.itemid = VSConstants.VSITEMID_ROOT
				vsItem.pHier = Nothing
				nodes.Add(vsItem)

                ' Also, solution folders won't call RegisterSccProject, so we have to enumerate them and register them with scc once the solution is controlled.
				Dim enumSolFolders As Hashtable = _sccProvider.GetSolutionFoldersEnum()
				For Each pHier As IVsHierarchy In enumSolFolders.Keys
                    ' Register this solution folder using the same location as the solution.
					Dim pSccProject As IVsSccProject2 = CType(pHier, IVsSccProject2)
					RegisterSccProject(pSccProject, _loadingControlledSolutionLocation, "", "", _sccProvider.ProviderName)

					vsItem.itemid = VSConstants.VSITEMID_ROOT
					vsItem.pHier = pHier
					nodes.Add(vsItem)
				Next pHier

                ' Refresh the glyphs now for solution and solution folders.
				_sccProvider.RefreshNodesGlyphs(nodes)
			End If

			_solutionLocation = _loadingControlledSolutionLocation

            ' Reset the flag now that solution open completed.
			_loadingControlledSolutionLocation = ""

			Return VSConstants.S_OK
		End Function

		Public Function OnBeforeCloseProject(<InAttribute> ByVal pHierarchy As IVsHierarchy, <InAttribute> ByVal fRemoved As Integer) As Integer Implements IVsSolutionEvents.OnBeforeCloseProject, IVsSolutionEvents2.OnBeforeCloseProject
			Return VSConstants.S_OK
		End Function

		Public Function OnBeforeCloseSolution(<InAttribute> ByVal pUnkReserved As Object) As Integer Implements IVsSolutionEvents.OnBeforeCloseSolution, IVsSolutionEvents2.OnBeforeCloseSolution
			' Since we registered the solution with source control from OnAfterOpenSolution, it would be nice to unregister it, too, when it gets closed.
			' Also, unregister the solution folders
			Dim enumSolFolders As Hashtable = _sccProvider.GetSolutionFoldersEnum()
			For Each pHier As IVsHierarchy In enumSolFolders.Keys
				Dim pSccProject As IVsSccProject2 = CType(pHier, IVsSccProject2)
				UnregisterSccProject(pSccProject)
			Next pHier

			UnregisterSccProject(Nothing)

			Return VSConstants.S_OK
		End Function

		Public Function OnBeforeUnloadProject(<InAttribute> ByVal pRealHierarchy As IVsHierarchy, <InAttribute> ByVal pStubHierarchy As IVsHierarchy) As Integer Implements IVsSolutionEvents.OnBeforeUnloadProject, IVsSolutionEvents2.OnBeforeUnloadProject
			Return VSConstants.S_OK
		End Function

		Public Function OnQueryCloseProject(<InAttribute> ByVal pHierarchy As IVsHierarchy, <InAttribute> ByVal fRemoving As Integer, <InAttribute> ByRef pfCancel As Integer) As Integer Implements IVsSolutionEvents.OnQueryCloseProject, IVsSolutionEvents2.OnQueryCloseProject
			Return VSConstants.S_OK
		End Function

		Public Function OnQueryCloseSolution(<InAttribute> ByVal pUnkReserved As Object, <InAttribute> ByRef pfCancel As Integer) As Integer Implements IVsSolutionEvents.OnQueryCloseSolution, IVsSolutionEvents2.OnQueryCloseSolution
			Return VSConstants.S_OK
		End Function

		Public Function OnQueryUnloadProject(<InAttribute> ByVal pRealHierarchy As IVsHierarchy, <InAttribute> ByRef pfCancel As Integer) As Integer Implements IVsSolutionEvents.OnQueryUnloadProject, IVsSolutionEvents2.OnQueryUnloadProject
			Return VSConstants.S_OK
		End Function

		Public Function OnAfterMergeSolution(<InAttribute> ByVal pUnkReserved As Object) As Integer Implements IVsSolutionEvents2.OnAfterMergeSolution
            ' Reset the flag now that solutions were merged and the merged solution completed opening.
			_loadingControlledSolutionLocation = ""

			Return VSConstants.S_OK
		End Function

		#End Region

		'--------------------------------------------------------------------------------
		' IVsQueryEditQuerySave2 specific functions
		'--------------------------------------------------------------------------------
		#Region "IVsQueryEditQuerySave2 interface functions"

		Public Function BeginQuerySaveBatch() As Integer Implements IVsQueryEditQuerySave2.BeginQuerySaveBatch
			Return VSConstants.S_OK
		End Function

		Public Function EndQuerySaveBatch() As Integer Implements IVsQueryEditQuerySave2.EndQuerySaveBatch
			Return VSConstants.S_OK
		End Function

		Public Function DeclareReloadableFile(<InAttribute> ByVal pszMkDocument As String, <InAttribute> ByVal rgf As UInteger, <InAttribute> ByVal pFileInfo As VSQEQS_FILE_ATTRIBUTE_DATA()) As Integer Implements IVsQueryEditQuerySave2.DeclareReloadableFile
			Return VSConstants.S_OK
		End Function

		Public Function DeclareUnreloadableFile(<InAttribute> ByVal pszMkDocument As String, <InAttribute> ByVal rgf As UInteger, <InAttribute> ByVal pFileInfo As VSQEQS_FILE_ATTRIBUTE_DATA()) As Integer Implements IVsQueryEditQuerySave2.DeclareUnreloadableFile
			Return VSConstants.S_OK
		End Function

		Public Function IsReloadable(<InAttribute> ByVal pszMkDocument As String, <System.Runtime.InteropServices.Out()> ByRef pbResult As Integer) As Integer Implements IVsQueryEditQuerySave2.IsReloadable
            ' Since we're not tracking which files are reloadable and which not, consider everything reloadable.
			pbResult = 1
			Return VSConstants.S_OK
		End Function

		Public Function OnAfterSaveUnreloadableFile(<InAttribute> ByVal pszMkDocument As String, <InAttribute> ByVal rgf As UInteger, <InAttribute> ByVal pFileInfo As VSQEQS_FILE_ATTRIBUTE_DATA()) As Integer Implements IVsQueryEditQuerySave2.OnAfterSaveUnreloadableFile
			Return VSConstants.S_OK
		End Function

		''' <summary>
        ''' Called by projects and editors before modifying a file.
		''' The function allows the source control systems to take the necessary actions (checkout, flip attributes)
        ''' to make the file writable in order to allow the edit to continue.
		'''
		''' There are a lot of cases to deal with during QueryEdit/QuerySave. 
		''' - called in commmand line mode, when UI cannot be displayed
		''' - called during builds, when save shoudn't probably be allowed
		''' - called during projects migration, when projects are not open and not registered yet with source control
		''' - checking out files may bring new versions from vss database which may be reloaded and the user may lose in-memory changes; some other files may not be reloadable
		''' - not all editors call QueryEdit when they modify the file the first time (buggy editors!), and the files may be already dirty in memory when QueryEdit is called
		''' - files on disk may be modified outside IDE and may have attributes incorrect for their scc status
		''' - checkouts may fail
		''' The sample provider won't deal with all these situations, but a real source control provider should!
		''' </summary>
		Public Function QueryEditFiles(<InAttribute> ByVal rgfQueryEdit As UInteger, <InAttribute> ByVal cFiles As Integer, <InAttribute> ByVal rgpszMkDocuments As String(), <InAttribute> ByVal rgrgf As UInteger(), <InAttribute> ByVal rgFileInfo As VSQEQS_FILE_ATTRIBUTE_DATA(), <System.Runtime.InteropServices.Out()> ByRef pfEditVerdict As UInteger, <System.Runtime.InteropServices.Out()> ByRef prgfMoreInfo As UInteger) As Integer Implements IVsQueryEditQuerySave2.QueryEditFiles
            ' Initialize output variables.
			pfEditVerdict = CUInt(tagVSQueryEditResult.QER_EditOK)
			prgfMoreInfo = 0

            ' In non-UI mode just allow the edit, because the user cannot be asked what to do with the file.
			If _sccProvider.InCommandLineMode() Then
				Return VSConstants.S_OK
			End If

			Try
                ' Iterate through all the files.
				For iFile As Integer = 0 To cFiles - 1

					Dim fEditVerdict As UInteger = CUInt(tagVSQueryEditResult.QER_EditNotOK)
					Dim fMoreInfo As UInteger = 0

					' Because of the way we calculate the status, it is not possible to have a 
                    ' checked in file that is writtable on disk, or a checked out file that is read-only on disk.
                    ' A source control provider would need to deal with those situations, too.
					Dim status As SourceControlStatus = GetFileStatus(rgpszMkDocuments(iFile))
					Dim fileExists As Boolean = File.Exists(rgpszMkDocuments(iFile))
					Dim isFileReadOnly As Boolean = False
					If fileExists Then
						isFileReadOnly = ((File.GetAttributes(rgpszMkDocuments(iFile)) And FileAttributes.ReadOnly) = FileAttributes.ReadOnly)
					End If

                    ' Allow the edits if the file does not exist or is writable.
					If (Not fileExists) OrElse (Not isFileReadOnly) Then
						fEditVerdict = CUInt(tagVSQueryEditResult.QER_EditOK)
					Else
                        ' If the IDE asks about a file that was already approved for in-memory edit, allow the edit without asking the user again.
						If _approvedForInMemoryEdit.ContainsKey(rgpszMkDocuments(iFile).ToLower()) Then
							fEditVerdict = CUInt(tagVSQueryEditResult.QER_EditOK)
							fMoreInfo = CUInt(tagVSQueryEditResultFlags.QER_InMemoryEdit)
						Else
							Select Case status
								Case SourceControlStatus.scsCheckedIn
									If (rgfQueryEdit And CUInt(tagVSQueryEditFlags.QEF_ReportOnly)) <> 0 Then
										fMoreInfo = CUInt(tagVSQueryEditResultFlags.QER_EditNotPossible Or tagVSQueryEditResultFlags.QER_ReadOnlyUnderScc)
									Else
                                        Dim dlgAskCheckout As New DlgQueryEditCheckedInFile(rgpszMkDocuments(iFile))
										If (rgfQueryEdit And CUInt(tagVSQueryEditFlags.QEF_SilentMode)) <> 0 Then
                                            ' When called in silent mode, attempt the checkout.
											' (The alternative is to deny the edit and return QER_NoisyPromptRequired and expect for a non-silent call)
											dlgAskCheckout.Answer = DlgQueryEditCheckedInFile.qecifCheckout
										Else
											dlgAskCheckout.ShowDialog()
										End If

										If dlgAskCheckout.Answer = DlgQueryEditCheckedInFile.qecifCheckout Then
                                            ' Checkout the file, and since it cannot fail, allow the edit.
											CheckoutFileAndRefreshProjectGlyphs(rgpszMkDocuments(iFile))
											fEditVerdict = CUInt(tagVSQueryEditResult.QER_EditOK)
											fMoreInfo = CUInt(tagVSQueryEditResultFlags.QER_MaybeCheckedout)
                                            ' Do not forget to set QER_Changed if the content of the file on disk changes during the query edit.
											' Do not forget to set QER_Reloaded if the source control reloads the file from disk after such changing checkout.
										ElseIf dlgAskCheckout.Answer = DlgQueryEditCheckedInFile.qecifEditInMemory Then
                                            ' Allow edit in memory.
											fEditVerdict = CUInt(tagVSQueryEditResult.QER_EditOK)
											fMoreInfo = CUInt(tagVSQueryEditResultFlags.QER_InMemoryEdit)
                                            ' Add the file to the list of files approved for edit, so if the IDE asks again about this file, we'll allow the edit without asking the user again.
											' UNDONE: Currently, a file gets removed from _approvedForInMemoryEdit list only when the solution is closed. Consider intercepting the 
											' IVsRunningDocTableEvents.OnAfterSave/OnAfterSaveAll interface and removing the file from the approved list after it gets saved once.
											_approvedForInMemoryEdit(rgpszMkDocuments(iFile).ToLower()) = True
										Else
											fEditVerdict = CUInt(tagVSQueryEditResult.QER_NoEdit_UserCanceled)
											fMoreInfo = CUInt(tagVSQueryEditResultFlags.QER_ReadOnlyUnderScc Or tagVSQueryEditResultFlags.QER_CheckoutCanceledOrFailed)
										End If

									End If
								Case SourceControlStatus.scsCheckedOut, SourceControlStatus.scsUncontrolled ' fall through
									If fileExists AndAlso isFileReadOnly Then
										If (rgfQueryEdit And CUInt(tagVSQueryEditFlags.QEF_ReportOnly)) <> 0 Then
											fMoreInfo = CUInt(tagVSQueryEditResultFlags.QER_EditNotPossible Or tagVSQueryEditResultFlags.QER_ReadOnlyNotUnderScc)
										Else
											Dim fChangeAttribute As Boolean = False
											If (rgfQueryEdit And CUInt(tagVSQueryEditFlags.QEF_SilentMode)) <> 0 Then
                                                ' When called in silent mode, deny the edit and return QER_NoisyPromptRequired and expect for a non-silent call.
												' (The alternative is to silently make the file writable and accept the edit)
												fMoreInfo = CUInt(tagVSQueryEditResultFlags.QER_EditNotPossible Or tagVSQueryEditResultFlags.QER_ReadOnlyNotUnderScc Or tagVSQueryEditResultFlags.QER_NoisyPromptRequired)
											Else
                                                ' This is a controlled file, warn the user.
												Dim uiShell As IVsUIShell = CType(_sccProvider.GetService(GetType(SVsUIShell)), IVsUIShell)
												Dim clsid As Guid = Guid.Empty
												Dim result As Integer = VSConstants.S_OK
												Dim messageText As String = Resources.ResourceManager.GetString("QEQS_EditUncontrolledReadOnly")
												Dim messageCaption As String = Resources.ResourceManager.GetString("ProviderName")
												If uiShell.ShowMessageBox(0, clsid, messageCaption, String.Format(CultureInfo.CurrentUICulture, messageText, rgpszMkDocuments(iFile)), String.Empty, 0, OLEMSGBUTTON.OLEMSGBUTTON_YESNO, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST, OLEMSGICON.OLEMSGICON_QUERY, 0, result) = VSConstants.S_OK AndAlso result = CInt(Fix(DialogResult.Yes)) Then
													fChangeAttribute = True
												End If
											End If

											If fChangeAttribute Then
                                                ' Make the file writable and allow the edit.
												File.SetAttributes(rgpszMkDocuments(iFile), FileAttributes.Normal)
												fEditVerdict = CUInt(tagVSQueryEditResult.QER_EditOK)
											End If
										End If
									Else
										fEditVerdict = CUInt(tagVSQueryEditResult.QER_EditOK)
									End If
							End Select
						End If
					End If

					' It's a bit unfortunate that we have to return only one set of flags for all the files involved in the operation
                    ' The edit can continue if all the files were approved for edit.
					prgfMoreInfo = prgfMoreInfo Or fMoreInfo
					pfEditVerdict = pfEditVerdict Or fEditVerdict
				Next iFile
			Catch e1 As Exception
                ' If an exception was caught, do not allow the edit.
				pfEditVerdict = CUInt(tagVSQueryEditResult.QER_EditNotOK)
				prgfMoreInfo = CUInt(tagVSQueryEditResultFlags.QER_EditNotPossible)
			End Try

			Return VSConstants.S_OK
		End Function

		''' <summary>
        ''' Called by editors and projects before saving the files.
		''' The function allows the source control systems to take the necessary actions (checkout, flip attributes)
        ''' to make the file writable in order to allow the file saving to continue.
		''' </summary>
		Public Function QuerySaveFile(<InAttribute> ByVal pszMkDocument As String, <InAttribute> ByVal rgf As UInteger, <InAttribute> ByVal pFileInfo As VSQEQS_FILE_ATTRIBUTE_DATA(), <System.Runtime.InteropServices.Out()> ByRef pdwQSResult As UInteger) As Integer Implements IVsQueryEditQuerySave2.QuerySaveFile
            ' Delegate to the other QuerySave function.
			Dim rgszDocuements As String() = New String(0){}
			Dim rgrgf As UInteger() = New UInteger(0){}
			rgszDocuements(0) = pszMkDocument
			rgrgf(0) = rgf
            Return QuerySaveFiles(Convert.ToUInt32(tagVSQuerySaveFlags.QSF_DefaultOperation), 1, rgszDocuements, rgrgf, pFileInfo, pdwQSResult)
		End Function

		''' <summary>
        ''' Called by editors and projects before saving the files.
		''' The function allows the source control systems to take the necessary actions (checkout, flip attributes)
        ''' to make the file writable in order to allow the file saving to continue.
		''' </summary>
		Public Function QuerySaveFiles(<InAttribute> ByVal rgfQuerySave As UInteger, <InAttribute> ByVal cFiles As Integer, <InAttribute> ByVal rgpszMkDocuments As String(), <InAttribute> ByVal rgrgf As UInteger(), <InAttribute> ByVal rgFileInfo As VSQEQS_FILE_ATTRIBUTE_DATA(), <System.Runtime.InteropServices.Out()> ByRef pdwQSResult As UInteger) As Integer Implements IVsQueryEditQuerySave2.QuerySaveFiles
            ' Initialize output variables.
			' It's a bit unfortunate that we have to return only one set of flags for all the files involved in the operation
            ' The last file will win setting this flag.
			pdwQSResult = CUInt(tagVSQuerySaveResult.QSR_SaveOK)

			' In non-UI mode attempt to silently flip the attributes of files or check them out 
            ' and allow the save, because the user cannot be asked what to do with the file.
			If _sccProvider.InCommandLineMode() Then
				rgfQuerySave = rgfQuerySave Or CUInt(tagVSQuerySaveFlags.QSF_SilentMode)
			End If

			Try
				For iFile As Integer = 0 To cFiles - 1
					Dim status As SourceControlStatus = GetFileStatus(rgpszMkDocuments(iFile))
					Dim fileExists As Boolean = File.Exists(rgpszMkDocuments(iFile))
					Dim isFileReadOnly As Boolean = False
					If fileExists Then
						isFileReadOnly = ((File.GetAttributes(rgpszMkDocuments(iFile)) And FileAttributes.ReadOnly) = FileAttributes.ReadOnly)
					End If

					Select Case status
						Case SourceControlStatus.scsCheckedIn
                            Dim dlgAskCheckout As New DlgQuerySaveCheckedInFile(rgpszMkDocuments(iFile))
							If (rgfQuerySave And CUInt(tagVSQuerySaveFlags.QSF_SilentMode)) <> 0 Then
                                ' When called in silent mode, attempt the checkout.
								' (The alternative is to deny the save, return QSR_NoSave_NoisyPromptRequired and expect for a non-silent call)
								dlgAskCheckout.Answer = DlgQuerySaveCheckedInFile.qscifCheckout
							Else
								dlgAskCheckout.ShowDialog()
							End If

							Select Case dlgAskCheckout.Answer
								Case DlgQueryEditCheckedInFile.qecifCheckout
                                    ' Checkout the file, and since it cannot fail, allow the save to continue.
									CheckoutFileAndRefreshProjectGlyphs(rgpszMkDocuments(iFile))
									pdwQSResult = CUInt(tagVSQuerySaveResult.QSR_SaveOK)
								Case DlgQuerySaveCheckedInFile.qscifForceSaveAs
									pdwQSResult = CUInt(tagVSQuerySaveResult.QSR_ForceSaveAs)
								Case DlgQuerySaveCheckedInFile.qscifSkipSave
									pdwQSResult = CUInt(tagVSQuerySaveResult.QSR_NoSave_Continue)
								Case Else
									pdwQSResult = CUInt(tagVSQuerySaveResult.QSR_NoSave_Cancel)
							End Select

						Case SourceControlStatus.scsCheckedOut, SourceControlStatus.scsUncontrolled ' fall through
							If fileExists AndAlso isFileReadOnly Then
                                ' Make the file writable and allow the save.
								File.SetAttributes(rgpszMkDocuments(iFile), FileAttributes.Normal)
							End If
                            ' Allow the save now. 
							pdwQSResult = CUInt(tagVSQuerySaveResult.QSR_SaveOK)
					End Select
				Next iFile
			Catch e1 As Exception
                ' If an exception was caught, do not allow the save.
				pdwQSResult = CUInt(tagVSQuerySaveResult.QSR_NoSave_Cancel)
			End Try

			Return VSConstants.S_OK
		End Function

		#End Region

		'--------------------------------------------------------------------------------
		' IVsTrackProjectDocumentsEvents2 specific functions
		'--------------------------------------------------------------------------------

		Public Function OnQueryAddFiles(<InAttribute> ByVal pProject As IVsProject, <InAttribute> ByVal cFiles As Integer, <InAttribute> ByVal rgpszMkDocuments As String(), <InAttribute> ByVal rgFlags As VSQUERYADDFILEFLAGS(), <OutAttribute> ByVal pSummaryResult As VSQUERYADDFILERESULTS(), <OutAttribute> ByVal rgResults As VSQUERYADDFILERESULTS()) As Integer Implements IVsTrackProjectDocumentsEvents2.OnQueryAddFiles
			Return VSConstants.E_NOTIMPL
		End Function

		''' <summary>
		''' Implement this function to update the project scc glyphs when the items are added to the project.
        ''' If a project doesn't call GetSccGlyphs as they should do (as solution folder do), this will update correctly the glyphs when the project is controled.
		''' </summary>
		Public Function OnAfterAddFilesEx(<InAttribute> ByVal cProjects As Integer, <InAttribute> ByVal cFiles As Integer, <InAttribute> ByVal rgpProjects As IVsProject(), <InAttribute> ByVal rgFirstIndices As Integer(), <InAttribute> ByVal rgpszMkDocuments As String(), <InAttribute> ByVal rgFlags As VSADDFILEFLAGS()) As Integer Implements IVsTrackProjectDocumentsEvents2.OnAfterAddFilesEx
            ' Start by iterating through all projects calling this function.
			For iProject As Integer = 0 To cProjects - 1
				Dim sccProject As IVsSccProject2 = TryCast(rgpProjects(iProject), IVsSccProject2)
				Dim pHier As IVsHierarchy = TryCast(rgpProjects(iProject), IVsHierarchy)

                ' If the project is not controllable, or is not controlled, skip it.
				If sccProject Is Nothing OrElse (Not IsProjectControlled(pHier)) Then
					Continue For
				End If

                ' Files in this project are in rgszMkOldNames, rgszMkNewNames arrays starting with iProjectFilesStart index and ending at iNextProjecFilesStart-1.
				Dim iProjectFilesStart As Integer = rgFirstIndices(iProject)
				Dim iNextProjecFilesStart As Integer = cFiles
				If iProject < cProjects - 1 Then
					iNextProjecFilesStart = rgFirstIndices(iProject + 1)
				End If

                ' Now that we know which files belong to this project, iterate the project files.
				For iFile As Integer = iProjectFilesStart To iNextProjecFilesStart - 1
                    ' Refresh the solution explorer glyphs for all projects containing this file.
					Dim nodes As IList(Of VSITEMSELECTION) = GetControlledProjectsContainingFile(rgpszMkDocuments(iFile))
					_sccProvider.RefreshNodesGlyphs(nodes)
				Next iFile
			Next iProject

			Return VSConstants.E_NOTIMPL
		End Function

		Public Function OnQueryAddDirectories(<InAttribute> ByVal pProject As IVsProject, <InAttribute> ByVal cDirectories As Integer, <InAttribute> ByVal rgpszMkDocuments As String(), <InAttribute> ByVal rgFlags As VSQUERYADDDIRECTORYFLAGS(), <OutAttribute> ByVal pSummaryResult As VSQUERYADDDIRECTORYRESULTS(), <OutAttribute> ByVal rgResults As VSQUERYADDDIRECTORYRESULTS()) As Integer Implements IVsTrackProjectDocumentsEvents2.OnQueryAddDirectories
			Return VSConstants.E_NOTIMPL
		End Function

		Public Function OnAfterAddDirectoriesEx(<InAttribute> ByVal cProjects As Integer, <InAttribute> ByVal cDirectories As Integer, <InAttribute> ByVal rgpProjects As IVsProject(), <InAttribute> ByVal rgFirstIndices As Integer(), <InAttribute> ByVal rgpszMkDocuments As String(), <InAttribute> ByVal rgFlags As VSADDDIRECTORYFLAGS()) As Integer Implements IVsTrackProjectDocumentsEvents2.OnAfterAddDirectoriesEx
			Return VSConstants.E_NOTIMPL
		End Function

		''' <summary>
		''' Implement OnQueryRemoveFilesevent to warn the user when he's deleting controlled files.
		''' The user gets the chance to cancel the file removal.
		''' </summary>
		Public Function OnQueryRemoveFiles(<InAttribute> ByVal pProject As IVsProject, <InAttribute> ByVal cFiles As Integer, <InAttribute> ByVal rgpszMkDocuments As String(), <InAttribute> ByVal rgFlags As VSQUERYREMOVEFILEFLAGS(), <OutAttribute> ByVal pSummaryResult As VSQUERYREMOVEFILERESULTS(), <OutAttribute> ByVal rgResults As VSQUERYREMOVEFILERESULTS()) As Integer Implements IVsTrackProjectDocumentsEvents2.OnQueryRemoveFiles
			pSummaryResult(0) = VSQUERYREMOVEFILERESULTS.VSQUERYREMOVEFILERESULTS_RemoveOK
            If rgResults IsNot Nothing Then
                For iFile As Integer = 0 To cFiles - 1
                    rgResults(iFile) = VSQUERYREMOVEFILERESULTS.VSQUERYREMOVEFILERESULTS_RemoveOK
                Next iFile
            End If

			Try
				Dim sccProject As IVsSccProject2 = TryCast(pProject, IVsSccProject2)
				Dim pHier As IVsHierarchy = TryCast(pProject, IVsHierarchy)
				Dim projectName As String = Nothing
				If sccProject Is Nothing Then
                    ' This is the solution calling.
					pHier = CType(_sccProvider.GetService(GetType(SVsSolution)), IVsHierarchy)
					projectName = _sccProvider.GetSolutionFileName()
				Else
                    ' If the project doesn't support source control, it will be skipped.
                    If sccProject IsNot Nothing Then
                        projectName = _sccProvider.GetProjectFileName(sccProject)
                    End If
				End If

                If projectName IsNot Nothing Then
                    For iFile As Integer = 0 To cFiles - 1
                        Dim storage As SccProviderStorage = TryCast(_controlledProjects(pHier), SccProviderStorage)
                        If storage IsNot Nothing Then
                            Dim status As SourceControlStatus = storage.GetFileStatus(rgpszMkDocuments(iFile))
                            If status <> SourceControlStatus.scsUncontrolled Then
                                ' This is a controlled file, warn the user.
                                Dim uiShell As IVsUIShell = CType(_sccProvider.GetService(GetType(SVsUIShell)), IVsUIShell)
                                Dim clsid As Guid = Guid.Empty
                                Dim result As Integer = VSConstants.S_OK
                                Dim messageText As String = Resources.ResourceManager.GetString("TPD_DeleteControlledFile")
                                Dim messageCaption As String = Resources.ResourceManager.GetString("ProviderName")
                                If uiShell.ShowMessageBox(0, clsid, messageCaption, String.Format(CultureInfo.CurrentUICulture, messageText, rgpszMkDocuments(iFile)), String.Empty, 0, OLEMSGBUTTON.OLEMSGBUTTON_YESNO, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST, OLEMSGICON.OLEMSGICON_QUERY, 0, result) <> VSConstants.S_OK OrElse result <> CInt(Fix(DialogResult.Yes)) Then
                                    pSummaryResult(0) = VSQUERYREMOVEFILERESULTS.VSQUERYREMOVEFILERESULTS_RemoveNotOK
                                    If rgResults IsNot Nothing Then
                                        rgResults(iFile) = VSQUERYREMOVEFILERESULTS.VSQUERYREMOVEFILERESULTS_RemoveNotOK
                                    End If
                                    ' Don't spend time iterating through the rest of the files once the rename has been cancelled.
                                    Exit For
                                End If
                            End If
                        End If
                    Next iFile
                End If
			Catch e1 As Exception
				pSummaryResult(0) = VSQUERYREMOVEFILERESULTS.VSQUERYREMOVEFILERESULTS_RemoveNotOK
                If rgResults IsNot Nothing Then
                    For iFile As Integer = 0 To cFiles - 1
                        rgResults(iFile) = VSQUERYREMOVEFILERESULTS.VSQUERYREMOVEFILERESULTS_RemoveNotOK
                    Next iFile
                End If
			End Try

			Return VSConstants.S_OK
		End Function

		Public Function OnAfterRemoveFiles(<InAttribute> ByVal cProjects As Integer, <InAttribute> ByVal cFiles As Integer, <InAttribute> ByVal rgpProjects As IVsProject(), <InAttribute> ByVal rgFirstIndices As Integer(), <InAttribute> ByVal rgpszMkDocuments As String(), <InAttribute> ByVal rgFlags As VSREMOVEFILEFLAGS()) As Integer Implements IVsTrackProjectDocumentsEvents2.OnAfterRemoveFiles
            ' The file deletes are not propagated into the store.
			Return VSConstants.E_NOTIMPL
		End Function

		Public Function OnQueryRemoveDirectories(<InAttribute> ByVal pProject As IVsProject, <InAttribute> ByVal cDirectories As Integer, <InAttribute> ByVal rgpszMkDocuments As String(), <InAttribute> ByVal rgFlags As VSQUERYREMOVEDIRECTORYFLAGS(), <OutAttribute> ByVal pSummaryResult As VSQUERYREMOVEDIRECTORYRESULTS(), <OutAttribute> ByVal rgResults As VSQUERYREMOVEDIRECTORYRESULTS()) As Integer Implements IVsTrackProjectDocumentsEvents2.OnQueryRemoveDirectories
			Return VSConstants.E_NOTIMPL
		End Function

		Public Function OnAfterRemoveDirectories(<InAttribute> ByVal cProjects As Integer, <InAttribute> ByVal cDirectories As Integer, <InAttribute> ByVal rgpProjects As IVsProject(), <InAttribute> ByVal rgFirstIndices As Integer(), <InAttribute> ByVal rgpszMkDocuments As String(), <InAttribute> ByVal rgFlags As VSREMOVEDIRECTORYFLAGS()) As Integer Implements IVsTrackProjectDocumentsEvents2.OnAfterRemoveDirectories
			Return VSConstants.E_NOTIMPL
		End Function

		Public Function OnQueryRenameFiles(<InAttribute> ByVal pProject As IVsProject, <InAttribute> ByVal cFiles As Integer, <InAttribute> ByVal rgszMkOldNames As String(), <InAttribute> ByVal rgszMkNewNames As String(), <InAttribute> ByVal rgFlags As VSQUERYRENAMEFILEFLAGS(), <OutAttribute> ByVal pSummaryResult As VSQUERYRENAMEFILERESULTS(), <OutAttribute> ByVal rgResults As VSQUERYRENAMEFILERESULTS()) As Integer Implements IVsTrackProjectDocumentsEvents2.OnQueryRenameFiles
			Return VSConstants.E_NOTIMPL
		End Function

		''' <summary>
        ''' Implement OnAfterRenameFiles event to rename a file in the source control store when it gets renamed in the project.
        ''' Also, rename the store if the project itself is renamed.
		''' </summary>
		Public Function OnAfterRenameFiles(<InAttribute> ByVal cProjects As Integer, <InAttribute> ByVal cFiles As Integer, <InAttribute> ByVal rgpProjects As IVsProject(), <InAttribute> ByVal rgFirstIndices As Integer(), <InAttribute> ByVal rgszMkOldNames As String(), <InAttribute> ByVal rgszMkNewNames As String(), <InAttribute> ByVal rgFlags As VSRENAMEFILEFLAGS()) As Integer Implements IVsTrackProjectDocumentsEvents2.OnAfterRenameFiles
            ' Start by iterating through all projects calling this function.
			For iProject As Integer = 0 To cProjects - 1
				Dim sccProject As IVsSccProject2 = TryCast(rgpProjects(iProject), IVsSccProject2)
				Dim pHier As IVsHierarchy = TryCast(rgpProjects(iProject), IVsHierarchy)
				Dim projectName As String = Nothing
				If sccProject Is Nothing Then
                    ' This is the solution calling.
					pHier = CType(_sccProvider.GetService(GetType(SVsSolution)), IVsHierarchy)
					projectName = _sccProvider.GetSolutionFileName()
				Else
					If sccProject Is Nothing Then
                        ' It is a project that doesn't support source control, in which case it should be ignored.
						Continue For
					End If

					projectName = _sccProvider.GetProjectFileName(sccProject)
				End If

                ' Files in this project are in rgszMkOldNames, rgszMkNewNames arrays starting with iProjectFilesStart index and ending at iNextProjecFilesStart-1.
				Dim iProjectFilesStart As Integer = rgFirstIndices(iProject)
				Dim iNextProjecFilesStart As Integer = cFiles
				If iProject < cProjects - 1 Then
					iNextProjecFilesStart = rgFirstIndices(iProject+1)
				End If

                ' Now that we know which files belong to this project, iterate the project files.
				For iFile As Integer = iProjectFilesStart To iNextProjecFilesStart - 1
					Dim storage As SccProviderStorage = TryCast(_controlledProjects(pHier), SccProviderStorage)
                    If storage IsNot Nothing Then
                        storage.RenameFileInStorage(rgszMkOldNames(iFile), rgszMkNewNames(iFile))

                        ' And refresh the solution explorer glyphs because we affected the source control status of this file.
                        ' Note that by now, the project should already know about the new file name being part of its hierarchy.
                        Dim nodes As IList(Of VSITEMSELECTION) = GetControlledProjectsContainingFile(rgszMkNewNames(iFile))
                        _sccProvider.RefreshNodesGlyphs(nodes)
                    End If
				Next iFile
			Next iProject

			Return VSConstants.S_OK
		End Function

		Public Function OnQueryRenameDirectories(<InAttribute> ByVal pProject As IVsProject, <InAttribute> ByVal cDirs As Integer, <InAttribute> ByVal rgszMkOldNames As String(), <InAttribute> ByVal rgszMkNewNames As String(), <InAttribute> ByVal rgFlags As VSQUERYRENAMEDIRECTORYFLAGS(), <OutAttribute> ByVal pSummaryResult As VSQUERYRENAMEDIRECTORYRESULTS(), <OutAttribute> ByVal rgResults As VSQUERYRENAMEDIRECTORYRESULTS()) As Integer Implements IVsTrackProjectDocumentsEvents2.OnQueryRenameDirectories
			Return VSConstants.E_NOTIMPL
		End Function

		Public Function OnAfterRenameDirectories(<InAttribute> ByVal cProjects As Integer, <InAttribute> ByVal cDirs As Integer, <InAttribute> ByVal rgpProjects As IVsProject(), <InAttribute> ByVal rgFirstIndices As Integer(), <InAttribute> ByVal rgszMkOldNames As String(), <InAttribute> ByVal rgszMkNewNames As String(), <InAttribute> ByVal rgFlags As VSRENAMEDIRECTORYFLAGS()) As Integer Implements IVsTrackProjectDocumentsEvents2.OnAfterRenameDirectories
			Return VSConstants.E_NOTIMPL
		End Function

		Public Function OnAfterSccStatusChanged(<InAttribute> ByVal cProjects As Integer, <InAttribute> ByVal cFiles As Integer, <InAttribute> ByVal rgpProjects As IVsProject(), <InAttribute> ByVal rgFirstIndices As Integer(), <InAttribute> ByVal rgpszMkDocuments As String(), <InAttribute> ByVal rgdwSccStatus As UInteger()) As Integer Implements IVsTrackProjectDocumentsEvents2.OnAfterSccStatusChanged
			Return VSConstants.E_NOTIMPL
		End Function

		#Region "Files and Project Management Functions"

		''' <summary>
		''' Returns whether this source control provider is the active scc provider.
		''' </summary>
		Public ReadOnly Property Active() As Boolean
			Get
				Return _active
			End Get
		End Property

		''' <summary>
        ''' Variable containing the solution location in source control if the solution being loaded is controlled.
		''' </summary>
		Public WriteOnly Property LoadingControlledSolutionLocation() As String
			Set(ByVal value As String)
				_loadingControlledSolutionLocation = value
			End Set
		End Property

		''' <summary>
        ''' Checks whether the specified project or solution (pHier==null) is under source control.
		''' </summary>
		''' <returns>True if project is controlled.</returns>
		Public Function IsProjectControlled(ByVal pHier As IVsHierarchy) As Boolean
			If pHier Is Nothing Then
				' this is solution, get the solution hierarchy
				pHier = CType(_sccProvider.GetService(GetType(SVsSolution)), IVsHierarchy)
			End If

			Return _controlledProjects.ContainsKey(pHier)
		End Function

		''' <summary>
        ''' Checks whether the specified project or solution (pHier==null) is offline.
		''' </summary>
		''' <returns>True if project is offline.</returns>
		Public Function IsProjectOffline(ByVal pHier As IVsHierarchy) As Boolean
			If pHier Is Nothing Then
				' this is solution, get the solution hierarchy
				pHier = CType(_sccProvider.GetService(GetType(SVsSolution)), IVsHierarchy)
			End If

			Return _offlineProjects.ContainsKey(pHier)
		End Function

		''' <summary>
        ''' Toggle the offline status of the specified project or solution.
		''' </summary>
		''' <returns>True if project is offline.</returns>
		Public Sub ToggleOfflineStatus(ByVal pHier As IVsHierarchy)
			If pHier Is Nothing Then
                ' This is solution, get the solution hierarchy.
				pHier = CType(_sccProvider.GetService(GetType(SVsSolution)), IVsHierarchy)
			End If

			If _offlineProjects.ContainsKey(pHier) Then
				_offlineProjects.Remove(pHier)
			Else
				_offlineProjects(pHier) = True
			End If
		End Sub

		''' <summary>
        ''' Adds the specified projects and solution to source control.
		''' </summary>
		Public Sub AddProjectsToSourceControl(ByRef hashUncontrolledProjects As Hashtable, ByVal addSolutionToSourceControl As Boolean)
			' A real source control provider will ask the user for a location where the projects will be controlled
			' From the user input it should create up to 4 strings that will pass them to the projects to persist, 
			' so next time the project is open from disk, it will callback source control package, and the package
			' could use the 4 binding strings to identify the correct database location of the project files.
			For Each pHier As IVsHierarchy In hashUncontrolledProjects.Keys
				Dim sccProject2 As IVsSccProject2 = CType(pHier, IVsSccProject2)
				sccProject2.SetSccLocation("<Project Location In Database>", "<Source Control Database>", "<Local Binding Root of Project>", _sccProvider.ProviderName)

                ' Add the newly controlled projects now to the list of controlled projects in this solution.
				_controlledProjects(pHier) = Nothing
			Next pHier

            ' Also, if the solution was selected to be added to scc, write in the solution properties the controlled status.
			If addSolutionToSourceControl Then
				Dim solHier As IVsHierarchy = CType(_sccProvider.GetService(GetType(SVsSolution)), IVsHierarchy)
				_controlledProjects(solHier) = Nothing
				_sccProvider.SolutionHasDirtyProps = True
			End If

            ' Now save all the modified files.
			Dim sol As IVsSolution = CType(_sccProvider.GetService(GetType(SVsSolution)), IVsSolution)
			sol.SaveSolutionElement(CUInt(__VSSLNSAVEOPTIONS.SLNSAVEOPT_SaveIfDirty), Nothing, 0)

			' Add now the solution and project files to the source control database
            ' which in our case means creating a text file containing the list of controlled files.
			For Each pHier As IVsHierarchy In hashUncontrolledProjects.Keys
				Dim sccProject2 As IVsSccProject2 = CType(pHier, IVsSccProject2)
				Dim files As IList(Of String) = _sccProvider.GetProjectFiles(sccProject2)
                Dim storage As New SccProviderStorage(_sccProvider.GetProjectFileName(sccProject2))
				storage.AddFilesToStorage(files)
				_controlledProjects(pHier) = storage
			Next pHier

            ' If adding solution to source control, create a storage for the solution, too.
			If addSolutionToSourceControl Then
				Dim solHier As IVsHierarchy = CType(_sccProvider.GetService(GetType(SVsSolution)), IVsHierarchy)
                Dim files As New List(Of String)()
				Dim solutionFile As String = _sccProvider.GetSolutionFileName()
				files.Add(solutionFile)
                Dim storage As New SccProviderStorage(solutionFile)
				storage.AddFilesToStorage(files)
				_controlledProjects(solHier) = storage
			End If

            ' For all the projects added to source control, refresh their source control glyphs.
            Dim nodes As New List(Of VSITEMSELECTION)()
			For Each pHier As IVsHierarchy In hashUncontrolledProjects.Keys
				Dim vsItem As VSITEMSELECTION
				vsItem.itemid = VSConstants.VSITEMID_ROOT
				vsItem.pHier = pHier
				nodes.Add(vsItem)
			Next pHier

            ' Also, add the solution if necessary to the list of glyphs to refresh.
			If addSolutionToSourceControl Then
				Dim vsItem As VSITEMSELECTION
				vsItem.itemid = VSConstants.VSITEMID_ROOT
				vsItem.pHier = Nothing
				nodes.Add(vsItem)
			End If

			_sccProvider.RefreshNodesGlyphs(nodes)
		End Sub

        ' The following methods are not very efficient.
		' A good source control provider should maintain maps to identify faster to which project does a file belong
        ' and check only the status of the files in that project; or simply, query one common storage about the file status.

		''' <summary>
        ''' Returns the source control status of the specified file.
		''' </summary>
		Public Function GetFileStatus(ByVal filename As String) As SourceControlStatus
			For Each storage As SccProviderStorage In _controlledProjects.Values
                If storage IsNot Nothing Then
                    Dim status As SourceControlStatus = storage.GetFileStatus(filename)
                    If status <> SourceControlStatus.scsUncontrolled Then
                        Return status
                    End If
                End If
			Next storage

			Return SourceControlStatus.scsUncontrolled
		End Function

		''' <summary>
        ''' Adds the specified file to source control; the file must be part of a controlled project.
		''' </summary>
		Public Sub AddFileToSourceControl(ByVal file As String)
            Dim filesToAdd As New List(Of String)()
			filesToAdd.Add(file)
            ' Get all controlled projects containing this file.
			Dim nodes As IList(Of VSITEMSELECTION) = GetControlledProjectsContainingFile(file)
			For Each vsItem As VSITEMSELECTION In nodes
				Dim storage As SccProviderStorage = TryCast(_controlledProjects(vsItem.pHier), SccProviderStorage)
                If storage IsNot Nothing Then
                    storage.AddFilesToStorage(filesToAdd)
                End If
			Next vsItem
		End Sub

		''' <summary>
        ''' Checks in the specified file.
		''' </summary>
		Public Sub CheckinFile(ByVal file As String)
			' Before checking in files, make sure all in-memory edits have been commited to disk 
			' by forcing a save of the solution. Ideally, only the files to be checked in should be saved...
			Dim sol As IVsSolution = CType(_sccProvider.GetService(GetType(SVsSolution)), IVsSolution)
			If sol.SaveSolutionElement(CUInt(__VSSLNSAVEOPTIONS.SLNSAVEOPT_SaveIfDirty), Nothing, 0) <> VSConstants.S_OK Then
                ' If saving the files failed, don't continue with the checkin.
				Return
			End If

			For Each storage As SccProviderStorage In _controlledProjects.Values
                If storage IsNot Nothing Then
                    Dim status As SourceControlStatus = storage.GetFileStatus(file)
                    If status <> SourceControlStatus.scsUncontrolled Then
                        storage.CheckinFile(file)
                        Return
                    End If
                End If
			Next storage
		End Sub

		''' <summary>
        ''' Checkout the specified file from source control.
		''' </summary>
		Public Sub CheckoutFile(ByVal file As String)
			For Each storage As SccProviderStorage In _controlledProjects.Values
                If storage IsNot Nothing Then
                    Dim status As SourceControlStatus = storage.GetFileStatus(file)
                    If status <> SourceControlStatus.scsUncontrolled Then
                        storage.CheckoutFile(file)
                        Return
                    End If
                End If
			Next storage
		End Sub

		''' <summary>
        ''' Returns a list of controlled projects containing the specified file.
		''' </summary>
		Public Function GetControlledProjectsContainingFile(ByVal file As String) As IList(Of VSITEMSELECTION)
            ' Accumulate all the controlled projects that contain this file.
            Dim nodes As New List(Of VSITEMSELECTION)()

			For Each pHier As IVsHierarchy In _controlledProjects.Keys
				Dim solHier As IVsHierarchy = CType(_sccProvider.GetService(GetType(SVsSolution)), IVsHierarchy)
				If solHier Is pHier Then
                    ' This is the solution.
					If file.ToLower().CompareTo(_sccProvider.GetSolutionFileName().ToLower()) = 0 Then
						Dim vsItem As VSITEMSELECTION
						vsItem.itemid = VSConstants.VSITEMID_ROOT
						vsItem.pHier = Nothing
						nodes.Add(vsItem)
					End If
				Else
					Dim pProject As IVsProject2 = TryCast(pHier, IVsProject2)
                    ' See if the file is member of this project.
					' Caveat: the IsDocumentInProject function is expensive for certain project types, 
                    ' you may want to limit its usage by creating your own maps of file2project or folder2project.
					Dim fFound As Integer
					Dim itemid As UInteger
					Dim prio As VSDOCUMENTPRIORITY() = New VSDOCUMENTPRIORITY(0){}
                    If pProject IsNot Nothing AndAlso pProject.IsDocumentInProject(file, fFound, prio, itemid) = VSConstants.S_OK AndAlso fFound <> 0 Then
                        Dim vsItem As VSITEMSELECTION
                        vsItem.itemid = itemid
                        vsItem.pHier = pHier
                        nodes.Add(vsItem)
                    End If
				End If
			Next pHier

			Return nodes
		End Function

		''' <summary>
        ''' Checkout the file from source control and refreshes the glyphs of the files containing the file.
		''' </summary>
		Public Sub CheckoutFileAndRefreshProjectGlyphs(ByVal file As String)
            ' First, checkout the file.
			CheckoutFile(file)

            ' And refresh the solution explorer glyphs of all the projects containing this file to reflect the checked out status.
			Dim nodes As IList(Of VSITEMSELECTION) = GetControlledProjectsContainingFile(file)
			_sccProvider.RefreshNodesGlyphs(nodes)
		End Sub

		#End Region
	End Class
End Namespace