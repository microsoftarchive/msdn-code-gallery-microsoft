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

Namespace Microsoft.Samples.VisualStudio.ComboBox
	Friend Class GuidList
		Public Const guidComboBoxPkgString As String = "CCE7D0F7-2597-4427-BCCE-16F1766BCBC0"
		Public Const guidComboBoxCmdSetString As String = "4BBE20F5-2329-4141-A004-69AE72FB6D73"

        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")> _
        Public Shared ReadOnly guidComboBoxPkg As New Guid(guidComboBoxPkgString)
        Public Shared ReadOnly guidComboBoxCmdSet As New Guid(guidComboBoxCmdSetString)
	End Class
End Namespace