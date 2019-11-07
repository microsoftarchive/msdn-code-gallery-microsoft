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

Namespace Microsoft.Samples.VisualStudio.MenuCommands
    ''' <summary>
    ''' This class is used to expose the list of the IDs of the commands implemented
    ''' by this package. This list of IDs must match the set of IDs defined inside the
    ''' Buttons section of the VSCT file.
    ''' </summary>
    Friend Class PkgCmdIDList
        ' Now define the list a set of public static members.
        Public Const cmdidMyCommand As Integer = &H2001
        Public Const cmdidMyGraph As Integer = &H2002
        Public Const cmdidMyZoom As Integer = &H2003
        Public Const cmdidDynamicTxt As Integer = &H2004
        Public Const cmdidDynVisibility1 As Integer = &H2005
        Public Const cmdidDynVisibility2 As Integer = &H2006
    End Class
End Namespace
