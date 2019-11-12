'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.ApplicationModel.Background
Imports Windows.Devices.Sms
Imports Windows.UI.Core
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation
Imports Windows.Storage
Imports SDKTemplate

Partial Public NotInheritable Class BackgroundTask
    Inherits SDKTemplate.Common.LayoutAwarePage
    Private Const BackgroundTaskEntryPoint As String = "SmsBackgroundTask.SampleSmsBackgroundTask"
    Private Const BackgroundTaskName As String = "SampleSmsBackgroundTask"
    Private sampleDispatcher As CoreDispatcher
    Private hasDeviceAccess As Boolean = False

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()

        ' Get dispatcher for dispatching updates to the UI thread.
        sampleDispatcher = Window.Current.CoreWindow.Dispatcher

        Try
            ' Initialize state-based registration of currently registered background tasks.
            InitializeRegisteredSmsBackgroundTasks()
        Catch ex As Exception
            rootPage.NotifyUser(ex.ToString, NotifyType.ErrorMessage)

        End Try
    End Sub

    ' Update the registration status text and the button states based on the background task's
    ' registration state.
    Private Sub UpdateBackgroundTaskUIState(Registered As Boolean)
        If Registered Then
            BackgroundTaskStatus.Text = "Registered"
            RegisterBackgroundTaskButton.IsEnabled = False
            UnregisterBackgroundTaskButton.IsEnabled = True
        Else
            BackgroundTaskStatus.Text = "Unregistered"
            RegisterBackgroundTaskButton.IsEnabled = True
            UnregisterBackgroundTaskButton.IsEnabled = False
        End If
    End Sub

    ' Handle request to register the background task
    Private Async Sub RegisterBackgroundTask_Click(sender As Object, e As RoutedEventArgs)
        ' SMS is a sensitive capability and the user may be prompted for consent. If the app
        ' does not obtain permission for the package to have access to SMS before the background
        ' work item is run (perhaps after the app is suspended or terminated), the background
        ' work item will not have access to SMS and will have no way to prompt the user for consent
        ' without an active window. Here, any available SMS device is activated in order to ensure
        ' consent. Your app will likely do something with the SMS device as part of its features.
        If Not hasDeviceAccess Then
            Try
                Dim smsDevice__1 As SmsDevice = DirectCast(Await SmsDevice.GetDefaultAsync(), SmsDevice)
                rootPage.NotifyUser("Successfully connnected to SMS device with account number: " & smsDevice__1.AccountPhoneNumber, NotifyType.StatusMessage)
                hasDeviceAccess = True
            Catch ex As Exception
                rootPage.NotifyUser("Failed to find SMS device" & vbLf & ex.Message, NotifyType.ErrorMessage)
                Exit Sub
            End Try
        End If

        Try
            ' Create a new background task builder. 
            Dim taskBuilder As New BackgroundTaskBuilder()

            ' Create a new SmsReceived trigger. 
            Dim trigger As New SystemTrigger(SystemTriggerType.SmsReceived, False)

            ' Associate the SmsReceived trigger with the background task builder.
            taskBuilder.SetTrigger(trigger)

            ' Specify the background task to run when the trigger fires.
            taskBuilder.TaskEntryPoint = BackgroundTaskEntryPoint

            ' Name the background task.
            taskBuilder.Name = BackgroundTaskName

            ' Register the background task. 
            Dim taskRegistration As BackgroundTaskRegistration = taskBuilder.Register()

            ' Associate completed event handler with the new background task.
            AddHandler taskRegistration.Completed, AddressOf OnCompleted

            UpdateBackgroundTaskUIState(True)
            rootPage.NotifyUser("Registered SMS background task", NotifyType.StatusMessage)
        Catch ex As Exception
            rootPage.NotifyUser(ex.ToString, NotifyType.ErrorMessage)
        End Try
    End Sub

    ' Handle request to unregister the background task
    Private Sub UnregisterBackgroundTask_Click(sender As Object, e As RoutedEventArgs)
        ' Loop through all background tasks and unregister our background task
        For Each cur In BackgroundTaskRegistration.AllTasks
            If cur.Value.Name = BackgroundTaskName Then
                cur.Value.Unregister(True)
            End If
        Next

        UpdateBackgroundTaskUIState(False)
        rootPage.NotifyUser("Unregistered SMS background task.", NotifyType.StatusMessage)
    End Sub

    ' Initialize state based on currently registered background tasks
    Public Sub InitializeRegisteredSmsBackgroundTasks()
        Try
            '
            ' Initialize UI elements based on currently registered background tasks
            ' and associate background task completed event handler with each background task.
            '
            UpdateBackgroundTaskUIState(False)

            For Each item In BackgroundTaskRegistration.AllTasks
                Dim task As IBackgroundTaskRegistration = item.Value
                If task.Name = BackgroundTaskName Then
                    UpdateBackgroundTaskUIState(True)
                    AddHandler task.Completed, AddressOf OnCompleted
                End If
            Next
        Catch ex As Exception
            rootPage.NotifyUser(ex.ToString, NotifyType.ErrorMessage)
        End Try
    End Sub

    ' Handle background task completion event.

    Private Async Sub OnCompleted(sender As IBackgroundTaskRegistration, e As BackgroundTaskCompletedEventArgs)
        ' Update the UI with the complrtion status reported by the background task.
        ' Dispatch an anonymous task to the UI thread to do the update.
        Await sampleDispatcher.RunAsync(CoreDispatcherPriority.Normal, Sub()
                                                                           Try
                                                                               If (sender IsNot Nothing) AndAlso (e IsNot Nothing) Then
                                                                                   ' this method throws if the event is reporting an error
                                                                                   e.CheckResult()

                                                                                   ' Update the UI with the background task's completion status.
                                                                                   ' The task stores status in the application data settings indexed by the task ID.
                                                                                   Dim key = sender.TaskId.ToString()
                                                                                   Dim settings = ApplicationData.Current.LocalSettings
                                                                                   BackgroundTaskStatus.Text = settings.Values(key).ToString()
                                                                               End If
                                                                           Catch ex As Exception
                                                                               rootPage.NotifyUser(ex.ToString, NotifyType.ErrorMessage)
                                                                           End Try
                                                                       End Sub)
    End Sub


End Class
