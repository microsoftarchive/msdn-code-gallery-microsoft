'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports SDKTemplate
Imports System
Imports System.Threading
Imports System.Threading.Tasks
Imports Windows.Devices.Geolocation
Imports Windows.Foundation
Imports Windows.ApplicationModel.Background
Imports Windows.Storage
Imports Windows.UI.Core
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario3
    Inherits SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Private _geolocTask As IBackgroundTaskRegistration = Nothing
    Private _cts As CancellationTokenSource = Nothing

    Private Const SampleBackgroundTaskName As String = "SampleLocationBackgroundTask"
    Private Const SampleBackgroundTaskEntryPoint As String = "BackgroundTask.LocationBackgroundTask"

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached. The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        Dim backgroundAccessStatus__1 As BackgroundAccessStatus = BackgroundExecutionManager.GetAccessStatus()

        ' Loop through all background tasks to see if SampleBackgroundTaskName is already registered
        For Each cur In BackgroundTaskRegistration.AllTasks
            If cur.Value.Name = SampleBackgroundTaskName Then
                _geolocTask = cur.Value
                Exit For
            End If
        Next

        If _geolocTask IsNot Nothing Then
            ' Associate an event handler with the existing background task
            AddHandler _geolocTask.Completed, AddressOf OnCompleted

            Select Case backgroundAccessStatus__1
                Case BackgroundAccessStatus.Unspecified, BackgroundAccessStatus.Denied
                    rootPage.NotifyUser("This application must be added to the lock screen before the background task will run.", NotifyType.ErrorMessage)
                    Exit Select
                Case Else
                    rootPage.NotifyUser("Background task is already registered. Waiting for next update...", NotifyType.ErrorMessage)
                    Exit Select
            End Select

            RegisterBackgroundTaskButton.IsEnabled = False
            UnregisterBackgroundTaskButton.IsEnabled = True
        Else
            RegisterBackgroundTaskButton.IsEnabled = True
            UnregisterBackgroundTaskButton.IsEnabled = False
        End If
    End Sub

    ''' <summary>
    ''' Invoked immediately before the Page is unloaded and is no longer the current source of a parent Frame.
    ''' </summary>
    ''' <param name="e">
    ''' Event data that can be examined by overriding code. The event data is representative
    ''' of the navigation that will unload the current Page unless canceled. The
    ''' navigation can potentially be canceled by setting Cancel.
    ''' </param>
    Protected Overrides Sub OnNavigatingFrom(e As NavigatingCancelEventArgs)
        ' Just in case the original GetGeopositionAsync call is still active
        CancelGetGeoposition()

        If _geolocTask IsNot Nothing Then
            ' Remove the event handler
            RemoveHandler _geolocTask.Completed, AddressOf OnCompleted
        End If

        MyBase.OnNavigatingFrom(e)
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Register' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Async Sub RegisterBackgroundTask(sender As Object, e As RoutedEventArgs)
        ' Get permission for a background task from the user. If the user has already answered once,
        ' this does nothing and the user must manually update their preference via PC Settings.
        Dim backgroundAccessStatus__1 As BackgroundAccessStatus = Await BackgroundExecutionManager.RequestAccessAsync()

        ' Regardless of the answer, register the background task. If the user later adds this application
        ' to the lock screen, the background task will be ready to run.
        ' Create a new background task builder
        Dim geolocTaskBuilder As New BackgroundTaskBuilder()

        geolocTaskBuilder.Name = SampleBackgroundTaskName
        geolocTaskBuilder.TaskEntryPoint = SampleBackgroundTaskEntryPoint

        ' Create a new timer triggering at a 15 minute interval
        Dim trigger = New TimeTrigger(15, False)

        ' Associate the timer trigger with the background task builder
        geolocTaskBuilder.SetTrigger(trigger)

        ' Register the background task
        _geolocTask = geolocTaskBuilder.Register()

        ' Associate an event handler with the new background task
        AddHandler _geolocTask.Completed, AddressOf OnCompleted

        RegisterBackgroundTaskButton.IsEnabled = False
        UnregisterBackgroundTaskButton.IsEnabled = True

        Select Case backgroundAccessStatus__1
            Case BackgroundAccessStatus.Unspecified, BackgroundAccessStatus.Denied
                rootPage.NotifyUser("This application must be added to the lock screen before the background task will run.", NotifyType.ErrorMessage)
                Exit Select
            Case Else
                ' Ensure we have presented the location consent prompt (by asynchronously getting the current
                ' position). This must be done here because thee background task cannot display UI.
                GetGeopositionAsync()
                Exit Select
        End Select
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Unregister' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub UnregisterBackgroundTask(sender As Object, e As RoutedEventArgs)
        ' Remove the application from the lock screen
        BackgroundExecutionManager.RemoveAccess()

        ' Unregister the background task
        _geolocTask.Unregister(True)
        _geolocTask = Nothing

        rootPage.NotifyUser("Background task unregistered", NotifyType.StatusMessage)

        ScenarioOutput_Latitude.Text = "No data"
        ScenarioOutput_Longitude.Text = "No data"
        ScenarioOutput_Accuracy.Text = "No data"

        RegisterBackgroundTaskButton.IsEnabled = True
        UnregisterBackgroundTaskButton.IsEnabled = False
    End Sub

    ''' <summary>
    ''' Helper method to invoke GetGeopositionAsync.
    ''' </summary>
    Private Async Sub GetGeopositionAsync()
        Try
            ' Get cancellation token
            _cts = New CancellationTokenSource()
            Dim token As CancellationToken = _cts.Token

            ' Get a geolocator object
            Dim geolocator As New Geolocator

            rootPage.NotifyUser("Getting initial position...", NotifyType.StatusMessage)

            ' Carry out the operation
            Dim pos As Geoposition = Await geolocator.GetGeopositionAsync().AsTask(token)

            rootPage.NotifyUser("Initial position. Waiting for update...", NotifyType.StatusMessage)

            ScenarioOutput_Latitude.Text = pos.Coordinate.Latitude.ToString()
            ScenarioOutput_Longitude.Text = pos.Coordinate.Longitude.ToString()
            ScenarioOutput_Accuracy.Text = pos.Coordinate.Accuracy.ToString()
        Catch generatedExceptionName As UnauthorizedAccessException
            rootPage.NotifyUser("Disabled by user. Enable access through the settings charm to enable the background task.", NotifyType.StatusMessage)

            ScenarioOutput_Latitude.Text = "No data"
            ScenarioOutput_Longitude.Text = "No data"
            ScenarioOutput_Accuracy.Text = "No data"
        Catch generatedExceptionName As TaskCanceledException
            rootPage.NotifyUser("Initial position operation canceled. Waiting for update...", NotifyType.StatusMessage)
        Finally
            _cts = Nothing
        End Try
    End Sub

    ''' <summary>
    ''' Helper method to cancel the GetGeopositionAsync request (if any).
    ''' </summary>
    Private Sub CancelGetGeoposition()
        If _cts IsNot Nothing Then
            _cts.Cancel()
            _cts = Nothing
        End If
    End Sub

    Private Async Sub OnCompleted(sender As IBackgroundTaskRegistration, e As BackgroundTaskCompletedEventArgs)
        If sender IsNot Nothing Then
            ' Update the UI with progress reported by the background task
            Await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, Sub()
                                                                         Try
                                                                             ' If the background task threw an exception, display the exception in
                                                                             ' the error text box.
                                                                             e.CheckResult()

                                                                             ' Update the UI with the completion status of the background task
                                                                             ' The Run method of the background task sets this status. 
                                                                             Dim settings = ApplicationData.Current.LocalSettings
                                                                             If settings.Values("Status") IsNot Nothing Then
                                                                                 rootPage.NotifyUser(settings.Values("Status").ToString(), NotifyType.StatusMessage)
                                                                             End If

                                                                             ' Extract and display Latitude
                                                                             If settings.Values("Latitude") IsNot Nothing Then
                                                                                 ScenarioOutput_Latitude.Text = settings.Values("Latitude").ToString()
                                                                             Else
                                                                                 ScenarioOutput_Latitude.Text = "No data"
                                                                             End If

                                                                             ' Extract and display Longitude
                                                                             If settings.Values("Longitude") IsNot Nothing Then
                                                                                 ScenarioOutput_Longitude.Text = settings.Values("Longitude").ToString()
                                                                             Else
                                                                                 ScenarioOutput_Longitude.Text = "No data"
                                                                             End If

                                                                             ' Extract and display Accuracy
                                                                             If settings.Values("Accuracy") IsNot Nothing Then
                                                                                 ScenarioOutput_Accuracy.Text = settings.Values("Accuracy").ToString()
                                                                             Else
                                                                                 ScenarioOutput_Accuracy.Text = "No data"
                                                                             End If
                                                                         Catch ex As Exception
                                                                             ' The background task had an error
                                                                             rootPage.NotifyUser(ex.ToString(), NotifyType.ErrorMessage)
                                                                         End Try
                                                                     End Sub)
        End If
    End Sub
End Class
