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

Namespace Microsoft.Samples.VisualStudio.IDE.ToolWindow
	''' <summary>
	''' This class is used only to expose the list of Guids used by this package.
	''' This list of guids must match the set of Guids used inside the VSCT file.
	''' </summary>
	Friend Class GuidsList
		' Now define the list of guids as public static members.
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")> _
        Public Const guidClientPkg As String = "C4AB616A-D70C-40eb-8169-9ECDBB8A0E8D"
        Public Const guidClientCmdSet As String = "F5DA9450-E980-4445-99EA-131EE710D22F"

		''' <summary>
		''' This Guid is the persistence guid for the output window.
		''' It can be found by running this sample, bringing up the output window,
		''' selecting it in the Persisted window and then looking in the Properties
		''' window.
		''' </summary>
        Public Const guidOutputWindowFrame As String = "34e76e81-ee4a-11d0-ae2e-00a0c90fffc3"
	End Class
End Namespace
