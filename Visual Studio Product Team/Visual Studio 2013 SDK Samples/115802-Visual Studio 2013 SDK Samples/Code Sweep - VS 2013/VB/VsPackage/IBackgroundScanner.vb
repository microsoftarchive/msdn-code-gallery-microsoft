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
Imports Microsoft.VisualStudio.Shell.Interop

Namespace Microsoft.Samples.VisualStudio.CodeSweep.VSPackage
    Friend Interface IBackgroundScanner
        Event Started As EventHandler
        Event Stopped As EventHandler

        ReadOnly Property IsRunning() As Boolean
        Sub Start(ByVal projects As IEnumerable(Of IVsProject))
        Sub RepeatLast()
        Sub StopIfRunning(ByVal blockUntilDone As Boolean)
    End Interface
End Namespace
