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
Imports SDKTemplate
Imports Windows.ApplicationModel.Background
Imports Windows.Storage
Imports Windows.UI.Core
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation
Imports Windows.Networking.Connectivity


	''' <summary>
	''' An empty page that can be used on its own or navigated to within a Frame.
	''' </summary>
Partial Public NotInheritable Class NetworkStatusWithInternetPresent
    Inherits SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current
    Private NetworkStatusWithInternetPresentDispatcher As CoreDispatcher

    'Save current internet profile and network adapter ID globally
    Private internetProfile As String = "Not connected to Internet"
    Private networkAdapter As String = "Not connected to Internet"


    Public Sub New()
        Me.InitializeComponent()

        NetworkStatusWithInternetPresentDispatcher = Window.Current.CoreWindow.Dispatcher
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        For Each task In BackgroundTaskRegistration.AllTasks
            If task.Value.Name = BackgroundTaskSample.SampleBackgroundTaskWithConditionName Then
                'Associate background task completed event handler with background task
                Dim isTaskRegistered = RegisterCompletedHandlerforBackgroundTask(task.Value)
                UpdateButton(isTaskRegistered)
            End If
        Next

    End Sub
    ''' <summary>
    ''' Register a SampleBackgroundTaskWithCondition.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub RegisterButton_Click(sender As Object, e As RoutedEventArgs)
        Try
            'Save current internet profile and network adapter ID globally
            Dim connectionProfile = NetworkInformation.GetInternetConnectionProfile()
            If connectionProfile IsNot Nothing Then
                internetProfile = connectionProfile.ProfileName
                Dim networkAdapterInfo = connectionProfile.NetworkAdapter
                If networkAdapterInfo IsNot Nothing Then
                    networkAdapter = networkAdapterInfo.NetworkAdapterId.ToString()
                Else
                    networkAdapter = "Not connected to Internet"
                End If
            Else
                internetProfile = "Not connected to Internet"
                networkAdapter = "Not connected to Internet"
            End If

            Dim task = BackgroundTaskSample.RegisterBackgroundTask(BackgroundTaskSample.SampleBackgroundTaskEntryPoint, BackgroundTaskSample.SampleBackgroundTaskWithConditionName, New SystemTrigger(SystemTriggerType.NetworkStateChange, False), New SystemCondition(SystemConditionType.InternetAvailable))

            'Associate background task completed event handler with background task.
            AddHandler task.Completed, AddressOf OnCompleted
            rootPage.NotifyUser("Registered for NetworkStatusChange background task with Internet present condition" & vbLf, NotifyType.StatusMessage)
            UpdateButton(True)
        Catch ex As Exception
            rootPage.NotifyUser(ex.ToString(), NotifyType.ErrorMessage)
        End Try
    End Sub

    ''' <summary>
    ''' Unregister a SampleBackgroundTaskWithCondition.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub UnregisterButton_Click(sender As Object, e As RoutedEventArgs)
        BackgroundTaskSample.UnregisterBackgroundTasks(BackgroundTaskSample.SampleBackgroundTaskWithConditionName)
        rootPage.NotifyUser("Unregistered for NetworkStatusChange background task with Internet present condition" & vbLf, NotifyType.StatusMessage)
        UpdateButton(False)
    End Sub

    ''' <summary>
    ''' Register completion handler for registered background task on application startup.
    ''' </summary>
    ''' <param name="task">The task to attach progress and completed handlers to.</param>
    Private Function RegisterCompletedHandlerforBackgroundTask(task As IBackgroundTaskRegistration) As Boolean
        Dim taskRegistered As Boolean = False
        Try
            'Associate background task completed event handler with background task.
            AddHandler task.Completed, AddressOf OnCompleted
            taskRegistered = True
        Catch ex As Exception
            rootPage.NotifyUser(ex.ToString(), NotifyType.ErrorMessage)
        End Try
        Return taskRegistered
    End Function

    ''' <summary>
    ''' Handle background task completion.
    ''' </summary>
    ''' <param name="task">The task that is reporting completion.</param>
    ''' <param name="e">Arguments of the completion report.</param>
    Private Async Sub OnCompleted(task As IBackgroundTaskRegistration, args As BackgroundTaskCompletedEventArgs)
        Dim localSettings As ApplicationDataContainer = ApplicationData.Current.LocalSettings

        Dim profile As [Object] = localSettings.Values("InternetProfile")
        Dim adapter As [Object] = localSettings.Values("NetworkAdapterId")

        Await NetworkStatusWithInternetPresentDispatcher.RunAsync(CoreDispatcherPriority.Normal, Sub()
                                                                                                     If (profile IsNot Nothing) AndAlso (adapter IsNot Nothing) Then
                                                                                                         'If internet profile has changed, display the new internet profile
                                                                                                         If (String.Equals(profile.ToString(), internetProfile, StringComparison.Ordinal) = False) OrElse (String.Equals(adapter.ToString(), networkAdapter, StringComparison.Ordinal) = False) Then
                                                                                                             internetProfile = profile.ToString()
                                                                                                             networkAdapter = adapter.ToString()
                                                                                                             rootPage.NotifyUser("Internet Profile changed" & vbLf + "=================" & vbLf + "Current Internet Profile : " + internetProfile, NotifyType.StatusMessage)
                                                                                                         End If
                                                                                                     End If
                                                                                                 End Sub)
    End Sub

    ''' <summary>
    ''' Update the Register/Unregister button.
    ''' </summary>
    Private Sub UpdateButton(registered As Boolean)
        RegisterButton.IsEnabled = Not registered
        UnregisterButton.IsEnabled = registered
    End Sub

End Class

