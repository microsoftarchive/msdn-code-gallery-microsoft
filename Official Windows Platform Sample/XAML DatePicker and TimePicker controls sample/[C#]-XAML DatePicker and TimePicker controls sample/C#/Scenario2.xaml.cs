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
using Windows.Globalization;
using Windows.Globalization.DateTimeFormatting;

namespace DateAndTimePickers
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario2 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Scenario2()
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
        /// This is the click handler for the 'changeDate' button; the DatePicker value is changed to 1/31/2013.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void changeDate_Click(object sender, RoutedEventArgs e)
        {
            // The DateTimeFormatter class formats dates and times with the user's default settings
            DateTimeFormatter dateFormatter = new DateTimeFormatter("shortdate");

            // A DateTimeOffset instantiated with a DateTime will have its Offset set to the user default
            //    (i.e. the same Offset used to display the DatePicker value)
            this.datePicker.Date = new DateTimeOffset(new DateTime(2013, 1, 31));

            rootPage.NotifyUser("DatePicker date set to " + dateFormatter.Format(this.datePicker.Date), NotifyType.StatusMessage);
        }

        /// <summary>
        /// This is the click handler for the 'changeYearRange' button; the DatePicker year range is changed to [2000, 2020].
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void changeYearRange_Click(object sender, RoutedEventArgs e)
        {
            // MinYear and MaxYear are type DateTimeOffset. We set the month to 2 to avoid time zone issues with January 1. 
            this.datePicker.MinYear = new DateTimeOffset(new DateTime(2000, 2, 1));
            this.datePicker.MaxYear = new DateTimeOffset(new DateTime(2020, 2, 1));
            rootPage.NotifyUser("DatePicker year range set from 2000 to 2020", NotifyType.StatusMessage);
        }
    }
}
