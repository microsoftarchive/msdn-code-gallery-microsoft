' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports SDKTemplate

Imports System
Imports System.Threading.Tasks
Imports Windows.ApplicationModel.Activation
Imports Windows.Foundation
Imports Windows.UI.Core
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Data
Imports Windows.UI.Xaml.Navigation

Partial Class ExtendedSplash
    Friend splashImageRect As Rect
    ' Rect to store splash screen image coordinates.
    Friend dismissed As Boolean = False
    ' Variable to track splash screen dismissal status.
    Private splash As SplashScreen
    ' Variable to hold the splash screen object.
    Friend rootFrame As Frame

    ''' <summary>
    ''' Constructor with splash screen information
    ''' </summary>
    Public Sub New(splashscreen As SplashScreen, loadState As Boolean)
        InitializeComponent()

        AddHandler LearnMoreButton.Click, AddressOf LearnMoreButton_Click
        ' Listen for window resize events to reposition the extended splash screen image accordingly.
        ' This is important to ensure that the extended splash screen is formatted properly in response to snapping, unsnapping, rotation, etc...
        AddHandler Window.Current.SizeChanged, AddressOf ExtendedSplash_OnResize

        splash = splashscreen

        If splash IsNot Nothing Then
            ' Register an event handler to be executed when the splash screen has been dismissed.
            AddHandler splash.Dismissed, AddressOf DismissedEventHandler

            ' Retrieve the window coordinates of the splash screen image.
            splashImageRect = splash.ImageLocation
            PositionImage()
        End If

        ' Create a Frame to act as the navigation context 
        rootFrame = New Frame()

        ' Restore the saved session state if necessary
        RestoreStateAsync(loadState)
    End Sub

    Public Async Sub RestoreStateAsync(loadState As Boolean)
        If loadState Then
            Await SuspensionManager.RestoreAsync()
        End If
        ' Normally you should start the time consuming task asynchronously here and 
        ' dismiss the extended splash screen in the completed handler of that task
        ' This sample dismisses extended splash screen  in the handler for "Learn More" button for demonstration
    End Sub

    ' Position the extended splash screen image in the same location as the system splash screen image.
    Private Sub PositionImage()
        extendedSplashImage.SetValue(Canvas.LeftProperty, splashImageRect.X)
        extendedSplashImage.SetValue(Canvas.TopProperty, splashImageRect.Y)
        extendedSplashImage.Height = splashImageRect.Height
        extendedSplashImage.Width = splashImageRect.Width
    End Sub

    Private Sub ExtendedSplash_OnResize(sender As Object, e As WindowSizeChangedEventArgs)
        ' Safely update the extended splash screen image coordinates. This function will be fired in response to snapping, unsnapping, rotation, etc...
        If splash IsNot Nothing Then
            ' Update the coordinates of the splash screen image.
            splashImageRect = splash.ImageLocation
            PositionImage()
        End If
    End Sub

    Private Sub LearnMoreButton_Click(sender As Object, e As RoutedEventArgs)
        ' Navigate to MainPage
        rootFrame.Navigate(GetType(MainPage))

        ' Set extended splash info on Main Page
        DirectCast(rootFrame.Content, MainPage).SetExtendedSplashInfo(splashImageRect, dismissed)

        ' Place the frame in the currrent window
        Window.Current.Content = rootFrame

    End Sub

    ' Include code to be executed when the system has transitioned from the splash screen to the extended splash screen (application's first view).
    Private Sub DismissedEventHandler(sender As SplashScreen, e As Object)
        dismissed = True

        ' Navigate away from the app's extended splash screen after completing setup operations here...
        ' This sample navigates away from the extended splash screen when the "Learn More" button is clicked.
    End Sub
End Class
