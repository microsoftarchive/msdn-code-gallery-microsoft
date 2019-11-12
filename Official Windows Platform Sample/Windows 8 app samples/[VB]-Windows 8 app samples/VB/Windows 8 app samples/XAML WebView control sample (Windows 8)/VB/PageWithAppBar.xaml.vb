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

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class PageWithAppBar
    Inherits Page
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()

        ' Hook the opned and closed events on the AppBar so we know when
        ' to adjust the WebView.
        AddHandler BottomAppBar.Opened, AddressOf BottomAppBar_Opened
        AddHandler BottomAppBar.Closed, AddressOf BottomAppBar_Closed
    End Sub

    Private Sub BottomAppBar_Opened(sender As Object, e As Object)
        ' AppBar has Opened so we need to put the WebView back to its
        ' original size/location.
        Dim bottomAppBar As AppBar = TryCast(sender, AppBar)
        If bottomAppBar IsNot Nothing Then
            ' Force layout so that we can guarantee that our AppBar's
            ' actual height has height
            Me.UpdateLayout()
            ' Get the height of the AppBar
            Dim appBarHeight As Double = bottomAppBar.ActualHeight
            ' Reduce the height of the WebView to allow for the AppBar
            WebView8.Height = WebView8.ActualHeight - appBarHeight
            ' Translate the WebView in the Y direction to reclaim the space occupied by 
            ' the AppBar.  Notice that we translate it by appBarHeight / 2.0.
            ' This is because the WebView has VerticalAlignment and HorizontalAlignment
            ' of 'Stretch' and when we reduce its size it reduces its overall size
            ' from top and bottom by half the amount.
            TranslateYOpen.[To] = -appBarHeight / 2.0
            ' Run our translate animation to match the AppBar
            OpenAppBar.Begin()
        End If
    End Sub

    Private Sub BottomAppBar_Closed(sender As Object, e As Object)
        ' AppBar has closed so we need to put the WebView back to its
        ' original size/location.
        Dim bottomAppBar As AppBar = TryCast(sender, AppBar)
        If bottomAppBar IsNot Nothing Then
            ' Force layout so that we can guarantee that our AppBar's
            ' actual height has height
            Me.UpdateLayout()
            ' Get the height of the AppBar
            Dim appBarHeight As Double = bottomAppBar.ActualHeight
            ' Increase the height of the WebView to allow for the space
            ' that was occupied by the AppBar
            WebView8.Height = WebView8.ActualHeight + appBarHeight
            ' Translate the WebView in the Y direction to allow for 
            ' the AppBar.  Notice that we translate it by appBarHeight / 2.0.
            ' This is because the WebView has VerticalAlignment and HorizontalAlignment
            ' of 'Stretch' and when we reduce its size it reduces its overall size
            ' from top and bottom by half the amount.
            TranslateYOpen.[To] = appBarHeight / 2.0
            ' Run our translate animation to match the AppBar
            CloseAppBar.Begin()
        End If
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        ' Navigate to some web site
        WebView8.Navigate(New Uri("http://www.microsoft.com"))
    End Sub

    Protected Overrides Sub OnNavigatedFrom(e As NavigationEventArgs)
        ' Unhook the events
        RemoveHandler BottomAppBar.Opened, AddressOf BottomAppBar_Opened
        RemoveHandler BottomAppBar.Closed, AddressOf BottomAppBar_Closed
    End Sub

    Private Sub Home_Click(sender As Object, e As RoutedEventArgs)
        If (rootPage.Frame.CanGoBack) Then
            rootPage.Frame.GoBack()
        End If
    End Sub
End Class
