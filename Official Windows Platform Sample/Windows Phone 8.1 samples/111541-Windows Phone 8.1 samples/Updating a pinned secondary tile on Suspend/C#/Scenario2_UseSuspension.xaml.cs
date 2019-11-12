// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TileUpdateAfterSuspension
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario2 : Page
    {
        // Uncomment the following line if you need a pointer back to the MainPage.
        // This value should be obtained in OnNavigatedTo in the e.Parameter argument
        //private MainPage rootPage;

        public Scenario2()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        const string SCENARIO2_TILEID = "Scenario2.Tile";

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
            App.OnNewTilePinned = UpdateTile;

            // Create the original Square150x150 tile. The image to display on the tile has a purple background and the word "Original" in white text.
            var tile = new SecondaryTile(SCENARIO2_TILEID, "Scenario 2", "/MainPage.xaml?scenario=Scenario2", new Uri("ms-appx:///Assets/originalTileImage.png"), TileSize.Default);
            tile.VisualElements.ShowNameOnSquare150x150Logo = true;
            await tile.RequestCreateAsync();

            // When a new tile is created, the app will be suspended and the new tile will be displayed to the user on the start screen.
            // Any code after the call to RequestCreateAsync is not guaranteed to run. 
            // For example, a common scenario is to associate a push channel with the newly created tile,
            // which involves a call to WNS to get a channel using the CreatePushNotificationChannelForSecondaryTileAsync() asynchronous operation. 
            // Another example is updating the secondary tile with data from a web service. Both of these are examples of actions that may not
            // complete before the app is suspended. To illustrate this, we'll create a delay and then attempt to update our secondary tile.
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
                Task.Delay(8000).Wait();
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
