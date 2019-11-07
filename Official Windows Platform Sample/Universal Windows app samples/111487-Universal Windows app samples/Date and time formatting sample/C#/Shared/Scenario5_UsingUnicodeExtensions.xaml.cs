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
    public sealed partial class UsingUnicodeExtensions: Page
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public UsingUnicodeExtensions()
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
            // to format a date/time by using a formatter that uses Unicode extenstion in the specified
            // language name

            // We keep results in this variable
            StringBuilder results = new StringBuilder();
            results.AppendLine("Current default application context language: " + ApplicationLanguages.Languages[0]);
            results.AppendLine();

            // Create formatters using various types of constructors specifying Language list with unicode extension in language names
            DateTimeFormatter[] unicodeExtensionFormatters = new[]
            {
                // Default application context  
                new DateTimeFormatter("longdate longtime"),
                // Telugu language, Gregorian Calendar and Latin Numeral System
                new DateTimeFormatter("longdate longtime", new[] { "te-in-u-ca-gregory-nu-latn", "en-US" }),
                // Hebrew language and Arabic Numeral System - calendar NOT specified in constructor
                new DateTimeFormatter(YearFormat.Default, 
                    MonthFormat.Default, 
                    DayFormat.Default, 
                    DayOfWeekFormat.Default,
                    HourFormat.Default,
                    MinuteFormat.Default,
                    SecondFormat.Default,
                    new[] { "he-IL-u-nu-arab", "en-US" }),             
                // Hebrew language and calendar - calendar specified in constructor
                // also, which overrides the one specified in Unicode extension
                new DateTimeFormatter(YearFormat.Default, 
                    MonthFormat.Default, 
                    DayFormat.Default, 
                    DayOfWeekFormat.Default,
                    HourFormat.Default,
                    MinuteFormat.Default,
                    SecondFormat.Default,
                    new[] { "he-IL-u-ca-hebrew-co-phonebk", "en-US" },
                    "US",
                    CalendarIdentifiers.Gregorian,
                    ClockIdentifiers.TwentyFourHour), 
             };

            // Create date/time to format and display.
            DateTime dateTime = DateTime.Now;

            // Format and display date/time along with other relevant properites
            foreach (DateTimeFormatter formatter in unicodeExtensionFormatters)
            {
                // Format and display date/time.
                results.AppendLine("Using DateTimeFormatter with Language List:   " + string.Join(", ", formatter.Languages));
                results.AppendLine("\t Template:   " + formatter.Template);
                results.AppendLine("\t Resolved Language:   " + formatter.ResolvedLanguage);
                results.AppendLine("\t Calendar System:   " + formatter.Calendar);
                results.AppendLine("\t Numeral System:   " + formatter.NumeralSystem);
                results.AppendLine("Formatted DateTime:   " + formatter.Format(dateTime));
                results.AppendLine();
            }

            // Display the results
            OutputTextBlock.Text = results.ToString();
        }
    }
}
