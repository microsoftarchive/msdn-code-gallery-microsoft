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
using Windows.UI.Notifications;
using NotificationsExtensions.TileContent;

namespace Tiles
{
    public sealed partial class EnableNotificationQueue : SDKTemplate.Common.LayoutAwarePage
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
            OutputTextBlock.Text = "Tile cleared";
        }

        void UpdateTile_Click(object sender, RoutedEventArgs e)
        {
            ITileWideText03 tileContent = TileContentFactory.CreateTileWideText03();
            tileContent.TextHeadingWrap.Text = TextContent.Text;

            ITileSquareText04 squareTileContent = TileContentFactory.CreateTileSquareText04();
            squareTileContent.TextBodyWrap.Text = TextContent.Text;
            tileContent.SquareContent = squareTileContent;

            TileNotification tileNotification = tileContent.CreateNotification();

            string tag = "TestTag01";
            if (!Id.Text.Equals(String.Empty))
            {
                tag = Id.Text;
            }
            
            // set the tag on the notification
            tileNotification.Tag = tag;
            TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification);

            OutputTextBlock.Text = "Tile notification sent. It is tagged with " + tag;
        }

        void EnableNotificationQueue_Click(object sender, RoutedEventArgs e)
        {
            // Enable the notification queue - this only needs to be called once in the lifetime of your app
            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(true);
            OutputTextBlock.Text = "Notification cycling enabled";
        }

        void DisableNotificationQueue_Click(object sender, RoutedEventArgs e)
        {
            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(false);
            OutputTextBlock.Text = "Notification cycling disabled";
        }
    }
}
