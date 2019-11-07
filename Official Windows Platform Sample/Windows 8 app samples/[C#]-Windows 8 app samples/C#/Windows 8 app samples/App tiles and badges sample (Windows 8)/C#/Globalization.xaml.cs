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
using Windows.ApplicationModel.Resources.Core;

namespace Tiles
{
    public sealed partial class Globalization : SDKTemplate.Common.LayoutAwarePage
    {
        #region TemplateCode
        MainPage rootPage = MainPage.Current;

        public Globalization()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }
        #endregion TemplateCode

        void ViewCurrentResources_Click(object sender, RoutedEventArgs e)
        {
            string asls;
            ResourceManager.Current.DefaultContext.QualifierValues.TryGetValue("Language", out asls);

            string scale;
            ResourceManager.Current.DefaultContext.QualifierValues.TryGetValue("Scale", out scale);

            string contrast;
            ResourceManager.Current.DefaultContext.QualifierValues.TryGetValue("Contrast", out contrast);

            OutputTextBlock.Text = "Your system is currently set to the following values: Application Language: " + asls + ", Scale: " + scale + ", Contrast: " + contrast + ". If using web images and AddImageQuery, the following query string would be appened to the URL: ?ms-lang=" + asls + "&ms-scale=" + scale + "&ms-contrast=" + contrast;
        }

        void SendTileNotificationWithQueryStrings_Click(object sender, RoutedEventArgs e)
        {
            ITileWideImageAndText01 tileContent = TileContentFactory.CreateTileWideImageAndText01();
            tileContent.TextCaptionWrap.Text = "This tile notification uses query strings for the image src.";

            tileContent.Image.Src = ImageUrl.Text;
            tileContent.Image.Alt = "Web image";

            // enable AddImageQuery on the notification
            tileContent.AddImageQuery = true;

            ITileSquareImage squareContent = TileContentFactory.CreateTileSquareImage();
            squareContent.Image.Src = ImageUrl.Text;
            squareContent.Image.Alt = "Web image";

            // include the square template.
            tileContent.SquareContent = squareContent;

            // send the notification to the app's application tile
            TileUpdateManager.CreateTileUpdaterForApplication().Update(tileContent.CreateNotification());

            OutputTextBlock.Text = MainPage.PrettyPrint(tileContent.GetContent());            
        }

        void SendTileNotification_Click(object sender, RoutedEventArgs e)
        {
            string scale;
            ResourceManager.Current.DefaultContext.QualifierValues.TryGetValue("Scale", out scale);

            ITileWideSmallImageAndText03 tileContent = TileContentFactory.CreateTileWideSmallImageAndText03();
            tileContent.TextBodyWrap.Text = "graySquare.png in the xml is actually graySquare.scale-" + scale + ".png";
            tileContent.Image.Src = "ms-appx:///images/graySquare.png";
            tileContent.Image.Alt = "Gray square";

            ITileSquareImage squareTileContent = TileContentFactory.CreateTileSquareImage();
            squareTileContent.Image.Src = "ms-appx:///images/graySquare.png";
            squareTileContent.Image.Alt = "Gray square";
            tileContent.SquareContent = squareTileContent;

            TileUpdateManager.CreateTileUpdaterForApplication().Update(tileContent.CreateNotification());

            OutputTextBlock.Text = MainPage.PrettyPrint(tileContent.GetContent());            
        }

        void SendTileNotificationText_Click(object sender, RoutedEventArgs e)
        {
            ITileWideText03 tileContent = TileContentFactory.CreateTileWideText03();

            // check out /en-US/resources.resw to understand where this string will come from
            tileContent.TextHeadingWrap.Text = "ms-resource:greeting";

            ITileSquareText04 squareTileContent = TileContentFactory.CreateTileSquareText04();
            squareTileContent.TextBodyWrap.Text = "ms-resource:greeting";
            tileContent.SquareContent = squareTileContent;

            TileUpdateManager.CreateTileUpdaterForApplication().Update(tileContent.CreateNotification());

            OutputTextBlock.Text = MainPage.PrettyPrint(tileContent.GetContent());            
        }
    }
}
