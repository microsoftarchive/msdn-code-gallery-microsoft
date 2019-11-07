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

Namespace Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider
	''' <summary>
	''' This class is used to expose the list of the IDs of the commands implemented
	''' by the client package. This list of IDs must match the set of IDs defined inside the
	''' VSCT file.
	''' </summary>
	Public Class CommandId
		' Define the list a set of public static members.

        ' Define the list of menus (these include toolbars).
		Public Const imnuToolWindowToolbarMenu As Integer = &H201

		Public Const icmdAddToSourceControl As Integer = &H100
		Public Const icmdCheckout As Integer = &H101
		Public Const icmdCheckin As Integer = &H102
		Public Const icmdViewToolWindow As Integer = &H103
		Public Const icmdToolWindowToolbarCommand As Integer = &H104
		Public Const icmdUseSccOffline As Integer = &H105

        ' Define the list of icons (use decimal numbers here, to match the resource IDs).
		Public Const iiconProductIcon As Integer = 400

        ' Define the list of bitmaps (use decimal numbers here, to match the resource IDs).
        Public Const ibmpToolWindowsImages As Integer = 501

        ' Glyph indexes in the bitmap used for tolwindows (ibmpToolWindowsImages).
		Public Const iconSccProviderToolWindow As Integer = 0
	End Class
End Namespace
