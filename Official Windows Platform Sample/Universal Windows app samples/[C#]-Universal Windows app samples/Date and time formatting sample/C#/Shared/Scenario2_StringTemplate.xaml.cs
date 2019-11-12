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
    public sealed partial class StringTemplate : Page
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public StringTemplate()
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
            // to format a date/time via a string template.  Note that the order specifed in the string pattern does
            // not determine the order of the parts of the formatted string.  The user's language and region preferences will
            // determine the pattern of the date returned based on the specified parts.

            // Create template-based date/time formatters.
            DateTimeFormatter[] dateFormatters = new[]
            {
                // Formatters for dates.
                new DateTimeFormatter("month day"),
                new DateTimeFormatter("month year"),
                new DateTimeFormatter("month day year"),
                new DateTimeFormatter("month day dayofweek year"),
                new DateTimeFormatter("dayofweek.abbreviated"),
                new DateTimeFormatter("month.abbreviated"),
                new DateTimeFormatter("year.abbreviated")
            };

            // Create template-based date/time formatters.
            DateTimeFormatter[] timeFormatters = new[]
            {
                // Formatters for time.
                new DateTimeFormatter("hour minute"),
                new DateTimeFormatter("hour minute second"),
                new DateTimeFormatter("hour")
            };

            // Create template-based date/time formatters.
            DateTimeFormatter[] timezoneFormatters = new[]
            {
                // Formatters for timezone.
                new DateTimeFormatter("timezone"),
                new DateTimeFormatter("timezone.full"),
                new DateTimeFormatter("timezone.abbreviated")
            };
                
            // Create template-based date/time formatters.
            DateTimeFormatter[] combinationFormatters = new[]
            {
                // Formatters for combinations.
                new DateTimeFormatter("hour minute second timezone.full"),
                new DateTimeFormatter("day month year hour minute timezone"),
                new DateTimeFormatter("dayofweek day month year hour minute second"),
                new DateTimeFormatter("dayofweek.abbreviated day month hour minute"),
                new DateTimeFormatter("dayofweek day month year hour minute second timezone.abbreviated"),
             };

            // Create date/time to format and display.
            DateTime dateTime = DateTime.Now;

            // We keep results in this variable
            StringBuilder results = new StringBuilder();
            results.AppendLine("Current application context language: " + ApplicationLanguages.Languages[0]);
            results.AppendLine();
            results.AppendLine("Formatted Dates:");

            // Format and display date/time. Calendar always supports Now. Otherwise you may need to verify dateTime is in supported range.
            foreach (DateTimeFormatter formatter in dateFormatters)
            {
                // Format and display date/time.
                results.AppendLine(formatter.Template + ": " + formatter.Format(dateTime));
            }

            results.AppendLine();
            results.AppendLine("Formatted Times:");

            // Format and display date/time. Calendar always supports Now. Otherwise you may need to verify dateTime is in supported range.
            foreach (DateTimeFormatter formatter in timeFormatters)
            {
                // Format and display date/time.
                results.AppendLine(formatter.Template + ": " + formatter.Format(dateTime));
            }

            results.AppendLine();
            results.AppendLine("Formatted timezones:");

            // Format and display date/time. Calendar always supports Now. Otherwise you may need to verify dateTime is in supported range.
            foreach (DateTimeFormatter formatter in timezoneFormatters)
            {
                // Format and display date/time.
                results.AppendLine(formatter.Template + ": " + formatter.Format(dateTime));
            }

            results.AppendLine();
            results.AppendLine("Formatted Date and Time Combinations:");

            // Format and display date/time. Calendar always supports Now. Otherwise you may need to verify dateTime is in supported range.
            foreach (DateTimeFormatter formatter in combinationFormatters)
            {
                // Format and display date/time.
                results.AppendLine(formatter.Template + ": " + formatter.Format(dateTime));
            }

            // Display the results
            OutputTextBlock.Text = results.ToString();
        }
    }
}
