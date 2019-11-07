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
    Friend Class MockTaskList
        Implements IVsTaskList, IVsTaskList2

        Public Class RefreshTasksArgs
            Inherits EventArgs
            Public ReadOnly Cookie As UInteger
            Public ReadOnly Provider As IVsTaskProvider

            Public Sub New(ByVal cookie As UInteger, ByVal provider As IVsTaskProvider)
                Me.Cookie = cookie
                Me.Provider = provider
            End Sub
        End Class

        Public Event OnRefreshTasks As EventHandler(Of RefreshTasksArgs)

        Public Class RegisterTaskProviderArgs
            Inherits EventArgs
            Public ReadOnly Provider As IVsTaskProvider
            Public ReadOnly Cookie As UInteger

            Public Sub New(ByVal provider As IVsTaskProvider, ByVal cookie As UInteger)
                Me.Provider = provider
                Me.Cookie = cookie
            End Sub
        End Class

        Public Event OnRegisterTaskProvider As EventHandler(Of RegisterTaskProviderArgs)

        Public Class UnregisterTaskProviderArgs
            Inherits EventArgs
            Public ReadOnly Cookie As UInteger

            Public Sub New(ByVal cookie As UInteger)
                Me.Cookie = cookie
            End Sub
        End Class

        Public Event OnUnregisterTaskProvider As EventHandler(Of UnregisterTaskProviderArgs)

        Public Class SetActiveProviderArgs
            Inherits EventArgs
            Public ReadOnly ProviderGuid As Guid

            Public Sub New(ByVal providerGuid As Guid)
                Me.ProviderGuid = providerGuid
            End Sub
        End Class

        Public Event OnSetActiveProvider As EventHandler(Of SetActiveProviderArgs)

        Private _providers As New Dictionary(Of UInteger, IVsTaskProvider)()
        Private _selection As New List(Of IVsTaskItem)()
        Private _activeProvider As IVsTaskProvider

        Public Sub SetSelected(ByVal item As IVsTaskItem, ByVal selected As Boolean)
            If (Not selected) Then
                _selection.Remove(item)
            Else
                If (Not _selection.Contains(item)) Then
                    _selection.Add(item)
                End If
            End If
        End Sub

        Public Sub Clear()
            _providers.Clear()
            _selection.Clear()
        End Sub

#Region "IVsTaskList Members"

        Public Function AutoFilter(ByVal cat As VSTASKCATEGORY) As Integer Implements IVsTaskList.AutoFilter
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function AutoFilter2(ByRef guidCustomView As Guid) As Integer Implements IVsTaskList.AutoFilter2
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function DumpOutput(ByVal dwReserved As UInteger, ByVal cat As VSTASKCATEGORY, ByVal pstmOutput As Microsoft.VisualStudio.OLE.Interop.IStream, <System.Runtime.InteropServices.Out()> ByRef pfOutputWritten As Integer) As Integer Implements IVsTaskList.DumpOutput
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function EnumTaskItems(<System.Runtime.InteropServices.Out()> ByRef ppenum As IVsEnumTaskItems) As Integer Implements IVsTaskList.EnumTaskItems
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function RefreshTasks(ByVal dwProviderCookie As UInteger) As Integer Implements IVsTaskList.RefreshTasks
            If OnRefreshTasksEvent IsNot Nothing Then
                RaiseEvent OnRefreshTasks(Me, New RefreshTasksArgs(dwProviderCookie, _providers(dwProviderCookie)))
            End If
            Return VSConstants.S_OK
        End Function

        Public Function RegisterCustomCategory(ByRef guidCat As Guid, ByVal dwSortOrder As UInteger, ByVal pCat As VSTASKCATEGORY()) As Integer Implements IVsTaskList.RegisterCustomCategory
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Private _nextCookie As UInteger = 0

        Public Function RegisterTaskProvider(ByVal pProvider As IVsTaskProvider, <System.Runtime.InteropServices.Out()> ByRef pdwProviderCookie As UInteger) As Integer Implements IVsTaskList.RegisterTaskProvider
            _nextCookie = CUInt(_nextCookie + 1)
            pdwProviderCookie = _nextCookie
            _providers.Add(pdwProviderCookie, pProvider)
            _activeProvider = pProvider

            If OnRegisterTaskProviderEvent IsNot Nothing Then
                RaiseEvent OnRegisterTaskProvider(Me, New RegisterTaskProviderArgs(pProvider, _nextCookie))
            End If
            Return VSConstants.S_OK
        End Function

        Public Function SetSilentOutputMode(ByVal fSilent As Integer) As Integer Implements IVsTaskList.SetSilentOutputMode
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function UnregisterCustomCategory(ByVal catAssigned As VSTASKCATEGORY) As Integer Implements IVsTaskList.UnregisterCustomCategory
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function UnregisterTaskProvider(ByVal dwProviderCookie As UInteger) As Integer Implements IVsTaskList.UnregisterTaskProvider
            _providers.Remove(dwProviderCookie)

            If OnUnregisterTaskProviderEvent IsNot Nothing Then
                RaiseEvent OnUnregisterTaskProvider(Me, New UnregisterTaskProviderArgs(dwProviderCookie))
            End If
            Return VSConstants.S_OK
        End Function

        Public Function UpdateProviderInfo(ByVal dwProviderCookie As UInteger) As Integer Implements IVsTaskList.UpdateProviderInfo
            Throw New Exception("The method or operation is not implemented.")
        End Function

#End Region

#Region "IVsTaskList2 Members"

        Public Function BeginTaskEdit(ByVal pItem As IVsTaskItem, ByVal iFocusField As Integer) As Integer Implements IVsTaskList2.BeginTaskEdit
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function EnumSelectedItems(<System.Runtime.InteropServices.Out()> ByRef ppEnum As IVsEnumTaskItems) As Integer Implements IVsTaskList2.EnumSelectedItems
            ppEnum = New MockTaskEnum(_selection)
            Return VSConstants.S_OK
        End Function

        Public Function GetActiveProvider(<System.Runtime.InteropServices.Out()> ByRef ppProvider As IVsTaskProvider) As Integer Implements IVsTaskList2.GetActiveProvider
            ppProvider = _activeProvider
            Return VSConstants.S_OK
        End Function

        Public Function GetCaretPos(<System.Runtime.InteropServices.Out()> ByRef ppItem As IVsTaskItem) As Integer Implements IVsTaskList2.GetCaretPos
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetSelectionCount(<System.Runtime.InteropServices.Out()> ByRef pnItems As Integer) As Integer Implements IVsTaskList2.GetSelectionCount
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function RefreshAllProviders() As Integer Implements IVsTaskList2.RefreshAllProviders
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function RefreshOrAddTasks(ByVal vsProviderCookie As UInteger, ByVal nTasks As Integer, ByVal prgTasks As IVsTaskItem()) As Integer Implements IVsTaskList2.RefreshOrAddTasks
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function RemoveTasks(ByVal vsProviderCookie As UInteger, ByVal nTasks As Integer, ByVal prgTasks As IVsTaskItem()) As Integer Implements IVsTaskList2.RemoveTasks
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function SelectItems(ByVal nItems As Integer, ByVal pItems As IVsTaskItem(), ByVal tsfSelType As UInteger, ByVal tsspScrollPos As UInteger) As Integer Implements IVsTaskList2.SelectItems
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function SetActiveProvider(ByRef rguidProvider As Guid) As Integer Implements IVsTaskList2.SetActiveProvider
            For Each provider As IVsTaskProvider3 In _providers.Values
                Dim thisGuid As Guid
                If VSConstants.S_OK = provider.GetProviderGuid(thisGuid) Then
                    If thisGuid.Equals(rguidProvider) Then
                        _activeProvider = TryCast(provider, IVsTaskProvider)
                    End If
                End If
            Next
            If OnSetActiveProviderEvent IsNot Nothing Then
                RaiseEvent OnSetActiveProvider(Me, New SetActiveProviderArgs(rguidProvider))
            End If
            Return VSConstants.S_OK
        End Function

#End Region
    End Class
End Namespace
