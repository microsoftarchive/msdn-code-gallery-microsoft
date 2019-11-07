'**************************************************************************
'
'Copyright (c) Microsoft Corporation. All rights reserved.
'THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
'ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
'IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
'PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'**************************************************************************

Imports System.ComponentModel
Imports System.Windows.Forms

''' <summary>
''' This Windows Forms control has a custom ToolboxItem that displays a wizard.
''' To prevent the control from being installed in the toolbox, replace the
''' existing ToolboxItem attribute with &lt;ToolboxItem(False)&gt;.
''' </summary>
<ToolboxItem(GetType(MyToolboxItem))> _
Public Class MyCustomTextBoxWithPopup
    Inherits TextBox

    Public Sub New()
    End Sub
End Class
