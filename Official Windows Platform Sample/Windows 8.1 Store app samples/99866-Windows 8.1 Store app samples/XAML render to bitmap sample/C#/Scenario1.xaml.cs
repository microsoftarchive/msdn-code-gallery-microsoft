//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************


using SDKTemplate;
using System;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Graphics.Display;

namespace RenderToBitmap
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario1 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Scenario1()
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
            CompositionTarget.SurfaceContentsLost += CompositionTarget_SurfaceContentsLost;
            DisplayInformation.GetForCurrentView().DpiChanged += OnDpiChanged;
            rootPage.MainPageResized += rootPage_MainPageResized;
            UpdateOutputLayout();
        }

        /// <summary>
        /// Invoked when this page will no longer be displayed in a Frame.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            CompositionTarget.SurfaceContentsLost -= CompositionTarget_SurfaceContentsLost;
            DisplayInformation.GetForCurrentView().DpiChanged -= OnDpiChanged;
            rootPage.MainPageResized -= rootPage_MainPageResized;
        }

        private void rootPage_MainPageResized(object sender, MainPageSizeChangedEventArgs e)
        {
            UpdateOutputLayout();
        }

        /// <summary>
        /// Event handler for the "Save as image source" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveImageSource_Click(object sender, RoutedEventArgs e)
        {
            RenderImageSource();
        }

        private async void RenderImageSource()
        {
            int width;
            int height;

            // Try to parse an integer from the given text. If invalid, use the default value.
            if (!int.TryParse(WidthTextBox.Text, out width))
            {
                WidthTextBox.Text = "0";
                width = 0;
            }

            // Try to parse an integer from the given text. If invalid, use the default value.
            if (!int.TryParse(HeightTextBox.Text, out height))
            {
                HeightTextBox.Text = "0";
                height = 0;
            }

            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap();
            await renderTargetBitmap.RenderAsync(RenderedGrid, width, height);

            RenderedImage.Source = renderTargetBitmap;
        }

        /// <summary>
        /// Event handler for the global CompositionTarget.SurfaceContentsLost event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CompositionTarget_SurfaceContentsLost(object sender, object e)
        {
            // If the SurfaceContentsLost event is raised then the video memory storing the RenderTargetBitmap contents was released
            // and the contents are no longer available.  This may occur, for example, if the system recovers after a Timeout Detection and Recovery 
            // process when the video card driver becomes unresponsive.  This means the old snapshots are no longer available, so the
            // list of snapshots is cleared.  If the contents need to be preserved indefinitely, the app should call GetPixelsAsync() and
            // retrieve a copy of the pixels contents.  Here, we simply re-render the current content, which may be different than last time.
            RenderImageSource();
        }

        /// <summary>
        /// Event handler for the DpiChanged event. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnDpiChanged(DisplayInformation sender, object args)
        {
            // If the DpiChanged event is raised then the DPI of the current view has changed. Since the image was generated
            // at the old DPI, it should be re-rendered if possible to ensure the content does not display rendering artifacts due
            // to scaling.  Note the resulting content may be different than before if the tree has changed.
            RenderImageSource();
        }

        /// <summary>
        /// Updates size and position of elements on the page when the size changes.
        /// </summary>
        private void UpdateOutputLayout()
        {
            Output.Width = (MainPage.Current.FindName("ContentRoot") as FrameworkElement).ActualWidth;
        }
    }
}
