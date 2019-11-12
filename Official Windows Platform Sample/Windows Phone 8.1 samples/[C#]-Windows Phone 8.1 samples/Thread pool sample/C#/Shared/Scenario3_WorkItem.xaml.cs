//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System;
using SDKTemplate;
using Windows.Foundation;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ThreadPool
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WorkItem : Page
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public WorkItem()
        {
            this.InitializeComponent();
            ThreadPoolSample.WorkItemScenaioro = this;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Priority.SelectedIndex = ThreadPoolSample.WorkItemSelectedIndex;
            UpdateUI(ThreadPoolSample.WorkItemStatus);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ThreadPoolSample.WorkItemSelectedIndex = Priority.SelectedIndex;
        }

        private void CreateThreadPoolWorkItem(object sender, RoutedEventArgs args)
        {
            // Variable that will be passed to WorkItemFunction, this variable is the number
            // of interlocked increments that the woker function will complete, used to simulate
            // work.

            long maxCount = 10000000;

            // Create a thread pool work item with specified priority.

            if (Priority.SelectionBoxItem != null)
            {
                switch (Priority.SelectionBoxItem.ToString())
                {
                    case "Low":
                        ThreadPoolSample.WorkItemPriority = WorkItemPriority.Low;
                        break;
                    case "Normal":
                        ThreadPoolSample.WorkItemPriority = WorkItemPriority.Normal;
                        break;
                    case "High":
                        ThreadPoolSample.WorkItemPriority = WorkItemPriority.High;
                        break;
                }
            }

            // Create the work item with the specified priority.

            ThreadPoolSample.ThreadPoolWorkItem = Windows.System.Threading.ThreadPool.RunAsync(
                (source) =>
                {
                    // Perform the thread pool work item activity.

                    long count = 0;
                    long oldProgress = 0;
                    while (count < maxCount)
                    {
                        // When WorkItem.Cancel is called, work items that have not started are canceled.
                        // If a work item is already running, it will run to completion unless it supports cancellation.
                        // To support cancellation, the work item should check IAsyncAction.Status for cancellation status
                        // and exit cleanly if it has been canceled.

                        if (source.Status == AsyncStatus.Canceled)
                        {
                            break;
                        }

                        // Simulate doing work.

                        System.Threading.Interlocked.Increment(ref count);

                        // Update work item progress in the UI.

                        long currentProgress = (long)(((double)count / (double)maxCount) * 100);
                        if (currentProgress > oldProgress)
                        {
                            // Only update if the progress value has changed.

                            var ignored = Dispatcher.RunAsync(
                                CoreDispatcherPriority.High,
                                () =>
                                {
                                    ThreadPoolSample.WorkItemScenaioro.UpdateWorkItemProgressUI(currentProgress);
                                });
                        }

                        oldProgress = currentProgress;
                    }
                },
            ThreadPoolSample.WorkItemPriority);

            // Register a completed-event handler to run when the work item finishes or is canceled.

            ThreadPoolSample.ThreadPoolWorkItem.Completed = new AsyncActionCompletedHandler(
                async (IAsyncAction source, AsyncStatus status) =>
                {
                    await Dispatcher.RunAsync(
                        CoreDispatcherPriority.High,
                        () =>
                        {
                            switch (status)
                            {
                                case AsyncStatus.Started:
                                    ThreadPoolSample.WorkItemScenaioro.UpdateUI(Status.Started);
                                    break;
                                case AsyncStatus.Completed:
                                    ThreadPoolSample.WorkItemScenaioro.UpdateUI(Status.Completed);
                                    break;
                                case AsyncStatus.Canceled:
                                    ThreadPoolSample.WorkItemScenaioro.UpdateUI(Status.Canceled);
                                    break;
                            }
                        });
                });

            UpdateUI(Status.Started);
        }

        private void CancelThreadPoolWorkItem(object sender, RoutedEventArgs args)
        {
            if (ThreadPoolSample.ThreadPoolWorkItem != null)
            {
                ThreadPoolSample.ThreadPoolWorkItem.Cancel();
            }
        }

        public void UpdateUI(Status status)
        {
            ThreadPoolSample.WorkItemStatus = status;

            WorkItemStatus.Text = status.ToString("g");
            WorkItemInfo.Text = string.Format("Work item priority = {0}", ThreadPoolSample.WorkItemPriority.ToString("g"));

            var createButtonEnabled = (status != Status.Started);
            CreateThreadPoolWorkItemButton.IsEnabled = createButtonEnabled;
            CancelThreadPoolWorkItemButton.IsEnabled = !createButtonEnabled;
        }

        public void UpdateWorkItemProgressUI(long percentComplete)
        {
            WorkItemStatus.Text = string.Format("Progress: {0}%", percentComplete.ToString());
        }
    }
}
