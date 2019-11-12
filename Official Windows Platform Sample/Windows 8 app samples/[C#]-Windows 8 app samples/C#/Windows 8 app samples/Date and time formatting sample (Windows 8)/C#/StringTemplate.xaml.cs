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
    public sealed partial class StringTemplate : SDKTemplate.Common.LayoutAwarePage
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

            // We keep results in this variable
            StringBuilder results = new StringBuilder();
            results.AppendLine("Current application context language: " + ApplicationLanguages.Languages[0]);
            results.AppendLine();

            // Create template-based date/time formatters.
            DateTimeFormatter[] templateFormatters = new[]
            {
                // Formatters for dates.
                new DateTimeFormatter("day month"),
                new DateTimeFormatter("month year"),
                new DateTimeFormatter("day month year"),
                new DateTimeFormatter("dayofweek day month year"),
                new DateTimeFormatter("dayofweek.abbreviated"),
                new DateTimeFormatter("month.abbreviated"),
                new DateTimeFormatter("year.abbreviated"),

                // Formatters for time.
                new DateTimeFormatter("hour"),
                new DateTimeFormatter("hour minute"),
                new DateTimeFormatter("hour minute second"),
             };


            // Create date/time to format and display.
            DateTime dateTime = DateTime.Now;

            // Try to format and display date/time if calendar supports it.
            foreach (DateTimeFormatter formatter in templateFormatters)
            {
                try
                {
                    // Format and display date/time.
                    results.AppendLine(formatter.Template + ": " + formatter.Format(dateTime));
                }
                catch (ArgumentException)
                {
                    // Retrieve and display formatter properties. 
                    results.AppendLine(String.Format(
                        "Unable to format Gregorian DateTime {0} using formatter with template {1} for languages [{2}], region {3}, calendar {4} and clock {5}",
                        dateTime,
                        formatter.Template,
                        string.Join(",", formatter.Languages),
                        formatter.GeographicRegion,
                        formatter.Calendar,
                        formatter.Clock));
                }
            }

            // Display the results
            rootPage.NotifyUser(results.ToString(), NotifyType.StatusMessage);
        }
    }
}
