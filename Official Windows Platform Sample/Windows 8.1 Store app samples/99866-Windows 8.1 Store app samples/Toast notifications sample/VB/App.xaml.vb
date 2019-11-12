' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports System
Imports Windows.ApplicationModel.Activation
Imports Windows.ApplicationModel

Partial Public Class App
    Private mainPage As MainPage = Nothing
    Public Sub New()
        InitializeComponent()
        AddHandler Suspending, AddressOf OnSuspending
    End Sub

    Protected Async Sub OnSuspending(ByVal sender As Object, ByVal args As SuspendingEventArgs)
        Dim deferral As SuspendingDeferral = args.SuspendingOperation.GetDeferral()
        Await SuspensionManager.SaveAsync()
        deferral.Complete()
    End Sub

    Protected Overrides Async Sub OnLaunched(ByVal args As LaunchActivatedEventArgs)
        If args.PreviousExecutionState = ApplicationExecutionState.Terminated Then
            '     Do an asynchronous restore
            Await SuspensionManager.RestoreAsync()
        End If
        If mainPage Is Nothing Then
            Dim rootFrame = New Frame()
            rootFrame.Navigate(GetType(MainPage))
            Window.Current.Content = rootFrame
            mainPage = TryCast(rootFrame.Content, MainPage)
            mainPage.RootNamespace = Me.GetType().Namespace
        End If
        If args.Arguments <> "" Then
            TryCast(mainPage.ScenariosFrame.Content, ScenarioList).SelectedIndex = 4
            TryCast(mainPage.InputFrame.Content, ScenarioInput5).LaunchedFromToast(args.Arguments)
        End If

        Window.Current.Activate()
    End Sub
End Class
