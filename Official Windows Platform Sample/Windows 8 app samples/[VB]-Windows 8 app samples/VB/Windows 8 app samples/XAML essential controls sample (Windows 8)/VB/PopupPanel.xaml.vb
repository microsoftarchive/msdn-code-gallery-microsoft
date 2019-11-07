Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading.Tasks
Imports Windows.Foundation
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Controls.Primitives
Imports Windows.UI.Xaml.Data

Partial NotInheritable Class PopupPanel
    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub ClosePopup(sender As Object, e As RoutedEventArgs)
        Dim hostPopup As Popup = TryCast(Me.Parent, Popup)
        hostPopup.IsOpen = False
    End Sub
End Class
