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

namespace Controls_WebView
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario6 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Scenario6()
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
            // We need to hook the DropDownOpened event of the ComboBox so that we are notified when it is opened
            // so that we can swap out the WebView control for a Rectangle that uses a fill brush created by
            // the WebViewBrush to give the illusion that the WebView is still there.  This solves airspace
            // problems that occur since the WebView is hosted in its own HWND and content cannot be rendered
            // on top of it.
            ComboBox1.DropDownOpened += ComboBox1_DropDownOpened;

            // Now we need to hook the DropDownClosed event so that we can undo what we did on DropDownOpened
            // and swap the WebView back.
            ComboBox1.DropDownClosed += ComboBox1_DropDownClosed;

            // Select on of the items in the ComboBox
            ComboBox1.SelectedIndex = 0;

            // Ensure that our Rectangle used to simulate the WebView is not shown initially
            Rect1.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

            WebView6.Navigate(new Uri("http://www.bing.com"));
        }


        /// <summary>
        /// This is the click handler for the 'Solution' button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Solution_Click(object sender, RoutedEventArgs e)
        {
            Rect1.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }


        /// <summary>
        /// When the ComboBox opens we have an airspace conflict and the ComboBox cannot render its content over
        /// the WebView.  Therefore, we create a WebViewBrush and set the WebView as its source and call the Redraw() method
        /// which will take a visual snapshot of the WebView.  We then use that brush to fill our Rectangle.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ComboBox1_DropDownOpened(object sender, object e)
        {
            if (Rect1.Visibility == Windows.UI.Xaml.Visibility.Visible)
            {
                WebViewBrush b = new WebViewBrush();
                b.SourceName = "WebView6";
                b.Redraw();
                Rect1.Fill = b;
                WebView6.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }

        /// <summary>
        /// When the ComboBox is closed we no longer need the simulated WebView so we put things back the way they were
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ComboBox1_DropDownClosed(object sender, object e)
        {
            WebView6.Visibility = Windows.UI.Xaml.Visibility.Visible;
            Rect1.Fill = new SolidColorBrush(Windows.UI.Colors.Transparent);
        }

    }
}
