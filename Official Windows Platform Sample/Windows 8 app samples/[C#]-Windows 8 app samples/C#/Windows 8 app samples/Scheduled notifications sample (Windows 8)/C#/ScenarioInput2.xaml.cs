// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

using System;
using System.Collections.Generic;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Navigation;
using SDKTemplateCS;

namespace ScheduledNotificationsSampleCS
{
    internal class NotificationData
    {
        public String ItemType { get; set; }
        public String ItemId { get; set; }
        public String DueTime { get; set; }
        public String InputString { get; set; }
        public Boolean IsTile { get; set; }
    }

    public sealed partial class ScenarioInput2 : Page
    {
        // A pointer back to the main page which is used to gain access to the input and output frames and their content.
        MainPage rootPage = null;

        public ScenarioInput2()
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
                    IsTile = false
                });
            }

            ItemGridView.ItemsSource = bindingList;
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Get a pointer to our main page
            rootPage = e.Parameter as MainPage;
            RefreshListView();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            ItemGridView.ItemsSource = null;
        }
    }
}
