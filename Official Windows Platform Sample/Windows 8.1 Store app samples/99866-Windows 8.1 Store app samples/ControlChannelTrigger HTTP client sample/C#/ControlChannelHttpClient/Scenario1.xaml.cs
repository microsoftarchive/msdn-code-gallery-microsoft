// Copyright (c) Microsoft Corporation. All rights reserved.

using DiagnosticsHelper;
using HttpClientTransportHelper;
using SDKTemplate;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ControlChannelHttpClient
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario1 : SDKTemplate.Common.LayoutAwarePage, IDisposable
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;
        CoreDispatcher coreDispatcher;
        CommunicationModule communicationModule;
        bool lockScreenAdded = false;
        TextBlock debugOutputTextBlock;
        Uri serverUri;

        public Scenario1()
        {
            this.InitializeComponent();
            coreDispatcher = Diag.coreDispatcher = Window.Current.Dispatcher;
            ConnectButton.Visibility = Visibility.Collapsed;
        }

        public void Dispose()
        {
            if (communicationModule != null)
            {
                communicationModule.Dispose();
                communicationModule = null;
            }
        }        

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            debugOutputTextBlock = Diag.debugOutputTextBlock = DebugTextBlock;

            // Initialize the client.
            InitializeClient();

            // Register for network status change notifications.
            RegisterNetworkChangeTask();
        }

        async void InitializeClient()
        {
            Diag.DebugPrint("Initializing client");

            communicationModule = new CommunicationModule();

            // Lock screen is required to allow in-process RealTimeCommunication related
            // background code to execute.
            if (!lockScreenAdded)
            {
                BackgroundAccessStatus status = await BackgroundExecutionManager.RequestAccessAsync();
                Diag.DebugPrint("Lock screen status: " + status);

                switch (status)
                {
                    case BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity:
                        // App is allowed to use RealTimeConnection  
                        // functionality even in low power mode.
                        lockScreenAdded = true;
                        break;
                    case BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity:
                        // App is allowed to use RealTimeConnection  
                        // functionality but not in low power mode.
                        lockScreenAdded = true;
                        break;
                    case BackgroundAccessStatus.Denied:
                        Diag.DebugPrint("Lock screen status was denied.");
                        break;
                }
            }

            if (lockScreenAdded)
            {
                // Now, enable the client settings.
                ClientSettings.Visibility = Visibility.Visible;
                ConnectButton.Visibility = Visibility.Visible;
            }

            Diag.DebugPrint("Initializing client done");
            return;
        }

        private enum ConnectionStates
        {
            NotConnected = 0,
            Connecting = 1,
            Connected = 2,
        };

        private static ConnectionStates connectionState = ConnectionStates.NotConnected;

        // Registers a background task with an network change system trigger.
        private void RegisterNetworkChangeTask()
        {
            try
            {
                // Delete previously registered network status change tasks as
                // the background triggers are persistent by nature across process
                // lifetimes.
                foreach (var task in BackgroundTaskRegistration.AllTasks)
                {
                    Diag.DebugPrint("Deleting background task " + task.Value.Name);
                    task.Value.Unregister(true);
                }

                BackgroundTaskBuilder myTaskBuilder = new BackgroundTaskBuilder();
                SystemTrigger myTrigger = new SystemTrigger(SystemTriggerType.NetworkStateChange, false);
                myTaskBuilder.SetTrigger(myTrigger);
                myTaskBuilder.TaskEntryPoint = "BackgroundTaskHelper.NetworkChangeTask";
                myTaskBuilder.Name = "Network change task";
                BackgroundTaskRegistration myTask = myTaskBuilder.Register();
            }
            catch (Exception ex)
            {
                Diag.DebugPrint("Exception caught while setting up system event: " + ex.ToString());
            }
        }

        async private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            if (connectionState == ConnectionStates.NotConnected)
            {
                if (!Uri.TryCreate(ServerUri.Text.Trim(), UriKind.Absolute, out serverUri))
                {
                    Diag.DebugPrint("Please provide a valid URI input.");
                    return;
                }

                ConnectButton.Content = "Connecting...";
                connectionState = ConnectionStates.Connecting;

                // Finally, initiate the connection and set up transport
                // to be CCT capable. But do this heavy lifting outside of the UI thread.
                bool result = await Task<bool>.Factory.StartNew(() =>
                {
                    return communicationModule.SetUpTransport(serverUri, GetType().Name);
                });
                Diag.DebugPrint("CommunicationModule setup result: " + result);
                if (result == true)
                {
                    ConnectButton.Content = "Disconnect";
                    connectionState = ConnectionStates.Connected;
                }
                else
                {
                    ConnectButton.Content = "Failed to connect. Click to retry.";
                    connectionState = ConnectionStates.NotConnected;
                }
            }
            else if (connectionState == ConnectionStates.Connected)
            {
                await Task.Factory.StartNew(() =>
                {
                    communicationModule.Reset();
                });

                connectionState = ConnectionStates.NotConnected;
                ConnectButton.Content = "Connect";
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            DebugTextBlock.Text = "";
        }
    }
}
