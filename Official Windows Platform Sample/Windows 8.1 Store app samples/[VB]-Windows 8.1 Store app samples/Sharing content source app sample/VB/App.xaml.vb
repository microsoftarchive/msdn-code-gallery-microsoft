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
            Dim rootFrame As Frame = Nothing
            If args.PreviousExecutionState = ApplicationExecutionState.Terminated Then
                ' Do an asynchronous restore
                Await SuspensionManager.RestoreAsync()
            End If
            If Window.Current.Content Is Nothing Then
                rootFrame = New Frame()
                rootFrame.Navigate(GetType(MainPage))
                Window.Current.Content = rootFrame
            End If
            CType(rootFrame.Content, MainPage).LaunchArgs = args
            Window.Current.Activate()
        End Sub

        ''' <summary>
        ''' Invoked when the application is launched by a protocol.
        ''' </summary>
        ''' <param name="args">Container for the activation information including the Uri used to activate this app.</param>
        Protected Overrides Sub OnActivated(ByVal args As IActivatedEventArgs)
            Dim rootFrame As Frame = TryCast(Window.Current.Content, Frame)
            If rootFrame Is Nothing Then
                rootFrame = New Frame()
                rootFrame.Navigate(GetType(MainPage))
                Window.Current.Content = rootFrame
            End If

            ' Protocol activation is the only type of activation that this app handles
            If args.Kind = ActivationKind.Protocol Then
                Dim protocolArgs As ProtocolActivatedEventArgs = TryCast(args, ProtocolActivatedEventArgs)
                CType(rootFrame.Content, MainPage).LoadScenarioForProtocolActivation(protocolArgs)
            End If

            Window.Current.Activate()
        End Sub
    End Class
End Namespace
