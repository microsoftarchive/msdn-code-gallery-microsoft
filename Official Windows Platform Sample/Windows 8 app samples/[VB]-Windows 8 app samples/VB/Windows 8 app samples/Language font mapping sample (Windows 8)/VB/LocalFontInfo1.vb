Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports Windows.Globalization.Fonts
Imports Windows.Graphics.Display
Imports Windows.UI.Xaml
Imports Windows.UI.Text
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Data
Imports Windows.UI.Xaml.Media

Class LocalFontInfo
    Private fontFamily As FontFamily
    Private fontWeight As FontWeight
    Private fontStyle As FontStyle
    Private fontStretch As FontStretch

    Public Sub [Set](textBlock As TextBlock)
        Me.fontFamily = textBlock.FontFamily
        Me.fontWeight = textBlock.FontWeight
        Me.fontStyle = textBlock.FontStyle
        Me.fontStretch = textBlock.FontStretch
    End Sub

    Public Sub Reset(textBlock As TextBlock)
        textBlock.FontFamily = Me.fontFamily
        textBlock.FontWeight = Me.fontWeight
        textBlock.FontStyle = Me.fontStyle
        textBlock.FontStretch = Me.fontStretch
    End Sub
End Class
