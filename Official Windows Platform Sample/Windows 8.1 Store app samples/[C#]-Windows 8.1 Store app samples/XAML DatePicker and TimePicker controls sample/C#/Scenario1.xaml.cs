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
using System.Globalization;
using Windows.Globalization.DateTimeFormatting;

namespace DateAndTimePickers
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario1 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Scenario1()
        {
            this.InitializeComponent();

            rootPage.NotifyUser("No selection changes made", NotifyType.StatusMessage);
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
        /// This is the handler for the DateChanged event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void datePicker_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            // The DateTimeFormatter class formats dates and times with the user's default settings
            DateTimeFormatter dateFormatter = new DateTimeFormatter("shortdate");

            rootPage.NotifyUser("Date changed to " + dateFormatter.Format(e.NewDate), NotifyType.StatusMessage);
        }

        /// <summary>
        /// This is the handler for the TimeChanged event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timePicker_TimeChanged(object sender, TimePickerValueChangedEventArgs e)
        {
            rootPage.NotifyUser("Time changed to " + e.NewTime.ToString(), NotifyType.StatusMessage);
        }

    }
}
