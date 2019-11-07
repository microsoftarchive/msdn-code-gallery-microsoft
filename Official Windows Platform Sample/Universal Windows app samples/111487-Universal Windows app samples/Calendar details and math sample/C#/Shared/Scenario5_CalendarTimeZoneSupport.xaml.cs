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
using Windows.Globalization;

namespace CalendarSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CalendarTimeZoneSupport: Page
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public CalendarTimeZoneSupport()
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
            // This scenario illustrates TimeZone support in Windows.Globalization.Calendar class

            // Displayed TimeZones (other than local timezone)
            String[] timeZones = new String[] { "UTC", "America/New_York", "Asia/Kolkata" };

            // Store results here.
            StringBuilder results = new StringBuilder();

            // Create default Calendar object
            Calendar calendar = new Calendar();
            String localTimeZone = calendar.GetTimeZone();

            // Show current time in timezones desired to be displayed including local timezone
            results.AppendLine("Current date and time -");
            results.AppendLine(GetFormattedCalendarDateTime(calendar));
            foreach (String timeZone in timeZones)
            {
                calendar.ChangeTimeZone(timeZone);
                results.AppendLine(GetFormattedCalendarDateTime(calendar));
            }
            results.AppendLine();
            calendar.ChangeTimeZone(localTimeZone);

            // Show a time on 14th day of second month of next year in local, GMT, New York and Indian Time Zones
            // This will show if there were day light savings in time
            results.AppendLine("Same time on 14th day of second month of next year -");
            calendar.AddYears(1); calendar.Month = 2; calendar.Day = 14;
            results.AppendLine(GetFormattedCalendarDateTime(calendar));
            foreach (String timeZone in timeZones)
            {
                calendar.ChangeTimeZone(timeZone);
                results.AppendLine(GetFormattedCalendarDateTime(calendar));
            }
            results.AppendLine();
            calendar.ChangeTimeZone(localTimeZone);

            // Show a time on 14th day of 10th month of next year in local, GMT, New York and Indian Time Zones
            // This will show if there were day light savings in time
            results.AppendLine("Same time on 14th day of tenth month of next year -");
            calendar.AddMonths(8);
            results.AppendLine(GetFormattedCalendarDateTime(calendar));
            foreach (String timeZone in timeZones)
            {
                calendar.ChangeTimeZone(timeZone);
                results.AppendLine(GetFormattedCalendarDateTime(calendar));
            }
            results.AppendLine();

            // Display the results
            OutputTextBlock.Text = results.ToString();
        }

        /// <summary>
        /// This is a helper function to display calendar's date-time in presentable format
        /// </summary>
        /// <param name="calendar"></param>
        private String GetFormattedCalendarDateTime(Calendar calendar)
        {
            // Display individual date/time elements.
            return(String.Format("In {0} TimeZone:   {1}   {2} {3}, {4}   {5}:{6}:{7} {8}  {9}",
                                 calendar.GetTimeZone(),
                                 calendar.DayOfWeekAsSoloString(), 
                                 calendar.MonthAsSoloString(),
                                 calendar.DayAsPaddedString(2),
                                 calendar.YearAsString(),
                                 calendar.HourAsPaddedString(2),
                                 calendar.MinuteAsPaddedString(2),
                                 calendar.SecondAsPaddedString(2),
                                 calendar.PeriodAsString(),
                                 calendar.TimeZoneAsString(3)));
        }
    }
}
