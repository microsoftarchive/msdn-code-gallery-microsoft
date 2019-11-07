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
Imports System.Runtime.InteropServices
Imports Microsoft.Build.Evaluation
Imports System.Linq
Imports System.Globalization

Namespace Microsoft.Samples.VisualStudio.CodeSweep.VSPackage
    Friend Class ProjectUtilities
        Private Shared _serviceProvider As IServiceProvider

        Public Shared Sub SetServiceProvider(ByVal provider As IServiceProvider)
            _serviceProvider = provider
        End Sub

        Public Shared Function GetProjectsOfCurrentSelections() As IList(Of IVsProject)
            Dim results As New List(Of IVsProject)()

            Dim hr As Integer = VSConstants.S_OK
            Dim selectionMonitor As IVsMonitorSelection = TryCast(_serviceProvider.GetService(GetType(IVsMonitorSelection)), IVsMonitorSelection)
            Dim hierarchyPtr As IntPtr = IntPtr.Zero
            Dim itemID As UInteger = 0
            Dim multiSelect As IVsMultiItemSelect = Nothing
            Dim containerPtr As IntPtr = IntPtr.Zero
            hr = selectionMonitor.GetCurrentSelection(hierarchyPtr, itemID, multiSelect, containerPtr)
            If IntPtr.Zero <> containerPtr Then
                Marshal.Release(containerPtr)
                containerPtr = IntPtr.Zero
            End If
            System.Diagnostics.Debug.Assert(hr = VSConstants.S_OK, "GetCurrentSelection failed.")

            If itemID = HierarchyConstants.VSITEMID_SELECTION Then
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

        Private Shared Function GetProjectOfItem(ByVal hierarchy As IVsHierarchy, ByVal itemID As UInteger) As IVsProject
            Return CType(hierarchy, IVsProject)
        End Function

        Public Shared Function GetProjectFilePath(ByVal project As IVsProject) As String
            Dim path As String = String.Empty
            Dim hr As Integer = project.GetMkDocument(HierarchyConstants.VSITEMID_ROOT, path)
            System.Diagnostics.Debug.Assert(hr = VSConstants.S_OK OrElse hr = VSConstants.E_NOTIMPL, "GetMkDocument failed for project.")

            Return path
        End Function

        Public Shared Function GetUniqueProjectNameFromFile(ByVal projectFile As String) As String
            Dim project As IVsProject = GetProjectByFileName(projectFile)

            If project IsNot Nothing Then
                Return GetUniqueUIName(project)
            End If

            Return Nothing
        End Function

        Public Shared Function GetUniqueUIName(ByVal project As IVsProject) As String
            Dim solution As IVsSolution3 = TryCast(_serviceProvider.GetService(GetType(SVsSolution)), IVsSolution3)
            Dim name As String = Nothing
            Dim hr As Integer = solution.GetUniqueUINameOfProject(CType(project, IVsHierarchy), name)
            System.Diagnostics.Debug.Assert(hr = VSConstants.S_OK, "GetUniqueUINameOfProject failed.")
            Return name
        End Function

        Public Shared ReadOnly Property LoadedProjects() As IEnumerable(Of IVsProject)
            Get
                Dim results As New List(Of IVsProject)
                Dim solution As IVsSolution = TryCast(_serviceProvider.GetService(GetType(SVsSolution)), IVsSolution)
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

        Public Shared Function GetProjectByFileName(ByVal projectFile As String) As IVsProject
            For Each project As IVsProject In LoadedProjects
                If String.Compare(projectFile, GetProjectFilePath(project), StringComparison.OrdinalIgnoreCase) = 0 Then
                    Return project
                End If
            Next project

            Return Nothing
        End Function

        Public Shared Function IsMSBuildProject(ByVal project As IVsProject) As Boolean
            Return ProjectCollection.GlobalProjectCollection.GetLoadedProjects(GetProjectFilePath(project)).FirstOrDefault() IsNot Nothing
        End Function
    End Class
End Namespace
