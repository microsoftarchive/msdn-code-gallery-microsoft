// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SDKTemplate
{
	 internal class NotificationData
    {
        public String ItemType { get; set; }
        public String ItemId { get; set; }
        public String DueTime { get; set; }
        public String InputString { get; set; }
        public Boolean IsTile { get; set; }
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario2 : Page
    {
MainPage rootPage = null;

        public Scenario2()
        {
            InitializeComponent();
            RefreshListButton.Click += RefreshList_Click;
            RemoveButton.Click += Remove_Click;
        }

        // Remove the notification by checking the list of scheduled notifications for a notification with matching ID.
        // While it would be possible to manage the notifications by storing a reference to each notification, such practice
        // causes memory leaks by not allowing the notifications to be collected once they have shown.
        // It's important to create unique IDs for each notification if they are to be managed later.
        void Remove_Click(object sender, RoutedEventArgs e)
        {
            IList<Object> items = ItemGridView.SelectedItems;
            for (int i = 0; i < items.Count; i++)
            {
                NotificationData item = (NotificationData)items[i];
                String itemId = item.ItemId;
                if (item.IsTile)
                {
                    TileUpdater updater = TileUpdateManager.CreateTileUpdaterForApplication();
                    IReadOnlyList<ScheduledTileNotification> scheduled = updater.GetScheduledTileNotifications();
                    for (int j = 0; j < scheduled.Count; j++)
                    {
                        if (scheduled[j].Id == itemId)
                        {
                            updater.RemoveFromSchedule(scheduled[j]);
                        }
                    }
                }
                else
                {
                    ToastNotifier notifier = ToastNotificationManager.CreateToastNotifier();
                    IReadOnlyList<ScheduledToastNotification> scheduled = notifier.GetScheduledToastNotifications();
                    for (int j = 0; j < scheduled.Count; j++)
                    {
                        if (scheduled[j].Id == itemId)
                        {
                            notifier.RemoveFromSchedule(scheduled[j]);
                        }
                    }
                }
            }
            rootPage.NotifyUser("Removed selected scheduled notifications", NotifyType.StatusMessage);
            RefreshListView();
        }

        void RefreshList_Click(object sender, RoutedEventArgs e)
        {
            RefreshListView();
        }


        void RefreshListView()
        {
            IReadOnlyList<ScheduledToastNotification> scheduledToasts = ToastNotificationManager.CreateToastNotifier().GetScheduledToastNotifications();
            IReadOnlyList<ScheduledTileNotification> scheduledTiles = TileUpdateManager.CreateTileUpdaterForApplication().GetScheduledTileNotifications();

            int toastLength = scheduledToasts.Count;
            int tileLength = scheduledTiles.Count;

            List<NotificationData> bindingList = new List<NotificationData>(toastLength + tileLength);
            for (int i = 0; i < toastLength; i++)
            {
                ScheduledToastNotification toast = scheduledToasts[i];

                bindingList.Add(new NotificationData()
                {
                    ItemType = "Toast",
                    ItemId = toast.Id,
                    DueTime = toast.DeliveryTime.ToLocalTime().ToString(),
                    InputString = toast.Content.GetElementsByTagName("text")[0].InnerText,
                    IsTile = false
                });
            }

            for (int i = 0; i < tileLength; i++)
            {
                ScheduledTileNotification tile = scheduledTiles[i];

                bindingList.Add(new NotificationData()
                {
                    ItemType = "Tile",
                    ItemId = tile.Id,
                    DueTime = tile.DeliveryTime.ToLocalTime().ToString(),
                    InputString = tile.Content.GetElementsByTagName("text")[0].InnerText,
                    IsTile = true
                });
            }

            ItemGridView.ItemsSource = bindingList;
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Get a pointer to our main page
            rootPage = MainPage.Current;
            RefreshListView();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            ItemGridView.ItemsSource = null;
        }
    }
}
