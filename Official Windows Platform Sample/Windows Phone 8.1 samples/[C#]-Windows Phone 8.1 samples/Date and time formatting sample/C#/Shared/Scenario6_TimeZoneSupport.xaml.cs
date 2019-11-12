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
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Globalization;
using Windows.Globalization.DateTimeFormatting;

namespace DateTimeFormatting
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TimeZoneSupport: Page
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public TimeZoneSupport()
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
        /// This is the click handler for the 'Display' button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Display_Click(object sender, RoutedEventArgs e)
        {
            // This scenario illustrates TimeZone support in DateTimeFormatter class

            // Displayed TimeZones (other than local timezone)
            String[] timeZones = new String[] { "UTC", "America/New_York", "Asia/Kolkata" };

            // Store results here.
            StringBuilder results = new StringBuilder();

            // Create formatter object using longdate and longtime template
            DateTimeFormatter formatter = new DateTimeFormatter("longdate longtime");

            // Create date/time to format and display.
            DateTime dateTime = DateTime.Now;

            // Show current time in timezones desired to be displayed including local timezone
            results.AppendLine("Current date and time -");
            results.AppendLine("In Local timezone:   " + formatter.Format(dateTime));
            foreach (String timeZone in timeZones)
            {
                results.AppendLine("In " + timeZone + " timezone:   " + formatter.Format(dateTime, timeZone));
            }
            results.AppendLine();

            // Show a time on 14th day of second month of next year in local, and other desired Time Zones
            // This will show if there were day light savings in time
            results.AppendLine("Same time on 14th day of second month of next year -");
            dateTime = new DateTime(dateTime.Year + 1, 2, 14, dateTime.Hour, dateTime.Minute, dateTime.Second);
            results.AppendLine("In Local timezone:   " + formatter.Format(dateTime));
            foreach (String timeZone in timeZones)
            {
                results.AppendLine("In " + timeZone + " timezone:   " + formatter.Format(dateTime, timeZone));
            }
            results.AppendLine();

            // Show a time on 14th day of 10th month of next year in local, and other desired Time Zones
            // This will show if there were day light savings in time
            results.AppendLine("Same time on 14th day of tenth month of next year -");
            dateTime = dateTime.AddMonths(8);
            results.AppendLine("In Local timezone:   " + formatter.Format(dateTime));
            foreach (String timeZone in timeZones)
            {
                results.AppendLine("In " + timeZone + " timezone:   " + formatter.Format(dateTime, timeZone));
            }
            results.AppendLine();

            // Display the results
            OutputTextBlock.Text = results.ToString();
        }

    }
}
