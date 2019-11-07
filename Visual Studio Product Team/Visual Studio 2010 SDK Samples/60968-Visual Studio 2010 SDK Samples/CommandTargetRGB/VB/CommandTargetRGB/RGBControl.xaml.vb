Imports System.Security.Permissions
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Media
Imports System.Windows.Media.Imaging


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

    Private tray As ToolBarTray = Nothing

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
    Public Sub SetTray(ByVal t As ToolBarTray)
        tray = t
        tray.Style = FindResource("ToolBarTrayStyle")
        grid.Children.Add(tray)
    End Sub
End Class