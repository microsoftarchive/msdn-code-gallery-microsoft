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
Imports Windows.Foundation
Imports Windows.System.Threading
Imports Windows.UI.Core
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation
Imports Threadpool.ThreadPool

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class WorkItem
    Inherits SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()

        '
        ' Take a reference to the main window dispatcher object to the UI.
        '
        ThreadPoolSample.WorkItemScenaioro = Me
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        Priority.SelectedIndex = ThreadPoolSample.WorkItemSelectedIndex
        UpdateUI(ThreadPoolSample.WorkItemStatus)
    End Sub

    Protected Overrides Sub OnNavigatedFrom(e As NavigationEventArgs)
        ThreadPoolSample.WorkItemSelectedIndex = Priority.SelectedIndex
    End Sub

    ''' <summary>
    ''' Create a thread pool work item.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub CreateThreadPoolWorkItem(sender As Object, args As RoutedEventArgs)
        '
        ' Variable that will be passed to WorkItemFunction, this variable is the number
        ' of interlocked increments that the woker function will complete, used to simulate
        ' work.
        '
        Dim maxCount As Long = 10000000

        '
        ' Create a thread pool work item with specified priority.
        '

        Select Case Priority.SelectionBoxItem.ToString
            Case "Low"
                ThreadPoolSample.WorkItemPriority = WorkItemPriority.Low
                Exit Select
            Case "Normal"
                ThreadPoolSample.WorkItemPriority = WorkItemPriority.Normal
                Exit Select
            Case "High"
                ThreadPoolSample.WorkItemPriority = WorkItemPriority.High
                Exit Select
        End Select

        '
        ' Create the work item with the specified priority.
        '
        ThreadPoolSample.ThreadPoolWorkItem = Windows.System.Threading.ThreadPool.RunAsync(New WorkItemHandler(Function(source As IAsyncAction)
                                                                                                                   '
                                                                                                                   ' Perform the thread pool work item activity.
                                                                                                                   '
                                                                                                                   Dim count As Long = 0
                                                                                                                   Dim oldProgress As Long = 0
                                                                                                                   While count < maxCount
                                                                                                                       '
                                                                                                                       ' When WorkItem.Cancel is called, work items that have not started are canceled.
                                                                                                                       ' If a work item is already running, it will run to completion unless it supports cancellation.
                                                                                                                       ' To support cancellation, the work item should check IAsyncAction.Status for cancellation status
                                                                                                                       ' and exit cleanly if it has been canceled.
                                                                                                                       '
                                                                                                                       If source.Status = AsyncStatus.Canceled Then
                                                                                                                           Exit While
                                                                                                                       End If

                                                                                                                       '
                                                                                                                       ' Simulate doing work.
                                                                                                                       '
                                                                                                                       System.Threading.Interlocked.Increment(count)

                                                                                                                       '
                                                                                                                       ' Update work item progress in the UI.
                                                                                                                       '
                                                                                                                       Dim currentProgress As Long = CLng((CDbl(count) / CDbl(maxCount)) * 100)
                                                                                                                       If currentProgress > oldProgress Then
                                                                                                                           '
                                                                                                                           ' Only update if the progress value has changed.
                                                                                                                           '
                                                                                                                           Dim ignored = Dispatcher.RunAsync(CoreDispatcherPriority.High, Sub()
                                                                                                                                                                                              ThreadPoolSample.WorkItemScenaioro.UpdateWorkItemProgressUI(currentProgress)
                                                                                                                                                                                          End Sub)
                                                                                                                       End If

                                                                                                                       oldProgress = currentProgress
                                                                                                                   End While
                                                                                                                   Return Nothing
                                                                                                               End Function), ThreadPoolSample.WorkItemPriority)

        '
        ' Register a completed-event handler to run when the work item finishes or is canceled.
        '
        ThreadPoolSample.ThreadPoolWorkItem.Completed = New AsyncActionCompletedHandler(Async Function(source As IAsyncAction, status__1 As AsyncStatus)
                                                                                            Await Dispatcher.RunAsync(CoreDispatcherPriority.High, Sub()
                                                                                                                                                       Dim action As IAsyncAction = TryCast(source, IAsyncAction)
                                                                                                                                                       If action IsNot Nothing Then
                                                                                                                                                           Select Case action.Status
                                                                                                                                                               Case AsyncStatus.Started
                                                                                                                                                                   ThreadPoolSample.WorkItemScenaioro.UpdateUI(Status.Started)
                                                                                                                                                                   Exit Select
                                                                                                                                                               Case AsyncStatus.Completed
                                                                                                                                                                   ThreadPoolSample.WorkItemScenaioro.UpdateUI(Status.Completed)
                                                                                                                                                                   Exit Select
                                                                                                                                                               Case AsyncStatus.Canceled
                                                                                                                                                                   ThreadPoolSample.WorkItemScenaioro.UpdateUI(Status.Canceled)
                                                                                                                                                                   Exit Select
                                                                                                                                                               Case AsyncStatus.[Error]
                                                                                                                                                                   '
                                                                                                                                                                   ' TODO: handle errors.
                                                                                                                                                                   '
                                                                                                                                                                   'ThreadPoolStatus.Text = "Error";
                                                                                                                                                                   Exit Select
                                                                                                                                                           End Select
                                                                                                                                                       End If
                                                                                                                                                   End Sub)
                                                                                        End Function)

        '
        ' Update the UI for the newly created work item.
        '
        UpdateUI(Status.Started)
    End Sub

    ''' <summary>
    ''' Cancel a thread pool work item. 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="args"></param>
    Private Sub CancelThreadPoolWorkItem(sender As Object, args As RoutedEventArgs)
        If ThreadPoolSample.ThreadPoolWorkItem IsNot Nothing Then
            ThreadPoolSample.ThreadPoolWorkItem.Cancel()
        End If
    End Sub

    ''' <summary>
    ''' Update the scenario UI.
    ''' </summary>
    ''' <param name="status"></param>
    Public Sub UpdateUI(status__1 As Status)
        ThreadPoolSample.WorkItemStatus = status__1

        WorkItemStatus.Text = status__1.ToString("g")
        WorkItemInfo.Text = String.Format("Work item priority = {0}", ThreadPoolSample.WorkItemPriority.ToString("g"))

        Dim createButtonEnabled = (status__1 <> Status.Started)
        CreateThreadPoolWorkItemButton.IsEnabled = createButtonEnabled
        CancelThreadPoolWorkItemButton.IsEnabled = Not createButtonEnabled
    End Sub

    ''' <summary>
    ''' Update teh work item progress.
    ''' </summary>
    ''' <param name="percentComplete"></param>
    Public Sub UpdateWorkItemProgressUI(percentComplete As Long)
        WorkItemStatus.Text = String.Format("Progress: {0}%", percentComplete.ToString)
    End Sub
End Class
