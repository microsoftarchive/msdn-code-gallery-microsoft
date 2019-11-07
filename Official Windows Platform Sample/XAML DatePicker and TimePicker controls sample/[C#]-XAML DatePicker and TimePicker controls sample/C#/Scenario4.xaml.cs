// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;

namespace DateAndTimePickers
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario4 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Scenario4()
        {
            this.InitializeComponent();
            this.toggleYear.IsChecked = true;
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
        /// This is the click handler for the 'showDayOfWeek' button.  The DatePicker is formatted to include days of the week.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showDayOfWeek_Click(object sender, RoutedEventArgs e)
        {
            // Explicitly set day format
            this.datePicker.DayFormat = "{day.integer} ({dayofweek.full})";

            rootPage.NotifyUser("DatePicker with format changed to include day of week", NotifyType.StatusMessage);
        }

        /// <summary>
        /// This is the click handler for the 'showMonthsAsNumber' button.  The months are formatted to be displayed as integers.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showMonthAsNumber_Click(object sender, RoutedEventArgs e)
        {
            // Explicitly set month format
            this.datePicker.MonthFormat = "{month.integer}";

            rootPage.NotifyUser("DatePicker with format changed to display month as a number", NotifyType.StatusMessage);
        }

        /// <summary>
        /// This is the click handler for the 'toggleYear' button and toggles the visibility of the year component of the DatePicker.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toggleYear_Update(object sender, RoutedEventArgs e)
        {
            // Explicitly set visibility of year
            if ((bool)this.toggleYear.IsChecked)
            {
                this.datePicker.YearVisible = true;
                rootPage.NotifyUser("DatePicker with visible year component", NotifyType.StatusMessage);
            }
            else
            {
                this.datePicker.YearVisible = false;
                rootPage.NotifyUser("DatePicker without visible year component", NotifyType.StatusMessage);
            }
        }
    }
}
