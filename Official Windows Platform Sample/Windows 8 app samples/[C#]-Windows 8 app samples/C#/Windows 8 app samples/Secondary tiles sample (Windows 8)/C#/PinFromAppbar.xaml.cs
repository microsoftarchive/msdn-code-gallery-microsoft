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
    public sealed partial class PinFromAppbar : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;
        Button pinToAppBar;
        StackPanel rightPanel;

        public PinFromAppbar()
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
            Init();
        }

        /// <summary>
        /// Invoked when this page is about to be navigated out in a Frame
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (rightPanel != null)
            {
                rootPage.BottomAppBar.Opened -= BottomAppBar_Opened;
                pinToAppBar.Click -= new RoutedEventHandler(pinToAppBar_Click);
                rightPanel.Children.Remove(pinToAppBar);
                rightPanel = null;
            }
        }

        // This toggles the Pin and unpin button in the app bar
        private void ToggleAppBarButton(bool showPinButton)
        {
            if (pinToAppBar != null)
            {
                pinToAppBar.Style = (showPinButton) ? (this.Resources["PinAppBarButtonStyle"] as Style) : (this.Resources["UnpinAppBarButtonStyle"] as Style);
            }
        }

        void Init()
        {
            rootPage.NotifyUser("To show the bar swipe up from the bottom of the screen, right-click, or press Windows Logo + z. To dismiss the bar, swipe, right-click, or press Windows Logo + z again.", NotifyType.StatusMessage);
            rootPage.BottomAppBar.IsOpen = true;
            rightPanel = rootPage.FindName("RightPanel") as StackPanel;
            rootPage.BottomAppBar.IsOpen = false;

            if (rightPanel != null)
            {
                pinToAppBar = new Button();
                ToggleAppBarButton(!SecondaryTile.Exists(MainPage.appbarTileId));
                // Set up the Click handler for the new button
                pinToAppBar.Click += new RoutedEventHandler(pinToAppBar_Click);
                rootPage.BottomAppBar.Opened += BottomAppBar_Opened;
                rightPanel.Children.Add(pinToAppBar);
            }
        }

        async void pinToAppBar_Click(object sender, RoutedEventArgs e)
        {
            rootPage.BottomAppBar.IsSticky = true;
            // Let us first verify if we need to pin or unpin
            if (SecondaryTile.Exists(MainPage.appbarTileId))
            {
                // First prepare the tile to be unpinned
                SecondaryTile secondaryTile = new SecondaryTile(MainPage.appbarTileId);
                // Now make the delete request.
                bool isUnpinned = await secondaryTile.RequestDeleteForSelectionAsync(MainPage.GetElementRect((FrameworkElement)sender), Windows.UI.Popups.Placement.Above);
                if (isUnpinned)
                {
                    rootPage.NotifyUser(MainPage.appbarTileId + " unpinned.", NotifyType.StatusMessage);
                }
                else
                {
                    rootPage.NotifyUser(MainPage.appbarTileId + " not unpinned.", NotifyType.ErrorMessage);
                }

                ToggleAppBarButton(isUnpinned);
            }
            else
            {

                // Prepare package images for use as the Tile Logo in our tile to be pinned
                Uri logo = new Uri("ms-appx:///Assets/squareTile-sdk.png");

                // During creation of secondary tile, an application may set additional arguments on the tile that will be passed in during activation.
                // These arguments should be meaningful to the application. In this sample, we'll pass in the date and time the secondary tile was pinned.
                string tileActivationArguments = MainPage.appbarTileId + " WasPinnedAt=" + DateTime.Now.ToLocalTime().ToString();

                // Create a 1x1 Secondary tile
                SecondaryTile secondaryTile = new SecondaryTile(MainPage.appbarTileId,
                                                                "Secondary tile pinned via AppBar",
                                                                "SDK Sample Secondary Tile pinned from AppBar",
                                                                tileActivationArguments,
                                                                TileOptions.ShowNameOnLogo,
                                                                logo);

                // Specify a foreground text value.
                // The tile background color is inherited from the parent unless a separate value is specified.
                secondaryTile.ForegroundText = ForegroundText.Dark;

                // OK, the tile is created and we can now attempt to pin the tile.
                // Note that the status message is updated when the async operation to pin the tile completes.
                bool isPinned = await secondaryTile.RequestCreateForSelectionAsync(MainPage.GetElementRect((FrameworkElement)sender), Windows.UI.Popups.Placement.Above);
                if (isPinned)
                {
                    rootPage.NotifyUser(MainPage.appbarTileId + " successfully pinned.", NotifyType.StatusMessage);
                }
                else
                {
                    rootPage.NotifyUser(MainPage.appbarTileId + " not pinned.", NotifyType.ErrorMessage);
                }

                ToggleAppBarButton(!isPinned);
            }
            rootPage.BottomAppBar.IsSticky = false;
        }

        void BottomAppBar_Opened(object sender, object e)
        {
            ToggleAppBarButton(!SecondaryTile.Exists(MainPage.appbarTileId));
        }
     }
}
