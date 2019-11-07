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
    public sealed partial class SendLocalImageTile : SDKTemplate.Common.LayoutAwarePage
    {
        #region TemplateCode
        MainPage rootPage = MainPage.Current;

        public SendLocalImageTile()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }
        #endregion TemplateCode

        void UpdateTileWithImage_Click(object sender, RoutedEventArgs e)
        {
            // Note: This sample contains an additional project, NotificationsExtensions.
            // NotificationsExtensions exposes an object model for creating notifications, but you can also 
            // modify the strings directly. See UpdateTileWithImageWithStringManipulation_Click for an example

            // Create notification content based on a visual template.
            ITileWideImageAndText01 tileContent = TileContentFactory.CreateTileWideImageAndText01();

            tileContent.TextCaptionWrap.Text = "This tile notification uses ms-appx images";
            tileContent.Image.Src = "ms-appx:///images/redWide.png";
            tileContent.Image.Alt = "Red image";

            // Users can resize tiles to square or wide.
            // Apps can choose to include only square assets (meaning the app's tile can never be wide), or
            // include both wide and square assets (the user can resize the tile to square or wide).
            // Apps should not include only wide assets.

            // Apps that support being wide should include square tile notifications since users
            // determine the size of the tile.

            // create the square template and attach it to the wide template
            ITileSquareImage squareContent = TileContentFactory.CreateTileSquareImage();
            squareContent.Image.Src = "ms-appx:///images/graySquare.png";
            squareContent.Image.Alt = "Gray image";
            tileContent.SquareContent = squareContent;

            // Send the notification to the app's application tile.
            TileUpdateManager.CreateTileUpdaterForApplication().Update(tileContent.CreateNotification());

            OutputTextBlock.Text = MainPage.PrettyPrint(tileContent.GetContent());
        }

        void UpdateTileWithImageWithStringManipulation_Click(object sender, RoutedEventArgs e)
        {
            // create a string with the tile template xml
            string tileXmlString = "<tile>"
                              + "<visual>"
                              + "<binding template='TileWideImageAndText01'>"
                              + "<text id='1'>This tile notification uses ms-appx images</text>"
                              + "<image id='1' src='ms-appx:///images/redWide.png' alt='Red image'/>"
                              + "</binding>"
                              + "<binding template='TileSquareImage'>"
                              + "<image id='1' src='ms-appx:///images/graySquare.png' alt='Gray image'/>"
                              + "</binding>"
                              + "</visual>"
                              + "</tile>";

            // create a DOM
            Windows.Data.Xml.Dom.XmlDocument tileDOM = new Windows.Data.Xml.Dom.XmlDocument();
            // load the xml string into the DOM, catching any invalid xml characters 
            tileDOM.LoadXml(tileXmlString);

            // create a tile notification
            TileNotification tile = new TileNotification(tileDOM);

            // send the notification to the app's application tile
            TileUpdateManager.CreateTileUpdaterForApplication().Update(tile);

            OutputTextBlock.Text = MainPage.PrettyPrint(tileDOM.GetXml());
        }

        private void ClearTile_Click(object sender, RoutedEventArgs e)
        {
            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
            OutputTextBlock.Text = "Tile cleared";
        }
    }
}
