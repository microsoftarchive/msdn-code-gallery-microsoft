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
    public sealed partial class OverrideSettings : Page
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public OverrideSettings()
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
            // to format a date/time by using a formatter that provides specific languages,
            // geographic region, calendar and clock

            // We keep results in this variable
            StringBuilder results = new StringBuilder();
            results.AppendLine("Current default application context language: " + ApplicationLanguages.Languages[0]);
            results.AppendLine();

            // Create formatters with individual format specifiers for date/time elements.
            DateTimeFormatter[] templateFormatters = new[]
            {
                // Example formatters for dates using string templates.
                new DateTimeFormatter("longdate", new[] { "ja-JP", "en-US" }, "JP", CalendarIdentifiers.Japanese, ClockIdentifiers.TwelveHour),
                new DateTimeFormatter("month day", new[] { "fr-FR", "en-US" }, "FR", CalendarIdentifiers.Gregorian, ClockIdentifiers.TwentyFourHour),    

                // Example formatters using paranetrized date templates.
                new DateTimeFormatter(YearFormat.Abbreviated, 
                    MonthFormat.Abbreviated, 
                    DayFormat.Default, 
                    DayOfWeekFormat.None,
                    HourFormat.None,
                    MinuteFormat.None,
                    SecondFormat.None,
                    new[] { "de-DE", "en-US" },
                    "DE",
                    CalendarIdentifiers.Gregorian,
                    ClockIdentifiers.TwelveHour), 
            
                // Example formatters for times.
                new DateTimeFormatter("longtime", new[] { "ja-JP", "en-US" }, "JP", CalendarIdentifiers.Japanese, ClockIdentifiers.TwelveHour),
                new DateTimeFormatter("hour minute", new[] { "fr-FR", "en-US" }, "FR", CalendarIdentifiers.Gregorian, ClockIdentifiers.TwentyFourHour),    
                
                // Example formatters using paranetrized time templates.
                new DateTimeFormatter(YearFormat.None, 
                    MonthFormat.None, 
                    DayFormat.None, 
                    DayOfWeekFormat.None,
                    HourFormat.Default,
                    MinuteFormat.Default,
                    SecondFormat.None,
                    new[] { "de-DE" },
                    "DE",
                    CalendarIdentifiers.Gregorian,
                    ClockIdentifiers.TwelveHour),
             };

            // Create date/time to format and display.
            DateTime dateTime = DateTime.Now;

            // Format and display date/time. Calendar always supports Now. Otherwise you may need to verify dateTime is in supported range.
            foreach (DateTimeFormatter formatter in templateFormatters)
            {
                // Format and display date/time.
                results.AppendLine(formatter.Template + ": (" + formatter.ResolvedLanguage + ") " + formatter.Format(dateTime));
            }

            // Display the results
            OutputTextBlock.Text = results.ToString();
        }
    }
}
