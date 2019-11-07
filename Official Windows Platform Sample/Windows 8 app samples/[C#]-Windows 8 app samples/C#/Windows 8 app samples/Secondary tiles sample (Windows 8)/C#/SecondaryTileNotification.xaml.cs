//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System;
using NotificationsExtensions.BadgeContent;
using NotificationsExtensions.TileContent;
using SDKTemplate;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace SecondaryTiles
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SecondaryTileNotification : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;
        AppBar appBar;

        public SecondaryTileNotification()
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
            // Preserve the app bar
            appBar = rootPage.BottomAppBar;
            // this ensures the app bar is not shown in this scenario
            rootPage.BottomAppBar = null;
            ToggleButtons(SecondaryTile.Exists(MainPage.dynamicTileId));
        }

        /// <summary>
        /// Invoked when this page is about to be navigated out in a Frame
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            // Restore the app bar
            rootPage.BottomAppBar = appBar;
        }


        /// <summary>
        /// Enables or disables the notification buttons
        /// </summary>
        /// <param name="isEnabled"> enables if true</param>
        private void ToggleButtons(bool isEnabled)
        {
            SendBadgeNotification.IsEnabled = isEnabled;
            SendTileNotification.IsEnabled = isEnabled;
            SendBadgeNotificationString.IsEnabled = isEnabled;
            SendTileNotificationString.IsEnabled = isEnabled;
        }

        /// <summary>
        /// This is the click handler for the 'Pin Tile' button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void PinLiveTile_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                // Prepare package images for use as the Tile Logo and small Logo in our tile to be pinned
                Uri logo = new Uri("ms-appx:///Assets/squareTile-sdk.png");
                Uri wideLogo = new Uri("ms-appx:///Assets/tile-sdk.png");

                // During creation of secondary tile, an application may set additional arguments on the tile that will be passed in during activation.
                // These arguments should be meaningful to the application. In this sample, we'll pass in the date and time the secondary tile was pinned.
                string tileActivationArguments = MainPage.dynamicTileId + " WasPinnedAt=" + DateTime.Now.ToLocalTime().ToString();

                // Create a Secondary tile
                SecondaryTile secondaryTile = new SecondaryTile(MainPage.dynamicTileId,
                                                                "A Live Secondary Tile",
                                                                "Secondary Tile Sample Live Secondary Tile",
                                                                tileActivationArguments,
                                                                TileOptions.ShowNameOnLogo | TileOptions.ShowNameOnWideLogo,
                                                                logo,
                                                                wideLogo);

                // Specify a foreground text value.
                // The tile background color is inherited from the parent unless a separate value is specified.
                secondaryTile.ForegroundText = ForegroundText.Light;

                // OK, the tile is created and we can now attempt to pin the tile.
                // Note that the status message is updated when the async operation to pin the tile completes.
                bool isPinned = await secondaryTile.RequestCreateForSelectionAsync(MainPage.GetElementRect((FrameworkElement)sender), Windows.UI.Popups.Placement.Below);

                if (isPinned)
                {
                    rootPage.NotifyUser("Secondary tile successfully pinned.", NotifyType.StatusMessage);
                    ToggleButtons(true);
                }
                else
                {
                    rootPage.NotifyUser("Secondary tile not pinned.", NotifyType.ErrorMessage);
                }
            }
        }

        /// <summary>
        /// This is the click handler for the 'Sending tile notification' button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendTileNotification_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                if (SecondaryTile.Exists(MainPage.dynamicTileId))
                {
	            // Note: This sample contains an additional reference, NotificationsExtensions, which you can use in your apps
        	    ITileWideText04 tileContent = TileContentFactory.CreateTileWideText04();
                    tileContent.TextBodyWrap.Text = "Sent to a secondary tile from NotificationsExtensions!";
            
                    ITileSquareText04 squareContent = TileContentFactory.CreateTileSquareText04();
                    squareContent.TextBodyWrap.Text = "Sent to a secondary tile from NotificationExtensions!";
                    tileContent.SquareContent = squareContent;

                    // Send the notification to the secondary tile by creating a secondary tile updater
                    TileUpdateManager.CreateTileUpdaterForSecondaryTile(MainPage.dynamicTileId).Update(tileContent.CreateNotification());

                    rootPage.NotifyUser("Tile notification sent to " + MainPage.dynamicTileId, NotifyType.StatusMessage);
                }
                else
                {
                    ToggleButtons(false);
                    rootPage.NotifyUser(MainPage.dynamicTileId + " not pinned.", NotifyType.ErrorMessage);
                }
            }
        }

        /// <summary>
        /// This is the click handler for the 'Other' button.  You would replace this with your own handler
        /// if you have a button or buttons on this page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendBadgeNotification_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                if (SecondaryTile.Exists(MainPage.dynamicTileId))
                {
                    BadgeNumericNotificationContent badgeContent = new BadgeNumericNotificationContent(6);

                    // Send the notification to the secondary tile
                    BadgeUpdateManager.CreateBadgeUpdaterForSecondaryTile(MainPage.dynamicTileId).Update(badgeContent.CreateNotification());

                    rootPage.NotifyUser("Badge notification sent to " + MainPage.dynamicTileId, NotifyType.StatusMessage);
                }
                else
                {
                    ToggleButtons(false);
                    rootPage.NotifyUser(MainPage.dynamicTileId + " not pinned.", NotifyType.ErrorMessage);
                }
            }
        }

        private void SendTileNotificationWithStringManipulation_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                string tileXmlString = "<tile>"
                                     + "<visual>"
                                     + "<binding template='TileWideText04'>"
                                     + "<text id='1'>Send to a secondary tile from strings</text>"
                                     + "</binding>"
                                     + "<binding template='TileSquareText04'>"
                                     + "<text id='1'>Send to a secondary tile from strings</text>"
                                     + "</binding>"
                                     + "</visual>"
                                     + "</tile>";

                Windows.Data.Xml.Dom.XmlDocument tileDOM = new Windows.Data.Xml.Dom.XmlDocument();
                tileDOM.LoadXml(tileXmlString);
                TileNotification tile = new TileNotification(tileDOM);

                // Send the notification to the secondary tile by creating a secondary tile updater
                TileUpdateManager.CreateTileUpdaterForSecondaryTile(MainPage.dynamicTileId).Update(tile);

                rootPage.NotifyUser("Tile notification sent to " + MainPage.dynamicTileId, NotifyType.StatusMessage);
            }
        }

        private void SendBadgeNotificationWithStringManipulation_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                string badgeXmlString = "<badge value='9'/>";
                Windows.Data.Xml.Dom.XmlDocument badgeDOM = new Windows.Data.Xml.Dom.XmlDocument();
                badgeDOM.LoadXml(badgeXmlString);
                BadgeNotification badge = new BadgeNotification(badgeDOM);

                // Send the notification to the secondary tile
                BadgeUpdateManager.CreateBadgeUpdaterForSecondaryTile(MainPage.dynamicTileId).Update(badge);

                rootPage.NotifyUser("Badge notification sent to " + MainPage.dynamicTileId, NotifyType.StatusMessage);
            }
        }
    }
}
