/****************************** Module Header ******************************\
 * Module Name:  MainPage.xaml.cs
 * Project:      CSUniversalAppImageToVideo.WindowsPhone
 * Copyright (c) Microsoft Corporation.
 * 
 *  This is the MainPage of the app.
 *
 * This source is subject to the Microsoft Public License.
 * See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
 * All other rights reserved.
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/
using CSUniversalAppImageToVideo.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using EncodeImages;
using Windows.Graphics.Imaging;
using Windows.ApplicationModel.Activation;

namespace CSUniversalAppImageToVideo
{
    
    public sealed partial class MainPage : Page, IFileSavePickerContinuable, IFileOpenPickerContinuable
    {  
        private const int m_videoWidth = 640;
        private const int m_videoHeight = 480;
        private PictureWriter m_picture;
        private List<StorageFile> m_files = new List<StorageFile>();
        
        private readonly NavigationHelper navigationHelper;
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();

        public MainPage()
        {
            this.InitializeComponent();
           

            this.NavigationCacheMode = NavigationCacheMode.Required;

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
            this.SizeChanged += MainPage_SizeChanged;

        }

        private void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Height < e.NewSize.Width)
            {
                VisualStateManager.GoToState(this, "LandscapeLayout", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "DefaultLayout", true);
            }
        }


        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            if ((e.NavigationParameter as List<StorageFile>) != null)
            {
                m_files = e.NavigationParameter as List<StorageFile>;
                statusText.Text = "Images opened! Encode the images now.";
            }
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            // TODO: Save the unique state of the page here.
        }



        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion


        /// <summary>
        /// Handle the returned file from file picker
        /// This method is triggered by ContinuationManager based on ActivationKind
        /// </summary>
        /// <param name="args">File save picker continuation activation argment. It cantains the file user selected with file save picker </param>
        public async void ContinueFileSavePicker(FileSavePickerContinuationEventArgs args)
        {
            StorageFile videoFile = args.File;
            if (videoFile != null)
            {
                IRandomAccessStream videoStream = await videoFile.OpenAsync(FileAccessMode.ReadWrite);
                m_picture = new PictureWriter(videoStream, m_videoWidth, m_videoHeight);

                // Add frames to the video.
                ProcessVideoRing.IsActive = true;
                VideoElement.AreTransportControlsEnabled = false;
                ImageBtn.IsEnabled = false;
                EncodeBtn.IsEnabled = false;
                statusText.Text = "Encoding...";

                foreach (StorageFile file in m_files)
                {
                    Windows.Storage.FileProperties.ImageProperties properties = await file.Properties.GetImagePropertiesAsync();

                    float scaleOfWidth = (float)m_videoWidth / properties.Width;
                    float scaleOfHeight = (float)m_videoHeight / properties.Height;
                    float scale = scaleOfHeight > scaleOfWidth ?
                    scaleOfWidth : scaleOfHeight;
                    uint width = (uint)(properties.Width * scale);
                    uint height = (uint)(properties.Height * scale);

                    using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read))
                    {
                        for (int i = 0; i < 10; ++i)
                        {
                            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
                            PixelDataProvider dataProvider = await decoder.GetPixelDataAsync(
                                BitmapPixelFormat.Bgra8,
                                BitmapAlphaMode.Straight,
                                new BitmapTransform { ScaledWidth = width, ScaledHeight = height },
                                ExifOrientationMode.RespectExifOrientation,
                                ColorManagementMode.ColorManageToSRgb);
                            m_picture.AddFrame(dataProvider.DetachPixelData(), (int)width, (int)height);
                        }
                    }
                }
                m_picture.Finalize();
                m_picture = null;

                VideoElement.AreTransportControlsEnabled = true;
                ImageBtn.IsEnabled = true;
                EncodeBtn.IsEnabled = true;
                statusText.Text = "The image files are encoded successfully. You can review the video.";
                ProcessVideoRing.IsActive = false;

                videoStream.Dispose();
                videoStream = null;
                videoStream = await videoFile.OpenAsync(FileAccessMode.Read);

                VideoElement.SetSource(videoStream, videoFile.ContentType);
            }           
        }

        /// <summary>
        /// Handle the returned files from file picker
        /// This method is triggered by ContinuationManager based on ActivationKind
        /// </summary>
        /// <param name="args">File open picker continuation activation argment. It cantains the list of files user selected with file open picker </param>
        public void ContinueFileOpenPicker(FileOpenPickerContinuationEventArgs args)
        {
            IReadOnlyList<StorageFile> files = args.Files;
            if (files.Count > 0)
            {
                m_files = files.ToList();
                this.Frame.Navigate(typeof(ImagesPage), m_files);
            }

        }

        private void ImageBtn_Click(object sender, RoutedEventArgs e)
        {
            if (m_files.Count != 0)
            {
                m_files.Clear();
            }
            statusText.Text = "";
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".bmp");            
            openPicker.PickMultipleFilesAndContinue();
        }

        private void EncodeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (m_files.Count == 0)
            {
                statusText.Text = "You must select one image at least.";
                return;
            }

            // Create the video file via file picker.
            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.VideosLibrary;
            savePicker.FileTypeChoices.Add("MP4 File", new List<string>() { ".mp4" });
            savePicker.SuggestedFileName = "output";
            savePicker.PickSaveFileAndContinue();            
        }

        private async void Footer_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri((sender as HyperlinkButton).Tag.ToString()));
        }
    }
}