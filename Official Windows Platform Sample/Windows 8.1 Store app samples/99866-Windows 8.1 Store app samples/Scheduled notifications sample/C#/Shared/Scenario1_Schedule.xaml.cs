// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using NotificationsExtensions.TileContent;
using NotificationsExtensions.ToastContent;
using Windows.UI.Notifications;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SDKTemplate
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario1 : Page
    {        
        private MainPage rootPage;

        public Scenario1()
        {
            InitializeComponent();
            ScheduleButton.Click += ScheduleButton_Click;
            ScheduleButtonString.Click += ScheduleButton_Click;
        }

        void ScheduleButton_Click(Object sender, RoutedEventArgs e)
        {
            bool useStrings = false;
            if (sender == ScheduleButtonString)
            {
                useStrings = true;
            }

            try
            {
                Int16 dueTimeInSeconds = Int16.Parse(FutureTimeBox.Text);
                if (dueTimeInSeconds <= 0) throw new ArgumentException();

                String updateString = StringBox.Text;
                DateTime dueTime = DateTime.Now.AddSeconds(dueTimeInSeconds);

                Random rand = new Random();
                int idNumber = rand.Next(0, 10000000);

                if (ToastRadio.IsChecked != null && (bool)ToastRadio.IsChecked)
                {
                    if (useStrings)
                    {
                        ScheduleToastWithStringManipulation(updateString, dueTime, idNumber);
                    }
                    else
                    {
                        ScheduleToast(updateString, dueTime, idNumber);
                    }
                }
                else
                {
                    if (useStrings)
                    {
                        ScheduleTileWithStringManipulation(updateString, dueTime, idNumber);
                    }
                    else
                    {
                        ScheduleTile(updateString, dueTime, idNumber);
                    }
                }
            }
            catch (Exception)
            {
                rootPage.NotifyUser("You must input a valid time in seconds.", NotifyType.ErrorMessage);
            }
        }

        void ScheduleToast(String updateString, DateTime dueTime, int idNumber)
        {
            // Scheduled toasts use the same toast templates as all other kinds of toasts.
            IToastText02 toastContent = ToastContentFactory.CreateToastText02();
            toastContent.TextHeading.Text = updateString;
            toastContent.TextBodyWrap.Text = "Received: " + dueTime.ToLocalTime();

            ScheduledToastNotification toast;
            if (RepeatBox.IsChecked != null && (bool)RepeatBox.IsChecked)
            {
                toast = new ScheduledToastNotification(toastContent.GetXml(), dueTime, TimeSpan.FromSeconds(60), 5);

                // You can specify an ID so that you can manage toasts later.
                // Make sure the ID is 15 characters or less.
                toast.Id = "Repeat" + idNumber;
            }
            else
            {
                toast = new ScheduledToastNotification(toastContent.GetXml(), dueTime);
                toast.Id = "Toast" + idNumber;
            }

            ToastNotificationManager.CreateToastNotifier().AddToSchedule(toast);
            rootPage.NotifyUser("Scheduled a toast with ID: " + toast.Id, NotifyType.StatusMessage);
        }

        void ScheduleTile(String updateString, DateTime dueTime, int idNumber)
        {
            // Set up the wide tile text
            ITileWide310x150Text09 tileContent = TileContentFactory.CreateTileWide310x150Text09();
            tileContent.TextHeading.Text = updateString;
            tileContent.TextBodyWrap.Text = "Received: " + dueTime.ToLocalTime();

            // Set up square tile text
            ITileSquare150x150Text04 squareContent = TileContentFactory.CreateTileSquare150x150Text04();
            squareContent.TextBodyWrap.Text = updateString;

            tileContent.Square150x150Content = squareContent;

            // Create the notification object
            ScheduledTileNotification futureTile = new ScheduledTileNotification(tileContent.GetXml(), dueTime);
            futureTile.Id = "Tile" + idNumber;

            // Add to schedule
            // You can update a secondary tile in the same manner using CreateTileUpdaterForSecondaryTile(tileId)
            // See "Tiles" sample for more details
            TileUpdateManager.CreateTileUpdaterForApplication().AddToSchedule(futureTile);
            rootPage.NotifyUser("Scheduled a tile with ID: " + futureTile.Id, NotifyType.StatusMessage);
        }


        void ScheduleToastWithStringManipulation(String updateString, DateTime dueTime, int idNumber)
        {
            // Scheduled toasts use the same toast templates as all other kinds of toasts.
            string toastXmlString = "<toast>"
            + "<visual version='2'>"
            + "<binding template='ToastText02'>"
            + "<text id='2'>" + updateString + "</text>"
            + "<text id='1'>" + "Received: " + dueTime.ToLocalTime() + "</text>"
            + "</binding>"
            + "</visual>"
            + "</toast>";

            Windows.Data.Xml.Dom.XmlDocument toastDOM = new Windows.Data.Xml.Dom.XmlDocument();
            try
            {
                toastDOM.LoadXml(toastXmlString);

                ScheduledToastNotification toast;
                if (RepeatBox.IsChecked != null && (bool)RepeatBox.IsChecked)
                {
                    toast = new ScheduledToastNotification(toastDOM, dueTime, TimeSpan.FromSeconds(60), 5);

                    // You can specify an ID so that you can manage toasts later.
                    // Make sure the ID is 15 characters or less.
                    toast.Id = "Repeat" + idNumber;
                }
                else
                {
                    toast = new ScheduledToastNotification(toastDOM, dueTime);
                    toast.Id = "Toast" + idNumber;
                }

                ToastNotificationManager.CreateToastNotifier().AddToSchedule(toast);
                rootPage.NotifyUser("Scheduled a toast with ID: " + toast.Id, NotifyType.StatusMessage);
            }
            catch (Exception)
            {
                rootPage.NotifyUser("Error loading the xml, check for invalid characters in the input", NotifyType.ErrorMessage);
            }
        }

        void ScheduleTileWithStringManipulation(String updateString, DateTime dueTime, int idNumber)
        {
            string tileXmlString = "<tile>"
                         + "<visual version='2'>"
                         + "<binding template='TileWide310x150Text09' fallback='TileWideText09'>"
                         + "<text id='1'>" + updateString + "</text>"
                         + "<text id='2'>" + "Received: " + dueTime.ToLocalTime() + "</text>"
                         + "</binding>"
                         + "<binding template='TileSquare150x150Text04' fallback='TileSquareText04'>"
                         + "<text id='1'>" + updateString + "</text>"
                         + "</binding>"
                         + "</visual>"
                         + "</tile>";

            Windows.Data.Xml.Dom.XmlDocument tileDOM = new Windows.Data.Xml.Dom.XmlDocument();
            try
            {
                tileDOM.LoadXml(tileXmlString);

                // Create the notification object
                ScheduledTileNotification futureTile = new ScheduledTileNotification(tileDOM, dueTime);
                futureTile.Id = "Tile" + idNumber;

                // Add to schedule
                // You can update a secondary tile in the same manner using CreateTileUpdaterForSecondaryTile(tileId)
                // See "Tiles" sample for more details
                TileUpdateManager.CreateTileUpdaterForApplication().AddToSchedule(futureTile);
                rootPage.NotifyUser("Scheduled a tile with ID: " + futureTile.Id, NotifyType.StatusMessage);
            }
            catch (Exception)
            {
                rootPage.NotifyUser("Error loading the xml, check for invalid characters in the input", NotifyType.ErrorMessage);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Get a pointer to our main page
            rootPage = MainPage.Current;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }
    }
}
