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
using Windows.UI.Xaml.Media;
using SDKTemplate;
using System;
using Windows.UI.Xaml.Media.Imaging;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Storage.Streams;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Collections.ObjectModel;

namespace Controls_WebView
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario8 : Page
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;
        ObservableCollection<BookmarkItem> bookmarks = new ObservableCollection<BookmarkItem>();

        public Scenario8()
        {
            this.InitializeComponent();
            this.bookmarkList.ItemsSource = bookmarks;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            webView8.Navigate(new Uri("http://www.microsoft.com"));
        }


        /// <summary>
        /// This is the click handler for the 'Solution' button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void bookmarkBtn_Click(object sender, RoutedEventArgs e)
        {
            InMemoryRandomAccessStream ms = new InMemoryRandomAccessStream();
            await webView8.CapturePreviewToStreamAsync(ms);
            
            //Create a small thumbnail
            int longlength = 180, width = 0, height = 0;
            double srcwidth = webView8.ActualWidth, srcheight = webView8.ActualHeight;
            double factor = srcwidth / srcheight;
            if (factor < 1)
            {
                height = longlength;
                width = (int)(longlength * factor);
            }
            else
            {
                width = longlength;
                height = (int)(longlength / factor);
            }
            BitmapSource small = await resize(width, height, ms);
            
            BookmarkItem item = new BookmarkItem();
            item.Title = webView8.DocumentTitle;
            item.PageUrl = webView8.Source;
            item.Preview = small;

            bookmarks.Add(item);
        }

  
        async Task<BitmapSource> resize(int width, int height, Windows.Storage.Streams.IRandomAccessStream source)
        {
            WriteableBitmap small = new WriteableBitmap(width, height);
            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(source);
            BitmapTransform transform = new BitmapTransform();
            transform.ScaledHeight = (uint)height;
            transform.ScaledWidth = (uint)width;
            PixelDataProvider pixelData = await decoder.GetPixelDataAsync(
                BitmapPixelFormat.Bgra8,
                BitmapAlphaMode.Straight,
                transform,
                ExifOrientationMode.RespectExifOrientation,
                ColorManagementMode.DoNotColorManage);
            pixelData.DetachPixelData().CopyTo(small.PixelBuffer);
            return small;
        }

        private void bookmarkList_ItemClick(object sender, ItemClickEventArgs e)
        {
            BookmarkItem b = (BookmarkItem)e.ClickedItem;
            webView8.Navigate(b.PageUrl);
        }

    }


     class BookmarkItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property. 
        // The CallerMemberName attribute that is applied to the optional propertyName 
        // parameter causes the property name of the caller to be substituted as an argument. 
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        private Uri _pageUrl;
        public Uri PageUrl
        {
            get { return this._pageUrl; }
            set
            {
                _pageUrl = value;
                NotifyPropertyChanged();
            }
        }

        private BitmapSource _preview;
        public BitmapSource Preview
        {
            get { return this._preview; }
            set
            {
                _preview = value;
                NotifyPropertyChanged();
            }
        }

        private String _title;
        public String Title
        {
            get { return this._title; }
            set
            {
                _title = value;
                NotifyPropertyChanged();
            }
        }
    }
}

