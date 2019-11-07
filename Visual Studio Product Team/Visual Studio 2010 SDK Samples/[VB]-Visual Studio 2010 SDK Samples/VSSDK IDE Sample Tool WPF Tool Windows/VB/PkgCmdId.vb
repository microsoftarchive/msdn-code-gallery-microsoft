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

Namespace Microsoft.Samples.VisualStudio.IDE.ToolWindow
	''' <summary>
	''' This class is used to expose the list of the IDs of the commands implemented
	''' by the client package. This list of IDs must match the set of IDs defined inside the
	''' BUTTONS section of the CTC file.
	''' </summary>
	Friend Class PkgCmdId
		' Define the list a set of public static members.
		Public Const cmdidPersistedWindow As Integer = &H2001
		Public Const cmdidUiEventsWindow As Integer = &H2002
		Public Const cmdidRefreshWindowsList As Integer = &H2003

        ' Define the list of menus.(these include toolbars).
		Public Const IDM_MyToolbar As Integer = &H0101
	End Class
End Namespace
