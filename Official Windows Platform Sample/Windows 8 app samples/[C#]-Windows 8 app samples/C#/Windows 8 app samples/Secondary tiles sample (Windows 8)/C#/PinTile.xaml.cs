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
using SDKTemplate;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace SecondaryTiles
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PinTile : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;
        AppBar appBar;

        public PinTile()
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
        /// This is the click handler for the 'PinButton' button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void PinButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                // Prepare package images for use as the Tile Logo and small Logo in our tile to be pinned
                Uri logo = new Uri("ms-appx:///Assets/squareTile-sdk.png");
                Uri smallLogo = new Uri("ms-appx:///Assets/smallTile-sdk.png");

                // During creation of secondary tile, an application may set additional arguments on the tile that will be passed in during activation.
                // These arguments should be meaningful to the application. In this sample, we'll pass in the date and time the secondary tile was pinned.
                string tileActivationArguments = MainPage.logoSecondaryTileId + " WasPinnedAt=" + DateTime.Now.ToLocalTime().ToString();

                // Create a 1x1 Secondary tile
                SecondaryTile secondaryTile = new SecondaryTile(MainPage.logoSecondaryTileId,
                                                                "Title text shown on the tile",
                                                                "Name of the tile the user sees when searching for the tile",
                                                                tileActivationArguments,
                                                                TileOptions.ShowNameOnLogo,
                                                                logo);

                // Specify a foreground text value.
                // The tile background color is inherited from the parent unless a separate value is specified.
                secondaryTile.ForegroundText = ForegroundText.Dark;

                // Like the background color, the small tile logo is inherited from the parent application tile by default. Let's override it, just to see how that's done.
                secondaryTile.SmallLogo = smallLogo;

                // OK, the tile is created and we can now attempt to pin the tile.
                // Note that the status message is updated when the async operation to pin the tile completes.
                bool isPinned = await secondaryTile.RequestCreateForSelectionAsync(MainPage.GetElementRect((FrameworkElement)sender), Windows.UI.Popups.Placement.Below);

                if (isPinned)
                {
                    rootPage.NotifyUser("Secondary tile successfully pinned.", NotifyType.StatusMessage);
                }
                else
                {
                    rootPage.NotifyUser("Secondary tile not pinned.", NotifyType.ErrorMessage);
                }
            }
        }
    }
}
