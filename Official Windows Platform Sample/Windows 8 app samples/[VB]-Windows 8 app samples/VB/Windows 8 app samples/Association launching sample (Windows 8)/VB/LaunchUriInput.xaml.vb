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

Partial Public NotInheritable Class LaunchUriInput
    Inherits Page
    ' A pointer back to the main page which is used to gain access to the input and output frames and their content.
    Private rootPage As RootPage = Nothing
    Private uriToLaunch As String = "http://www.bing.com"

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

    ' Launch a URI.
    Private Async Sub LaunchUriButton_Click(sender As Object, e As RoutedEventArgs)
        ' Create the URI to launch from a string.
        Dim uri = New Uri(uriToLaunch)

        ' Launch the URI.
        Dim success As Boolean = Await Windows.System.Launcher.LaunchUriAsync(uri)
        If success Then
            rootPage.NotifyUser("URI launched: " & uri.AbsoluteUri, NotifyType.StatusMessage)
        Else
            rootPage.NotifyUser("URI launch failed.", NotifyType.ErrorMessage)
        End If
    End Sub

    ' Launch a URI. Show a warning prompt.
    Private Async Sub LaunchUriWithWarningButton_Click(sender As Object, e As RoutedEventArgs)
        ' Create the URI to launch from a string.
        Dim uri = New Uri(uriToLaunch)

        ' Configure the warning prompt.
        Dim options = New Windows.System.LauncherOptions
        options.TreatAsUntrusted = True

        ' Launch the URI.
        Dim success As Boolean = Await Windows.System.Launcher.LaunchUriAsync(uri, options)
        If success Then
            rootPage.NotifyUser("URI launched: " & uri.AbsoluteUri, NotifyType.StatusMessage)
        Else
            rootPage.NotifyUser("URI launch failed.", NotifyType.ErrorMessage)
        End If

    End Sub
    ' Launch a URI. Show an Open With dialog that lets the user chose the handler to use.
    Private Async Sub LaunchUriOpenWithButton_Click(sender As Object, e As RoutedEventArgs)
        ' Create the URI to launch from a string.
        Dim uri = New Uri(uriToLaunch)

        ' Calulcate the position for the Open With dialog.
        ' An alternative to using the point is to set the rect of the UI element that triggered the launch.
        Dim openWithPosition As Point = GetOpenWithPosition(LaunchUriOpenWithButton)

        ' Next, configure the Open With dialog.
        Dim options = New Windows.System.LauncherOptions
        options.DisplayApplicationPicker = True
        options.UI.InvocationPoint = openWithPosition
        options.UI.PreferredPlacement = Windows.UI.Popups.Placement.Below

        ' Launch the URI.
        Dim success As Boolean = Await Windows.System.Launcher.LaunchUriAsync(uri, options)
        If success Then
            rootPage.NotifyUser("URI launched: " & uri.AbsoluteUri, NotifyType.StatusMessage)
        Else
            rootPage.NotifyUser("URI launch failed.", NotifyType.ErrorMessage)
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
