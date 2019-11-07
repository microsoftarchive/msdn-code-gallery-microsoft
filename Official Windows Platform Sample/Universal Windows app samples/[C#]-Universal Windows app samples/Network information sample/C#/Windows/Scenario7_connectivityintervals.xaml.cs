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
using Windows.Networking.Connectivity;
using Windows.Foundation;

namespace NetworkInformationApi
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProfileConnectivityIntervals : Page
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        // These are set in the UI
        private ConnectionProfile InternetConnectionProfile;
        private NetworkUsageStates NetworkUsageStates;
        private DateTimeOffset StartTime;
        private DateTimeOffset EndTime;

        public ProfileConnectivityIntervals()
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
            InternetConnectionProfile = NetworkInformation.GetInternetConnectionProfile();

            NetworkUsageStates = new NetworkUsageStates();
            StartTimePicker.Time -= TimeSpan.FromHours(1);
        }

        private string PrintConnectivityInterval(ConnectivityInterval connectivityInterval)
        {
            string result = "Connectivity Interval from " + connectivityInterval.StartTime + " to " + (connectivityInterval.StartTime + connectivityInterval.ConnectionDuration) + "\n";

            return result;
        }

        // This is used to print to OutputText while running outside of the UI thread
        async void PrintOutputAsync(string output)
        {
            await OutputText.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                OutputText.Text = output;
            });
        }

        // This is used to print a status message while running outside of the UI thread
        async void PrintStatusAsync(string output)
        {
            await rootPage.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                rootPage.NotifyUser(output, NotifyType.StatusMessage);
            });
        }

        // This is used to print an error message while running outside of the UI thread
        async void PrintErrorAsync(string output)
        {
            await rootPage.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                rootPage.NotifyUser(output, NotifyType.ErrorMessage);
            });
        }

        private void GetConnectivityIntervalsAsyncHandler(IAsyncOperation<IReadOnlyList<ConnectivityInterval>> asyncInfo, AsyncStatus asyncStatus)
        {
            if (asyncStatus == AsyncStatus.Completed)
            {
                try
                {
                    String outputString = string.Empty;
                    IReadOnlyList<ConnectivityInterval> connectivityIntervals = asyncInfo.GetResults();

                    if (connectivityIntervals == null)
                    {
                        PrintErrorAsync("The Start Time cannot be later than the End Time, or in the future");
                        return;
                    }

                    // Get the NetworkUsage for each ConnectivityInterval
                    foreach (ConnectivityInterval connectivityInterval in connectivityIntervals)
                    {
                        outputString += PrintConnectivityInterval(connectivityInterval);
                    }

                    PrintOutputAsync(outputString);
                    PrintStatusAsync("Success");
                }
                catch (Exception ex)
                {
                    PrintErrorAsync("An unexpected error occurred: " + ex.Message);
                }
            }
            else
            {
                PrintErrorAsync("GetConnectivityIntervalsAsync failed with message:\n" + asyncInfo.ErrorCode.Message);
            }
        }

        private TriStates ParseTriStates(string input)
        {
            switch (input)
            {
                case "Yes":
                    return TriStates.Yes;
                case "No":
                    return TriStates.No;
                default:
                    return TriStates.DoNotCare;
            }
        }

        /// <summary>
        /// This is the click handler for the 'ProfileLocalUsageDataButton' button.  You would replace this with your own handler
        /// if you have a button or buttons on this page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProfileConnectivityIntervalsButton_Click(object sender, RoutedEventArgs e)
        {      
            //
            //Get Internet Connection Profile and display local data usage for the profile for the past 1 hour
            //

            try
            {
                NetworkUsageStates.Roaming = ParseTriStates(((ComboBoxItem)RoamingComboBox.SelectedItem).Content.ToString());
                NetworkUsageStates.Shared = ParseTriStates(((ComboBoxItem)SharedComboBox.SelectedItem).Content.ToString());
                StartTime = (StartDatePicker.Date.Date + StartTimePicker.Time);
                EndTime = (EndDatePicker.Date.Date + EndTimePicker.Time);

                if (InternetConnectionProfile == null)
                {
                    rootPage.NotifyUser("Not connected to Internet\n", NotifyType.StatusMessage);
                }
                else
                {
                    InternetConnectionProfile.GetConnectivityIntervalsAsync(StartTime,
                        EndTime, NetworkUsageStates).Completed = GetConnectivityIntervalsAsyncHandler;
                }
            }
            catch (Exception ex)
            {
                rootPage.NotifyUser("Unexpected exception occurred: " + ex.ToString(), NotifyType.ErrorMessage);
            }
        }
    }
}