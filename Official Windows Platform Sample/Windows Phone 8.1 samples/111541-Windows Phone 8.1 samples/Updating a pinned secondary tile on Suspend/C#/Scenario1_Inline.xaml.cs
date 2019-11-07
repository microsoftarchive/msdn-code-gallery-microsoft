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
    /// When you create and pin a tile in Windows Phone 8.1, your app will be suspended.
    /// Any code that follows the called to RequestCreateAsync is therefore not guaranteed to run.
    /// This scenario demonstrates this behavior. This approach is not recommended. It is shown here
    /// for illustration purposes only. 
    /// See Scenario 2 for the correct pattern to use when you need to perform
    /// an action when the secondary tile is created. 
    /// </summary>
    public sealed partial class Scenario1 : Page
    {
        private MainPage rootPage;

        public Scenario1()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            rootPage = MainPage.Current;
        }

        const string SCENARIO1_TILEID = "Scenario1.Tile";

        /// <summary>
        /// Removes all secondary tiles that were pinned by this app in previous runs of this scenario
        /// from the start screen.
        /// </summary>
        private async void Clean_Click(object sender, RoutedEventArgs e)
        {
            // Find all secondary tiles.
            var secondaryTiles = await SecondaryTile.FindAllAsync();
            foreach (var secondaryTile in secondaryTiles)
            {
                // We'll be good citizens and only remove the secondary tile belonging
                // to this scenario. To remove all secondary tiles, remove this check.
                if (secondaryTile.TileId == SCENARIO1_TILEID)
                {
                    // Delete the secondary tile.
                    // Note: On Windows Phone, the call to RequestDeleteAsync deletes the tile without prompting the user.
                    await secondaryTile.RequestDeleteAsync();
                }
            }
        }

        
        private async void PinAndUpdate_Click(object sender, RoutedEventArgs e)
        {
            // Create the secondary tile
            var tile = new SecondaryTile(SCENARIO1_TILEID, "Scenario 1", "/MainPage.xaml?scenario=Scenario1", new Uri("ms-appx:///Assets/originalTileImage.png"), TileSize.Default);
            tile.VisualElements.ShowNameOnSquare150x150Logo = true;
            await tile.RequestCreateAsync();

            // When a new tile is created, the app is suspended and the new tile is displayed to the user on the start screen.
            // Any code after the call to RequestCreateAsync is not guaranteed to run. 
            // For example, a common scenario is to associate a push channel with the newly created tile,
            // which involves a call to WNS to get a channel using the CreatePushNotificationChannelForSecondaryTileAsync() asynchronous operation. 
            // Another example is updating the secondary tile with data from a web service. Both of these are examples of actions that may not
            // complete before the app is suspended. To illustrate this, we'll create a delay and then attempt to update our secondary tile.

            // Simulate a long-running task
            if (Debugger.IsAttached)
            {
                // Set a larger delay to give you time to select "Suspend" from the "LifetimeEvents" dropdown in Visual Studio in 
                // order to simulate the app being suspended when the new tile is created. 
                await Task.Delay(8000);
            }
            else
            {
                // When the app is not attached to the debugger, the app will be suspended so we can use a 
                // more realistic delay.
                await Task.Delay(2000);
            }

            // If the app is suspended before reaching this point, the following code will never run.

            // Update the tile we created using a notification.
            var tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150Image);

            // retrieve the single image element from the TileSquare150x150Image template. .
            var imageElement = tileXml.GetElementsByTagName("image").Single();

            // Set the src propertry on the image entry.
            imageElement.Attributes.GetNamedItem("src").NodeValue = "ms-appx:///Assets/updatedTileImage.png";

            // Create a new tile notification.
            var notification = new Windows.UI.Notifications.TileNotification(tileXml);

            // Create a tile updater.
            var updater = TileUpdateManager.CreateTileUpdaterForSecondaryTile(SCENARIO1_TILEID);

            // Send the update notification for the tile. 
            updater.Update(notification);
        }
    }
}
