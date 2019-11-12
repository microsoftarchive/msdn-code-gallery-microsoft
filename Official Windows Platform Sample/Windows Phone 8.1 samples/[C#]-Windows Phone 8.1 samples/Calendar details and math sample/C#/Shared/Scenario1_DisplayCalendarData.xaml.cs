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
    public sealed partial class DisplayCalendarData : Page
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public DisplayCalendarData()
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
            // This scenario uses the Windows.Globalization.Calendar class to display the parts of a date.

            // Store results here.
            StringBuilder results = new StringBuilder();

            // Create Calendar objects using different constructors.
            Calendar calendar = new Calendar();
            Calendar japaneseCalendar = new Calendar(new[] { "ja-JP" }, CalendarIdentifiers.Japanese, ClockIdentifiers.TwelveHour);
            Calendar hebrewCalendar = new Calendar(new[] { "he-IL" }, CalendarIdentifiers.Hebrew, ClockIdentifiers.TwentyFourHour);

            // Display individual date/time elements.
            results.AppendLine("User's default calendar system: " + calendar.GetCalendarSystem());
            results.AppendLine("Name of Month: " + calendar.MonthAsSoloString());
            results.AppendLine("Day of Month: " + calendar.DayAsPaddedString(2));
            results.AppendLine("Day of Week: " + calendar.DayOfWeekAsSoloString());
            results.AppendLine("Year: " + calendar.YearAsString());
            results.AppendLine();
            results.AppendLine("Calendar system: " + japaneseCalendar.GetCalendarSystem());
            results.AppendLine("Name of Month: " + japaneseCalendar.MonthAsSoloString());
            results.AppendLine("Day of Month: " + japaneseCalendar.DayAsPaddedString(2));
            results.AppendLine("Day of Week: " + japaneseCalendar.DayOfWeekAsSoloString());
            results.AppendLine("Year: " + japaneseCalendar.YearAsString());
            results.AppendLine();
            results.AppendLine("Calendar system: " + hebrewCalendar.GetCalendarSystem());
            results.AppendLine("Name of Month: " + hebrewCalendar.MonthAsSoloString());
            results.AppendLine("Day of Month: " + hebrewCalendar.DayAsPaddedString(2));
            results.AppendLine("Day of Week: " + hebrewCalendar.DayOfWeekAsSoloString());
            results.AppendLine("Year: " + hebrewCalendar.YearAsString());
            results.AppendLine();

            // Display the results
            OutputTextBlock.Text=results.ToString();
        }
    }
}
