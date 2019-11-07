'**************************************************************************
'
'Copyright (c) Microsoft Corporation. All rights reserved.
'THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
'ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
'IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
'PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'**************************************************************************

Imports System.Windows
Imports System.Windows.Controls


Public Enum RGBControlColor
    Red
    Green
    Blue
End Enum

'''<summary>
'''  Interaction logic for RGBControl.xaml
'''</summary>
Partial Public Class RGBControl
    Inherits System.Windows.Controls.UserControl

    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()
    End Sub

    Public Shared ColorProperty As DependencyProperty = DependencyProperty.Register("Color", GetType(RGBControlColor), GetType(RGBControl), New FrameworkPropertyMetadata(RGBControlColor.Red))

    Public Property Color As RGBControlColor
        Get
            Return MyBase.GetValue(RGBControl.ColorProperty)
        End Get
        Set(ByVal value As RGBControlColor)
            MyBase.SetValue(RGBControl.ColorProperty, value)
        End Set
    End Property

    ' Allow the tool window to create the toolbar tray.  Set its style and
    ' add it to the grid.
    Public Sub SetTray(ByVal tray As ToolBarTray)
        tray.Style = FindResource("ToolBarTrayStyle")
        grid.Children.Add(tray)
    End Sub
End Class