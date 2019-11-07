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
Imports SDKTemplate


Partial Public Class App
    Public Sub New()
        InitializeComponent()
        AddHandler Me.Suspending, AddressOf OnSuspending
    End Sub

    Protected Async Sub OnSuspending(sender As Object, args As SuspendingEventArgs)
        Dim deferral As SuspendingDeferral = args.SuspendingOperation.GetDeferral
        Await SuspensionManager.SaveAsync
        deferral.Complete()
    End Sub

    Protected Overrides Async Sub OnLaunched(args As LaunchActivatedEventArgs)
        If args.PreviousExecutionState = ApplicationExecutionState.Terminated Then
            '     Do an asynchronous restore
            Await SuspensionManager.RestoreAsync
        End If
        Dim rootFrame = New Frame
        rootFrame.Navigate(GetType(Global.SDKTemplate.MainPage))
        Window.Current.Content = rootFrame
        Dim p As Global.SDKTemplate.MainPage = TryCast(rootFrame.Content, Global.SDKTemplate.MainPage)
        p.RootNamespace = Me.GetType.Namespace
        Window.Current.Activate()
    End Sub
End Class
