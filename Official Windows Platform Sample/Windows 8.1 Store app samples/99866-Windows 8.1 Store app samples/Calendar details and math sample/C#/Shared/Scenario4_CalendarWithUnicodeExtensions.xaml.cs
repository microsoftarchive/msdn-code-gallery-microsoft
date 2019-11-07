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
    public sealed partial class CalendarWithUnicodeExtensions: Page
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public CalendarWithUnicodeExtensions()
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

            // Create Calendar objects using different Unicode extensions for different languages.
            // NOTE: Calendar (ca) and numeral system (nu) are the only supported extensions with any others being ignored 
            // (note that collation (co) extension is ignored in the last example).
            Calendar cal1 = new Calendar();
            Calendar cal2 = new Calendar(new[] { "ar-SA-u-ca-gregory-nu-Latn" });
            Calendar cal3 = new Calendar(new[] { "he-IL-u-nu-arab" });
            Calendar cal4 = new Calendar(new[] { "he-IL-u-ca-hebrew-co-phonebk" });

            // Display individual date/time elements.
            results.AppendLine("User's default Calendar object : ");
            results.AppendLine(GetCalendarProperties(cal1));

            results.AppendLine("Calendar object with Arabic language, Gregorian Calendar and Latin Numeral System (ar-SA-ca-gregory-nu-Latn) :");
            results.AppendLine(GetCalendarProperties(cal2));

            results.AppendLine("Calendar object with Hebrew language, Default Calendar for that language and Arab Numeral System (he-IL-u-nu-arab) :");
            results.AppendLine(GetCalendarProperties(cal3));

            results.AppendLine("Calendar object with Hebrew language, Hebrew Calendar, Default Numeral System for that language and Phonebook collation (he-IL-u-ca-hebrew-co-phonebk) :");
            results.AppendLine(GetCalendarProperties(cal4));

            // Display the results
            OutputTextBlock.Text = results.ToString();
        }
        
        /// <summary>
        /// This is a helper function to display calendar's properties in presentable format
        /// </summary>
        /// <param name="calendar"></param>
        private String GetCalendarProperties(Calendar calendar)
        {
            StringBuilder returnString = new StringBuilder();
            
            returnString.AppendLine("Calendar system: " + calendar.GetCalendarSystem());
            returnString.AppendLine("Numeral System: " + calendar.NumeralSystem);
            returnString.AppendLine("Resolved Language " + calendar.ResolvedLanguage);
            returnString.AppendLine("Name of Month: " + calendar.MonthAsSoloString());
            returnString.AppendLine("Day of Month: " + calendar.DayAsPaddedString(2));
            returnString.AppendLine("Day of Week: " + calendar.DayOfWeekAsSoloString());
            returnString.AppendLine("Year: " + calendar.YearAsString());
            returnString.AppendLine();

            return(returnString.ToString());
        }
    }
}
