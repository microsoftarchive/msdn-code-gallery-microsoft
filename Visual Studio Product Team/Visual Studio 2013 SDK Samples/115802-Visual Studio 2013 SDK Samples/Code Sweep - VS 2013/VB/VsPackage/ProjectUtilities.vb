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
Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.VisualStudio.Shell.Interop
Imports Microsoft.VisualStudio
Imports System.Runtime.InteropServices
Imports Microsoft.Build.Evaluation
Imports System.Linq
Imports System.Globalization
Imports System.Diagnostics

Namespace Microsoft.Samples.VisualStudio.CodeSweep.VSPackage
    Module ProjectUtilities
        Private _serviceProvider As IServiceProvider

        Public Sub SetServiceProvider(ByVal provider As IServiceProvider)
            _serviceProvider = provider
        End Sub

        Public Function GetProjectsOfCurrentSelections() As IList(Of IVsProject)
            Dim results As New List(Of IVsProject)()

            Dim hr As Integer = VSConstants.S_OK
            Dim selectionMonitor As IVsMonitorSelection = TryCast(_serviceProvider.GetService(GetType(SVsShellMonitorSelection)), IVsMonitorSelection)
            If selectionMonitor Is Nothing Then
                Debug.Fail("Failed to get SVsShellMonitorSelection service.")
                Return results
            End If

            Dim hierarchyPtr As IntPtr = IntPtr.Zero
            Dim itemID As UInteger = 0
            Dim multiSelect As IVsMultiItemSelect = Nothing
            Dim containerPtr As IntPtr = IntPtr.Zero
            hr = selectionMonitor.GetCurrentSelection(hierarchyPtr, itemID, multiSelect, containerPtr)
            If IntPtr.Zero <> containerPtr Then
                Marshal.Release(containerPtr)
                containerPtr = IntPtr.Zero
            End If
            Debug.Assert(hr = VSConstants.S_OK, "GetCurrentSelection failed.")

            If itemID = VSConstants.VSITEMID.Selection Then
                Dim itemCount As UInteger = 0
                Dim fSingleHierarchy As Integer = 0
                hr = multiSelect.GetSelectionInfo(itemCount, fSingleHierarchy)
                System.Diagnostics.Debug.Assert(hr = VSConstants.S_OK, "GetSelectionInfo failed.")

                Dim items As VSITEMSELECTION() = New VSITEMSELECTION(CInt(itemCount - 1)) {}
                hr = multiSelect.GetSelectedItems(0, itemCount, items)
                System.Diagnostics.Debug.Assert(hr = VSConstants.S_OK, "GetSelectedItems failed.")

                For Each item As VSITEMSELECTION In items
                    Dim project As IVsProject = GetProjectOfItem(item.pHier, item.itemid)
                    If (Not results.Contains(project)) Then
                        results.Add(project)
                    End If
                Next item
            Else
                ' Case where no visible project is open (single file).
                If hierarchyPtr <> System.IntPtr.Zero Then
                    Dim hierarchy As IVsHierarchy = CType(Marshal.GetUniqueObjectForIUnknown(hierarchyPtr), IVsHierarchy)
                    results.Add(GetProjectOfItem(hierarchy, itemID))
                End If
            End If

            Return results
        End Function

        Private Function GetProjectOfItem(ByVal hierarchy As IVsHierarchy, ByVal itemID As UInteger) As IVsProject
            Return CType(hierarchy, IVsProject)
        End Function

        Public Function GetProjectFilePath(ByVal project As IVsProject) As String
            Dim path As String = String.Empty
            Dim hr As Integer = project.GetMkDocument(VSConstants.VSITEMID.Root, path)
            Debug.Assert(hr = VSConstants.S_OK OrElse hr = VSConstants.E_NOTIMPL, "GetMkDocument failed for project.")

            Return path
        End Function

        Public Function GetUniqueProjectNameFromFile(ByVal projectFile As String) As String
            Dim project As IVsProject = GetProjectByFileName(projectFile)

            If project IsNot Nothing Then
                Return GetUniqueUIName(project)
            End If

            Return Nothing
        End Function

        Public Function GetUniqueUIName(ByVal project As IVsProject) As String
            Dim solution As IVsSolution3 = TryCast(_serviceProvider.GetService(GetType(SVsSolution)), IVsSolution3)
            If solution Is Nothing Then
                Debug.Fail("Failed to get SVsSolution service.")
                Return Nothing
            End If

            Dim name As String = Nothing
            Dim hr As Integer = solution.GetUniqueUINameOfProject(CType(project, IVsHierarchy), name)
            Debug.Assert(hr = VSConstants.S_OK, "GetUniqueUINameOfProject failed.")
            Return name
        End Function

        Public ReadOnly Property LoadedProjects() As IEnumerable(Of IVsProject)
            Get
                Dim results As New List(Of IVsProject)
                Dim solution As IVsSolution = TryCast(_serviceProvider.GetService(GetType(SVsSolution)), IVsSolution)
                If solution Is Nothing Then
                    Debug.Fail("Failed to get SVsSolution service.")
                    Return results
                End If

                Dim enumerator As IEnumHierarchies = Nothing
                Dim guid As Guid = guid.Empty
                solution.GetProjectEnum(CUInt(__VSENUMPROJFLAGS.EPF_LOADEDINSOLUTION), guid, enumerator)
                Dim hierarchy As IVsHierarchy() = New IVsHierarchy(0) {Nothing}
                Dim fetched As UInteger = 0
                enumerator.Reset()
                Do While ((enumerator.Next(1, hierarchy, fetched) = VSConstants.S_OK) And (fetched = 1))
                    results.Add(CType(hierarchy(0), IVsProject))
                Loop
                Return results
            End Get
        End Property

        Public Function GetProjectByFileName(ByVal projectFile As String) As IVsProject
            Return LoadedProjects.FirstOrDefault(Function(p) String.Compare(projectFile, GetProjectFilePath(p), StringComparison.OrdinalIgnoreCase) = 0)
        End Function

        Public Function IsMSBuildProject(ByVal project As IVsProject) As Boolean
            Return ProjectCollection.GlobalProjectCollection.GetLoadedProjects(GetProjectFilePath(project)).Any()
        End Function
    End Module
End Namespace
