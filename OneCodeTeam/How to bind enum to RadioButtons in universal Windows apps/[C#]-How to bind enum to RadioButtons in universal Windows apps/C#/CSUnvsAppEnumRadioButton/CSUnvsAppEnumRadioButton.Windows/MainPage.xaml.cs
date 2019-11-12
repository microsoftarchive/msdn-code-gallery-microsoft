/****************************** Module Header ******************************\
 * Module Name:  MainPage.xaml.cs
 * Project:      CSUnvsAppEnumRadioButton.Windows
 * Copyright (c) Microsoft Corporation.
 * 
 * This sample demonstrates how to bind enum to RadioButton
 *  
 * 
 * This source is subject to the Microsoft Public License.
 * See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
 * All other rights reserved.
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


namespace CSUnvsAppEnumRadioButton
{
    public sealed partial class MainPage : Page
    {
        // Selected customer to edit
        private Customer selectedCustomer;

        public MainPage()
        {
            this.InitializeComponent();

            // Bind the customer collection to GridView
            CustomerGridView.ItemsSource = CustomerCollection.Customers;
        }

        private async void Footer_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton hyperlinkButton = sender as HyperlinkButton;
            if (hyperlinkButton != null)
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri(hyperlinkButton.Tag.ToString()));
            }
        }

        private void CustomerGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedCustomer = CustomerGridView.SelectedItem as Customer;
            // Navigate to Edit page with the selected item as parameter
            if (selectedCustomer != null)
            {
                Frame.Navigate(typeof(EditPage), selectedCustomer);
            }

        }
    }


}
