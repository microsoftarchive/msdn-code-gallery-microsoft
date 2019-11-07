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
Imports Microsoft.Samples.VisualStudio.CodeSweep.Scanner

Namespace Microsoft.Samples.VisualStudio.CodeSweep.VSPackage
    Friend Interface IProjectConfigurationStore
        ReadOnly Property TermTableFiles() As ICollection(Of String)
        ReadOnly Property IgnoreInstances() As ICollection(Of BuildTask.IIgnoreInstance)
        Property RunWithBuild() As Boolean
        ReadOnly Property HasConfiguration() As Boolean
        Sub CreateDefaultConfiguration()
    End Interface
End Namespace
