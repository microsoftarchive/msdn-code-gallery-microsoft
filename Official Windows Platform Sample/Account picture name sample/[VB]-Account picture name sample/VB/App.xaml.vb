'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.ApplicationModel
Imports Windows.ApplicationModel.Activation
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation

Namespace Global.SDKTemplate
    ''' <summary>
    ''' Provides application-specific behavior to supplement the default Application class.
    ''' </summary>
    Partial NotInheritable Class App
        Inherits Application

        Public LaunchArgs As LaunchActivatedEventArgs

        ''' <summary>
        ''' Initializes the singleton application object.  This is the first line of authored code
        ''' executed, and as such is the logical equivalent of main() or WinMain().
        ''' </summary>
        Public Sub New()
            Me.InitializeComponent()
            AddHandler Suspending, AddressOf OnSuspending
        End Sub

        Private Async Sub OnSuspending(ByVal sender As Object, ByVal args As SuspendingEventArgs)
            Dim deferral As SuspendingDeferral = args.SuspendingOperation.GetDeferral()
            Await SuspensionManager.SaveAsync()
            deferral.Complete()
        End Sub

        ''' <summary>
        ''' Invoked when the application is launched normally by the end user.  Other entry points
        ''' will be used when the application is launched to open a specific file, to display
        ''' search results, and so forth.
        ''' </summary>
        ''' <param name="args">Details about the launch request and process.</param>
        Protected Overrides Async Sub OnLaunched(ByVal args As LaunchActivatedEventArgs)
            Me.LaunchArgs = args

            Dim rootFrame As New Frame()
            If args.PreviousExecutionState = ApplicationExecutionState.Terminated Then
                ' Do an asynchronous restore
                Await SuspensionManager.RestoreAsync()
            End If
            If Window.Current.Content Is Nothing Then
                rootFrame.Navigate(GetType(MainPage))
                Window.Current.Content = rootFrame
            End If
            Window.Current.Activate()
        End Sub

        Protected Overrides Sub OnActivated(ByVal args As IActivatedEventArgs)
            ' Check to see if the app was activated via a protocol
            If args.Kind = ActivationKind.Protocol Then
                Dim protocolArgs = CType(args, ProtocolActivatedEventArgs)

                ' This app was activated via the Account picture apps section in PC Settings / Personalize / Account picture.
                ' Here you would do app-specific logic so that the user receives account picture selection UX.
                If protocolArgs.Uri.Scheme = "ms-accountpictureprovider" Then
                    ' The Content might be null if App has not yet been activated, if so first activate the main page.
                    If Window.Current.Content Is Nothing Then
                        ConstructMainPage()
                    End If
                    ' The scenario is set to 4 (Set Account Picture) explicitly if Content has already been loaded
                    MainPage.Current.NavigateToSetAccountPictureAndListen()
                End If
            End If
        End Sub

        Private Sub ConstructMainPage()
            Dim rootFrame As New Frame()

            If Window.Current.Content Is Nothing Then
                rootFrame.Navigate(GetType(MainPage))
                Window.Current.Content = rootFrame
            End If
            Window.Current.Activate()
        End Sub
    End Class
End Namespace
