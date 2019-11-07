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
Imports Microsoft.Samples.VisualStudio.CodeSweep.Scanner

Namespace Microsoft.Samples.VisualStudio.CodeSweep.VSPackage
    Friend Interface ITaskProvider
        Inherits IVsTaskProvider, IVsTaskProvider3
        Sub AddResult(ByVal result As IScanResult, ByVal projectFile As String)
        Sub Clear()
        Sub SetAsActiveProvider()
        Sub ShowTaskList()
        ReadOnly Property IsShowingIgnoredInstances() As Boolean
    End Interface
End Namespace
