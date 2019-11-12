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
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Graphics.Printing;
using Windows.UI.Xaml.Printing;
using Windows.UI.Xaml.Documents;
using SDKTemplateCS;

namespace PrintSample
{
    /// <summary>
    /// Basic scenario for printing (how to register)
    /// </summary>
    public sealed partial class ScenarioInput1 : BasePrintPage
    {
        /// <summary>
        /// A boolean which tracks whether the app has been registered for printing
        /// </summary>
        private bool isRegisteredForPrinting = false;

        public ScenarioInput1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// This is the click handler for the 'Register' button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RegisterForPrintingButtonClick(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;

            // Check to see if the application is registered for printing
            if (isRegisteredForPrinting)
            {
                // Unregister for printing 
                UnregisterForPrinting();

                // Change the text on the button
                clickedButton.Content = "Register";

                // Tell the user that the print contract has been unregistered
                rootPage.NotifyUser("Print contract unregistered.", NotifyType.StatusMessage);
            }
            else
            {
                // Register for printing               
                RegisterForPrinting();

                // Change the text on the button
                clickedButton.Content = "Unregister";

                // Tell the user that the print contract has been registered
                rootPage.NotifyUser("Print contract registered, use the Charms Bar to print.", NotifyType.StatusMessage);
            }

            // Toggle the value of isRegisteredForPrinting
            isRegisteredForPrinting = !isRegisteredForPrinting;
        }

        /// <summary>
        /// Provide print content for scenario 1 first page
        /// </summary>
        protected override void PreparePrintContent()
        {
            if (firstPage == null)
            {
                firstPage = new ScenarioOutput1();
                StackPanel header = (StackPanel)firstPage.FindName("header");
                header.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }

            // Add the (newley created) page to the printing root which is part of the visual tree and force it to go
            // through layout so that the linked containers correctly distribute the content inside them.
            PrintingRoot.Children.Add(firstPage);
            PrintingRoot.InvalidateMeasure();
            PrintingRoot.UpdateLayout();
        }

        #region Navigation
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Get a pointer to our main page
            rootPage = e.Parameter as MainPage;
        }
        #endregion        
    }
}
