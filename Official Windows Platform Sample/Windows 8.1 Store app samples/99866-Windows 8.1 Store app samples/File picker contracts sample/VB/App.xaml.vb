'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

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
            If args.PreviousExecutionState = ApplicationExecutionState.Terminated Then
                ' Do an asynchronous restore
                Await SuspensionManager.RestoreAsync()

            End If
            If Window.Current.Content Is Nothing Then
                Dim rootFrame = New Frame()
                rootFrame.Navigate(GetType(MainPage))
                CType(rootFrame.Content, MainPage).LaunchArgs = args
                Window.Current.Content = rootFrame
            End If
            Window.Current.Activate()
        End Sub

        Protected Overrides Sub OnFileOpenPickerActivated(ByVal args As FileOpenPickerActivatedEventArgs)
            Dim FileOpenPickerPage = New SDKTemplate.FileOpenPickerPage()
            FileOpenPickerPage.Activate(args)
        End Sub

        Protected Overrides Sub OnFileSavePickerActivated(ByVal args As FileSavePickerActivatedEventArgs)
            Dim FileSavePickerPage = New SDKTemplate.FileSavePickerPage()
            FileSavePickerPage.Activate(args)
        End Sub

        Protected Overrides Sub OnCachedFileUpdaterActivated(ByVal args As CachedFileUpdaterActivatedEventArgs)
            Dim CachedFileUpdaterPage = New SDKTemplate.CachedFileUpdaterPage()
            CachedFileUpdaterPage.Activate(args)
        End Sub
    End Class
End Namespace
