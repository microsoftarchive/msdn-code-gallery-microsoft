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
using SDKTemplate;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Tiles
{
#if WINDOWS_PHONE_APP
    public sealed partial class ImageProtocols : Page, IFileOpenPickerContinuable
#else
    public sealed partial class ImageProtocols : Page
#endif
    {
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

#if WINDOWS_PHONE_APP
        public async void ContinueFileOpenPicker(FileOpenPickerContinuationEventArgs args)
        {
            await CopyImageToLocalFolderAsync(args.Files[0]);
        }
#endif

#if WINDOWS_PHONE_APP
        private void PickImage_Click(object sender, RoutedEventArgs e)
#else
        private async void PickImage_Click(object sender, RoutedEventArgs e)
#endif
        {
            FileOpenPicker picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".gif");
            picker.CommitButtonText = "Copy";
#if WINDOWS_PHONE_APP
            picker.PickSingleFileAndContinue();
#else
            StorageFile file = await picker.PickSingleFileAsync();
            await CopyImageToLocalFolderAsync(file);
#endif
        }

        private async Task CopyImageToLocalFolderAsync(StorageFile file)
        {
            OutputTextBlock.Text = string.Empty;
            if (file != null)
            {
                StorageFile newFile = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync(file.Name, Windows.Storage.CreationCollisionOption.GenerateUniqueName);
                await file.CopyAndReplaceAsync(newFile);
                this.imageRelativePath = newFile.Path.Substring(newFile.Path.LastIndexOf("\\") + 1);
                rootPage.NotifyUser("Image copied to application data local storage: " + newFile.Path, NotifyType.StatusMessage);
            }
            else
            {
                rootPage.NotifyUser("File was not copied due to error or cancelled by user.", NotifyType.ErrorMessage);
            }
        }

        private void ProtocolList_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

        private void SendTileNotification_Click(object sender, RoutedEventArgs e)
        {
            IWide310x150TileNotificationContent wide310x150TileContent = null;
            if (ProtocolList.SelectedItem == package) //using the ms-appx:/// protocol
            {
                ITileWide310x150ImageAndText01 wide310x150ImageAndTextContent = TileContentFactory.CreateTileWide310x150ImageAndText01();

                wide310x150ImageAndTextContent.RequireSquare150x150Content = false;
                wide310x150ImageAndTextContent.TextCaptionWrap.Text = "The image is in the appx package";
                wide310x150ImageAndTextContent.Image.Src = "ms-appx:///images/redWide310x150.png";
                wide310x150ImageAndTextContent.Image.Alt = "Red image";

                wide310x150TileContent = wide310x150ImageAndTextContent;
            }
            else if (ProtocolList.SelectedItem == appdata) //using the appdata:///local/ protocol
            {
                ITileWide310x150Image wide310x150ImageContent = TileContentFactory.CreateTileWide310x150Image();

                wide310x150ImageContent.RequireSquare150x150Content = false;
                wide310x150ImageContent.Image.Src = "ms-appdata:///local/" + this.imageRelativePath;
                wide310x150ImageContent.Image.Alt = "App data";

                wide310x150TileContent = wide310x150ImageContent;
            }
            else if (ProtocolList.SelectedItem == http) //using http:// protocol
            {
                // Important - The Internet (Client) capability must be checked in the manifest in the Capabilities tab
                ITileWide310x150PeekImageCollection04 wide310x150PeekImageCollectionContent = TileContentFactory.CreateTileWide310x150PeekImageCollection04();

                wide310x150PeekImageCollectionContent.RequireSquare150x150Content = false;
                try
                {
                    wide310x150PeekImageCollectionContent.BaseUri = HTTPBaseURI.Text;
                }
                catch (ArgumentException exception)
                {
                    OutputTextBlock.Text = exception.Message;
                    return;
                }
                wide310x150PeekImageCollectionContent.TextBodyWrap.Text = "The base URI is " + HTTPBaseURI.Text;
                wide310x150PeekImageCollectionContent.ImageMain.Src = HTTPImage1.Text;
                wide310x150PeekImageCollectionContent.ImageSmallColumn1Row1.Src = HTTPImage2.Text;
                wide310x150PeekImageCollectionContent.ImageSmallColumn1Row2.Src = HTTPImage3.Text;
                wide310x150PeekImageCollectionContent.ImageSmallColumn2Row1.Src = HTTPImage4.Text;
                wide310x150PeekImageCollectionContent.ImageSmallColumn2Row2.Src = HTTPImage5.Text;

                wide310x150TileContent = wide310x150PeekImageCollectionContent;
            }

            wide310x150TileContent.RequireSquare150x150Content = false;
            TileUpdateManager.CreateTileUpdaterForApplication().Update(wide310x150TileContent.CreateNotification());

            OutputTextBlock.Text = MainPage.PrettyPrint(wide310x150TileContent.GetContent());
            rootPage.NotifyUser("Tile notification sent", NotifyType.StatusMessage);
        }
    }
}