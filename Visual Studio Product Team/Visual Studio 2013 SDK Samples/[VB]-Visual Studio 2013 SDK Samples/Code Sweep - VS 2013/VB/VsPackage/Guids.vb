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

Namespace Microsoft.Samples.VisualStudio.CodeSweep.VSPackage

    ' MUST match VSPackage.vsct

    Module GuidStrings
        Public Const GuidVSPackagePkg As String = "32E9CE10-DFC9-4b7d-B22A-396DC0B8F6E3"
        Public Const GuidVSPackageCmdSet As String = "4D0C001B-DA7A-4db9-BA55-C76CB3105EEA"
    End Module

    Module GuidList
        Public ReadOnly guidVSPackagePkg As New Guid(GuidStrings.GuidVSPackagePkg)
        Public ReadOnly guidVSPackageCmdSet As New Guid(GuidStrings.GuidVSPackageCmdSet)
    End Module
End Namespace