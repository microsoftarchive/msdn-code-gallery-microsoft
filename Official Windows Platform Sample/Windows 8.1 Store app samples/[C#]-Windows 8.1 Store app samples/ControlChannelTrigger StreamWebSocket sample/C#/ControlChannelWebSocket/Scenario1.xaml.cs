//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************
using SDKTemplate;
using DiagnosticsHelper;
using StreamWebSocketTransportHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace ControlChannelWebSocket
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario1 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;
        CoreDispatcher coreDispatcher;
        CommModule commModule;
        bool lockScreenAdded = false;
        TextBlock debugOutputTextBlock;
        public Scenario1()
        {
            InitializeComponent();
            coreDispatcher = Diag.coreDispatcher = Window.Current.Dispatcher;
            ConnectButton.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            debugOutputTextBlock = Diag.debugOutputTextBlock = DebugTextBlock;
        }

        public void Dispose()
        {
            if (commModule != null)
            {
                commModule.Dispose();
                commModule = null;
            }
        }
       
        #region Click Handlers

        private void ClientRole_Click(object sender, RoutedEventArgs e)
        {
            Diag.DebugPrint("Client role selected");

            // In order to simplify the sample and focus on the core controlchanneltrigger
            // related concepts, once a role is selected, the app has
            // to be restarted to change the role.
            ClientRoleButton.IsEnabled = false;
            ClientInit();
        }
        #endregion
        async void ClientInit()
        {
            commModule = new CommModule();

            // In the client role, we require the application to be on lock screen.
            // Lock screen is required to let in-process RealTimeCommunication related
            // background code to execute.
            if (lockScreenAdded == false)
            {
                BackgroundAccessStatus status = await BackgroundExecutionManager.RequestAccessAsync();
                Diag.DebugPrint("Lock screen status" + status);

                switch (status)
                {
                    case BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity:

                        // App is allowed to use RealTimeConnection broker 
                        // functionality even in low power mode.
                        lockScreenAdded = true;
                        break;
                    case BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity:

                        // App is allowed to use RealTimeConnection broker 
                        // functionality but not in low power mode.
                        lockScreenAdded = true;
                        break;
                    case BackgroundAccessStatus.Denied:

                        // App should switch to polling mode (example: poll for email based on time triggers)
                        Diag.DebugPrint("As Lockscreen status was Denied, App should switch to polling mode such as email based on time triggers.");
                        break;
                }
            }

            ClientSettings.Visibility = Visibility.Visible;
            ConnectButton.Visibility = Visibility.Visible;
            return;
        }

        private string GetServerUri()
        {
            return ServerUri.Text;
        }

        private enum connectionStates
        {
            notConnected = 0,
            connecting = 1,
            connected = 2,
        };

        private static connectionStates connectionState = connectionStates.notConnected;

        // Registers a background task with a network change system trigger.
        private void RegisterNetworkChangeTask()
        {
            try
            {
                // Delete previously registered network status change tasks as
                // the background triggers are persistent by nature across process
                // lifetimes.
                foreach (var cur in BackgroundTaskRegistration.AllTasks)
                {
                    Diag.DebugPrint("Deleting Background Task " + cur.Value.Name);
                    cur.Value.Unregister(true);
                }

                var myTaskBuilder = new BackgroundTaskBuilder();
                var myTrigger = new SystemTrigger(SystemTriggerType.NetworkStateChange, false);
                myTaskBuilder.SetTrigger(myTrigger);
                myTaskBuilder.TaskEntryPoint = "BackgroundTaskHelper.NetworkChangeTask";
                myTaskBuilder.Name = "Network change task";
                var myTask = myTaskBuilder.Register();
            }
            catch (Exception exp)
            {
                Diag.DebugPrint("Exception caught while setting up system event" + exp.ToString());
            }
        }

        async private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            if (connectionState == connectionStates.notConnected)
            {
                ConnectButton.Content = "Connecting...";
                connectionState = connectionStates.connecting;
                string serverUri = GetServerUri();

                RegisterNetworkChangeTask();

                // Finally, initiate the connection and set up transport
                // to be RTC capable. But do this heavy lifting outside of the UI thread.
                bool result = await Task<bool>.Factory.StartNew(() =>
                {
                    return commModule.SetupTransport(serverUri);
                });
                Diag.DebugPrint("CommModule setup result: " + result);
                if (result == true)
                {
                    ConnectButton.Content = "Disconnect";
                    connectionState = connectionStates.connected;
                }
                else
                {
                    ConnectButton.Content = "failed to connect. click to retry";
                    connectionState = connectionStates.notConnected;
                }
            }
            else if (connectionState == connectionStates.connected)
            {
                await Task.Factory.StartNew(() =>
                {
                    commModule.Reset();
                });

                connectionState = connectionStates.notConnected;
                ConnectButton.Content = "Connect";
            }
        }

        // Try sending the message to the client.
        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Text != "" && commModule != null)
            {
                string message = MessageBox.Text;
                await Task.Factory.StartNew(() =>
                {
                    lock (commModule)
                    {
                        commModule.SendMessage(message);
                    }
                });
            }
        }

    }
}

