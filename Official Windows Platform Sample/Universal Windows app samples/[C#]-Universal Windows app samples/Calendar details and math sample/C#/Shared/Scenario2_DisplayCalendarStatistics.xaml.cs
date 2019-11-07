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
    public sealed partial class DisplayCalendarStatistics : Page
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public DisplayCalendarStatistics()
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
            // This scenario uses the Windows.Globalization.Calendar class to display the calendar
            // system statistics.
            
            // Store results here.
            StringBuilder results = new StringBuilder();

            // Create Calendar objects using different constructors.
            Calendar calendar = new Calendar();
            Calendar japaneseCalendar = new Calendar(new[] { "ja-JP" }, CalendarIdentifiers.Japanese, ClockIdentifiers.TwelveHour);
            Calendar hebrewCalendar = new Calendar(new[] { "he-IL" }, CalendarIdentifiers.Hebrew, ClockIdentifiers.TwentyFourHour);

            // Display individual date/time elements.
            results.AppendLine("User's default calendar system: " + calendar.GetCalendarSystem());
            results.AppendLine("Months in this Year: " + calendar.NumberOfMonthsInThisYear);
            results.AppendLine("Days in this Month: " + calendar.NumberOfDaysInThisMonth);
            results.AppendLine("Hours in this Period: " + calendar.NumberOfHoursInThisPeriod);
            results.AppendLine("Era: " + calendar.EraAsString());
            results.AppendLine();
            results.AppendLine("Calendar system: " + japaneseCalendar.GetCalendarSystem());
            results.AppendLine("Months in this Year: " + japaneseCalendar.NumberOfMonthsInThisYear);
            results.AppendLine("Days in this Month: " + japaneseCalendar.NumberOfDaysInThisMonth);
            results.AppendLine("Hours in this Period: " + japaneseCalendar.NumberOfHoursInThisPeriod);
            results.AppendLine("Era: " + japaneseCalendar.EraAsString());
            results.AppendLine();
            results.AppendLine("Calendar system: " + hebrewCalendar.GetCalendarSystem());
            results.AppendLine("Months in this Year: " + hebrewCalendar.NumberOfMonthsInThisYear);
            results.AppendLine("Days in this Month: " + hebrewCalendar.NumberOfDaysInThisMonth);
            results.AppendLine("Hours in this Period: " + hebrewCalendar.NumberOfHoursInThisPeriod);
            results.AppendLine("Era: " + hebrewCalendar.EraAsString());
            results.AppendLine();

            // Display results
            OutputTextBlock.Text = results.ToString();
        }
    }
}
