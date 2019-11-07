'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports System
Imports Windows.ApplicationModel
Imports Windows.ApplicationModel.Activation
Imports Windows.ApplicationModel.Search

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

        Private Async Function EnsureMainPageActivatedAsync(ByVal args As IActivatedEventArgs) As Task
            If args.PreviousExecutionState = ApplicationExecutionState.Terminated Then
                ' Do an asynchronous restore
                Await SuspensionManager.RestoreAsync()

            End If

            If Window.Current.Content Is Nothing Then
                Dim rootFrame = New Frame()
                rootFrame.Navigate(GetType(MainPage))
                Window.Current.Content = rootFrame
            End If

            Window.Current.Activate()
        End Function

        ''' <summary>
        ''' Invoked when the application is launched normally by the end user.  Other entry points
        ''' will be used when the application is launched to open a specific file, to display
        ''' search results, and so forth.
        ''' </summary>
        ''' <param name="args">Details about the launch request and process.</param>
        Protected Overrides Async Sub OnLaunched(ByVal args As LaunchActivatedEventArgs)
            Await EnsureMainPageActivatedAsync(args)
        End Sub

        ''' <summary>
        ''' This is the handler for Search activation.
        ''' </summary>
        ''' <param name="args">This is the list of arguments for search activation, including QueryText and Language</param>
        Protected Overrides Async Sub OnSearchActivated(ByVal args As SearchActivatedEventArgs)
            Await EnsureMainPageActivatedAsync(args)
            If args.QueryText = "" Then
                ' navigate to landing page.
            Else
                ' display search results.
                MainPage.Current.ProcessQueryText(args.QueryText)
            End If
        End Sub

        Private Sub OnQuerySubmitted(ByVal sender As Object, ByVal args As SearchPaneQuerySubmittedEventArgs)
            If MainPage.Current IsNot Nothing Then
                MainPage.Current.ProcessQueryText(args.QueryText)
            End If
        End Sub

        Protected Overrides Sub OnWindowCreated(ByVal args As WindowCreatedEventArgs)
            ' Register QuerySubmitted handler for the window at window creation time and only registered once
            ' so that the app can receive user queries at any time.
            AddHandler SearchPane.GetForCurrentView().QuerySubmitted, AddressOf OnQuerySubmitted
        End Sub
    End Class
End Namespace
