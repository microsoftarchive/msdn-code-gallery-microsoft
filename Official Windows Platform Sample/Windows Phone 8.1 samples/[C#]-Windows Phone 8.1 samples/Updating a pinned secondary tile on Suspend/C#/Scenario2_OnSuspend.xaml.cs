// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;


namespace TileUpdateAfterSuspension
{
    /// <summary>
    /// Demonstrates how to update a newly pinned tile during the Suspending event. 
    /// </summary>
    public sealed partial class Scenario2 : Page
    {
        public Scenario2()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        const string SCENARIO2_TILEID = "Scenario2.Tile";

        /// <summary>
        /// Removes all secondary tiles that were pinned by this app in previous runs of this scenario
        /// from the start screen.
        /// </summary>
        private async void Clean_Click(object sender, RoutedEventArgs e)
        {
            // Find all secondary tiles
            var secondaryTiles = await SecondaryTile.FindAllAsync();
            foreach (var secondaryTile in secondaryTiles)
            {
                // We'll be good citizens and only remove the secondary tile belonging
                // to this scenario. To remove all secondary tiles, remove this check.
                if (secondaryTile.TileId == SCENARIO2_TILEID)
                {
                    // Delete the secondary tile.
                    // Note: On Windows Phone, the call to RequestDeleteAsync deletes the tile without prompting the user
                    await secondaryTile.RequestDeleteAsync();
                }
            }
        }

        private async void PinAndUpdate_Click(object sender, RoutedEventArgs e)
        {
            // Before creating and pinning the new tile, we'll set an action
            // to be called during the Suspending event to update the tile. 
            // This action is called from OnSuspending in App.xaml.cs
            App.OnNewTilePinned = UpdateTile;

            // Create the original Square150x150 tile. The image to display on the tile has a purple background and the word "Original" in white text.
            var tile = new SecondaryTile(SCENARIO2_TILEID, "Scenario 2", "/MainPage.xaml?scenario=Scenario2", new Uri("ms-appx:///Assets/originalTileImage.png"), TileSize.Default);
            tile.VisualElements.ShowNameOnSquare150x150Logo = true;
            await tile.RequestCreateAsync();
        }

        /// <summary>
        /// Delegate to update the secondary tile. This is called from the OnSuspending event handler in App.xaml.cs
        /// </summary>
        private void UpdateTile()
        {
            // Simulate a long-running task. For illustration purposes only. 
            if (Debugger.IsAttached)
            {
                // Set a larger delay to give you time to select "Suspend" from the "LifetimeEvents" dropdown in Visual Studio in 
                // order to simulate the app being suspended when the new tile is created. 
                Task.Delay(5000).Wait();
            }
            else
            {
                // When the app is not attached to the debugger, the app will be suspended so we can use a 
                // more realistic delay.
                Task.Delay(2000).Wait();
            }

            // Update the tile we created using a notification.
            var tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150Image);

            // The TileSquare150x150Image template only contains one image entry, so retrieve it.
            var imageElement = tileXml.GetElementsByTagName("image").Single();

            // Set the src propertry on the image entry. The image in this sample is a lime green image with the word "Updated" in white text 
            imageElement.Attributes.GetNamedItem("src").NodeValue = "ms-appx:///Assets/updatedTileImage.png";

            // Create a new tile notification.
            var notification = new Windows.UI.Notifications.TileNotification(tileXml);

            // Create a tile updater.
            var updater = TileUpdateManager.CreateTileUpdaterForSecondaryTile(SCENARIO2_TILEID);

            // Send the update notification for the tile. 
            updater.Update(notification);
        }
    }
}
