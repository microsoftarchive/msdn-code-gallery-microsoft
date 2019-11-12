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
    public sealed partial class NotificationExpiration : Page
    {
        #region TemplateCode
        MainPage rootPage = MainPage.Current;

        public NotificationExpiration()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        #endregion TemplateCode

        void UpdateTileExpiring_Click(object sender, RoutedEventArgs e)
        {
            int seconds;
            if (!Int32.TryParse(Time.Text, out seconds))
            {
                seconds = 10;
            }

            Windows.Globalization.Calendar cal = new Windows.Globalization.Calendar();
            cal.SetToNow();
            cal.AddSeconds(seconds);

            var longTime = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter("longtime");
            DateTimeOffset expiryTime = cal.GetDateTime();
            string expiryTimeString = longTime.Format(expiryTime);

            // Create a notification for the Square310x310 tile using one of the available templates for the size.
            ITileSquare310x310Text09 tileSquare310x310Content = TileContentFactory.CreateTileSquare310x310Text09();
            tileSquare310x310Content.TextHeadingWrap.Text = "This notification will expire at " + expiryTimeString;

            // Create a notification for the Wide310x150 tile using one of the available templates for the size.
            ITileWide310x150Text04 wide310x150TileContent = TileContentFactory.CreateTileWide310x150Text04();
            wide310x150TileContent.TextBodyWrap.Text = "This notification will expire at " + expiryTimeString;

            // Create a notification for the Square150x150 tile using one of the available templates for the size.
            ITileSquare150x150Text04 square150x150TileContent = TileContentFactory.CreateTileSquare150x150Text04();
            square150x150TileContent.TextBodyWrap.Text = "This notification will expire at " + expiryTimeString;

            // Attach the Square150x150 template to the Wide310x150 template.
            wide310x150TileContent.Square150x150Content = square150x150TileContent;

            // Attach the Wide310x150 template to the Square310x310 template.
            tileSquare310x310Content.Wide310x150Content = wide310x150TileContent;

            TileNotification tileNotification = tileSquare310x310Content.CreateNotification();

            // Set the expiration time and update the tile.
            tileNotification.ExpirationTime = expiryTime;
            TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification);

            rootPage.NotifyUser("Tile notification sent. It will expire at " + expiryTime, NotifyType.StatusMessage);
        }
    }
}