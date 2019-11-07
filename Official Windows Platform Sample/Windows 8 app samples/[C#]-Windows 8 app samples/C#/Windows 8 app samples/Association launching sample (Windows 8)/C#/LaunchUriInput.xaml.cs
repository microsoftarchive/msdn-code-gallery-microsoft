// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

using System;
using System.Linq;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using AssociationLaunching;

namespace AssociationLaunching
{
    public sealed partial class LaunchUriInput : Page
    {
        // A pointer back to the main page which is used to gain access to the input and output frames and their content.
        rootPage rootPage = null;
        string uriToLaunch = @"http://www.bing.com";

        public LaunchUriInput()
        {
            InitializeComponent();
        }

        #region Template-Related Code - Do not remove
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Get a pointer to our main page
            rootPage = e.Parameter as rootPage;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
        }
        #endregion

        // Launch a URI.
        private async void LaunchUriButton_Click(object sender, RoutedEventArgs e)
        {
            // Create the URI to launch from a string.
            var uri = new Uri(uriToLaunch);

            // Launch the URI.
            bool success = await Windows.System.Launcher.LaunchUriAsync(uri);
            if (success)
            {
                rootPage.NotifyUser("URI launched: " + uri.AbsoluteUri, NotifyType.StatusMessage);
            }
            else
            {
                rootPage.NotifyUser("URI launch failed.", NotifyType.ErrorMessage);
            }
        }

        // Launch a URI. Show a warning prompt.
        private async void LaunchUriWithWarningButton_Click(object sender, RoutedEventArgs e)
        {
            // Create the URI to launch from a string.
            var uri = new Uri(uriToLaunch);

            // Configure the warning prompt.
            var options = new Windows.System.LauncherOptions();
            options.TreatAsUntrusted = true;

            // Launch the URI.
            bool success = await Windows.System.Launcher.LaunchUriAsync(uri, options);
            if (success)
            {
                rootPage.NotifyUser("URI launched: " + uri.AbsoluteUri, NotifyType.StatusMessage);
            }
            else
            {
                rootPage.NotifyUser("URI launch failed.", NotifyType.ErrorMessage);
            }

        }
        // Launch a URI. Show an Open With dialog that lets the user chose the handler to use.
        private async void LaunchUriOpenWithButton_Click(object sender, RoutedEventArgs e)
        {
            // Create the URI to launch from a string.
            var uri = new Uri(uriToLaunch);

            // Calulcate the position for the Open With dialog.
            // An alternative to using the point is to set the rect of the UI element that triggered the launch.
            Point openWithPosition = GetOpenWithPosition(LaunchUriOpenWithButton);

            // Next, configure the Open With dialog.
            var options = new Windows.System.LauncherOptions();
            options.DisplayApplicationPicker = true;
            options.UI.InvocationPoint = openWithPosition;
            options.UI.PreferredPlacement = Windows.UI.Popups.Placement.Below;

            // Launch the URI.
            bool success = await Windows.System.Launcher.LaunchUriAsync(uri, options);
            if (success)
            {
                rootPage.NotifyUser("URI launched: " + uri.AbsoluteUri, NotifyType.StatusMessage);
            }
            else
            {
                rootPage.NotifyUser("URI launch failed.", NotifyType.ErrorMessage);
            }
        }

        // The Open With dialog should be displayed just under the element that triggered it.
        private Windows.Foundation.Point GetOpenWithPosition(FrameworkElement element)
        {
            Windows.UI.Xaml.Media.GeneralTransform buttonTransform = element.TransformToVisual(null);

            Point desiredLocation = buttonTransform.TransformPoint(new Point());
            desiredLocation.Y = desiredLocation.Y + element.ActualHeight;

            return desiredLocation;
        }
    }
}