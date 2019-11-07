' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports Windows.Foundation
Imports Windows.Foundation.Collections
Imports Windows.Graphics.Display
Imports Windows.UI.ViewManagement
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Controls.Primitives
Imports Windows.UI.Xaml.Data
Imports Windows.UI.Xaml.Input
Imports Windows.UI.Xaml.Media
Imports Windows.UI.Xaml.Navigation
Imports AssociationLaunching
Imports SDKTemplate

Partial Public NotInheritable Class LaunchFileInput
    Inherits Page
    ' A pointer back to the main page which is used to gain access to the input and output frames and their content.
    Private rootPage As RootPage = Nothing
    Private fileToLaunch As String = "Assets\Icon.Targetsize-256.png"

    Public Sub New
        InitializeComponent
    End Sub

#Region "Template-Related Code - Do not remove"
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        ' Get a pointer to our main page
        rootPage = TryCast(e.Parameter, RootPage)
    End Sub

    Protected Overrides Sub OnNavigatedFrom(e As NavigationEventArgs)
    End Sub
#End Region

    ' Launch a .png file that came with the package.
    Private Async Sub LaunchFileButton_Click(sender As Object, e As RoutedEventArgs)
        ' First, get the image file from the package's image directory.   
        Dim file = Await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync(fileToLaunch)
            ' Next, launch the file.
            Dim success As Boolean = Await Windows.System.Launcher.LaunchFileAsync(file)
            If success Then
                rootPage.NotifyUser("File launched: " & file.Name, NotifyType.StatusMessage)
            Else
                rootPage.NotifyUser("File launch failed.", NotifyType.ErrorMessage)
            End If
    End Sub

    ' Launch a .png file that came with the package. Show a warning prompt.
    Private Async Sub LaunchFileWithWarningButton_Click(sender As Object, e As RoutedEventArgs)
        ' First, get the image file from the package's image directory.   
        Dim file = Await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync(fileToLaunch)

            ' Next, configure the warning prompt.
            Dim options = New Windows.System.LauncherOptions
            options.TreatAsUntrusted = True

            ' Finally, launch the file.
            Dim success As Boolean = Await Windows.System.Launcher.LaunchFileAsync(file, options)
            If success Then
                rootPage.NotifyUser("File launched: " & file.Name, NotifyType.StatusMessage)
            Else
                rootPage.NotifyUser("File launch failed.", NotifyType.ErrorMessage)
            End If
    End Sub

    ' Launch a .png file that came with the package. Show an Open With dialog that lets the user chose the handler to use.
    Private Async Sub LaunchFileOpenWithButton_Click(sender As Object, e As RoutedEventArgs)
        ' First, get the image file from the package's image directory.   
        Dim file = Await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync(fileToLaunch)

            ' Calculate the position for the Open With dialog.
            ' An alternative to using the point is to set the rect of the UI element that triggered the launch.
            Dim openWithPosition As Point = GetOpenWithPosition(LaunchFileOpenWithButton)

            ' Next, configure the Open With dialog.
            Dim options = New Windows.System.LauncherOptions
            options.DisplayApplicationPicker = True
            options.UI.InvocationPoint = openWithPosition
            options.UI.PreferredPlacement = Windows.UI.Popups.Placement.Below

            ' Finally, launch the file.
            Dim success As Boolean = Await Windows.System.Launcher.LaunchFileAsync(file, options)
            If success Then
                rootPage.NotifyUser("File launched: " & file.Name, NotifyType.StatusMessage)
            Else
                rootPage.NotifyUser("File launch failed.", NotifyType.ErrorMessage)
            End If
    End Sub

    ' Have the user pick a file, then launch it.
    Private Async Sub PickAndLaunchFileButton_Click(sender As Object, e As RoutedEventArgs)
        ' First, get a file via the picker.
        ' To use the picker, the sample must not be snapped.
        If Windows.UI.ViewManagement.ApplicationView.Value = Windows.UI.ViewManagement.ApplicationViewState.Snapped Then
            If Not Windows.UI.ViewManagement.ApplicationView.TryUnsnap() Then
                rootPage.NotifyUser("Unable to unsnap the sample.", NotifyType.ErrorMessage)
                Exit Sub
            End If
        End If

        Dim openPicker = New Windows.Storage.Pickers.FileOpenPicker()
        openPicker.FileTypeFilter.Add("*")

        Dim file As Windows.Storage.StorageFile = Await openPicker.PickSingleFileAsync()
        If file IsNot Nothing Then
            ' Next, launch the file.
            Dim success As Boolean = Await Windows.System.Launcher.LaunchFileAsync(file)
            If success Then
                rootPage.NotifyUser("File launched: " & file.Name, NotifyType.StatusMessage)
            Else
                rootPage.NotifyUser("File launch failed.", NotifyType.ErrorMessage)
            End If
        Else
            rootPage.NotifyUser("No file was picked.", NotifyType.ErrorMessage)
        End If
    End Sub
    
    ' The Open With dialog should be displayed just under the element that triggered it.
    Private Function GetOpenWithPosition(element As FrameworkElement) As Windows.Foundation.Point
        Dim buttonTransform As Windows.UI.Xaml.Media.GeneralTransform = element.TransformToVisual(Nothing)

        Dim desiredLocation As Point = buttonTransform.TransformPoint(New Point)
        desiredLocation.Y = desiredLocation.Y + element.ActualHeight

        Return desiredLocation
    End Function
End Class
