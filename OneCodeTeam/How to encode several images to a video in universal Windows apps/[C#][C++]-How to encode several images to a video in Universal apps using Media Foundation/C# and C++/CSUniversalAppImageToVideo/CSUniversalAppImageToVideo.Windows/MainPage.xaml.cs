/****************************** Module Header ******************************\
 * Module Name:  MainPage.xaml.cs
 * Project:      CSUniversalAppImageToVideo.Windows
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

using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using CSUniversalAppImageToVideo.Common;
using EncodeImages;
using System.Collections.ObjectModel;
using Windows.Graphics.Imaging;

namespace CSUniversalAppImageToVideo
{
	
	public sealed partial class MainPage : Page
	{
		private const int m_videoWidth = 640;
		private const int m_videoHeight = 480;
		private PictureWriter m_picture;
		private ObservableCollection<Image> m_images = new ObservableCollection<Image>();
		private List<StorageFile> m_files = new List<StorageFile>();
		private NavigationHelper navigationHelper;
		private ObservableDictionary defaultViewModel = new ObservableDictionary();

		/// <summary>
		/// Gets the NavigationHelper used to aid in navigation and process lifetime management.
		/// </summary>
		public NavigationHelper NavigationHelper
		{
			get { return this.navigationHelper; }
		}

		/// <summary>
		/// Gets the DefaultViewModel. This can be changed to a strongly typed view model.
		/// </summary>
		public ObservableDictionary DefaultViewModel
		{
			get { return this.defaultViewModel; }
		}

		public MainPage()
		{
			this.InitializeComponent();
			this.navigationHelper = new NavigationHelper(this);
			this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
			
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
		}


		#region NavigationHelper registration

		/// <summary>
		/// The methods provided in this section are simply used to allow
		/// NavigationHelper to respond to the page's navigation methods.
		/// Page specific logic should be placed in event handlers for the  
		/// <see cref="Common.NavigationHelper.LoadState"/>
		/// and <see cref="Common.NavigationHelper.SaveState"/>.
		/// The navigation parameter is available in the LoadState method 
		/// in addition to page state preserved during an earlier session.
		/// </summary>
		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			this.navigationHelper.OnNavigatedTo(e);
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			this.navigationHelper.OnNavigatedFrom(e);
		}

		#endregion

		private async void ImageBtn_Click(object sender, RoutedEventArgs e)
		{
            if (m_images.Count != 0)
            {
                m_images.Clear();
            }
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
			IReadOnlyList<StorageFile> files = await openPicker.PickMultipleFilesAsync();
			if (files.Count > 0)
			{                
				foreach(StorageFile file in files)
				{
					m_files.Add(file);
					using( IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read))
					{
						BitmapImage bitmapImage = new BitmapImage();
						await bitmapImage.SetSourceAsync(stream);
						Image image = new Image();
						image.Source = bitmapImage;
						m_images.Add(image);                        
					}
					
				}
				ImageGV.DataContext = m_images;
			}
		}

		private async void EncodeBtn_Click(object sender, RoutedEventArgs e)
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
            StorageFile videoFile = await savePicker.PickSaveFileAsync();
            if(videoFile != null)
            {
                IRandomAccessStream videoStream = await videoFile.OpenAsync(FileAccessMode.ReadWrite);
                m_picture = new PictureWriter(videoStream, m_videoWidth, m_videoHeight);

                // Add frames to the video.		
                ProcessVideoRing.IsActive = true;
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

                statusText.Text = "The image files are encoded successfully. You can review the video.";
                ProcessVideoRing.IsActive = false;

                videoStream.Dispose();
                videoStream = null;
                videoStream = await videoFile.OpenAsync(FileAccessMode.Read);

                VideoElement.SetSource(videoStream, videoFile.ContentType);
            }            
		}
		

		private void ImageGV_ItemClick(object sender, ItemClickEventArgs e)
		{
            m_files.RemoveAt(m_images.IndexOf((Image)e.ClickedItem));
            m_images.Remove((Image)e.ClickedItem);
		}

		private async void Footer_Click(object sender, RoutedEventArgs e)
		{
            await Windows.System.Launcher.LaunchUriAsync(new Uri((sender as HyperlinkButton).Tag.ToString()));
		}

		private void pageRoot_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (e.NewSize.Width < 800)
			{
				VisualStateManager.GoToState(this, "MinimalLayout", true);
			}
			else
			{
				VisualStateManager.GoToState(this, "DefaultLayout", true);
			}
		}        
	}
}
