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
    public sealed partial class PeriodicTimer : Page
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public PeriodicTimer()
        {
            this.InitializeComponent();
            ThreadPoolSample.PeriodicTimerScenario = this;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            PeriodMs.Text = ThreadPoolSample.PeriodicTimerMilliseconds.ToString();
            UpdateUI(ThreadPoolSample.PeriodicTimerStatus);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ulong result;
            if (ulong.TryParse(PeriodMs.Text, out result))
            {
                ThreadPoolSample.PeriodicTimerMilliseconds = result;
            }
        }

        /// <summary>
        /// Create a periodic timer that fires every time the period elapses.
        /// When the timer expires, its callback handler is called and the timer is reset.
        /// This behavior continues until the periodic timer is cancelled.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreatePeriodicTimer(object sender, RoutedEventArgs args)
        {
            if (ulong.TryParse(PeriodMs.Text, out ThreadPoolSample.PeriodicTimerMilliseconds))
            {
                ThreadPoolSample.PeriodicTimer = ThreadPoolTimer.CreatePeriodicTimer(
                    async (timer) =>
                    {
                        System.Threading.Interlocked.Increment(ref ThreadPoolSample.PeriodicTimerCount);
                        await Dispatcher.RunAsync(
                            CoreDispatcherPriority.High, () =>
                            {
                                ThreadPoolSample.PeriodicTimerScenario.UpdateUI(Status.Completed);
                            });
                    },
                    TimeSpan.FromMilliseconds(ThreadPoolSample.PeriodicTimerMilliseconds));

                UpdateUI(Status.Started);
            }
        }

        private void CancelPeriodicTimer(object sender, RoutedEventArgs args)
        {
            if (ThreadPoolSample.PeriodicTimer != null)
            {
                ThreadPoolSample.PeriodicTimer.Cancel();
                ThreadPoolSample.PeriodicTimerCount = 0;
                UpdateUI(Status.Canceled);
            }
        }

        public void UpdateUI(Status status)
        {
            ThreadPoolSample.PeriodicTimerStatus = status;

            switch (status)
            {
                case Status.Completed:
                    PeriodicTimerStatus.Text = string.Format("Completion count: {0}", ThreadPoolSample.PeriodicTimerCount);
                    break;
                default:
                    PeriodicTimerStatus.Text = status.ToString("g");
                    break;
            }

            PeriodicTimerInfo.Text = string.Format("Timer Period = {0} ms.", ThreadPoolSample.PeriodicTimerMilliseconds);

            if (((status != Status.Started) && (status != Status.Completed)) ||
                ((status == Status.Completed) && (ThreadPoolSample.PeriodicTimerMilliseconds == 0)))
            {
                CreatePeriodicTimerButton.IsEnabled = true;
                CreatePeriodicTimerButton.Focus(Windows.UI.Xaml.FocusState.Keyboard);
                CancelPeriodicTimerButton.IsEnabled = false;
            }
            else
            {
                CancelPeriodicTimerButton.IsEnabled = true;
                CancelPeriodicTimerButton.Focus(Windows.UI.Xaml.FocusState.Keyboard);
                CreatePeriodicTimerButton.IsEnabled = false; 
            }
        }

        private void PeriodMs_TextChanged(object sender, TextChangedEventArgs e)
        {
            ThreadPoolSample.PeriodicTimerMilliseconds = ThreadPoolSample.ValidateTimeValue(PeriodMs.Text, ThreadPoolSample.PeriodicTimerMilliseconds);
            PeriodMs.Text = ThreadPoolSample.PeriodicTimerMilliseconds.ToString();
        }
    }
}
