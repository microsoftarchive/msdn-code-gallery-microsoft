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
using NotificationsExtensions.TileContent;
using Windows.UI.Notifications;

namespace Tiles
{
    public sealed partial class NotificationExpiration : SDKTemplate.Common.LayoutAwarePage
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

            ITileWideText04 tileContent = TileContentFactory.CreateTileWideText04();
            tileContent.TextBodyWrap.Text = "This notification will expire at " + expiryTimeString;

            ITileSquareText04 squareTileContent = TileContentFactory.CreateTileSquareText04();
            squareTileContent.TextBodyWrap.Text = "This notification will expire at " + expiryTimeString;
            tileContent.SquareContent = squareTileContent;

            TileNotification tileNotification = tileContent.CreateNotification();

            // set the expirationTime
            tileNotification.ExpirationTime = expiryTime;
            TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification);

            OutputTextBlock.Text = "Tile notification sent. It will expire at " + expiryTimeString;
        }
    }
}
