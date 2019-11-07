'**************************************************************************

'Copyright (c) Microsoft Corporation. All rights reserved.
'This code is licensed under the Visual Studio SDK license terms.
'THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
'ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
'IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
'PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

'**************************************************************************

' PkgCmdID.vb
' MUST match PkgCmdID.h

Imports Microsoft.VisualBasic
Imports System

Namespace MyCompany.RdtEventExplorer
	Friend Class PkgCmdIDList
		Public Const cmdidMyTool As UInteger = &H2001
		Public Const cmdidClearWindowsList As Integer = &H2002
		Public Const cmdidRefreshWindowsList As Integer = &H2003

        ' Define the list of menus (these include toolbars).
		Public Const IDM_MyToolbar As Integer = &H0101
	End Class
End Namespace