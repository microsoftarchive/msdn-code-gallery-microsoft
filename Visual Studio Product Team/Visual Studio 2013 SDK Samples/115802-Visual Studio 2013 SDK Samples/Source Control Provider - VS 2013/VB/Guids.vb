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
	Public Class GuidStrings
		Public Const GuidSccProvider As String = "46F2569F-73B0-4cb8-8799-99D4D55CD96D"
		Public Const GuidSccProviderService As String = "9D2BD0A0-B81F-4961-AB70-421072C07CA9"
		Public Const GuidSccProviderPkg As String = "8C902FDC-CE93-49b7-BF66-0144082555F9"
		Public Const GuidSccProviderCmdSet As String = "2362E455-8807-4b64-A5CB-E8A2A36B3BD6"
	End Class

	''' <summary>
	''' This class is used only to expose the list of Guids used by this package.
	''' This list of guids must match the set of Guids used inside the VSCT file.
	''' </summary>
	Public Class GuidList
	' Now define the list of guids as public static members.

        ' Unique ID of the source control provider; this is also used as the command UI context to show/hide the pacakge UI.
        Public Shared ReadOnly guidSccProvider As New Guid(GuidStrings.GuidSccProvider)
        ' The guid of the source control provider service (implementing IVsSccProvider interface).
        Public Shared ReadOnly guidSccProviderService As New Guid(GuidStrings.GuidSccProviderService)
        ' The guid of the source control provider package (implementing IVsPackage interface).
        Public Shared ReadOnly guidSccProviderPkg As New Guid(GuidStrings.GuidSccProviderPkg)
        ' Other guids for menus and commands.
        Public Shared ReadOnly guidSccProviderCmdSet As New Guid(GuidStrings.GuidSccProviderCmdSet)
	End Class
End Namespace
