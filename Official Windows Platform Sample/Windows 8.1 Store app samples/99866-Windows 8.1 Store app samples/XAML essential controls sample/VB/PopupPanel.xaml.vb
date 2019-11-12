
Partial NotInheritable Class PopupPanel
    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub ClosePopup(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim hostPopup As Popup = TryCast(Me.Parent, Popup)
        hostPopup.IsOpen = False
    End Sub
End Class

