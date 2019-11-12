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
    public sealed partial class SchemeHandler : Page
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public SchemeHandler()
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
            rootPage.ExtensionManager.RegisterSchemeHandler("GeometricSource.GeometricSchemeHandler", "myscheme:");

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
        /// This is the click handler for the 'Circle' button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Circle_Click(object sender, RoutedEventArgs e)
        {
            Video.Source = new Uri("myscheme://circle");
        }

        /// <summary>
        /// This is the click handler for the 'Square' button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Square_Click(object sender, RoutedEventArgs e)
        {
            Video.Source = new Uri("myscheme://square");
        }

        /// <summary>
        /// This is the click handler for the 'Triangle' button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Triangle_Click(object sender, RoutedEventArgs e)
        {
            Video.Source = new Uri("myscheme://triangle");
        }
    }
}
