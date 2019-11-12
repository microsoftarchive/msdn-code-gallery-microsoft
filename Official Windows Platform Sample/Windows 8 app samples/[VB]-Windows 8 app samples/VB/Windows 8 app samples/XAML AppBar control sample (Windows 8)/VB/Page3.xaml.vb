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
Imports SDKTemplate

Partial Public NotInheritable Class Page3
    Inherits Page
    Private rootPage As MainPage = Nothing
    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        rootPage = TryCast(e.Parameter, MainPage)
        BottomAppBar.IsOpen = True
    End Sub

    Private Sub Back_Click(sender As Object, e As RoutedEventArgs)
        rootPage.Frame.GoBack()
    End Sub

    Private Sub RemoveSaveButton_Click(sender As Object, e As RoutedEventArgs)
        If Save IsNot Nothing Then
            LeftPanel.Children.Remove(Save)
        End If
    End Sub

    Private Sub AddFavoriteButton_Click(sender As Object, e As RoutedEventArgs)
        Dim favButton As New Button()
        favButton.Style = TryCast(App.Current.Resources("FavoriteAppBarButtonStyle"), Style)
        LeftPanel.Children.Insert(2, favButton)
    End Sub
End Class

