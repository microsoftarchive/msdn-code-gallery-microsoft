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
    public sealed partial class ParametrizedTemplate : Page
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public ParametrizedTemplate()
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
        /// This is the click handler for the 'Default' button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Display_Click(object sender, RoutedEventArgs e)
        {
            // This scenario uses the Windows.Globalization.DateTimeFormatting.DateTimeFormatter class
            // to format a date/time by specifying a template via parameters.  Note that the user's language
            // and region preferences will determine the pattern of the date returned based on the
            // specified parts.

            // We keep results in this variable
            StringBuilder results = new StringBuilder();
            results.AppendLine("Current application context language: " + ApplicationLanguages.Languages[0]);
            results.AppendLine();

            // Create formatters with individual format specifiers for date/time elements.
            DateTimeFormatter[] dateFormatters = new[]
            {
                // Example formatters for dates.
                new DateTimeFormatter(
                    YearFormat.Full, 
                    MonthFormat.Abbreviated, 
                    DayFormat.Default, 
                    DayOfWeekFormat.Abbreviated),
                new DateTimeFormatter(
                    YearFormat.Abbreviated, 
                    MonthFormat.Abbreviated, 
                    DayFormat.Default, 
                    DayOfWeekFormat.None),
                new DateTimeFormatter(
                    YearFormat.Full, 
                    MonthFormat.Full, 
                    DayFormat.None, 
                    DayOfWeekFormat.None),
                new DateTimeFormatter(
                    YearFormat.None, 
                    MonthFormat.Full, 
                    DayFormat.Default, 
                    DayOfWeekFormat.None)
            };

            // Create formatters with individual format specifiers for time elements.
            DateTimeFormatter[] timeFormatters = new[]
            {
                // Example formatters for times.
                new DateTimeFormatter(
                    HourFormat.Default, 
                    MinuteFormat.Default, 
                    SecondFormat.Default),
                new DateTimeFormatter(
                    HourFormat.Default, 
                    MinuteFormat.Default, 
                    SecondFormat.None),
                new DateTimeFormatter(
                    HourFormat.Default, 
                    MinuteFormat.None, 
                    SecondFormat.None),
             };

            // Create date/time to format and display.
            DateTime dateTime = DateTime.Now;

            results.AppendLine("Formatted Dates:");
            results.AppendLine();

            // Format and display date/time. Calendar always supports Now. Otherwise you may need to verify dateTime is in supported range.
            foreach (DateTimeFormatter formatter in dateFormatters)
            {
                // Format and display date/time.
                results.AppendLine(formatter.Template + ": " + formatter.Format(dateTime));
            }

            results.AppendLine();
            results.AppendLine("Formatted Times:");
            results.AppendLine();

            // Format and display date/time. Calendar always supports Now. Otherwise you may need to verify dateTime is in supported range.
            foreach (DateTimeFormatter formatter in timeFormatters)
            {
                // Format and display date/time.
                results.AppendLine(formatter.Template + ": " + formatter.Format(dateTime));
            }

            // Display the results
            OutputTextBlock.Text = results.ToString();
        }
    }
}
