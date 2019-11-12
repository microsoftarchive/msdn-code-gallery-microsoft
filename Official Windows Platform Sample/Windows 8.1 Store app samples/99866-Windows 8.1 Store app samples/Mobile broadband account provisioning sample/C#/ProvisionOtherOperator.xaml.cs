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
    public sealed partial class ProvisionOtherOperator : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;
        Util util;

        public ProvisionOtherOperator()
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
        /// This is the click handler for the 'ProvisionOtherOperatorButton' button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ProvisionOtherOperator_Click(object sender, RoutedEventArgs e)
        {
            if (provXmlText.Text == "")
            {
                rootPage.NotifyUser("Provisioning XML cannot be empty", NotifyType.ErrorMessage);
                return;
            }

            string provisioningXML = provXmlText.Text;
            ProvisionOtherOperatorButton.IsEnabled = false;

            try
            {
                // Create provisioning agent
                Windows.Networking.NetworkOperators.ProvisioningAgent provisioningAgent = new Windows.Networking.NetworkOperators.ProvisioningAgent();

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

            ProvisionOtherOperatorButton.IsEnabled = true;
        }
    }
}
