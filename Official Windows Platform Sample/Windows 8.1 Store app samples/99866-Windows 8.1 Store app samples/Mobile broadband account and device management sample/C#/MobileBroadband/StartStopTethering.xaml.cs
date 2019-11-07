//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;
using System.Collections.Generic;
using Windows.UI.Core;
using Windows.Networking.NetworkOperators;
using Windows.ApplicationModel.Background;
using Windows.Storage;

namespace MobileBroadband
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StartStopTethering : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        private CoreDispatcher sampleDispatcher;
        // mobile operator notifiaction completion handler
        BackgroundTaskCompletedEventHandler currentHandler = null;

        // tethering manager 
        NetworkOperatorTetheringManager tetheringManager = null;

        public StartStopTethering()
        {
            this.InitializeComponent();
            sampleDispatcher = Window.Current.CoreWindow.Dispatcher;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            // remove completion handler since the scenario is exiting
            foreach (var cur in BackgroundTaskRegistration.AllTasks)
            {
                if (cur.Value.Name == "MobileOperatorNotificationHandler")
                {
                    cur.Value.Completed -= currentHandler;
                    currentHandler = null;
                }
            }
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // register for NetworkOperatorEvents to handle notifications for this network account
            foreach (var cur in BackgroundTaskRegistration.AllTasks)
            {
                if (cur.Value.Name == "MobileOperatorNotificationHandler")
                {                
                    currentHandler = new BackgroundTaskCompletedEventHandler(OnCompleted);
                    cur.Value.Completed += currentHandler;
                }
            }
            MakeTetheringManagerAvailable();
            // Update UI to reflect tethering state
            UpdateUIWithTetheringState();
        }

        /// <summary>
        /// Load tethering manager if available.
        /// </summary>
        private bool MakeTetheringManagerAvailable()
        {
            if (tetheringManager == null)
            {
                // verify tethering capabilities
                var allAccounts = MobileBroadbandAccount.AvailableNetworkAccountIds;
                if (allAccounts.Count == 0)
                {
                    rootPage.NotifyUser("There is no avaiable network account ID.", NotifyType.ErrorMessage);
                    return false;
                }
                string networkAccountId = allAccounts[0];
                TetheringCapability capabilities = 0;
                try
                {
                    capabilities = NetworkOperatorTetheringManager.GetTetheringCapability(networkAccountId);
                    if (capabilities != TetheringCapability.Enabled)
                    {
                        OnCapabilityError(capabilities);
                    }
                    else
                    {
                        if (tetheringManager == null)
                        {
                            tetheringManager = NetworkOperatorTetheringManager.CreateFromNetworkAccountId(networkAccountId);
                            if (tetheringManager == null)
                            {
                                rootPage.NotifyUser("Failed to create NetworkOperatorTetheringManager object using : " + networkAccountId, NotifyType.ErrorMessage);
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    rootPage.NotifyUser("Unexpected exception occured: " + ex.ToString(), NotifyType.ErrorMessage);
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// update UI using latest tethering state
        /// </summary>
        private void UpdateUIWithTetheringState()
        {
            if (tetheringManager == null)
            {
                StartTetheringButton.IsEnabled = false;
                StopTetheringButton.IsEnabled = false;
                Apply.IsEnabled = false;
            }
            else
            {
                var newState = tetheringManager.TetheringOperationalState;
                Apply.IsEnabled = true;
                switch (newState)
                {
                    case TetheringOperationalState.Unknown:
                        StartTetheringButton.IsEnabled = false;
                        StopTetheringButton.IsEnabled = false;
                        break;
                    case TetheringOperationalState.On:
                        StartTetheringButton.IsEnabled = false;
                        StopTetheringButton.IsEnabled = true;
                        break;
                    case TetheringOperationalState.Off:
                        StartTetheringButton.IsEnabled = true;
                        StopTetheringButton.IsEnabled = false;
                        break;
                    case TetheringOperationalState.InTransition:
                        StartTetheringButton.IsEnabled = false;
                        StopTetheringButton.IsEnabled = false;
                        break;
                }
                if (newState == TetheringOperationalState.On)
                {
                    // query the current number of clients
                    var connectedClients = tetheringManager.ClientCount;
                    rootPage.NotifyUser(connectedClients.ToString() +
                                            " of " +
                                            tetheringManager.MaxClientCount.ToString() +
                                            " are connected to your tethering network",
                                            NotifyType.StatusMessage);
                }
                NetworkOperatorTetheringAccessPointConfiguration current = tetheringManager.GetCurrentAccessPointConfiguration();
                NetworkName.Text = current.Ssid;
                Password.Text = current.Passphrase;
            }
        }

        /// <summary>
        /// handle tethering disabled by capability
        /// </summary>
        /// <param name="capability"></param>
        private void OnCapabilityError(TetheringCapability capability)
        {
            var errorString = "";

            switch (capability)
            {
                case TetheringCapability.DisabledByGroupPolicy:
                    errorString =
                        "Your network administrator has disabled tethering on your machine.";
                    break;
                case TetheringCapability.DisabledByHardwareLimitation:
                    errorString =
                        "Your device hardware doesn't support tethering.";
                    break;
                case TetheringCapability.DisabledByOperator:
                    errorString =
                        "Your Mobile Broadband Operator has disabled tethering on your device.";
                    break;
                case TetheringCapability.DisabledBySku:
                    errorString =
                        "This version of Windows doesn't support tethering.";
                    break;
                case TetheringCapability.DisabledByRequiredAppNotInstalled:
                    errorString =
                        "Required  app is not installed.";
                    break;
                case TetheringCapability.DisabledDueToUnknownCause:
                    errorString =
                        "Unknown issue.";
                    break;
            }

            rootPage.NotifyUser("Unexpected exception occured: " + errorString, NotifyType.ErrorMessage);
            StartTetheringButton.IsEnabled = false;
        }

        /// <summary>
        /// map tethering errors to strings
        /// </summary>
        /// <param name="error"></param>
        public string GetTetheringErrorString(TetheringOperationStatus error)
        {
            var errorString = "";
            switch (error)
            {
                case TetheringOperationStatus.Success:
                    errorString = "No error";
                    break;
                case TetheringOperationStatus.Unknown:
                    errorString = "Unknown error has occurred.";
                    break;
                case TetheringOperationStatus.MobileBroadbandDeviceOff:
                    errorString = "Please make sure your MB device is turned on.";
                    break;
                case TetheringOperationStatus.WiFiDeviceOff:
                    errorString = "Please make sure your WiFi device is turned on.";
                    break;
                case TetheringOperationStatus.EntitlementCheckTimeout:
                    errorString = "We coudn't contact your Mobile Broadband operator to verify your ability to enable tethering, please contact your Mobile Operator.";
                    break;
                case TetheringOperationStatus.EntitlementCheckFailure:
                    errorString = "You Mobile Broadband operator does not allow tethering on this device.";
                    break;
                case TetheringOperationStatus.OperationInProgress:
                    errorString = "The system is busy, please try again later.";
                    break;
            }
            return errorString;
        }

        /// <summary>
        /// start/stop tethering
        /// </summary>
        /// <param name="enable"></param>
        private async void EnableTethering(bool enable)
        {
            try
            {
                NetworkOperatorTetheringOperationResult result;
                if (enable)
                {
                    result = await tetheringManager.StartTetheringAsync();
                }
                else
                {
                    result = await tetheringManager.StopTetheringAsync();
                }
                if (result.Status != TetheringOperationStatus.Success)
                {
                    var errorString = "";
                    if (result.AdditionalErrorMessage != "")
                    {
                        errorString = result.AdditionalErrorMessage;
                    }
                    else
                    {
                        errorString = GetTetheringErrorString(result.Status);
                    }
                    if (enable)
                    {
                        rootPage.NotifyUser("StartTetheringAsync function failed: " + errorString, NotifyType.ErrorMessage);
                    }
                    else
                    {
                        rootPage.NotifyUser("StopTetheringAsync function failed: " + errorString, NotifyType.ErrorMessage);
                    }
                }
                else
                {
                    rootPage.NotifyUser("Operation succeeded.", NotifyType.StatusMessage);
                    UpdateUIWithTetheringState();
                }
            }
            catch (Exception ex)
            {
                rootPage.NotifyUser("Unexpected exception occured: " + ex.ToString(), NotifyType.ErrorMessage);
                return;
            }
        }

        private void StopTetheringButton_Click(object sender, RoutedEventArgs e)
        {
            rootPage.NotifyUser("Stopping...", NotifyType.StatusMessage);
            EnableTethering(false);
        }

        private void StartTetheringButton_Click(object sender, RoutedEventArgs e)
        {
            rootPage.NotifyUser("Starting...", NotifyType.StatusMessage);
            EnableTethering(true);
        }

        private async void Apply_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // read the new configuration from the UI
                NetworkOperatorTetheringAccessPointConfiguration newConfiguration =
                            new NetworkOperatorTetheringAccessPointConfiguration();
                newConfiguration.Ssid = NetworkName.Text;
                newConfiguration.Passphrase = Password.Text;
                await tetheringManager.ConfigureAccessPointAsync(newConfiguration);
                rootPage.NotifyUser("Operation completed.", NotifyType.StatusMessage);
            }
            catch (Exception ex)
            {
                rootPage.NotifyUser("Unexpected exception occured: " + ex.ToString(), NotifyType.ErrorMessage);
                return;
            }
        }

        /// <summary>
        /// background task completion handler
        /// 
        /// </summary>
        private async void OnCompleted(IBackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs e)
        {
            // Update the UI with the completion status reported by the background task.
            // Dispatch an anonymous task to the UI thread to do the update.
            await sampleDispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    try
                    {
                        e.CheckResult();

                        // get the completion status
                        var key = sender.TaskId.ToString() + "_type";
                        var settings = ApplicationData.Current.LocalSettings;
                        var status = settings.Values[key].ToString();
                        if ((status == NetworkOperatorEventMessageType.TetheringOperationalStateChanged.ToString()) ||
                            (status == NetworkOperatorEventMessageType.TetheringNumberOfClientsChanged.ToString()))
                        {
                            UpdateUIWithTetheringState();
                        }
                    }
                    catch (Exception ex)
                    {
                        rootPage.NotifyUser("Unexpected exception occured: " + ex.ToString(), NotifyType.ErrorMessage);
                    }
                });
        }

    }
}

