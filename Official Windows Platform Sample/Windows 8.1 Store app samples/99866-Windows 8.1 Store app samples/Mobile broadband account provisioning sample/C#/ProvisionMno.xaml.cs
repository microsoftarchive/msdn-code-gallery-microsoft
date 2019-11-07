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
using Windows.Networking.NetworkOperators;

namespace ProvisioningAgent
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProvisionMno : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;
        Util util;

        public ProvisionMno()
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
        }


        /// <summary>
        /// This is the click handler for the 'ProvisionMnoButton' button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ProvisionMno_Click(object sender, RoutedEventArgs e)
        {
            if (provXmlText.Text == "")
            {
                rootPage.NotifyUser("Provisioning XML cannot be empty", NotifyType.ErrorMessage);
                return;
            }

            string provisioningXML = provXmlText.Text;

            try
            {
                // Get the network account ID.
                IReadOnlyList<string> networkAccIds = Windows.Networking.NetworkOperators.MobileBroadbandAccount.AvailableNetworkAccountIds;

                if (networkAccIds.Count == 0)
                {
                    rootPage.NotifyUser("No network account ID found", NotifyType.ErrorMessage);
                    return;
                }

                ProvisionMnoButton.IsEnabled = false;

                // For the sake of simplicity, assume we want to use the first account.
                // Refer to the MobileBroadbandAccount API's how to select a specific account ID.
                string networkAccountId = networkAccIds[0];

                // Create provisioning agent for specified network account ID
                Windows.Networking.NetworkOperators.ProvisioningAgent provisioningAgent =
                    Windows.Networking.NetworkOperators.ProvisioningAgent.CreateFromNetworkAccountId(networkAccountId);

                // Provision using XML
                ProvisionFromXmlDocumentResults result = await provisioningAgent.ProvisionFromXmlDocumentAsync(provisioningXML);

                if (result.AllElementsProvisioned)
                {
                    // Provisioning is done successfully
                    rootPage.NotifyUser("Device was successfully configured", NotifyType.StatusMessage);
                }
                else
                {
                    // Error has occured during provisioning
                    // And hence displaying result XML containing
                    // errors
                    rootPage.NotifyUser(util.ParseResultXML(result.ProvisionResultsXml), NotifyType.StatusMessage);
                }
            }
            catch (Exception ex)
            {
                rootPage.NotifyUser("Unexpected exception occured: " + ex.ToString(), NotifyType.ErrorMessage);
            }

            ProvisionMnoButton.IsEnabled = true;
        }
    }
}
