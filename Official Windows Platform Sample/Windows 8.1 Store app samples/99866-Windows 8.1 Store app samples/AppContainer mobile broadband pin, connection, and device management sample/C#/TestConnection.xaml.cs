//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;

namespace MobileBroadbandComApi
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TestConnection : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        private MBApiImplementation mbApiInstance;

        public TestConnection()
        {
            this.InitializeComponent();

            // Get the MB API instance
            mbApiInstance = MBApiImplementation.GetInstance();
            mbApiInstance.EnableScenarioButtons += new MBApiImplementation.EnableScenarioButtonsHandler(OnEnableScenarioButtons);
            mbApiInstance.DisableScenarioButtons += new MBApiImplementation.DisableScenarioButtonsHandler(OnDisableScenarioButtons);

            // Initialize MBN managers
            mbApiInstance.InitializeManagers();
            
            // Register for app suspend and resume handlers
            App.Current.Suspending += new SuspendingEventHandler(AppSuspending);
            App.Current.Resuming += new System.EventHandler<object>(AppResuming);
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Register for button handlers
            ConnectDisconnectButton.Click += new RoutedEventHandler(ConnectDisconnectButton_Click);
            ConnectDisconnect.SelectionChanged += new SelectionChangedEventHandler(ConnectDisconnect_SelectionChanged);
            ConnectDisconnect.SelectedItem = ConnectWithXml;

            subscriberText.Text = "";
            accessStringText.Text = "";
            userNameText.Text = "";
            passwordText.Text = "";
        }

        private void OnEnableScenarioButtons(object sender, EventArgs args)
        {
            // Enable scenario buttons
            ConnectDisconnectButton.IsEnabled = true;
        }

        private void OnDisableScenarioButtons(object sender, EventArgs args)
        {
            // Disable scenario buttons
            ConnectDisconnectButton.IsEnabled = false;
        }

        void AppSuspending(object sender, Windows.ApplicationModel.SuspendingEventArgs args)
        {
            mbApiInstance.EnableScenarioButtons -= new MBApiImplementation.EnableScenarioButtonsHandler(OnEnableScenarioButtons);
            mbApiInstance.DisableScenarioButtons -= new MBApiImplementation.DisableScenarioButtonsHandler(OnDisableScenarioButtons);
        }

        void AppResuming(object sender, object e)
        {
            mbApiInstance.EnableScenarioButtons += new MBApiImplementation.EnableScenarioButtonsHandler(OnEnableScenarioButtons);
            mbApiInstance.DisableScenarioButtons += new MBApiImplementation.DisableScenarioButtonsHandler(OnDisableScenarioButtons);
        }

        /// <summary>
        /// This is the click handler for the 'ConnectDisconnectButton' button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConnectDisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            if (ConnectDisconnect.SelectedItem == ConnectWithXml)
            {
                if (subscriberText.Text == "" ||
                    accessStringText.Text == "")
                {
                    rootPage.NotifyUser("Fields cannot be empty", NotifyType.ErrorMessage);
                    return;
                }

                ConnectDisconnectButton.IsEnabled = false;

                // Issue connect
                mbApiInstance.Connect(
                                subscriberText.Text,
                                accessStringText.Text,
                                userNameText.Text,
                                passwordText.Text);
                ConnectDisconnectButton.IsEnabled = true;
            }
            else if (ConnectDisconnect.SelectedItem == Disconnect)
            {
                ConnectDisconnectButton.IsEnabled = false;

                // Issue disconnect
                mbApiInstance.Disconnect();
                ConnectDisconnectButton.IsEnabled = true;
            }
        }

        void ConnectDisconnect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ConnectDisconnect.SelectedItem == ConnectWithXml)
            {
                EntersubscriberIDText.Visibility = Visibility.Visible;
                EntersubscriberIDTextBox.Visibility = Visibility.Visible;
                EnterAccessStringText.Visibility = Visibility.Visible;
                EnterAccessStringTextBox.Visibility = Visibility.Visible;
                EnterUserNameText.Visibility = Visibility.Visible;
                EnterUserNameTextBox.Visibility = Visibility.Visible;
                EnterPasswordText.Visibility = Visibility.Visible;
                EnterPasswordTextBox.Visibility = Visibility.Visible;

                Scenario2ConnectDisconnectButton.Visibility = Visibility.Visible;
            }
            else if (ConnectDisconnect.SelectedItem == Disconnect)
            {
                EntersubscriberIDText.Visibility = Visibility.Collapsed;
                EntersubscriberIDTextBox.Visibility = Visibility.Collapsed;
                EnterAccessStringText.Visibility = Visibility.Collapsed;
                EnterAccessStringTextBox.Visibility = Visibility.Collapsed;
                EnterUserNameText.Visibility = Visibility.Collapsed;
                EnterUserNameTextBox.Visibility = Visibility.Collapsed;
                EnterPasswordText.Visibility = Visibility.Collapsed;
                EnterPasswordTextBox.Visibility = Visibility.Collapsed;
            }
        }
    }
}
