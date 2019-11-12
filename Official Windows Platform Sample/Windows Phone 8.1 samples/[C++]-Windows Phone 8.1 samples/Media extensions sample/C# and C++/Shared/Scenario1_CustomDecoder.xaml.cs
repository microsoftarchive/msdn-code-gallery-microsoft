//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;

namespace MediaExtensions
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CustomDecoder : Page
    {
        private readonly Guid MFVideoFormat_MPG1 = Guid.Parse("3147504d-0000-0010-8000-00aa00389b71");

        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public CustomDecoder()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            rootPage.ExtensionManager.RegisterByteStreamHandler("MPEG1Source.MPEG1ByteStreamHandler", ".mpg", "video/mpeg");
            rootPage.ExtensionManager.RegisterVideoDecoder("MPEG1Decoder.MPEG1Decoder", MFVideoFormat_MPG1, Guid.Empty);

            Video.MediaFailed += new ExceptionRoutedEventHandler(rootPage.VideoOnError);
        }

        /// <summary>
        /// Called when a page is no longer the active page in a frame. 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Video.MediaFailed -= new ExceptionRoutedEventHandler(rootPage.VideoOnError);
        }

        /// <summary>
        /// This is the click handler for the 'Open' button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            rootPage.PickSingleFileAndSet(new string[] { ".mpg" }, Video);
        }

        /// <summary>
        /// This is the click handler for the 'Stop' button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            Video.Source = null;
        }
    }
}
