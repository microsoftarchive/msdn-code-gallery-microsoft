'***************************** Module Header *******************************\
'Module Name:  ResizablePanel.vb
'Project:      RuntimeResizablePanel
'Copyright (c) Microsoft Corporation.

'ResizablePanel is a custom panel control. 
'This panel will be the panel which user would resize on runtime.

'This source is subject to the Microsoft Public License.
'See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
'All other rights reserved.

'THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
'EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
'WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************** Module Header *******************************\

Imports System.Windows
Imports System.Windows.Controls


Public Class ResizablePanel
    Inherits ContentControl
    Shared Sub New()
        ' This will allow us to create a Style in Generic.xaml with target type ResizablePanel.
        DefaultStyleKeyProperty.OverrideMetadata(GetType(ResizablePanel), New FrameworkPropertyMetadata(GetType(ResizablePanel)))
    End Sub
End Class

