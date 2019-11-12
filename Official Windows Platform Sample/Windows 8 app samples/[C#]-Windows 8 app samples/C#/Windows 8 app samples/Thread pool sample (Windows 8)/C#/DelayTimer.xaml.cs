//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.System.Threading;
using SDKTemplate;
using System;

namespace ThreadPool
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DelayTimer : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public DelayTimer()
        {
            this.InitializeComponent();
            ThreadPoolSample.DelayTimerScenario = this;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DelayMs.SelectedIndex = ThreadPoolSample.DelayTimerSelectedIndex;
            UpdateUI(ThreadPoolSample.DelayTimerStatus);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ThreadPoolSample.DelayTimerSelectedIndex = DelayMs.SelectedIndex;
        }

        /// <summary>
        /// Create a Delay timer that fires once after the specified delay elapses.
        /// When the timer expires, its callback hander is called.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateDelayTimer(object sender, RoutedEventArgs args)
        {
            if (int.TryParse(DelayMs.SelectionBoxItem.ToString(), out ThreadPoolSample.DelayTimerMilliseconds))
            {
                ThreadPoolSample.DelayTimer = ThreadPoolTimer.CreateTimer(
                    async (timer) =>
                    {
                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                            () =>
                            {
                                ThreadPoolSample.DelayTimerScenario.UpdateUI(Status.Completed);
                            });
                    },
                    TimeSpan.FromMilliseconds(ThreadPoolSample.DelayTimerMilliseconds));

                UpdateUI(Status.Started);
            }
        }

        private void CancelDelayTimer(object sender, RoutedEventArgs args)
        {
            if (ThreadPoolSample.DelayTimer != null)
            {
                ThreadPoolSample.DelayTimer.Cancel();
                UpdateUI(Status.Canceled);
            }
        }

        public void UpdateUI(Status status)
        {
            ThreadPoolSample.DelayTimerStatus = status;
            DelayTimerInfo.Text = string.Format("Timer delay = {0} ms.", ThreadPoolSample.DelayTimerMilliseconds);
            DelayTimerStatus.Text = status.ToString("g");

            var createButtonEnabled = (status == Status.Started);
            CreateDelayTimerButton.IsEnabled = !createButtonEnabled;
            CancelDelayTimerButton.IsEnabled = createButtonEnabled;
        }
    }
}
