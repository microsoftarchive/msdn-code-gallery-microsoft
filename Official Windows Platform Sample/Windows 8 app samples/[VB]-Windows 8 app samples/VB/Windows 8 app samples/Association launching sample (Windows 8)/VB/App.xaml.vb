' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports System
Imports System.Threading.Tasks
Imports Windows.ApplicationModel.Activation
Imports Windows.ApplicationModel
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports AssociationLaunching
Imports SDKTemplate

Partial Public Class App
    Public Sub New()
        InitializeComponent()
        AddHandler Me.Suspending, AddressOf OnSuspending
    End Sub

    Protected Async Sub OnSuspending(sender As Object, args As SuspendingEventArgs)
        Dim deferral As SuspendingDeferral = args.SuspendingOperation.GetDeferral()
        Await SuspensionManager.SaveAsync()
        deferral.Complete()
    End Sub

    Protected Overrides Async Sub OnLaunched(args As LaunchActivatedEventArgs)
        If args.PreviousExecutionState = ApplicationExecutionState.Terminated Then
            '  Do an asynchronous restore.
            Await SuspensionManager.RestoreAsync()
        End If

        Dim rootFrame = New Frame()
        rootFrame.Navigate(GetType(rootPage))
        Window.Current.Content = rootFrame
        Dim p As rootPage = TryCast(rootFrame.Content, rootPage)
        p.RootNamespace = Me.[GetType]().[Namespace]
        p.FileEvent = Nothing
        p.ProtocolEvent = Nothing

        Window.Current.Activate()
    End Sub

    ' Handle file activations.
    Protected Overrides Sub OnFileActivated(args As FileActivatedEventArgs)
        Dim rootFrame = New Frame
        rootFrame.Navigate(GetType(rootPage))
        Window.Current.Content = rootFrame
        Dim p As rootPage = TryCast(rootFrame.Content, rootPage)
        p.RootNamespace = Me.GetType.Namespace

        ' Shuttle the event args to the scenario selector to display the proper scenario.
        p.FileEvent = args
        p.ProtocolEvent = Nothing

        Window.Current.Activate()
    End Sub

    ' Handle protocol activations.
    Protected Overrides Sub OnActivated(args As IActivatedEventArgs)
        If args.Kind = ActivationKind.Protocol Then
            Dim protocolArgs As ProtocolActivatedEventArgs = TryCast(args, ProtocolActivatedEventArgs)

            Dim rootFrame = New Frame()
            rootFrame.Navigate(GetType(rootPage))
            Window.Current.Content = rootFrame
            Dim p As rootPage = TryCast(rootFrame.Content, rootPage)
            p.RootNamespace = Me.GetType.Namespace

            ' Shuttle the event args to the scenario selector to display the proper scenario.
            p.ProtocolEvent = protocolArgs
            p.FileEvent = Nothing
        End If

        Window.Current.Activate()
    End Sub
End Class
