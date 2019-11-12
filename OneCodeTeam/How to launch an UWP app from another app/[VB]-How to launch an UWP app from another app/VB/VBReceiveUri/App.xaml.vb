''' <summary>
''' Provides application-specific behavior to supplement the default Application class.
''' </summary>
NotInheritable Class App
    Inherits Application

    Protected Overrides Sub OnActivated(args As IActivatedEventArgs)
        If args.Kind = ActivationKind.Protocol Then
            Dim rootFrame = CType(Window.Current.Content, Frame)

            If rootFrame Is Nothing Then
                rootFrame = New Frame()
                Window.Current.Content = rootFrame
                AddHandler rootFrame.NavigationFailed, AddressOf OnNavigationFailed
            End If

            'because this is in (args.Kind == ActivationKind.Protocol) block, so the type of args must is ProtocolActivatedEventArgs
            'convert to type ProtocolActivatedEventArgs, and we can visit Uri property in type ProtocolActivatedEventArgs
            Dim protocolEventArgs = CType(args, ProtocolActivatedEventArgs)
            'Switch to a view by Scheme
            Select Case protocolEventArgs.Uri.Scheme
                'under case is the protocol scheme in the Package.appxmanifest
                'Navigate to target page with Uri as parameter
                Case "test-launchmainpage"
                    rootFrame.Navigate(GetType(MainPage), protocolEventArgs.Uri)
                Case "test-launchpage1"
                    rootFrame.Navigate(GetType(Page1), protocolEventArgs.Uri)
                Case Else
                    rootFrame.Navigate(GetType(MainPage), protocolEventArgs.Uri)
            End Select

            'start show UI
            Window.Current.Activate()
        End If
    End Sub

    ''' <summary>
    ''' Invoked when the application is launched normally by the end user.  Other entry points
    ''' will be used when the application is launched to open a specific file, to display
    ''' search results, and so forth.
    ''' </summary>
    ''' <param name="e">Details about the launch request and process.</param>
    Protected Overrides Sub OnLaunched(e As Windows.ApplicationModel.Activation.LaunchActivatedEventArgs)
        Dim rootFrame As Frame = TryCast(Window.Current.Content, Frame)

        ' Do not repeat app initialization when the Window already has content,
        ' just ensure that the window is active

        If rootFrame Is Nothing Then
            ' Create a Frame to act as the navigation context and navigate to the first page
            rootFrame = New Frame()

            AddHandler rootFrame.NavigationFailed, AddressOf OnNavigationFailed

            If e.PreviousExecutionState = ApplicationExecutionState.Terminated Then
                ' TODO: Load state from previously suspended application
            End If
            ' Place the frame in the current Window
            Window.Current.Content = rootFrame
        End If

        If e.PrelaunchActivated = False Then
            If rootFrame.Content Is Nothing Then
                ' When the navigation stack isn't restored navigate to the first page,
                ' configuring the new page by passing required information as a navigation
                ' parameter
                rootFrame.Navigate(GetType(MainPage), e.Arguments)
            End If

            ' Ensure the current window is active
            Window.Current.Activate()
        End If
    End Sub

    ''' <summary>
    ''' Invoked when Navigation to a certain page fails
    ''' </summary>
    ''' <param name="sender">The Frame which failed navigation</param>
    ''' <param name="e">Details about the navigation failure</param>
    Private Sub OnNavigationFailed(sender As Object, e As NavigationFailedEventArgs)
        Throw New Exception("Failed to load Page " + e.SourcePageType.FullName)
    End Sub

    ''' <summary>
    ''' Invoked when application execution is being suspended.  Application state is saved
    ''' without knowing whether the application will be terminated or resumed with the contents
    ''' of memory still intact.
    ''' </summary>
    ''' <param name="sender">The source of the suspend request.</param>
    ''' <param name="e">Details about the suspend request.</param>
    Private Sub OnSuspending(sender As Object, e As SuspendingEventArgs) Handles Me.Suspending
        Dim deferral As SuspendingDeferral = e.SuspendingOperation.GetDeferral()
        ' TODO: Save application state and stop any background activity
        deferral.Complete()
    End Sub

End Class
