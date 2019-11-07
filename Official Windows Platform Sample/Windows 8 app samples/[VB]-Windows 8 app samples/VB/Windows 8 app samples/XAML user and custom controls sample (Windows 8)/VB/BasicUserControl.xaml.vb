Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports Windows.Foundation
Imports Windows.Foundation.Collections
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Controls.Primitives
Imports Windows.UI.Xaml.Data
Imports Windows.UI.Xaml.Input
Imports Windows.UI.Xaml.Media
Imports Windows.UI.Xaml.Navigation

' The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

Partial Public NotInheritable Class BasicUserControl
    Inherits UserControl
    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Private Sub ClickMeButtonClicked(sender As Object, e As RoutedEventArgs)
        OutputText.Text = String.Format("Hello {0}", NameInput.Text)
    End Sub
End Class
