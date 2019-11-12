'**************************************************************************

'Copyright (c) Microsoft Corporation. All rights reserved.
'This code is licensed under the Visual Studio SDK license terms.
'THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
'ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
'IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
'PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

'**************************************************************************

' Guids.vb
' MUST match guids.h

Imports Microsoft.VisualBasic
Imports System

Namespace MyCompany.RdtEventExplorer
	Friend Class GuidsList
		Public Const guidRdtEventExplorerPkgString As String = "537A9C02-4B42-4b36-8305-4E12412B683A"
		Public Const guidRdtEventExplorerCmdSetString As String = "06FFB273-12AC-4de9-94E0-76FE35F875BE"
		Public Const guidToolWindowPersistanceString As String = "4982E2D7-FCB1-4681-A744-3A7DF55A1B4D"

        Public Shared ReadOnly guidRdtEventExplorerPkg As New Guid(guidRdtEventExplorerPkgString)
        Public Shared ReadOnly guidRdtEventExplorerCmdSet As New Guid(guidRdtEventExplorerCmdSetString)
        Public Shared ReadOnly guidToolWindowPersistance As New Guid(guidToolWindowPersistanceString)
	End Class
End Namespace