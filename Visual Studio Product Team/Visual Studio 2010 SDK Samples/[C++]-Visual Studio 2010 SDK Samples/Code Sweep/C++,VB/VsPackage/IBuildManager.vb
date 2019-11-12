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
Imports Microsoft.Build.Construction

Namespace Microsoft.Samples.VisualStudio.CodeSweep.VSPackage
    Friend Delegate Sub EmptyEvent()

    Friend Interface IBuildManager
        Event BuildStarted As EmptyEvent
        Event BuildStopped As EmptyEvent

        Property IsListeningToBuildEvents() As Boolean
        Function GetBuildTask(ByVal project As IVsProject, ByVal createIfNecessary As Boolean) As ProjectTaskElement
        Function AllItemsInProject(ByVal project As IVsProject) As ICollection(Of String)
        Sub SetProperty(ByVal project As IVsProject, ByVal name As String, ByVal value As String)
        Function GetProperty(ByVal project As IVsProject, ByVal name As String) As String
        Sub CreatePerUserFilesAsNecessary()
    End Interface
End Namespace
