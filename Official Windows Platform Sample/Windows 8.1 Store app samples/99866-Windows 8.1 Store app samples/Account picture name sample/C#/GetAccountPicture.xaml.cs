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
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Windows.System.UserProfile;

namespace AccountPictureName
{
    public sealed partial class GetAccountPicture : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public GetAccountPicture()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private async void GetSmallImageButton_Click(object sender, RoutedEventArgs e)
        {
            // The small picture returned by GetAccountPicture() is 96x96 pixels in size.
            StorageFile image = UserInformation.GetAccountPicture(AccountPictureKind.SmallImage) as StorageFile;
            if (image != null)
            {
                rootPage.NotifyUser("SmallImage path = " + image.Path, NotifyType.StatusMessage);

                try
                {
                    IRandomAccessStream imageStream = await image.OpenReadAsync();
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.SetSource(imageStream);
                    smallImage.Source = bitmapImage;

                    smallImage.Visibility = Visibility.Visible;
                    largeImage.Visibility = Visibility.Collapsed;
                    mediaPlayer.Visibility = Visibility.Collapsed;
                }
                catch (Exception ex)
                {
                    rootPage.NotifyUser("Error opening stream: " + ex.ToString(), NotifyType.ErrorMessage);
                }
            }
            else
            {
                rootPage.NotifyUser("Small Account Picture is not available", NotifyType.StatusMessage);
                mediaPlayer.Visibility = Visibility.Collapsed;
                smallImage.Visibility = Visibility.Collapsed;
                largeImage.Visibility = Visibility.Collapsed;
            }
        }

        private async void GetLargeImageButton_Click(object sender, RoutedEventArgs e)
        {
            // The large picture returned by GetAccountPicture() is 448x448 pixels in size.
            StorageFile image = UserInformation.GetAccountPicture(AccountPictureKind.LargeImage) as StorageFile;
            if (image != null)
            {
                rootPage.NotifyUser("LargeImage path = " + image.Path, NotifyType.StatusMessage);

                try
                {
                    IRandomAccessStream imageStream = await image.OpenReadAsync();
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.SetSource(imageStream);
                    largeImage.Source = bitmapImage;
                    largeImage.Visibility = Visibility.Visible;
                    smallImage.Visibility = Visibility.Collapsed;
                    mediaPlayer.Visibility = Visibility.Collapsed;
                }
                catch (Exception ex)
                {
                    rootPage.NotifyUser("Error opening stream: " + ex.ToString(), NotifyType.ErrorMessage);
                }
            }
            else
            {
                rootPage.NotifyUser("Large Account Picture is not available", NotifyType.StatusMessage);
                mediaPlayer.Visibility = Visibility.Collapsed;
                smallImage.Visibility = Visibility.Collapsed;
                largeImage.Visibility = Visibility.Collapsed;
            }
        }

        private async void GetVideoButton_Click(object sender, RoutedEventArgs e)
        {
            // The video returned from getAccountPicture is 448x448 pixels in size.
            StorageFile video = UserInformation.GetAccountPicture(AccountPictureKind.Video) as StorageFile;
            if (video != null)
            {
                rootPage.NotifyUser("Video path = " + video.Path, NotifyType.StatusMessage);

                try
                {
                    IRandomAccessStream videoStream = await video.OpenAsync(FileAccessMode.Read);

                    mediaPlayer.SetSource(videoStream, "video/mp4");
                    mediaPlayer.Visibility = Visibility.Visible;
                    smallImage.Visibility = Visibility.Collapsed;
                    largeImage.Visibility = Visibility.Collapsed;
                }
                catch (Exception ex)
                {
                    rootPage.NotifyUser("Error opening stream: " + ex.ToString(), NotifyType.ErrorMessage);
                }
            }
            else
            {
                rootPage.NotifyUser("Video is not available", NotifyType.StatusMessage);
                mediaPlayer.Visibility = Visibility.Collapsed;
                smallImage.Visibility = Visibility.Collapsed;
                largeImage.Visibility = Visibility.Collapsed;
            }
        }
    }
}
