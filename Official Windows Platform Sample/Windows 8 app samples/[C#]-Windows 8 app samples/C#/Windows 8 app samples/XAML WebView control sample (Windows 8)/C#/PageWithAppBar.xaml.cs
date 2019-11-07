using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;

namespace Controls_WebView
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PageWithAppBar : Page
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public PageWithAppBar()
        {
            this.InitializeComponent();

            // Hook the opned and closed events on the AppBar so we know when
            // to adjust the WebView.
            BottomAppBar.Opened += BottomAppBar_Opened;
            BottomAppBar.Closed += BottomAppBar_Closed;
        }

        void BottomAppBar_Opened(object sender, object e)
        {
            // AppBar has Opened so we need to put the WebView back to its
            // original size/location.
            AppBar bottomAppBar = sender as AppBar;
            if (bottomAppBar != null)
            {
                // Force layout so that we can guarantee that our AppBar's
                // actual height has height
                this.UpdateLayout();
                // Get the height of the AppBar
                double appBarHeight = bottomAppBar.ActualHeight;
                // Reduce the height of the WebView to allow for the AppBar
                WebView8.Height = WebView8.ActualHeight - appBarHeight;
                // Translate the WebView in the Y direction to reclaim the space occupied by 
                // the AppBar.  Notice that we translate it by appBarHeight / 2.0.
                // This is because the WebView has VerticalAlignment and HorizontalAlignment
                // of 'Stretch' and when we reduce its size it reduces its overall size
                // from top and bottom by half the amount.
                TranslateYOpen.To = -appBarHeight / 2.0;
                // Run our translate animation to match the AppBar
                OpenAppBar.Begin();
            }
        }

        void BottomAppBar_Closed(object sender, object e)
        {
            // AppBar has closed so we need to put the WebView back to its
            // original size/location.
            AppBar bottomAppBar = sender as AppBar;
            if (bottomAppBar != null)
            {
                // Force layout so that we can guarantee that our AppBar's
                // actual height has height
                this.UpdateLayout();
                // Get the height of the AppBar
                double appBarHeight = bottomAppBar.ActualHeight;
                // Increase the height of the WebView to allow for the space
                // that was occupied by the AppBar
                WebView8.Height = WebView8.ActualHeight + appBarHeight;
                // Translate the WebView in the Y direction to allow for 
                // the AppBar.  Notice that we translate it by appBarHeight / 2.0.
                // This is because the WebView has VerticalAlignment and HorizontalAlignment
                // of 'Stretch' and when we reduce its size it reduces its overall size
                // from top and bottom by half the amount.
                TranslateYOpen.To = appBarHeight / 2.0;
                // Run our translate animation to match the AppBar
                CloseAppBar.Begin();
            }
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Navigate to some web site
            WebView8.Navigate(new Uri("http://www.microsoft.com"));
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // Unhook the events
            BottomAppBar.Opened -= BottomAppBar_Opened;
            BottomAppBar.Closed -= BottomAppBar_Closed;
        }

        private void Home_Click(object sender, RoutedEventArgs e)
        {
            if (rootPage.Frame.CanGoBack)
            {
                rootPage.Frame.GoBack();
            }
        }
    }
}
