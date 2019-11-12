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
Imports Microsoft.VisualStudio.Shell.Interop
Imports Microsoft.VisualStudio

Namespace Microsoft.Samples.VisualStudio.CodeSweep.UnitTests
    Friend Class MockSolution
        Implements IVsSolution, IVsSolution3
        Private ReadOnly _projects As New List(Of MockIVsProject)()
        Private ReadOnly _eventSinks As New List(Of IVsSolutionEvents)()

        Public Sub AddProject(ByVal project As MockIVsProject)
            _projects.Add(project)
            For Each sink As IVsSolutionEvents In _eventSinks
                If sink IsNot Nothing Then
                    sink.OnAfterOpenProject(project, 1)
                End If
            Next sink
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
                    rgbstrProjectNames(i) = _projects(i).FullPath
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
            Throw New Exception("The method or operation is not implemented.")
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
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetUniqueNameOfProject(ByVal pHierarchy As IVsHierarchy, <System.Runtime.InteropServices.Out()> ByRef pbstrUniqueName As String) As Integer Implements IVsSolution.GetUniqueNameOfProject
            Throw New Exception("The method or operation is not implemented.")
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
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function SetProperty(ByVal propid As Integer, ByVal var As Object) As Integer Implements IVsSolution.SetProperty
            Throw New Exception("The method or operation is not implemented.")
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

            pbstrUniqueName = "Unique name of " & project.FullPath
            Return VSConstants.S_OK
        End Function

        Public Function UpdateProjectFileLocationForUpgrade(ByVal pszCurrentLocation As String, ByVal pszUpgradedLocation As String) As Integer Implements IVsSolution3.UpdateProjectFileLocationForUpgrade
            Throw New Exception("The method or operation is not implemented.")
        End Function

#End Region
    End Class
End Namespace
