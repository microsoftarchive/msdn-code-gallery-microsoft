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
using Windows.Foundation;
using Windows.UI.Xaml.Input;
using Windows.System;

namespace AdvancedManipulations
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario1 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        //private variables
        private bool isDoubleTapped = false;
        private bool isZoomIn = false;
        private Point p;
        private float incZoom = 1.0F;

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
        }


        private void image_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            //Get the precise pointer position at the time of doubletap in order to center the view at that location
            p = e.GetPosition(sender as Image);
            //set the internal flag to track the doubletapped gesture
            isDoubleTapped = true;

        }
        private async void image_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            //Logic to call zoom in or out functions based on user action
            if (isDoubleTapped)
            {
                isDoubleTapped = false;
                VirtualKeyModifiers vkm = e.KeyModifiers;
                if ((vkm & VirtualKeyModifiers.Shift) == VirtualKeyModifiers.Shift)
                {
                    //Shift+double tap user action should cause zoom out of the image
                    isZoomIn = false;
                }
                else
                {
                    //Simple double tap user action should cause zoom in of the image
                    isZoomIn = true;
                }

                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, ZoomInOrOut);
            }
        }

        /// <summary>
        /// Called to perform programmatic pan and zoom
        /// </summary>
        private void ZoomInOrOut()
        {

            double targetHO = 0;
            double targetVO = 0;
            float targetZF = 1;

            if (isZoomIn && scrollViewer.ZoomFactor <= scrollViewer.MaxZoomFactor-1)
            {
                //Simple double tap user action has occured. This should cause zooming in to the location of the double tap                
                float newZoom = scrollViewer.ZoomFactor + incZoom;
                targetHO = (p.X * newZoom) - scrollViewer.Width / (2);
                targetVO = (p.Y * newZoom) - scrollViewer.Height / (2);
                targetZF = newZoom;
            }
            else if (!isZoomIn && scrollViewer.ZoomFactor > scrollViewer.MinZoomFactor)
            {
                //Shift+double tap user action has occured. This should cause zooming out from the location of the double tap
                float newZoom = scrollViewer.ZoomFactor - incZoom;
                targetHO = (p.X * newZoom) - scrollViewer.Width / (2);
                targetVO = (p.Y * newZoom) - scrollViewer.Height / (2);
                targetZF = newZoom;
            }
            else
            {
		//If the content has already been zoomed in to the maximum allowed zoom factor OR zoomed out to the minimum allowed zoom factor, do nothing
                return;
            }

            isZoomIn = false;
            //The new scrollViewer.ChangeView method called as shown below results in the location of the double tap zoomed in/out and brought into the center of the viewport
            bool success = scrollViewer.ChangeView(targetHO, targetVO, targetZF, false);
        }

    }
}
