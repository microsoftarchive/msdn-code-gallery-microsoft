Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports SDKTemplate
Imports Windows.Foundation
Imports Windows.Foundation.Collections
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Controls.Primitives
Imports Windows.UI.Xaml.Data
Imports Windows.UI.Xaml.Input
Imports Windows.UI.Xaml.Media
Imports Windows.UI.Xaml.Navigation

Partial Public NotInheritable Class GlobalPage
    Inherits Page
    Private rootPage As MainPage = Nothing

    Public Sub New()
        Me.InitializeComponent()
        AddHandler Back.Click, AddressOf Back_Click
    End Sub

    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        rootPage = TryCast(e.Parameter, MainPage)

        Frame1.Navigate(GetType(Page1), Me)
    End Sub

    Private Sub Back_Click(sender As Object, e As RoutedEventArgs)
        If Frame1.CanGoBack Then
            Frame1.GoBack()
        Else
            rootPage.Frame.GoBack()
        End If
    End Sub
End Class
