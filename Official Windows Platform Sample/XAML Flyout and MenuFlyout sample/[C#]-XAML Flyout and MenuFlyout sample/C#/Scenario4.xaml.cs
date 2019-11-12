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
    public sealed partial class Scenario4 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;
        int viewCount = 0;

        public Scenario4()
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
        /// This is the click handler for the default button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (b != null)
            {
                rootPage.NotifyUser("You opened a Flyout", NotifyType.StatusMessage);
            }
        }

        private void flyout_Opening(object sender, object e)
        {
            var f = sender as Flyout;
            if (f != null)
            {
                this.viewCount++;
                TextBlock tb = new TextBlock();
                tb.Text = "This Flyout has been opened " + this.viewCount + " time(s).";
                tb.Style = (Style)Application.Current.Resources["BasicTextStyle"];
                f.Content = tb;
            }
        }

    }
}
