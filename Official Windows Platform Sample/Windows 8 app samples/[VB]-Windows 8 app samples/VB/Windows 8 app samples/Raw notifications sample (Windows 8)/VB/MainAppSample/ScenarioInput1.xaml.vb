' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports System
Imports System.Linq
Imports System.Collections.Generic
Imports Windows.ApplicationModel.Background
Imports Windows.Foundation
Imports Windows.Foundation.Collections
Imports Windows.Graphics.Display
Imports Windows.Networking.PushNotifications
Imports Windows.Storage
Imports Windows.UI.Core
Imports Windows.UI.ViewManagement
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Controls.Primitives
Imports Windows.UI.Xaml.Data
Imports Windows.UI.Xaml.Input
Imports Windows.UI.Xaml.Media
Imports Windows.UI.Xaml.Navigation
Imports SDKTemplate

Partial Public NotInheritable Class ScenarioInput1
    Inherits Page
    ' A pointer back to the main page which is used to gain access to the input and output frames and their content
    Private rootPage As MainPage = Nothing
    Private Const SAMPLE_TASK_NAME As String = "SampleBackgroundTask"
    Private Const SAMPLE_TASK_ENTRY_POINT As String = "BackgroundTasks.SampleBackgroundTask"
    Private _dispatcher As CoreDispatcher

    Public Sub New()
        InitializeComponent()
        _dispatcher = Window.Current.Dispatcher
    End Sub

#Region "Template-Related Code - Do not remove"
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        ' Get a pointer to our main page
        rootPage = TryCast(e.Parameter, MainPage)

        ' We want to be notified with the OutputFrame is loaded so we can get to the content
        AddHandler rootPage.OutputFrameLoaded, AddressOf rootPage_OutputFrameLoaded
    End Sub

    Protected Overrides Sub OnNavigatedFrom(e As NavigationEventArgs)
        RemoveHandler rootPage.OutputFrameLoaded, AddressOf rootPage_OutputFrameLoaded
    End Sub

#End Region

    Private Sub rootPage_OutputFrameLoaded(sender As Object, e As Object)
        ' At this point, we know that the Output Frame has been loaded and we can go ahead
        ' and reference elements in the page contained in the Output Frame

        ' Get a pointer to the content within the OutputFrame
        Dim outputFrame As Page = DirectCast(rootPage.OutputFrame.Content, Page)

        If rootPage.Channel IsNot Nothing Then
            OutputToTextBox(rootPage.Channel.Uri)
        End If
    End Sub

    Private Sub OutputToTextBox(text As String)
        ' Find the output text box on the page
        Dim outputFrame As Page = DirectCast(rootPage.OutputFrame.Content, Page)
        Dim textBox As TextBox = DirectCast(outputFrame.FindName("Scenario1ChannelOutput"), TextBox)

        ' Write text
        textBox.Text = text
    End Sub

    Private Async Sub Scenario1Open_Click(sender As Object, e As RoutedEventArgs)
        ' Applications must have lock screen privileges in order to receive raw notifications
        Dim backgroundStatus As BackgroundAccessStatus = Await BackgroundExecutionManager.RequestAccessAsync()

        ' Make sure the user allowed privileges
        If backgroundStatus <> BackgroundAccessStatus.Denied AndAlso backgroundStatus <> BackgroundAccessStatus.Unspecified Then
            OpenChannelAndRegisterTask()
        Else
            ' This event comes back in a background thread, so we need to move to the UI thread to access any UI elements
            Await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, Sub()
                                                                          rootPage.NotifyUser("Lock screen access is denied", NotifyType.ErrorMessage)
                                                                      End Sub)
        End If
    End Sub

    Private Sub Scenario1Unregister_Click(sender As Object, e As RoutedEventArgs)
        If UnregisterBackgroundTask() Then
            rootPage.NotifyUser("Task unregistered", NotifyType.StatusMessage)
        Else
            rootPage.NotifyUser("No task is registered", NotifyType.ErrorMessage)
        End If
    End Sub

    Private Async Sub OpenChannelAndRegisterTask()
        ' Open the channel. See the "Push and Polling Notifications" sample for more detail
        Try
            If rootPage.Channel Is Nothing Then
                Dim channel As PushNotificationChannel = Await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync()
                Dim uri As String = channel.Uri
                rootPage.Channel = channel
                ' This event comes back in a background thread, so we need to move to the UI thread to access any UI elements
                Await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, Sub()
                                                                              OutputToTextBox(uri)
                                                                              rootPage.NotifyUser("Channel request succeeded!", NotifyType.StatusMessage)
                                                                          End Sub)
            End If

            ' Clean out the background task just for the purpose of this sample
            UnregisterBackgroundTask()
            RegisterBackgroundTask()
            rootPage.NotifyUser("Task registered", NotifyType.StatusMessage)
        Catch ex As Exception
            rootPage.NotifyUser("Could not create a channel. Error number:" & ex.Message, NotifyType.ErrorMessage)
        End Try
    End Sub

    Private Sub RegisterBackgroundTask()
        Dim taskBuilder As New BackgroundTaskBuilder()
        Dim trigger As New PushNotificationTrigger()
        taskBuilder.SetTrigger(trigger)

        ' Background tasks must live in separate DLL, and be included in the package manifest
        ' Also, make sure that your main application project includes a reference to this DLL
        taskBuilder.TaskEntryPoint = SAMPLE_TASK_ENTRY_POINT
        taskBuilder.Name = SAMPLE_TASK_NAME

        Try
            Dim task As BackgroundTaskRegistration = taskBuilder.Register()
            AddHandler task.Completed, AddressOf BackgroundTaskCompleted
        Catch ex As Exception
            rootPage.NotifyUser("Registration error: " & ex.Message, NotifyType.ErrorMessage)
            UnregisterBackgroundTask()
        End Try
    End Sub

    Private Function UnregisterBackgroundTask() As Boolean
        For Each iter In BackgroundTaskRegistration.AllTasks
            Dim task As IBackgroundTaskRegistration = iter.Value
            If task.Name = SAMPLE_TASK_NAME Then
                task.Unregister(True)
                Return True
            End If
        Next
        Return False
    End Function

    Private Async Sub BackgroundTaskCompleted(sender As BackgroundTaskRegistration, args As BackgroundTaskCompletedEventArgs)
        Try
            ' This sample assumes the payload is a string. It can be of any type, though.
            Dim payload As String = DirectCast(ApplicationData.Current.LocalSettings.Values(SAMPLE_TASK_NAME), String)
            Await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, Sub()
                                                                          rootPage.NotifyUser("Background work item triggered by raw notification with payload = " & payload & " has completed!", NotifyType.StatusMessage)
                                                                      End Sub)
        Catch generatedExceptionName As Exception
            CallExceptionMessage()
        End Try
    End Sub

    Async Sub CallExceptionMessage()
        Await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, Sub()
                                                                      rootPage.NotifyUser("Failed to retrieve background payload", NotifyType.ErrorMessage)
                                                                  End Sub)
    End Sub
End Class
