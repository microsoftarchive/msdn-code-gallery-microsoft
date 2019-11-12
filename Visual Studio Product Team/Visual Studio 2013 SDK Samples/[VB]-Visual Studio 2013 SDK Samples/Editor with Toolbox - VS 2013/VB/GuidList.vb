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

Namespace Microsoft.Samples.VisualStudio.IDE.EditorWithToolbox
	''' <summary>
	''' This class contains a list of GUIDs specific to this sample, 
	''' especially the package GUID and the commands group GUID. 
	''' </summary>
	Public Class GuidStrings
		Public Const GuidClientPackage As String = "74D1A709-1703-4365-9A21-42464AA1D0B1"
		Public Const GuidClientCmdSet As String = "FDE20BCD-FE56-4b9b-A8F8-3BEC9B045A1D"
		Public Const GuidEditorFactory As String = "5459FDB6-C123-428a-9661-A5029047E089"
	End Class
	''' <summary>
	''' List of the GUID objects.
	''' </summary>
	Friend Class GuidList
        Public Shared ReadOnly guidEditorCmdSet As New Guid(GuidStrings.GuidClientCmdSet)
        Public Shared ReadOnly guidEditorFactory As New Guid(GuidStrings.GuidEditorFactory)
	End Class
End Namespace