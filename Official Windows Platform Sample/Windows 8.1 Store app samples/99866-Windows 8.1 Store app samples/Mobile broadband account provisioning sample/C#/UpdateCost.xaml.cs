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
using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.Networking.NetworkOperators;

namespace ProvisioningAgent
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UpdateCost : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;
        Util util;
        ProfileMediaType profileMediaType;
        NetworkCostType networkCostType;

        public UpdateCost()
        {
            this.InitializeComponent();
            util = new Util();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            mediaType.SelectionChanged += new SelectionChangedEventHandler(MediaType_SelectionChanged);
            mediaType.SelectedItem = mediaType_Wlan;

            networkCostCategory.SelectionChanged += new SelectionChangedEventHandler(NetworkCostCategory_SelectionChanged);
            networkCostCategory.SelectedItem = cost_unknown;
        }

        private void MediaType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (mediaType.SelectedItem == mediaType_Wlan)
            {
                profileMediaType = ProfileMediaType.Wlan;
            }
            else if (mediaType.SelectedItem == mediaType_Wwan)
            {
                profileMediaType = ProfileMediaType.Wwan;
            }
        }

        private void NetworkCostCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (networkCostCategory.SelectedItem == cost_unknown)
            {
                networkCostType = NetworkCostType.Unknown;
            }
            else if (networkCostCategory.SelectedItem == cost_unrestricted)
            {
                networkCostType = NetworkCostType.Unrestricted;
            }
            else if (networkCostCategory.SelectedItem == cost_fixed)
            {
                networkCostType = NetworkCostType.Fixed;
            }
            else if (networkCostCategory.SelectedItem == cost_variable)
            {
                networkCostType = NetworkCostType.Variable;
            }
        }

        /// <summary>
        /// This is the click handler for the 'UpdateCostButton' button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateCost_Click(object sender, RoutedEventArgs e)
        {
            if (profileNameText.Text == "")
            {
                rootPage.NotifyUser("Profile name cannot be empty", NotifyType.ErrorMessage);
                return;
            }

            string profileName = profileNameText.Text;

            try
            {
                // Get the network account ID.
                IReadOnlyList<string> networkAccIds = Windows.Networking.NetworkOperators.MobileBroadbandAccount.AvailableNetworkAccountIds;

                if (networkAccIds.Count == 0)
                {
                    rootPage.NotifyUser("No network account ID found", NotifyType.ErrorMessage);
                    return;
                }

                UpdateCostButton.IsEnabled = false;

                // For the sake of simplicity, assume we want to use the first account.
                // Refer to the MobileBroadbandAccount API's how to select a specific account ID.
                string networkAccountId = networkAccIds[0];

                // Create provisioning agent for specified network account ID
                Windows.Networking.NetworkOperators.ProvisioningAgent provisioningAgent =
                   Windows.Networking.NetworkOperators.ProvisioningAgent.CreateFromNetworkAccountId(networkAccountId);

                // Retrieve associated provisioned profile
                ProvisionedProfile provisionedProfile = provisioningAgent.GetProvisionedProfile(profileMediaType, profileName);

                // Set the new cost
                provisionedProfile.UpdateCost(networkCostType);

                rootPage.NotifyUser("Profile " + profileName + " has been updated with the cost type as " + networkCostType, NotifyType.StatusMessage);
            }
            catch (Exception ex)
            {
                rootPage.NotifyUser("Unexpected exception occured: " + ex.ToString(), NotifyType.ErrorMessage);
            }

            UpdateCostButton.IsEnabled = true;
        }
    }
}
