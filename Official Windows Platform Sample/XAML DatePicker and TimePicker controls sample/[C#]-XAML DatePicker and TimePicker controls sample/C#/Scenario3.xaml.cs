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
    public sealed partial class Scenario3 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Scenario3()
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
        /// This is the click handler for the 'combine' button.  The values of the TimePicker and DatePicker are combined into a single DateTimeOffset.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void combine_Click(object sender, RoutedEventArgs e)
        {
            DateTimeFormatter dateFormatter = new DateTimeFormatter("shortdate");
            DateTimeFormatter timeFormatter = new DateTimeFormatter("shorttime");

            // We use a calendar to determine daylight savings time transition days
            Calendar calendar = new Calendar();
            calendar.ChangeClock("24HourClock");

            // The value of the selected time in a TimePicker is stored as a TimeSpan, so it is possible to add it directly to the value of the selected date
            DateTimeOffset selectedDate = this.datePicker.Date;
            DateTimeOffset combinedValue = new DateTimeOffset(new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day) + this.timePicker.Time);

            calendar.SetDateTime(combinedValue);

            // If the day does not have 24 hours, then the user has selected a day in which a Daylight Savings Time transition occurs.
            //    It is the app developer's responsibility for validating the combination of the date and time values.
            if (calendar.NumberOfHoursInThisPeriod != 24)
            {
                rootPage.NotifyUser("You selected a DST transition day", NotifyType.StatusMessage);
            }
            else
            {
                rootPage.NotifyUser("Combined value: " + dateFormatter.Format(combinedValue) + " " + timeFormatter.Format(combinedValue), NotifyType.StatusMessage);
            }
        }
    }
}
