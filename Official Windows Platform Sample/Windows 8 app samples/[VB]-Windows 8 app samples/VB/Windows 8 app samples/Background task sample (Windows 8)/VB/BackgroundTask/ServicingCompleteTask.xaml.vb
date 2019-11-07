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

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class ServicingCompleteTask
    Inherits SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        For Each task In BackgroundTaskRegistration.AllTasks
            If task.Value.Name = BackgroundTaskSample.ServicingCompleteTaskName Then
                AttachProgressAndCompletedHandlers(task.Value)
                BackgroundTaskSample.UpdateBackgroundTaskStatus(BackgroundTaskSample.ServicingCompleteTaskName, True)
                Exit For
            End If
        Next

        UpdateUI()
    End Sub

    ''' <summary>
    ''' Attach progress and completed handers to a background task.
    ''' </summary>
    ''' <param name="task">The task to attach progress and completed handlers to.</param>
    Private Sub AttachProgressAndCompletedHandlers(task As IBackgroundTaskRegistration)
        AddHandler task.Progress, AddressOf OnProgress
        AddHandler task.Completed, AddressOf OnCompleted
    End Sub

    ''' <summary>
    ''' Handle background task progress.
    ''' </summary>
    ''' <param name="task">The task that is reporting progress.</param>
    ''' <param name="e">Arguments of the progress report.</param>
    Private Sub OnProgress(task As IBackgroundTaskRegistration, args As BackgroundTaskProgressEventArgs)
        Dim progress = "Progress: " & args.Progress & "%"
        BackgroundTaskSample.ServicingCompleteTaskProgress = progress
        UpdateUI()
    End Sub

    ''' <summary>
    ''' Handle background task completion.
    ''' </summary>
    ''' <param name="task">The task that is reporting completion.</param>
    ''' <param name="e">Arguments of the completion report.</param>
    Private Sub OnCompleted(task As IBackgroundTaskRegistration, args As BackgroundTaskCompletedEventArgs)
        UpdateUI()
    End Sub

    ''' <summary>
    ''' Update the scenario UI.
    ''' </summary>
    Private Async Sub UpdateUI()
        Await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, Sub()
                                                                     Progress.Text = BackgroundTaskSample.ServicingCompleteTaskProgress
                                                                     Status.Text = BackgroundTaskSample.GetBackgroundTaskStatus(BackgroundTaskSample.ServicingCompleteTaskName)
                                                                 End Sub)
    End Sub

End Class
