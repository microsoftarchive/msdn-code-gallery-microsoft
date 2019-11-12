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
    public sealed partial class LaunchFileInput : Page
    {
        // A pointer back to the main page which is used to gain access to the input and output frames and their content.
        rootPage rootPage = null;
        string fileToLaunch = @"images\Icon.Targetsize-256.png";

        public LaunchFileInput()
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

        // Launch a .png file that came with the package.
        private async void LaunchFileButton_Click(object sender, RoutedEventArgs e)
        {
            // First, get the image file from the package's image directory.
            var file = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync(fileToLaunch);

            // Next, launch the file.
            bool success = await Windows.System.Launcher.LaunchFileAsync(file);
            if (success)
            {
                rootPage.NotifyUser("File launched: " + file.Name, NotifyType.StatusMessage);
            }
            else
            {
                rootPage.NotifyUser("File launch failed.", NotifyType.ErrorMessage);
            }
        }

        // Launch a .png file that came with the package. Show a warning prompt.
        private async void LaunchFileWithWarningButton_Click(object sender, RoutedEventArgs e)
        {
            // First, get the image file from the package's image directory.
            var file = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync(fileToLaunch);

            // Next, configure the warning prompt.
            var options = new Windows.System.LauncherOptions();
            options.TreatAsUntrusted = true;

            // Finally, launch the file.
            bool success = await Windows.System.Launcher.LaunchFileAsync(file, options);
            if (success)
            {
                rootPage.NotifyUser("File launched: " + file.Name, NotifyType.StatusMessage);
            }
            else
            {
                rootPage.NotifyUser("File launch failed.", NotifyType.ErrorMessage);
            }
        }

        // Launch a .png file that came with the package. Show an Open With dialog that lets the user chose the handler to use.
        private async void LaunchFileOpenWithButton_Click(object sender, RoutedEventArgs e)
        {
            // First, get the image file from the package's image directory.
            var file = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync(fileToLaunch);

            // Calculate the position for the Open With dialog.
            // An alternative to using the point is to set the rect of the UI element that triggered the launch.
            Point openWithPosition = GetOpenWithPosition(LaunchFileOpenWithButton);

            // Next, configure the Open With dialog.
            var options = new Windows.System.LauncherOptions();
            options.DisplayApplicationPicker = true;
            options.UI.InvocationPoint = openWithPosition;
            options.UI.PreferredPlacement = Windows.UI.Popups.Placement.Below;

            // Finally, launch the file.
            bool success = await Windows.System.Launcher.LaunchFileAsync(file, options);
            if (success)
            {
                rootPage.NotifyUser("File launched: " + file.Name, NotifyType.StatusMessage);
            }
            else
            {
                rootPage.NotifyUser("File launch failed.", NotifyType.ErrorMessage);
            }
        }

        // Have the user pick a file, then launch it.
        private async void PickAndLaunchFileButton_Click(object sender, RoutedEventArgs e)
        {
            // First, get a file via the picker.
            // To use the picker, the sample must not be snapped.
            if (Windows.UI.ViewManagement.ApplicationView.Value == Windows.UI.ViewManagement.ApplicationViewState.Snapped)
            {
                if (!Windows.UI.ViewManagement.ApplicationView.TryUnsnap())
                {
                    rootPage.NotifyUser("Unable to unsnap the sample.", NotifyType.ErrorMessage);
                    return;
                }
            }

            var openPicker = new Windows.Storage.Pickers.FileOpenPicker();
            openPicker.FileTypeFilter.Add("*");

            Windows.Storage.StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                // Next, launch the file.
                bool success = await Windows.System.Launcher.LaunchFileAsync(file);
                if (success)
                {
                    rootPage.NotifyUser("File launched: " + file.Name, NotifyType.StatusMessage);
                }
                else
                {
                    rootPage.NotifyUser("File launch failed.", NotifyType.ErrorMessage);
                }
            }
            else
            {
                rootPage.NotifyUser("No file was picked.", NotifyType.ErrorMessage);
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