//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Calls;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;

namespace LockScreenCall
{
    /// <summary>
    /// This page represents a call in progress.
    /// </summary>
    public sealed partial class CallPage : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        // A reference to the UI on the lock screen, if the call is being displayed on the lock screen.
        LockScreenCallUI callUI;

        // The timer that simulates the caller hanging up.
        ThreadPoolTimer hangUpTimer;

        public CallPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var args = e.Parameter as ILaunchActivatedEventArgs;

            // This sample uses the same page for both lock screen calls and normal calls.
            // The OnLaunched method in App.xaml.cs handles normal call activation,
            // and the OnActivated method handles lock screen call activation.
            // Both pass the ILaunchActivatedEventArgs as the page parameter. If activated from
            // a lock screen call, the parameter will be a LockScreenCallActivatedEventArgs,
            // and the CallUI member gives us access to the lock screen.
            // The Arguments member is the toast command string specified in the XML generated
            // in the xmlPayload variable in Toast.xaml.cs. It takes the form
            //   "<call mode> <caller identity> <simulated call duration>"

            string[] callArguments = args.Arguments.Split(new char[] { ' ' });
            string callMode = callArguments[0];
            string callerIdentity = callArguments[1];
            int callDuration = int.Parse(callArguments[2]);

            string imageText = "";
            switch (callerIdentity) {
                case "Dad":
                    imageText = "\ud83d\udc68";
                    break;
                case "Mom":
                    imageText = "\ud83d\udc69";
                    break;
                case "Grandpa":
                    imageText = "\ud83d\udc74";
                    break;
                case "Grandma":
                    imageText = "\ud83d\udc75";
                    break;
            }
            if (callMode == "Voice") {
                imageText = "\ud83d\udd0a";
            }

            string callTitle = callMode + " call from " + callerIdentity;

            var lockScreenArgs = args as LockScreenCallActivatedEventArgs;
            if (lockScreenArgs != null)
            {
                callUI = lockScreenArgs.CallUI;

                // Hide controls since they are not interactive on the lock screen.
                EndCallButton.Visibility = Visibility.Collapsed;

                // Hook up events.
                callUI.EndRequested += OnEndRequested;
                callUI.Closed += OnClosed;

                // Set the title.
                callUI.CallTitle = callTitle;
            }

            CallImage.Text = imageText;

            // Assign a random light background color so that each call looks
            // slightly different.
            LayoutRoot.Background = RandomLightSolidColor();
            
            CallTitle.Text = callTitle;

            if (callDuration > 0)
            {
                hangUpTimer = ThreadPoolTimer.CreateTimer(OtherPartyHangsUp, TimeSpan.FromSeconds(callDuration));
            }
        }

        // When the user leaves the page, throw away the CallUI object and cancel
        // any pending simulated "hang up" operation.
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            callUI = null;
            if (hangUpTimer != null)
            {
                hangUpTimer.Cancel();
                hangUpTimer = null;
            }
            base.OnNavigatedFrom(e);
        }

        static private SolidColorBrush RandomLightSolidColor()
        {
            Random rand = new Random();
            var color = Windows.UI.Color.FromArgb(255, (byte)rand.Next(128, 255), (byte)rand.Next(128, 255), (byte)rand.Next(128, 255));
            return new SolidColorBrush(color);
        }

        private void NavigateToMainPage()
        {
            Frame.Navigate(typeof(MainPage), null);
        }

        // Queue code to run on the UI thread and return immediately.
        private void QueueOnUIThread(DispatchedHandler e)
        {
            var ignored = this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, e);
        }

        private System.Threading.Tasks.Task FadeToBlackAsync()
        {
            CallFadeOut.Visibility = Visibility.Visible;
            var tcs = new System.Threading.Tasks.TaskCompletionSource<object>();
            EventHandler<object> completed = null;
            completed = (o, e) =>
            {
                FadeToBlack.Completed -= completed;
                tcs.SetResult(null);
            };
            FadeToBlack.Completed += completed;
            FadeToBlack.Begin();
            return tcs.Task;
        }

        private void OtherPartyHangsUp(ThreadPoolTimer t)
        {
            hangUpTimer = null;
            QueueOnUIThread(async () =>
            {
                if (callUI != null)
                {
                    await FadeToBlackAsync();
                }
                // Re-check callUI because it may have been nulled out during the animation.
                if (callUI != null)
                {
                    callUI.Dismiss();
                }
                NavigateToMainPage();
            });
        }

        /// <summary>
        /// This is the event handler for the EndRequested event. It is invoked when the user ends
        /// the call directly from the lock screen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnEndRequested(object sender, LockScreenCallEndRequestedEventArgs e)
        {
            var deferral = e.GetDeferral();

            QueueOnUIThread(async () =>
            {
                await FadeToBlackAsync();
                deferral.Complete();
                NavigateToMainPage();
            });
        }

        /// <summary>
        /// This is the event handler for the Closed event. It is invoked when the call is
        /// removed from the lock screen by whatever means.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClosed(object sender, object e)
        {
            QueueOnUIThread(() =>
            {
                callUI = null;
                // Show the "End Call" button in our app.
                EndCallButton.Visibility = Visibility.Visible;
            });
        }

        /// <summary>
        /// This is the click handler for the 'End Call' button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EndCallButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToMainPage();
        }
    }
}
