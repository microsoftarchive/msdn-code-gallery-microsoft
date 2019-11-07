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

namespace Flyouts
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario1 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Scenario1()
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

        private void flyout_Opened(object sender, object e)
        {
            Flyout f = sender as Flyout;
            if (f != null)
            {
                rootPage.NotifyUser("You opened a Flyout", NotifyType.StatusMessage);
            }
        }

        private void menuFlyout_Opened(object sender, object e)
        {
            MenuFlyout m = sender as MenuFlyout;
            if (m != null)
            {
                rootPage.NotifyUser("You opened a MenuFlyout", NotifyType.StatusMessage);
            }
        }

        private void confirmPurchase_Click(object sender, RoutedEventArgs e)
        {
            Flyout f = this.buttonWithFlyout.Flyout as Flyout;
            if (f != null)
            {
                f.Hide();

                rootPage.NotifyUser("You bought an item!", NotifyType.StatusMessage);
            }
        }

        private void option_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem item = sender as MenuFlyoutItem;
            if (item != null)
            {
                rootPage.NotifyUser("You selected option '" + item.Text + "'", NotifyType.StatusMessage);
            }
        }

        private void enable_Click(object sender, RoutedEventArgs e)
        {
            ToggleMenuFlyoutItem item = sender as ToggleMenuFlyoutItem;
            if (item != null)
            {
                if (item.IsChecked)
                {
                    rootPage.NotifyUser("You enabled X", NotifyType.StatusMessage);
                }
                else
                {
                    rootPage.NotifyUser("You disabled X", NotifyType.StatusMessage);
                }
            }
        }
    }
}
