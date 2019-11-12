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
using Windows.Storage.Pickers;
using Windows.Storage;
using System.Threading.Tasks;

namespace Tiles
{
    public sealed partial class ImageProtocols : SDKTemplate.Common.LayoutAwarePage
    {
        #region TemplateCode
        MainPage rootPage = MainPage.Current;
        string imageRelativePath = String.Empty; //used for copying an image to localstorage

        public ImageProtocols()
        {
            this.InitializeComponent();
            ProtocolList.SelectedIndex = 0;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        async void PickImage_Click(object sender, RoutedEventArgs e)
        {
            await CopyImageToLocalFolderAsync();
        }

        async Task CopyImageToLocalFolderAsync()
        {
            if (rootPage.EnsureUnsnapped())
            {
                FileOpenPicker picker = new Windows.Storage.Pickers.FileOpenPicker();
                picker.ViewMode = PickerViewMode.Thumbnail;
                picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                picker.FileTypeFilter.Add(".jpg");
                picker.FileTypeFilter.Add(".jpeg");
                picker.FileTypeFilter.Add(".png");
                picker.FileTypeFilter.Add(".gif");
                picker.CommitButtonText = "Copy";
                StorageFile file = await picker.PickSingleFileAsync();
                if (file != null)
                {
                    StorageFile newFile = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync(file.Name, Windows.Storage.CreationCollisionOption.GenerateUniqueName);
                    await file.CopyAndReplaceAsync(newFile);
                    this.imageRelativePath = newFile.Path.Substring(newFile.Path.LastIndexOf("\\") + 1);
                    OutputTextBlock.Text = "Image copied to application data local storage: " + newFile.Path;
                }
                else
                {
                    OutputTextBlock.Text = "File was not copied due to error or cancelled by user.";
                }
            }
            else
            {
                OutputTextBlock.Text = "Cannot unsnap the sample application.";
            }
        }

        void ProtocolList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LocalFolder.Visibility = Visibility.Collapsed;
            HTTP.Visibility = Visibility.Collapsed;

            if (ProtocolList.SelectedItem == appdata)
            {
                LocalFolder.Visibility = Visibility.Visible;
            }
            else if (ProtocolList.SelectedItem == http)
            {
                HTTP.Visibility = Visibility.Visible;
            }
        }
        #endregion TemplateCode

        void SendTileNotification_Click(object sender, RoutedEventArgs e)
        {
            IWideTileNotificationContent tileContent = null;
            if (ProtocolList.SelectedItem == package) //using the ms-appx:/// protocol
            {
                ITileWideImageAndText01 wideContent = TileContentFactory.CreateTileWideImageAndText01();

                wideContent.RequireSquareContent = false;
                wideContent.TextCaptionWrap.Text = "The image is in the appx package";
                wideContent.Image.Src = "ms-appx:///images/redWide.png";
                wideContent.Image.Alt = "Red image";

                tileContent = wideContent;
            }
            else if (ProtocolList.SelectedItem == appdata) //using the appdata:///local/ protocol
            {
                ITileWideImage wideContent = TileContentFactory.CreateTileWideImage();

                wideContent.RequireSquareContent = false;
                wideContent.Image.Src = "ms-appdata:///local/" + this.imageRelativePath;
                wideContent.Image.Alt = "App data";

                tileContent = wideContent;
            }
            else if (ProtocolList.SelectedItem == http) //using http:// protocol
            {
                // Important - The Internet (Client) capability must be checked in the manifest in the Capabilities tab
                ITileWidePeekImageCollection04 wideContent = TileContentFactory.CreateTileWidePeekImageCollection04();

                wideContent.RequireSquareContent = false;
                try
                {
                    wideContent.BaseUri = HTTPBaseURI.Text;
                }
                catch (ArgumentException exception)
                {
                    OutputTextBlock.Text = exception.Message;
                    return;
                }
                wideContent.TextBodyWrap.Text = "The base URI is " + HTTPBaseURI.Text;
                wideContent.ImageMain.Src = HTTPImage1.Text;
                wideContent.ImageSmallColumn1Row1.Src = HTTPImage2.Text;
                wideContent.ImageSmallColumn1Row2.Src = HTTPImage3.Text;
                wideContent.ImageSmallColumn2Row1.Src = HTTPImage4.Text;
                wideContent.ImageSmallColumn2Row2.Src = HTTPImage5.Text;

                tileContent = wideContent;
            }

            tileContent.RequireSquareContent = false;
            TileUpdateManager.CreateTileUpdaterForApplication().Update(tileContent.CreateNotification());

            OutputTextBlock.Text = MainPage.PrettyPrint(tileContent.GetContent());
        }
    }
}