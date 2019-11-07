'**************************************************************************
'
'Copyright (c) Microsoft Corporation. All rights reserved.
'THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
'ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
'IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
'PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'**************************************************************************

Imports System.Windows.Forms

''' <summary>
''' This is a simple custom control that will appear on the Windows Forms designer toolbox.
''' </summary>
Public Class MyCustomTextBox
    Inherits TextBox

    Public Sub New()
        Me.Multiline = True
        Me.Size = New System.Drawing.Size(100, 50)
    End Sub
End Class
