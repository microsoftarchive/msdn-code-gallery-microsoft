' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
' 
' Copyright (c) Microsoft Corporation. All rights reserved

Imports Windows.ApplicationModel
Imports Windows.UI.Xaml.Navigation
Imports SDKTemplate

Namespace Global.SDKTemplate
    ''' <summary>
    ''' Provides application-specific behavior to supplement the default Application class.
    ''' </summary>
    Partial NotInheritable Class App
        Inherits Application

        ''' <summary>
        ''' Initializes the singleton application object.  This is the first line of authored code
        ''' executed, and as such is the logical equivalent of main() or WinMain().
        ''' </summary>
        Public Sub New()
            Me.InitializeComponent()
            AddHandler Me.Suspending, AddressOf OnSuspending
        End Sub

        Private Async Sub OnSuspending(sender As Object, args As SuspendingEventArgs)
            Dim deferral As SuspendingDeferral = args.SuspendingOperation.GetDeferral
            Await SuspensionManager.SaveAsync
            deferral.Complete()
        End Sub

        ''' <summary>
        ''' Invoked when the application is launched normally by the end user.  Other entry points
        ''' will be used when the application is launched to open a specific file, to display
        ''' search results, and so forth.
        ''' </summary>
        ''' <param name="args">Details about the launch request and process.</param>
        Protected Overrides Async Sub OnLaunched(args As LaunchActivatedEventArgs)
            If args.PreviousExecutionState = ApplicationExecutionState.Terminated Then
                ' Do an asynchronous restore
                Await SuspensionManager.RestoreAsync
            End If
            If Window.Current.Content Is Nothing Then
                Dim rootFrame = New Frame
                rootFrame.Navigate(GetType(Global.SDKTemplate.MainPage))
                Window.Current.Content = rootFrame
            End If
            Window.Current.Activate()
        End Sub
    End Class
End Namespace

