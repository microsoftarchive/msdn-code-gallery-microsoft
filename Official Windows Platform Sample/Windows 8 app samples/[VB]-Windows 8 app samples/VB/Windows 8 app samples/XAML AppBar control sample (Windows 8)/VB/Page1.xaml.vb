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

Partial Public NotInheritable Class Page1
    Inherits Page
    Private rootPage As GlobalPage = Nothing
    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        rootPage = TryCast(e.Parameter, GlobalPage)
    End Sub

    Private Sub PageTwo_Click(sender As Object, e As RoutedEventArgs)
        Me.Frame.Navigate(GetType(Page2), rootPage)
    End Sub
End Class

