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
Imports System.IO
Imports Microsoft.VisualStudio.OLE.Interop
Imports Microsoft.VisualStudio.Shell.Interop
Imports Microsoft.VisualStudio

Namespace Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.UnitTests
	Friend Class MockSolution
		Implements IVsSolution, IVsSolution3, IVsHierarchy
        Private ReadOnly _projects As New List(Of MockIVsProject)()
		Private ReadOnly _eventSinks As List(Of IVsSolutionEvents) = New List(Of IVsSolutionEvents)()
		Private _solutionFile As String

		Protected Overrides Sub Finalize()
            ' Cleanup the solution and storage file from disk.
			Dim _items As List(Of String) = New List(Of String)()
			_items.Add(_solutionFile)
			_items.Add(_solutionFile & ".storage")

			For Each file As String In _items
                If System.IO.File.Exists(file) Then
                    System.IO.File.SetAttributes(file, FileAttributes.Normal)
                    System.IO.File.Delete(file)
                End If
			Next file
		End Sub

		Public Property SolutionFile() As String
			Get
				Return _solutionFile
			End Get
			Set(ByVal value As String)
				_solutionFile = value.ToLower()
			End Set
		End Property

		Public Sub AddProject(ByVal project As MockIVsProject)
            If _solutionFile IsNot Nothing Then
                _projects.Add(project)
                For Each sink As IVsSolutionEvents In _eventSinks
                    If sink IsNot Nothing Then
                        sink.OnAfterOpenProject(project, 1)
                    End If
                Next sink
            End If
		End Sub

		Public ReadOnly Property Projects() As IEnumerable(Of MockIVsProject)
			Get
				Return _projects
			End Get
		End Property

		#Region "IVsSolution Members"

		Public Function AddVirtualProject(ByVal pHierarchy As IVsHierarchy, ByVal grfAddVPFlags As UInteger) As Integer Implements IVsSolution.AddVirtualProject
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function AddVirtualProjectEx(ByVal pHierarchy As IVsHierarchy, ByVal grfAddVPFlags As UInteger, ByRef rguidProjectID As Guid) As Integer Implements IVsSolution.AddVirtualProjectEx
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function AdviseSolutionEvents(ByVal pSink As IVsSolutionEvents, <System.Runtime.InteropServices.Out()> ByRef pdwCookie As UInteger) As Integer Implements IVsSolution.AdviseSolutionEvents
			_eventSinks.Add(pSink)
			pdwCookie = CUInt(_eventSinks.Count)
			Return VSConstants.S_OK
		End Function

		Public Function CanCreateNewProjectAtLocation(ByVal fCreateNewSolution As Integer, ByVal pszFullProjectFilePath As String, <System.Runtime.InteropServices.Out()> ByRef pfCanCreate As Integer) As Integer Implements IVsSolution.CanCreateNewProjectAtLocation
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function CloseSolutionElement(ByVal grfCloseOpts As UInteger, ByVal pHier As IVsHierarchy, ByVal docCookie As UInteger) As Integer Implements IVsSolution.CloseSolutionElement
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function CreateNewProjectViaDlg(ByVal pszExpand As String, ByVal pszSelect As String, ByVal dwReserved As UInteger) As Integer Implements IVsSolution.CreateNewProjectViaDlg
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function CreateProject(ByRef rguidProjectType As Guid, ByVal lpszMoniker As String, ByVal lpszLocation As String, ByVal lpszName As String, ByVal grfCreateFlags As UInteger, ByRef iidProject As Guid, <System.Runtime.InteropServices.Out()> ByRef ppProject As IntPtr) As Integer Implements IVsSolution.CreateProject
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function CreateSolution(ByVal lpszLocation As String, ByVal lpszName As String, ByVal grfCreateFlags As UInteger) As Integer Implements IVsSolution.CreateSolution
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function GenerateNextDefaultProjectName(ByVal pszBaseName As String, ByVal pszLocation As String, <System.Runtime.InteropServices.Out()> ByRef pbstrProjectName As String) As Integer Implements IVsSolution.GenerateNextDefaultProjectName
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function GenerateUniqueProjectName(ByVal lpszRoot As String, <System.Runtime.InteropServices.Out()> ByRef pbstrProjectName As String) As Integer Implements IVsSolution.GenerateUniqueProjectName
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function GetGuidOfProject(ByVal pHierarchy As IVsHierarchy, <System.Runtime.InteropServices.Out()> ByRef pguidProjectID As Guid) As Integer Implements IVsSolution.GetGuidOfProject
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function GetItemInfoOfProjref(ByVal pszProjref As String, ByVal propid As Integer, <System.Runtime.InteropServices.Out()> ByRef pvar As Object) As Integer Implements IVsSolution.GetItemInfoOfProjref
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function GetItemOfProjref(ByVal pszProjref As String, <System.Runtime.InteropServices.Out()> ByRef ppHierarchy As IVsHierarchy, <System.Runtime.InteropServices.Out()> ByRef pitemid As UInteger, <System.Runtime.InteropServices.Out()> ByRef pbstrUpdatedProjref As String, ByVal puprUpdateReason As VSUPDATEPROJREFREASON()) As Integer Implements IVsSolution.GetItemOfProjref
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function GetProjectEnum(ByVal grfEnumFlags As UInteger, ByRef rguidEnumOnlyThisType As Guid, <System.Runtime.InteropServices.Out()> ByRef ppenum As IEnumHierarchies) As Integer Implements IVsSolution.GetProjectEnum
			ppenum = New MockEnumHierarchies(_projects)
			Return VSConstants.S_OK
		End Function

		Public Function GetProjectFactory(ByVal dwReserved As UInteger, ByVal pguidProjectType As Guid(), ByVal pszMkProject As String, <System.Runtime.InteropServices.Out()> ByRef ppProjectFactory As IVsProjectFactory) As Integer Implements IVsSolution.GetProjectFactory
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function GetProjectFilesInSolution(ByVal grfGetOpts As UInteger, ByVal cProjects As UInteger, ByVal rgbstrProjectNames As String(), <System.Runtime.InteropServices.Out()> ByRef pcProjectsFetched As UInteger) As Integer Implements IVsSolution.GetProjectFilesInSolution
			If cProjects = 0 Then
				pcProjectsFetched = CUInt(_projects.Count)
			Else
                For i As Integer = 0 To CInt(cProjects - 1)
                    rgbstrProjectNames(i) = _projects(i).ProjectFile
                Next i
				pcProjectsFetched = cProjects
			End If

			Return VSConstants.S_OK
		End Function

		Public Function GetProjectInfoOfProjref(ByVal pszProjref As String, ByVal propid As Integer, <System.Runtime.InteropServices.Out()> ByRef pvar As Object) As Integer Implements IVsSolution.GetProjectInfoOfProjref
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function GetProjectOfGuid(ByRef rguidProjectID As Guid, <System.Runtime.InteropServices.Out()> ByRef ppHierarchy As IVsHierarchy) As Integer Implements IVsSolution.GetProjectOfGuid
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function GetProjectOfProjref(ByVal pszProjref As String, <System.Runtime.InteropServices.Out()> ByRef ppHierarchy As IVsHierarchy, <System.Runtime.InteropServices.Out()> ByRef pbstrUpdatedProjref As String, ByVal puprUpdateReason As VSUPDATEPROJREFREASON()) As Integer Implements IVsSolution.GetProjectOfProjref
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function GetProjectOfUniqueName(ByVal pszUniqueName As String, <System.Runtime.InteropServices.Out()> ByRef ppHierarchy As IVsHierarchy) As Integer Implements IVsSolution.GetProjectOfUniqueName
			' Unique name of projects are in general based on the solution.
			' They can be the project name relativized to the solution's folder, or the full project path
            ' when such relativization cannot be done (e.g. project on different drive or web project, etc). 

            ' However, for our testing purpose, the full project file path was used and should be good enough.
			For iProject As Integer = 0 To _projects.Count - 1
				If pszUniqueName = _projects(iProject).ProjectFile Then
					ppHierarchy = TryCast(_projects(iProject), IVsHierarchy)
                    If ppHierarchy IsNot Nothing Then
                        Return VSConstants.S_OK
                    End If
					Exit For
				End If
			Next iProject

			ppHierarchy = Nothing
			Return VSConstants.E_FAIL
		End Function

		Public Function GetProjectTypeGuid(ByVal dwReserved As UInteger, ByVal pszMkProject As String, <System.Runtime.InteropServices.Out()> ByRef pguidProjectType As Guid) As Integer Implements IVsSolution.GetProjectTypeGuid
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function GetProjrefOfItem(ByVal pHierarchy As IVsHierarchy, ByVal itemid As UInteger, <System.Runtime.InteropServices.Out()> ByRef pbstrProjref As String) As Integer Implements IVsSolution.GetProjrefOfItem
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function GetProjrefOfProject(ByVal pHierarchy As IVsHierarchy, <System.Runtime.InteropServices.Out()> ByRef pbstrProjref As String) As Integer Implements IVsSolution.GetProjrefOfProject
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function GetProperty(ByVal propid As Integer, <System.Runtime.InteropServices.Out()> ByRef pvar As Object) As Integer Implements IVsSolution.GetProperty
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function GetSolutionInfo(<System.Runtime.InteropServices.Out()> ByRef pbstrSolutionDirectory As String, <System.Runtime.InteropServices.Out()> ByRef pbstrSolutionFile As String, <System.Runtime.InteropServices.Out()> ByRef pbstrUserOptsFile As String) As Integer Implements IVsSolution.GetSolutionInfo
            If _solutionFile IsNot Nothing AndAlso _solutionFile.Length > 0 Then
                pbstrSolutionFile = _solutionFile
                pbstrSolutionDirectory = Path.GetDirectoryName(_solutionFile)
                pbstrUserOptsFile = Path.Combine(pbstrSolutionDirectory, Path.GetFileNameWithoutExtension(_solutionFile) & ".suo")
                Return VSConstants.S_OK
            Else
                pbstrSolutionFile = Nothing
                pbstrSolutionDirectory = Nothing
                pbstrUserOptsFile = Nothing
                Return VSConstants.S_FALSE
            End If
		End Function

		Public Function GetUniqueNameOfProject(ByVal pHierarchy As IVsHierarchy, <System.Runtime.InteropServices.Out()> ByRef pbstrUniqueName As String) As Integer Implements IVsSolution.GetUniqueNameOfProject
			' Unique name of projects are in general based on the solution.
			' They can be the project name relativized to the solution's folder, or the full project path
            ' when such relativization cannot be done (e.g. project on different drive or web project, etc). 

            ' However, for our testing purpose, returning the full project file path should be good enough.
			For iProject As Integer = 0 To _projects.Count - 1
				If pHierarchy Is TryCast(_projects(iProject), IVsHierarchy) Then
					pbstrUniqueName = _projects(iProject).ProjectFile
					Return VSConstants.S_OK
				End If
			Next iProject

			pbstrUniqueName = Nothing
			Return VSConstants.E_FAIL
		End Function

		Public Function GetVirtualProjectFlags(ByVal pHierarchy As IVsHierarchy, <System.Runtime.InteropServices.Out()> ByRef pgrfAddVPFlags As UInteger) As Integer Implements IVsSolution.GetVirtualProjectFlags
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function OnAfterRenameProject(ByVal pProject As IVsProject, ByVal pszMkOldName As String, ByVal pszMkNewName As String, ByVal dwReserved As UInteger) As Integer Implements IVsSolution.OnAfterRenameProject
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function OpenSolutionFile(ByVal grfOpenOpts As UInteger, ByVal pszFilename As String) As Integer Implements IVsSolution.OpenSolutionFile
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function OpenSolutionViaDlg(ByVal pszStartDirectory As String, ByVal fDefaultToAllProjectsFilter As Integer) As Integer Implements IVsSolution.OpenSolutionViaDlg
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function QueryEditSolutionFile(<System.Runtime.InteropServices.Out()> ByRef pdwEditResult As UInteger) As Integer Implements IVsSolution.QueryEditSolutionFile
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function QueryRenameProject(ByVal pProject As IVsProject, ByVal pszMkOldName As String, ByVal pszMkNewName As String, ByVal dwReserved As UInteger, <System.Runtime.InteropServices.Out()> ByRef pfRenameCanContinue As Integer) As Integer Implements IVsSolution.QueryRenameProject
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function RemoveVirtualProject(ByVal pHierarchy As IVsHierarchy, ByVal grfRemoveVPFlags As UInteger) As Integer Implements IVsSolution.RemoveVirtualProject
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function SaveSolutionElement(ByVal grfSaveOpts As UInteger, ByVal pHier As IVsHierarchy, ByVal docCookie As UInteger) As Integer Implements IVsSolution.SaveSolutionElement
			' Report success when saving files or projects.
			Return VSConstants.S_OK
		End Function

		Public Function SetProperty(ByVal propid As Integer, ByVal var As Object) As Integer Implements IVsSolution.SetProperty
			Throw New Exception("Other properties are not supported.")
		End Function

		Public Function UnadviseSolutionEvents(ByVal dwCookie As UInteger) As Integer Implements IVsSolution.UnadviseSolutionEvents
			_eventSinks(CInt(Fix(dwCookie - 1))) = Nothing
			Return VSConstants.S_OK
		End Function

		#End Region

		#Region "IVsSolution3 Members"

		Public Function CheckForAndSaveDeferredSaveSolution(ByVal fCloseSolution As Integer, ByVal pszMessage As String, ByVal pszTitle As String, ByVal grfFlags As UInteger) As Integer Implements IVsSolution3.CheckForAndSaveDeferredSaveSolution
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function CreateNewProjectViaDlgEx(ByVal pszDlgTitle As String, ByVal pszTemplateDir As String, ByVal pszExpand As String, ByVal pszSelect As String, ByVal pszHelpTopic As String, ByVal cnpvdeFlags As UInteger, ByVal pBrowse As IVsBrowseProjectLocation) As Integer Implements IVsSolution3.CreateNewProjectViaDlgEx
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function GetUniqueUINameOfProject(ByVal pHierarchy As IVsHierarchy, <System.Runtime.InteropServices.Out()> ByRef pbstrUniqueName As String) As Integer Implements IVsSolution3.GetUniqueUINameOfProject
			Dim project As MockIVsProject = TryCast(pHierarchy, MockIVsProject)

			pbstrUniqueName = "Unique name of " & project.ProjectFile
			Return VSConstants.S_OK
		End Function

		Public Function UpdateProjectFileLocationForUpgrade(ByVal pszCurrentLocation As String, ByVal pszUpgradedLocation As String) As Integer Implements IVsSolution3.UpdateProjectFileLocationForUpgrade
			Throw New Exception("The method or operation is not implemented.")
		End Function

		#End Region

		#Region "IVsHierarchy Members"

		Public Function AdviseHierarchyEvents(ByVal pEventSink As IVsHierarchyEvents, <System.Runtime.InteropServices.Out()> ByRef pdwCookie As UInteger) As Integer Implements IVsHierarchy.AdviseHierarchyEvents
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function Close() As Integer Implements IVsHierarchy.Close
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function GetCanonicalName(ByVal itemid As UInteger, <System.Runtime.InteropServices.Out()> ByRef pbstrName As String) As Integer Implements IVsHierarchy.GetCanonicalName
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function GetGuidProperty(ByVal itemid As UInteger, ByVal propid As Integer, <System.Runtime.InteropServices.Out()> ByRef pguid As Guid) As Integer Implements IVsHierarchy.GetGuidProperty
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function GetNestedHierarchy(ByVal itemid As UInteger, ByRef iidHierarchyNested As Guid, <System.Runtime.InteropServices.Out()> ByRef ppHierarchyNested As IntPtr, <System.Runtime.InteropServices.Out()> ByRef pitemidNested As UInteger) As Integer Implements IVsHierarchy.GetNestedHierarchy
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function GetProperty(ByVal itemid As UInteger, ByVal propid As Integer, <System.Runtime.InteropServices.Out()> ByRef pvar As Object) As Integer Implements IVsHierarchy.GetProperty
			If itemid = VSConstants.VSITEMID_ROOT Then
				If propid = CInt(Fix(__VSHPROPID.VSHPROPID_FirstChild)) Then
					If _projects.Count > 0 Then
						pvar = 0
					Else
                        'unchecked
                        pvar = CInt(Fix(VSConstants.VSITEMID_NIL))
                    End If
					Return VSConstants.S_OK
				End If
			ElseIf itemid >= 0 AndAlso itemid < _projects.Count Then
				If propid = CInt(Fix(__VSHPROPID.VSHPROPID_NextSibling)) Then
					If itemid < _projects.Count - 1 Then
						pvar = CInt(Fix(itemid)) + 1
					Else
                        'unchecked
                        pvar = CInt(Fix(VSConstants.VSITEMID_NIL))
                    End If
					Return VSConstants.S_OK
				ElseIf propid = CInt(Fix(__VSHPROPID.VSHPROPID_FirstChild)) Then
                    'unchecked
                    pvar = CInt(Fix(VSConstants.VSITEMID_NIL))
                    Return VSConstants.S_OK
				End If
			End If
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function GetSite(<System.Runtime.InteropServices.Out()> ByRef ppSP As Microsoft.VisualStudio.OLE.Interop.IServiceProvider) As Integer Implements IVsHierarchy.GetSite
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function ParseCanonicalName(ByVal pszName As String, <System.Runtime.InteropServices.Out()> ByRef pitemid As UInteger) As Integer Implements IVsHierarchy.ParseCanonicalName
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function QueryClose(<System.Runtime.InteropServices.Out()> ByRef pfCanClose As Integer) As Integer Implements IVsHierarchy.QueryClose
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function SetGuidProperty(ByVal itemid As UInteger, ByVal propid As Integer, ByRef rguid As Guid) As Integer Implements IVsHierarchy.SetGuidProperty
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function SetProperty(ByVal itemid As UInteger, ByVal propid As Integer, ByVal var As Object) As Integer Implements IVsHierarchy.SetProperty
			If propid = CInt(Fix(__VSHPROPID.VSHPROPID_StateIconIndex)) Then
				Return VSConstants.S_OK
			End If

			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function SetSite(ByVal psp As Microsoft.VisualStudio.OLE.Interop.IServiceProvider) As Integer Implements IVsHierarchy.SetSite
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function UnadviseHierarchyEvents(ByVal dwCookie As UInteger) As Integer Implements IVsHierarchy.UnadviseHierarchyEvents
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function Unused0() As Integer Implements IVsHierarchy.Unused0
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function Unused1() As Integer Implements IVsHierarchy.Unused1
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function Unused2() As Integer Implements IVsHierarchy.Unused2
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function Unused3() As Integer Implements IVsHierarchy.Unused3
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function Unused4() As Integer Implements IVsHierarchy.Unused4
			Throw New Exception("The method or operation is not implemented.")
		End Function

		#End Region
	End Class
End Namespace
