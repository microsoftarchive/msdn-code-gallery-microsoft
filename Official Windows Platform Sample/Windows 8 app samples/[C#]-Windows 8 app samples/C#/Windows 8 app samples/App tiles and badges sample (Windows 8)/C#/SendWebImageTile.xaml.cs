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
    public sealed partial class SendWebImageTile : SDKTemplate.Common.LayoutAwarePage
    {
        #region TemplateCode
        MainPage rootPage = MainPage.Current;

        public SendWebImageTile()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }
        #endregion TemplateCode

        void UpdateTileWithWebImage_Click(object sender, RoutedEventArgs e)
        {
            // Note: This sample contains an additional project, NotificationsExtensions.
            // NotificationsExtensions exposes an object model for creating notifications, but you can also 
            // modify the strings directly. See UpdateTileWithWebImageWithStringManipulation_Click for an example

            // Create notification content based on a visual template.
            ITileWideImageAndText01 tileContent = TileContentFactory.CreateTileWideImageAndText01();

            tileContent.TextCaptionWrap.Text = "This tile notification uses web images.";

            // !Important!
            // The Internet (Client) capability must be checked in the manifest in the Capabilities tab
            // to display web images in tiles (either the http:// or https:// protocols)

            tileContent.Image.Src = ImageUrl.Text;
            tileContent.Image.Alt = "Web image";

            // Users can resize tiles to square or wide.
            // Apps can choose to include only square assets (meaning the app's tile can never be wide), or
            // include both wide and square assets (the user can resize the tile to square or wide).
            // Apps cannot include only wide assets.

            // Apps that support being wide should include square tile notifications since users
            // determine the size of the tile.

            // Create square notification content based on a visual template.
            ITileSquareImage squareContent = TileContentFactory.CreateTileSquareImage();

            squareContent.Image.Src = ImageUrl.Text;
            squareContent.Image.Alt = "Web image";

            // include the square template.
            tileContent.SquareContent = squareContent;

            // send the notification to the app's application tile
            TileUpdateManager.CreateTileUpdaterForApplication().Update(tileContent.CreateNotification());

            OutputTextBlock.Text = MainPage.PrettyPrint(tileContent.GetContent());
        }

        void UpdateTileWithWebImageWithStringManipulation_Click(object sender, RoutedEventArgs e)
        {
            // create a string with the tile template xml
            string tileXmlString = "<tile>"
                             + "<visual>"
                             + "<binding template='TileWideImageAndText01'>"
                             + "<text id='1'>This tile notification uses web images</text>"
                             + "<image id='1' src='" + ImageUrl.Text + "' alt='Web image'/>"
                             + "</binding>"
                             + "<binding template='TileSquareImage'>"
                             + "<image id='1' src='" + ImageUrl.Text + "' alt='Web image'/>"
                             + "</binding>"
                             + "</visual>"
                             + "</tile>";

            // create a DOM
            Windows.Data.Xml.Dom.XmlDocument tileDOM = new Windows.Data.Xml.Dom.XmlDocument();
            try
            {
                // load the xml string into the DOM, catching any invalid xml characters 
                tileDOM.LoadXml(tileXmlString);

                // create a tile notification
                TileNotification tile = new TileNotification(tileDOM);

                // send the notification to the app's application tile
                TileUpdateManager.CreateTileUpdaterForApplication().Update(tile);

                OutputTextBlock.Text = MainPage.PrettyPrint(tileDOM.GetXml());
            }
            catch (Exception)
            {
                OutputTextBlock.Text = "Error loading the xml, check for invalid characters in the input";
            }
        }

        private void ClearTile_Click(object sender, RoutedEventArgs e)
        {
            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
            OutputTextBlock.Text = "Tile cleared";
        }
    }
}
