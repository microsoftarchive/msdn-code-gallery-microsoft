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

Namespace Microsoft.Samples.VisualStudio.Services
    ''' <summary>
    ''' This class is used only to expose the list of Guids used by this package.
    ''' This list of guids must match the set of Guids used inside the VSCT file.
    ''' </summary>
    Friend Class GuidsList
        ' Now define the list of guids as public static members.
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")> _
        Public Const guidClientPkg As String = "A9D87AE5-0770-41d8-80E9-DEDA292F436D"
        Public Const guidClientCmdSet As String = "993A1656-6714-4c33-9989-DEB103129C98"
    End Class
End Namespace
