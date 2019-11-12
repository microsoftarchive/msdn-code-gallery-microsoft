//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using NotificationsExtensions.TileContent;
using Windows.UI.Xaml.Controls;using SDKTemplate;
using System;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace Tiles
{
    public sealed partial class EnableNotificationQueue : Page
    {
        #region TemplateCode
        MainPage rootPage = MainPage.Current;

        public EnableNotificationQueue()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }
        #endregion TemplateCode

        void ClearTile_Click(object sender, RoutedEventArgs e)
        {
            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
            rootPage.NotifyUser("Tile cleared", NotifyType.StatusMessage);
        }

        void UpdateTile_Click(object sender, RoutedEventArgs e)
        {
            // Create a notification for the Square310x310 tile using one of the available templates for the size.
            ITileSquare310x310Text09 square310x310TileContent = TileContentFactory.CreateTileSquare310x310Text09();
            square310x310TileContent.TextHeadingWrap.Text = TextContent.Text;

            // Create a notification for the Wide310x150 tile using one of the available templates for the size.
            ITileWide310x150Text03 wide310x150TileContent = TileContentFactory.CreateTileWide310x150Text03();
            wide310x150TileContent.TextHeadingWrap.Text = TextContent.Text;

            // Create a notification for the Square150x150 tile using one of the available templates for the size.
            ITileSquare150x150Text04 square150x150TileContent = TileContentFactory.CreateTileSquare150x150Text04();
            square150x150TileContent.TextBodyWrap.Text = TextContent.Text;

            // Attach the Square150x150 template to the Wide310x150 template.
            wide310x150TileContent.Square150x150Content = square150x150TileContent;

            // Attach the Wide310x150 template to the Square310x310 template.
            square310x310TileContent.Wide310x150Content = wide310x150TileContent;

            TileNotification tileNotification = square310x310TileContent.CreateNotification();

            string tag = "TestTag01";
            if (!Id.Text.Equals(String.Empty))
            {
                tag = Id.Text;
            }

            // Set the tag on the notification.
            tileNotification.Tag = tag;
            TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification);

            rootPage.NotifyUser("Tile notification sent. It is tagged with '" + tag + "'.", NotifyType.StatusMessage);
        }

        void EnableNotificationQueue_Click(object sender, RoutedEventArgs e)
        {
            // Enable the notification queue - this only needs to be called once in the lifetime of your app.
            // Note that the default is false.
            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(true);
            rootPage.NotifyUser("Notification cycling enabled for all tile sizes.", NotifyType.StatusMessage);
        }

        void DisableNotificationQueue_Click(object sender, RoutedEventArgs e)
        {
            // Disable the notification queue.
            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(false);
            rootPage.NotifyUser("Notification cycling disabled for all tile sizes.", NotifyType.StatusMessage);
        }

        void EnableSquare150x150NotificationQueue_Click(object sender, RoutedEventArgs e)
        {
            // Enable the notification queue for the medium (Square150x150) tile size, without affecting the setting for the other tile sizes.
            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueueForSquare150x150(true);
            rootPage.NotifyUser("Notification cycling enabled for medium (Square150x150) tiles.", NotifyType.StatusMessage);
        }

        void DisableSquare150x150NotificationQueue_Click(object sender, RoutedEventArgs e)
        {
            // Disable the notification queue for the medium (Square150x150) tile size, without affecting the setting for the other tile sizes.
            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueueForSquare150x150(false);
            rootPage.NotifyUser("Notification cycling disabled for medium (Square150x150) tiles.", NotifyType.StatusMessage);
        }

        void EnableWide310x150NotificationQueue_Click(object sender, RoutedEventArgs e)
        {
            // Enable the notification queue for the wide (Wide310x150) tile size, without affecting the setting for the other tile sizes.
            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueueForWide310x150(true);
            rootPage.NotifyUser("Notification cycling enabled for wide (Wide310x150) tiles.", NotifyType.StatusMessage);
        }

        void DisableWide310x150NotificationQueue_Click(object sender, RoutedEventArgs e)
        {
            // Disable the notification queue for the wide (Wide310x150) tile size, without affecting the setting for the other tile sizes.
            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueueForWide310x150(false);
            rootPage.NotifyUser("Notification cycling disabled for wide (Wide310x150) tiles.", NotifyType.StatusMessage);
        }

        void EnableSquare310x310NotificationQueue_Click(object sender, RoutedEventArgs e)
        {
            // Enable the notification queue for the large (Square310x310)tile size, without affecting the setting for the other tile sizes.
            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueueForSquare310x310(true);
            rootPage.NotifyUser("Notification cycling enabled for large (Square310x310) tiles.", NotifyType.StatusMessage);
        }

        void DisableSquare310x310NotificationQueue_Click(object sender, RoutedEventArgs e)
        {
            // Disable the notification queue for the large (Square310x310) tile size, without affecting the setting for the other tile sizes.
            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueueForSquare310x310(false);
            rootPage.NotifyUser("Notification cycling disabled for large (Square310x310) tiles.", NotifyType.StatusMessage);
        }
    }
}