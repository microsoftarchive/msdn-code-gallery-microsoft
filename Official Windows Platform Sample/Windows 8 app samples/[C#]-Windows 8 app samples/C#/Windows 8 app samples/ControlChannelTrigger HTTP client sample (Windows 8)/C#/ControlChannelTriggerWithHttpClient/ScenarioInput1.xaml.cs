// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved


using DiagnosticsHelper;
using HttpControlChannelTrigger;
using HttpTransportHelper;
using System;
using System.Linq;
using System.Collections.Generic;
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

namespace HttpControlChannelTrigger
{
    public sealed partial class ScenarioInput1 : Page, IDisposable
    {
        // A pointer back to the main page which is used to gain access to the input and output frames and their content.
        MainPage rootPage = null;
        CoreDispatcher coreDispatcher;
        CommModule commModule;
        bool lockScreenAdded = false;
        Page outputFrame;
        TextBlock debugOutputTextBlock;
        private static Guid NetworkTaskRegistrationGuid = Guid.Empty;
        Uri serverUri;

        public ScenarioInput1()
        {
            InitializeComponent();
            coreDispatcher = Diag.coreDispatcher = Window.Current.Dispatcher;
            ConnectButton.Visibility = Visibility.Collapsed;
        }

        public void Dispose()
        {
            if (commModule != null)
            {
                commModule.Dispose();
                commModule = null;
            }
        }

        #region Template-Related Code - Do not remove
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Get a pointer to our main page
            rootPage = e.Parameter as MainPage;

            // We want to be notified with the OutputFrame is loaded so we can get to the content.
            rootPage.OutputFrameLoaded += new System.EventHandler(rootPage_OutputFrameLoaded);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            rootPage.OutputFrameLoaded -= new System.EventHandler(rootPage_OutputFrameLoaded);
        }

        #endregion

        #region Use this code if you need access to elements in the output frame - otherwise delete
        void rootPage_OutputFrameLoaded(object sender, object e)
        {
            // At this point, we know that the Output Frame has been loaded and we can go ahead
            // and reference elements in the page contained in the Output Frame.

            // Get a pointer to the content within the OutputFrame.
            outputFrame = (Page)rootPage.OutputFrame.Content;

            // Go find the elements that we need for this scenario.
            debugOutputTextBlock = Diag.debugOutputTextBlock = outputFrame.FindName("DebugTextBlock") as TextBlock;
        }

        #endregion

        #region Click Handlers

        private void ClientRole_Click(object sender, RoutedEventArgs e)
        {
            // To keep things simple, this button is disabled once clicked.
            Diag.DebugPrint("Client role selected");
            ClientRoleButton.IsEnabled = false;
            ClientInit();
        }
        #endregion

        async void ClientInit()
        {
            Diag.DebugPrint("Initializing client");

            commModule = new CommModule();

            // Lock screen is required to let in-process RealTimeCommunication related
            // background code to execute.
            if (!lockScreenAdded)
            {
                BackgroundAccessStatus status = await BackgroundExecutionManager.RequestAccessAsync();
                Diag.DebugPrint("Lock screen status" + status);

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

                        // App should switch to polling mode (example: poll for email based on time triggers)
                        Diag.DebugPrint("As Lockscreen status was Denied, App should switch to polling mode such as email based on time triggers.");
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
        private static bool sendingRequest = false;

        // Registers a background task with an network change system trigger.
        private void RegisterNetworkChangeTask()
        {
            try
            {
                if (NetworkTaskRegistrationGuid != Guid.Empty)
                {
                    IReadOnlyDictionary<Guid, IBackgroundTaskRegistration> allTasks = BackgroundTaskRegistration.AllTasks;
                    if (allTasks.ContainsKey(NetworkTaskRegistrationGuid))
                    {
                        Diag.DebugPrint("Network task is already registered.");
                        return;
                    }
                }

                BackgroundTaskBuilder myTaskBuilder = new BackgroundTaskBuilder();
                SystemTrigger myTrigger = new SystemTrigger(SystemTriggerType.NetworkStateChange, false);
                myTaskBuilder.SetTrigger(myTrigger);
                myTaskBuilder.TaskEntryPoint = "Background.NetworkChangeTask";
                myTaskBuilder.Name = "Network change task";
                BackgroundTaskRegistration myTask = myTaskBuilder.Register();
                NetworkTaskRegistrationGuid = myTask.TaskId;
            }
            catch (Exception exp)
            {
                Diag.DebugPrint("Exception caught while setting up system event" + exp.ToString());
            }
        }

        async private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            if (connectionState == ConnectionStates.NotConnected)
            {
                if (!Uri.TryCreate(ServerUri.Text.Trim(), UriKind.Absolute, out serverUri))
                {
                    Diag.DebugPrint("Please provide valid uri input.");
                    return;
                }

                ConnectButton.Content = "Connecting...";
                connectionState = ConnectionStates.Connecting;

                // Register for network status change notification
                RegisterNetworkChangeTask();

                // Finally, initiate the connection and set up transport
                // to be CCT capable. But do this heavy lifting outside of the UI thread.
                bool result = await Task<bool>.Factory.StartNew(() =>
                {
                    return commModule.SetupTransport(serverUri);
                });
                Diag.DebugPrint("CommModule setup result: " + result);
                if (result == true)
                {
                    ConnectButton.Content = "Disconnect";
                    connectionState = ConnectionStates.Connected;
                }
                else
                {
                    ConnectButton.Content = "failed to connect. click to retry";
                    connectionState = ConnectionStates.NotConnected;
                }
            }
            else if (connectionState == ConnectionStates.Connected)
            {
                await Task.Factory.StartNew(() =>
                {
                    commModule.Reset();
                });

                connectionState = ConnectionStates.NotConnected;
                ConnectButton.Content = "Connect";
            }
        }


        // Try sending the message to the Server.
        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (commModule == null)
                {
                    SendButton.Content = "Connect again then click here to send the request.";
                    return;
                }
                if (sendingRequest)
                {
                    SendButton.Content = "Still sending previous request, please wait.";
                    return;
                }

                SendButton.Content = "Sending...";
                sendingRequest = true;

                // Send http request. But do this heavy lifting outside of the UI thread.
                bool result = await Task<bool>.Factory.StartNew(() =>
                {
                    return commModule.SendHttpQuery();
                });
                if (!result)
                {
                    Diag.DebugPrint("Failed to send HttpRequest. Please try again.");
                    SendButton.Content = "Failed to send. Please try again";
                }
                else
                {
                    SendButton.Content = "SendRequest";
                }
                sendingRequest = false;
            }
            catch (Exception ex)
            {
                SendButton.Content = "Failed to send. Please try again";
                sendingRequest = false;
                Diag.DebugPrint("Exception occured while sending HttpRequest. Exception: " + ex.ToString());
            }
        }
    }
}


