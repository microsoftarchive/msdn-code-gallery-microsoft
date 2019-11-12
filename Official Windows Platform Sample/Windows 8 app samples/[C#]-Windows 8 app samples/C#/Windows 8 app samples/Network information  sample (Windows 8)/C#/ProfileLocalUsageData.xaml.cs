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

namespace NetworkInformationApi
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProfileLocalUsageData : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public ProfileLocalUsageData()
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
        }


        /// <summary>
        /// This is the click handler for the 'ProfileLocalUsageDataButton' button.  You would replace this with your own handler
        /// if you have a button or buttons on this page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProfileLocalUsageData_Click(object sender, RoutedEventArgs e)
        {      
            //
            //Get Internet Connection Profile and display local data usage for the profile for the past 1 hour
            //
            string localDataUsage = string.Empty;
            DateTime CurrTime = DateTime.Now;
            TimeSpan TimeDiff = new TimeSpan(1, 0, 0);

            try
            {
                ConnectionProfile InternetConnectionProfile = NetworkInformation.GetInternetConnectionProfile();

                if (InternetConnectionProfile == null)
                {
                    rootPage.NotifyUser("Not connected to Internet\n", NotifyType.StatusMessage);
                }
                else
                {
                    var LocalUsage = InternetConnectionProfile.GetLocalUsage(CurrTime.Subtract(TimeDiff), CurrTime);

                    localDataUsage = "Local Data Usage:\n";
                    localDataUsage += " Bytes Sent     : " + LocalUsage.BytesSent + "\n";
                    localDataUsage += " Bytes Received : " + LocalUsage.BytesReceived + "\n";
                    rootPage.NotifyUser(localDataUsage, NotifyType.StatusMessage);
                }
            }
            catch (Exception ex)
            {
                rootPage.NotifyUser("Unexpected exception occured: " + ex.ToString(), NotifyType.ErrorMessage);
            }
        }
    }
}
