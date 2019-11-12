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
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI;

namespace Input
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario2 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;
        int _pointerCount;

        public Scenario2()
        {
            this.InitializeComponent();
            bEnteredExited.PointerEntered += new PointerEventHandler(bEnteredExited_PointerEntered);
            bEnteredExited.PointerExited += new PointerEventHandler(bEnteredExited_PointerExited);
            bEnteredExited.PointerPressed += new PointerEventHandler(bEnteredExited_PointerPressed);
            bEnteredExited.PointerReleased += new PointerEventHandler(bEnteredExited_PointerReleased);
            bEnteredExited.PointerMoved += new PointerEventHandler(bEnteredExited_PointerMoved);

            //To code for multiple Pointers (i.e. Fingers) we track how many entered/exited.
            _pointerCount = 0;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        void bEnteredExited_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            Scenario2UpdateVisuals(sender as Border, "Moved");
        }

        void bEnteredExited_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            ((Border)sender).ReleasePointerCapture(e.Pointer);
            txtCaptureStatus.Text = string.Empty;
        }

        //Can only get capture on PointerPressed (i.e. touch down, mouse click, pen press)
        void bEnteredExited_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (tbPointerCapture.IsOn)
            {
                bool _hasCapture = ((Border)sender).CapturePointer(e.Pointer);
                txtCaptureStatus.Text = "Got Capture: " + _hasCapture;
            }
        }

        void bEnteredExited_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            _pointerCount--;
            Scenario2UpdateVisuals(sender as Border, "Exited");
        }

        void bEnteredExited_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            _pointerCount++;
            Scenario2UpdateVisuals(sender as Border, "Entered");

        }

        void Scenario2UpdateVisuals(Border border, String eventDescription)
        {
            switch (eventDescription.ToLower())
            {
                case "exited":
                    if (_pointerCount <= 0)
                    {
                        border.Background = new SolidColorBrush(Colors.Red);
                        bEnteredExitedTextBlock.Text = eventDescription;
                    }
                    break;
                case "moved":

                    RotateTransform rt = (RotateTransform)bEnteredExitedTimer.RenderTransform;
                    rt.Angle += 2;
                    if (rt.Angle > 360) rt.Angle -= 360;
                    break;
                default:
                    border.Background = new SolidColorBrush(Colors.Green);
                    bEnteredExitedTextBlock.Text = eventDescription;
                    break;

            }
        }

        void Scenario2Reset(object sender, RoutedEventArgs e)
        {
            Scenario2Reset();
        }

        void Scenario2Reset()
        {
            bEnteredExited.Background = new SolidColorBrush(Colors.Red);
            bEnteredExitedTextBlock.Text = string.Empty;
        }
    }
}
