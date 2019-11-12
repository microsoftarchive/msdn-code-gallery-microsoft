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
        Protected Overrides Async Sub OnLaunched(args As LaunchActivatedEventArgs)
            If args.PreviousExecutionState = ApplicationExecutionState.Terminated Then
                ' Do an asynchronous restore

                Await SuspensionManager.RestoreAsync()
            End If
            If Window.Current.Content Is Nothing Then
                Dim rootFrame = New Frame()
                rootFrame.Navigate(GetType(MainPage))
                DirectCast(rootFrame.Content, MainPage).LaunchArgs = args
                Window.Current.Content = rootFrame
            End If
            Window.Current.Activate()
        End Sub

        Protected Overrides Sub OnFileOpenPickerActivated(args As FileOpenPickerActivatedEventArgs)
            Dim FileOpenPickerPage = New Global.SDKTemplate.FileOpenPickerPage()
            FileOpenPickerPage.Activate(args)
        End Sub

        Protected Overrides Sub OnFileSavePickerActivated(args As FileSavePickerActivatedEventArgs)
            Dim FileSavePickerPage = New Global.SDKTemplate.FileSavePickerPage()
            FileSavePickerPage.Activate(args)
        End Sub

        Protected Overrides Sub OnCachedFileUpdaterActivated(args As CachedFileUpdaterActivatedEventArgs)
            Dim CachedFileUpdaterPage = New Global.SDKTemplate.CachedFileUpdaterPage()
            CachedFileUpdaterPage.Activate(args)
        End Sub
    End Class
End Namespace
