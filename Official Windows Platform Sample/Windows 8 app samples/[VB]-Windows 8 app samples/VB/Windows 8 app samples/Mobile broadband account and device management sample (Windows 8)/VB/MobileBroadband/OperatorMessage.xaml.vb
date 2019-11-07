'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation
Imports Windows.ApplicationModel.Background
Imports Windows.UI.Core
Imports Windows.Storage
Imports Windows.Networking.NetworkOperators

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class OperatorMessage
    Inherits SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Private sampleDispatcher As CoreDispatcher
    Private Const OperatorNotificationTaskEntryPoint As String = "OperatorNotificationTask.OperatorNotification"
    Private Const OperatorNotificationTaskName As String = "OperatorNotificationTask"

    Public Sub New()
        Me.InitializeComponent()

        sampleDispatcher = Window.Current.CoreWindow.Dispatcher

        Try
            '
            ' Register a background task for the network operator notification system event.
            ' This event is triggered when the application is updated.
            '
            RegisterOperatorNotificationTask()
        Catch ex As Exception
            rootPage.NotifyUser(ex.ToString, NotifyType.ErrorMessage)
        End Try
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
    End Sub

    '
    ' Registers a background task for the operator notification system event.
    ' This event occurs when the application is updated.
    '
    Private Sub RegisterOperatorNotificationTask()
        ' Check whether the operator notification background task is already registered.
        For Each cur In BackgroundTaskRegistration.AllTasks
            If cur.Value.Name = "MobileOperatorNotificationHandler" Then
                AddHandler cur.Value.Completed, AddressOf OnCompleted
                OperatorNotificationStatus.Text = "Completion handler registered"
            End If
        Next

        ' Get all active Mobilebroadband accounts
        Dim allAccounts = MobileBroadbandAccount.AvailableNetworkAccountIds
        If allAccounts.Count > 0 Then
            ' Update ui to show the task is registered.
            rootPage.NotifyUser("Mobile broadband account found", NotifyType.StatusMessage)
        Else
            rootPage.NotifyUser("No Mobilebroadband accounts found", NotifyType.StatusMessage)
        End If
    End Sub

    ' Handle background task completion event.
    Private Async Sub OnCompleted(sender As IBackgroundTaskRegistration, e As BackgroundTaskCompletedEventArgs)
        ' Update the UI with the completion status reported by the background task.
        ' Dispatch an anonymous task to the UI thread to do the update.
        Await sampleDispatcher.RunAsync(CoreDispatcherPriority.Normal, Sub()
                                                                           Try
                                                                               If (sender IsNot Nothing) AndAlso (e IsNot Nothing) Then
                                                                                   ' this method throws if the event is reporting an error
                                                                                   e.CheckResult()

                                                                                   ' Update the UI with the background task's completion status.
                                                                                   ' The task stores status in the application data settings indexed by the task ID.
                                                                                   Dim key = sender.TaskId.ToString
                                                                                   Dim settings = ApplicationData.Current.LocalSettings
                                                                                   OperatorNotificationStatus.Text = settings.Values(key).ToString
                                                                                   rootPage.NotifyUser("Operator Notification background task completed", NotifyType.StatusMessage)
                                                                               End If
                                                                           Catch ex As Exception
                                                                               OperatorNotificationStatus.Text = "Background task error"
                                                                               rootPage.NotifyUser(ex.ToString, NotifyType.ErrorMessage)
                                                                           End Try
                                                                       End Sub)
    End Sub
End Class
