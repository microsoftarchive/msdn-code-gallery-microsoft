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

Namespace Microsoft.Samples.VisualStudio.SourceControlIntegration.BasicSccProvider
	Public Class GuidStrings
		Public Const GuidSccProvider As String = "5AB75EC4-FB13-42b5-9EC6-A67ABC74BB94"
		Public Const GuidSccProviderService As String = "5C543225-975C-402a-8651-F620C3711544"
		Public Const GuidSccProviderPkg As String = "035B72D3-9FB4-45c0-8588-C29DD85C56C2"
		Public Const GuidSccProviderCmdSet As String = "2ED10F5B-0DB7-433e-8768-FA75C1B3E9CB"
	End Class

	''' <summary>
	''' This class is used only to expose the list of Guids used by this package.
	''' This list of guids must match the set of Guids used inside the VSCT file.
	''' </summary>
	Friend Class GuidList
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
