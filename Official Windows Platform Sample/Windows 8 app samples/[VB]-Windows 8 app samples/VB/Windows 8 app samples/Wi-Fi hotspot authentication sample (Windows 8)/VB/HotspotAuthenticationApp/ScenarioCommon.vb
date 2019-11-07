'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports HotspotAuthenticationTask
Imports SDKTemplate
Imports Windows.ApplicationModel.Background
Imports Windows.UI.Core

' A delegate type for hooking up foreground authentication notifications.
Public Delegate Sub ForegroundAuthenticationDelegate(sender As Object, e As EventArgs)

' Shared code for all scenario pages
Class ScenarioCommon
    ' Singleton reference to share a single instance with all pages
    Shared scenarioCommonSingleton As ScenarioCommon

    ' The entry point name of the background task handler:
    Public Const BackgroundTaskEntryPoint As String = "HotspotAuthenticationTask.AuthenticationTask"

    ' The (arbitrarily chosen) name assigned to the background task:
    Public Const BackgroundTaskName As String = "AuthenticationBackgroundTask"

    ' An delegate for subscribing for foreground authentication events
    Public ForegroundAuthenticationCallback As ForegroundAuthenticationDelegate

    ' A pointer back to the main page.  This is needed to call methods in MainPage such as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    ' A reference to the main window dispatcher object to the UI.
    Private coreDispatcher As CoreDispatcher = Windows.UI.Xaml.Window.Current.CoreWindow.Dispatcher

    ' A flag to remember if a background task handler has been registered
    Private HasRegisteredBackgroundTaskHandler As Boolean = False

    Public Shared ReadOnly Property Instance() As ScenarioCommon
        Get
            If scenarioCommonSingleton Is Nothing Then
                scenarioCommonSingleton = New ScenarioCommon()
            End If
            Return scenarioCommonSingleton
        End Get
    End Property

    ''' <summary>
    ''' Register completion handler for registered background task on application startup.
    ''' </summary>
    ''' <returns>True if a registerd task was found</returns>
    Public Function RegisteredCompletionHandlerForBackgroundTask() As Boolean
        If Not HasRegisteredBackgroundTaskHandler Then
            Try
                ' Associate background task completed event handler with background task.
                For Each cur In BackgroundTaskRegistration.AllTasks
                    If cur.Value.Name = BackgroundTaskName Then
                        AddHandler cur.Value.Completed, AddressOf OnBackgroundTaskCompleted
                        HasRegisteredBackgroundTaskHandler = True
                        Exit For
                    End If
                Next
            Catch ex As Exception
                rootPage.NotifyUser(ex.ToString, NotifyType.ErrorMessage)
            End Try
        End If
        Return HasRegisteredBackgroundTaskHandler
    End Function

    ''' <summary>
    ''' Background task completion handler. When authenticating through the foreground app, this triggers the authentication flow if the app is currently running.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Public Async Sub OnBackgroundTaskCompleted(sender As IBackgroundTaskRegistration, e As BackgroundTaskCompletedEventArgs)
        ' Update the UI with progress reported by the background task.
        Await coreDispatcher.RunAsync(CoreDispatcherPriority.Normal, New DispatchedHandler(Sub()
                                                                                               Try
                                                                                                   If (sender IsNot Nothing) AndAlso (e IsNot Nothing) Then

                                                                                                       ' If the background task threw an exception, display the exception in the error text box.
                                                                                                       e.CheckResult()

                                                                                                       ' Update the UI with the completion status of the background task
                                                                                                       ' The Run method of the background task sets this status.
                                                                                                       If sender.Name = BackgroundTaskName Then
                                                                                                           rootPage.NotifyUser("Background task completed", NotifyType.StatusMessage)

                                                                                                           ' Signal event for foreground authentication
                                                                                                           If (Not ConfigStore.AuthenticateThroughBackgroundTask) AndAlso (ForegroundAuthenticationCallback IsNot Nothing) Then
                                                                                                               ForegroundAuthenticationCallback(Me, Nothing)
                                                                                                           End If
                                                                                                       End If
                                                                                                   End If
                                                                                               Catch ex As Exception
                                                                                                   rootPage.NotifyUser(ex.ToString, NotifyType.ErrorMessage)
                                                                                               End Try
                                                                                           End Sub))
    End Sub
End Class
