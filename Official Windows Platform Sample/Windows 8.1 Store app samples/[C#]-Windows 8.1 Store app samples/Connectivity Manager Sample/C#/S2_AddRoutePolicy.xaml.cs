//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;
using Windows.Networking.Connectivity;
using Windows.Networking;

namespace ConnectivityManager
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class S2_AddRoutePolicy : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public S2_AddRoutePolicy()
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

        private void AddRoutePolicy_Click(object sender, RoutedEventArgs e)
        {
            if (rootPage.g_ConnectionSession == null)
            {
                OutputText.Text = "Please establish a connection using the \"Create Connection\" scenario first.";
                return;
            }

            try
            {
                HostName hostName = new HostName(HostName.Text);
                DomainNameType domainNameType = ParseDomainNameType(((ComboBoxItem)DomainNameTypeComboBox.SelectedItem).Content.ToString());
                RoutePolicy routePolicy = new RoutePolicy(rootPage.g_ConnectionSession.ConnectionProfile, hostName, domainNameType);

                Windows.Networking.Connectivity.ConnectivityManager.AddHttpRoutePolicy(routePolicy);

                OutputText.Text = "Added Route Policy\nTraffic to " + routePolicy.HostName.ToString() +
                    " will be routed through " + routePolicy.ConnectionProfile.ProfileName;
            }
            catch (ArgumentException ex)
            {
                OutputText.Text = "Failed to add Route Policy with HostName = \"" + HostName.Text + "\"\n" +
                    ex.Message;
            }
        }

        private void RemoveRoutePolicy_Click(object sender, RoutedEventArgs e)
        {
            if (rootPage.g_ConnectionSession == null)
            {
                OutputText.Text = "Please establish a connection using the \"Create Connection\" scenario first.";

                return;
            }

            try
            {
                HostName hostName = new HostName(HostName.Text);
                DomainNameType domainNameType = ParseDomainNameType(((ComboBoxItem)DomainNameTypeComboBox.SelectedItem).Content.ToString());
                RoutePolicy routePolicy = new RoutePolicy(rootPage.g_ConnectionSession.ConnectionProfile, hostName, domainNameType);

                Windows.Networking.Connectivity.ConnectivityManager.RemoveHttpRoutePolicy(routePolicy);

                OutputText.Text = "Removed Route Policy\nTraffic to " + routePolicy.HostName.ToString() +
                    " will no longer be routed through " + routePolicy.ConnectionProfile.ProfileName;
            }
            catch (ArgumentException ex)
            {
                OutputText.Text = "Failed to remove Route Policy with HostName = \"" + HostName.Text + "\"\n" +
                    ex.Message;
            }
        }

        private DomainNameType ParseDomainNameType(string input)
        {
            switch (input)
            {
                case "Suffix":
                    return DomainNameType.Suffix;
                default:
                    return DomainNameType.FullyQualified;
            }
        }
    }
}
