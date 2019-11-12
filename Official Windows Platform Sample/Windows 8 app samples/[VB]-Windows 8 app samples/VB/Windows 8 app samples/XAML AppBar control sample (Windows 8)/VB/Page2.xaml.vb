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

Partial Public NotInheritable Class Page2
    Inherits Page
    Private rootPage As GlobalPage = Nothing
    Private starButton As Button = Nothing
    Private rightPanel As StackPanel = Nothing

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        rootPage = TryCast(e.Parameter, GlobalPage)

        ' While our AppBar has global commands for navigation, we can demonstrate
        ' how we can add contextual AppBar command buttons that only pertain to 
        ' particular pages.  

        ' In this case, whenever we navigate to this page, we want to add a new command button
        ' to our AppBar

        ' We want to add command buttons to the right side StackPanel within the AppBar.
        rightPanel = TryCast(rootPage.FindName("RightCommands"), StackPanel)
        If rightPanel IsNot Nothing Then
            ' Create the button to add
            starButton = New Button()

            ' Hook up the custom button style so that it looks like an AppBar button
            starButton.Style = TryCast(App.Current.Resources.MergedDictionaries(0)("StarAppBarButtonStyle"), Style)

            ' Set up the Click handler for the new button
            AddHandler starButton.Click, AddressOf starButton_Click

            ' Add the button to the AppBar
            rightPanel.Children.Add(starButton)
        End If
    End Sub

    Protected Overrides Sub OnNavigatingFrom(e As NavigatingCancelEventArgs)
        If rightPanel IsNot Nothing Then
            ' Unhook the Click event handler for the button
            RemoveHandler starButton.Click, AddressOf starButton_Click

            ' Remove the button from the AppBar
            rightPanel.Children.Remove(starButton)
        End If
    End Sub

    ' This is the handler for our ApplicationBar button that is only available on this page
    Private Async Sub starButton_Click(sender As Object, e As RoutedEventArgs)
        Dim dialog As New Windows.UI.Popups.MessageDialog("You're a Superstar!")
        Await dialog.ShowAsync()
    End Sub

End Class

