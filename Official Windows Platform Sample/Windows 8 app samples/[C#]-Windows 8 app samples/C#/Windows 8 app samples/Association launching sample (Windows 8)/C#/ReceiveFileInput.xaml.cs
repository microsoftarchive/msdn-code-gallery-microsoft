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
    public sealed partial class ReceiveFileInput : Page
    {
        // A pointer back to the main page which is used to gain access to the input and output frames and their content.
        rootPage rootPage = null;

        public ReceiveFileInput()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Get a pointer to our main page
            rootPage = e.Parameter as rootPage;

            // Display the result of the file activation if we got here as a result of being activated for a file.
            if (rootPage.FileEvent != null)
            {
                rootPage.NotifyUser("File activation received. The number of files received is " + rootPage.FileEvent.Files.Count + ". The first received file is " + rootPage.FileEvent.Files[0].Name + ".", NotifyType.StatusMessage);
            }
        }
    }
}