//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using NotificationsExtensions.TileContent;
using Windows.UI.Xaml.Controls;using SDKTemplate;
using Windows.ApplicationModel.Resources.Core;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace Tiles
{
    public sealed partial class Globalization : Page
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
            ResourceContext defaultContextForCurrentView = ResourceContext.GetForCurrentView();

            string asls;
            defaultContextForCurrentView.QualifierValues.TryGetValue("Language", out asls);

            string scale;
            defaultContextForCurrentView.QualifierValues.TryGetValue("Scale", out scale);

            string contrast;
            defaultContextForCurrentView.QualifierValues.TryGetValue("Contrast", out contrast);

            rootPage.NotifyUser("Your system is currently set to the following values: Application Language: " + asls + ", Scale: " + scale + ", Contrast: " + contrast + ". If using web images and addImageQuery, the following query string would be appened to the URL: ?ms-lang=" + asls + "&ms-scale=" + scale + "&ms-contrast=" + contrast, NotifyType.StatusMessage);
        }

        void SendTileNotificationWithQueryStrings_Click(object sender, RoutedEventArgs e)
        {
            ITileSquare310x310Image square310x310TileContent = TileContentFactory.CreateTileSquare310x310Image();
            square310x310TileContent.Image.Src = ImageUrl.Text;
            square310x310TileContent.Image.Alt = "Web image";

            // enable AddImageQuery on the notification
            square310x310TileContent.AddImageQuery = true;

            ITileWide310x150ImageAndText01 wide310x150TileContent = TileContentFactory.CreateTileWide310x150ImageAndText01();
            wide310x150TileContent.TextCaptionWrap.Text = "This tile notification uses query strings for the image src.";
            wide310x150TileContent.Image.Src = ImageUrl.Text;
            wide310x150TileContent.Image.Alt = "Web image";

            ITileSquare150x150Image square150x150TileContent = TileContentFactory.CreateTileSquare150x150Image();
            square150x150TileContent.Image.Src = ImageUrl.Text;
            square150x150TileContent.Image.Alt = "Web image";

            ITileSquare71x71Image square71x71TileContent = TileContentFactory.CreateTileSquare71x71Image();
            square71x71TileContent.Image.Src = ImageUrl.Text;
            square71x71TileContent.Image.Alt = "Web image";

            square150x150TileContent.Square71x71Content = square71x71TileContent;
            wide310x150TileContent.Square150x150Content = square150x150TileContent;
            square310x310TileContent.Wide310x150Content = wide310x150TileContent;

            TileUpdateManager.CreateTileUpdaterForApplication().Update(square310x310TileContent.CreateNotification());

            OutputTextBlock.Text = MainPage.PrettyPrint(square310x310TileContent.GetContent());
            rootPage.NotifyUser("Tile notification with image query strings sent.", NotifyType.StatusMessage);
        }

        void SendScaledImageTileNotification_Click(object sender, RoutedEventArgs e)
        {
            string scale;
            ResourceContext.GetForCurrentView().QualifierValues.TryGetValue("Scale", out scale);

            ITileSquare310x310Image square310x310TileContent = TileContentFactory.CreateTileSquare310x310Image();
            square310x310TileContent.Image.Src = "ms-appx:///images/purpleSquare310x310.png";
            square310x310TileContent.Image.Alt = "Purple square";

            ITileWide310x150ImageAndText01 wide310x150TileContent = TileContentFactory.CreateTileWide310x150ImageAndText01();
            wide310x150TileContent.TextCaptionWrap.Text = "scaled version of blueWide310x150.png in the xml is selected based on the current Start scale";
            wide310x150TileContent.Image.Src = "ms-appx:///images/blueWide310x150.png";
            wide310x150TileContent.Image.Alt = "Blue wide";

            ITileSquare150x150Image square150x150TileContent = TileContentFactory.CreateTileSquare150x150Image();
            square150x150TileContent.Image.Src = "ms-appx:///images/graySquare150x150.png";
            square150x150TileContent.Image.Alt = "Gray square";

            ITileSquare71x71Image square71x71TileContent = TileContentFactory.CreateTileSquare71x71Image();
            square71x71TileContent.Image.Src = "ms-appx:///images/graySquare150x150.png";
            square71x71TileContent.Image.Alt = "Gray square";

            square150x150TileContent.Square71x71Content = square71x71TileContent;
            wide310x150TileContent.Square150x150Content = square150x150TileContent;
            square310x310TileContent.Wide310x150Content = wide310x150TileContent;

            TileUpdateManager.CreateTileUpdaterForApplication().Update(square310x310TileContent.CreateNotification());

            OutputTextBlock.Text = MainPage.PrettyPrint(square310x310TileContent.GetContent());
            rootPage.NotifyUser("Tile notification with scaled images sent.", NotifyType.StatusMessage);
        }

        void SendTextResourceTileNotification_Click(object sender, RoutedEventArgs e)
        {
            ITileSquare310x310Text09 square310x310TileContent = TileContentFactory.CreateTileSquare310x310Text09();
            // Check out /en-US/resources.resw to understand where this string will come from.
            square310x310TileContent.TextHeadingWrap.Text = "ms-resource:greeting";

            ITileWide310x150Text03 wide310x150TileContent = TileContentFactory.CreateTileWide310x150Text03();
            wide310x150TileContent.TextHeadingWrap.Text = "ms-resource:greeting";

            ITileSquare150x150Text04 square150x150TileContent = TileContentFactory.CreateTileSquare150x150Text04();
            square150x150TileContent.TextBodyWrap.Text = "ms-resource:greeting";

            wide310x150TileContent.Square150x150Content = square150x150TileContent;
            square310x310TileContent.Wide310x150Content = wide310x150TileContent;

            TileUpdateManager.CreateTileUpdaterForApplication().Update(square310x310TileContent.CreateNotification());

            OutputTextBlock.Text = MainPage.PrettyPrint(square310x310TileContent.GetContent());
            rootPage.NotifyUser("Tile notification with localized text resources sent.", NotifyType.StatusMessage);
        }
    }
}