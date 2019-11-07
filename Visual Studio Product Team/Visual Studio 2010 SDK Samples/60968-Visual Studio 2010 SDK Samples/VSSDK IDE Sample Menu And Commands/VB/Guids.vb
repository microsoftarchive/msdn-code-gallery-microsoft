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

Namespace Microsoft.Samples.VisualStudio.MenuCommands
    ''' <summary>
    ''' This class is used only to expose the list of Guids used by this package.
    ''' This list of guids must match the set of Guids used inside the VSCT file.
    ''' </summary>
    Friend Class GuidsList
        ' Now define the list of guids as public static members.
        Public Const guidMenuAndCommandsPkg As String = "7CED3767-EBCF-45d2-B130-5F5A10ADBA90"
        Public Const guidMenuAndCommandsCmdSet As String = "32CE7E52-4C35-402b-9305-1965FB24F5E8"

    End Class
End Namespace
