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
    public sealed partial class LanId : Page
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public LanId()
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
        /// This is the click handler for the 'Default' button.  You would replace this with your own handler
        /// if you have a button or buttons on this page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LanId_Click(object sender, RoutedEventArgs e)
        {
            //
            //Display Lan Identifiers - Infrastructure ID, Port ID, Network Adapter ID
            //
            string lanIdentifierData = string.Empty;
            try
            {
                var lanIdentifiers = NetworkInformation.GetLanIdentifiers();
                if (lanIdentifiers.Count != 0)
                {
                    lanIdentifierData = "Number of Lan Identifiers retrieved: " + lanIdentifiers.Count + "\n";
                    lanIdentifierData += "=============================================\n";
                    for (var i = 0; i < lanIdentifiers.Count; i++)
                    {
                        //Display Lan Identifier data for each identifier
                        lanIdentifierData += GetLanIdentifierData(lanIdentifiers[i]);
                        lanIdentifierData += "------------------------------------------------\n";
                    }

                    OutputText.Text = lanIdentifierData;
                    rootPage.NotifyUser("Success", NotifyType.StatusMessage);
                }
                else
                {
                    rootPage.NotifyUser("No Lan Identifier Data found", NotifyType.StatusMessage);
                }
            }
            catch (Exception ex)
            {
                rootPage.NotifyUser("Unexpected exception occurred: " + ex.ToString(), NotifyType.ErrorMessage);
            }
        }

        //
        //Get Lan Identifier Data
        //
        string GetLanIdentifierData(LanIdentifier lanIdentifier)
        {
            string lanIdentifierData = string.Empty;
            if (lanIdentifier == null)
            {
                return lanIdentifierData;
            }

            if (lanIdentifier.InfrastructureId != null)
            {
                lanIdentifierData += "Infrastructure Type: " + lanIdentifier.InfrastructureId.Type + "\n";
                lanIdentifierData += "Infrastructure Value: ";
                var infrastructureIdValue = lanIdentifier.InfrastructureId.Value;
                foreach (var value in infrastructureIdValue)
                {
                    lanIdentifierData += value + " ";
                }
            }

            if (lanIdentifier.PortId != null)
            {
                lanIdentifierData += "\nPort Type : " + lanIdentifier.PortId.Type + "\n";
                lanIdentifierData += "Port Value: ";
                var portIdValue = lanIdentifier.PortId.Value;
                foreach (var value in portIdValue)
                {
                    lanIdentifierData += value + " ";
                }
            }

            if (lanIdentifier.NetworkAdapterId != null)
            {
                lanIdentifierData += "\nNetwork Adapter Id : " + lanIdentifier.NetworkAdapterId + "\n";
            }
            return lanIdentifierData;
        }
    }
}
