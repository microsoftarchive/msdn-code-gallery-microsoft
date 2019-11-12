/****************************** Module Header ******************************\
 * Module Name:  MainPage.xaml.cs
 * Project:      CSUnvsAppEnumRadioButton.WindowsPhone
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
using CSUnvsAppEnumRadioButton.Common;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace CSUnvsAppEnumRadioButton
{
    public sealed partial class MainPage : Page
    {
        private NavigationHelper navigationHelper;
        // Selected customer to edit
        private Customer selectedCustomer;

        private ScrollViewer gridScrollViewer;
        public MainPage()
        {
            this.InitializeComponent();

            //this.NavigationCacheMode = NavigationCacheMode.Required;


            // Bind the customer collection to GridView
            CustomerGridView.ItemsSource = CustomerCollection.Customers;
            
            this.navigationHelper = new NavigationHelper(this);

            navigationHelper.LoadState += navigationHelper_LoadState;
            navigationHelper.SaveState += navigationHelper_SaveState;

            Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;

            
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Rstore GridView's scroll position.
            gridScrollViewer = FindVisualChild<ScrollViewer>(CustomerGridView);
            var frameState = SuspensionManager.SessionStateForFrame(this.Frame);
            if(frameState.Count > 0)
            {
                Dictionary<String, Object> pos;
                if(frameState.ContainsKey("Page-" + (Frame.BackStackDepth - 2)))
                {
                    pos = frameState["Page-" + (Frame.BackStackDepth - 2)] as Dictionary<String, Object>;
                    if (pos.ContainsKey("ScrollPosition"))
                    {
                        gridScrollViewer.ChangeView(0.0f, (double)pos["ScrollPosition"], 1.0f);
                    }
                }                
                
            }
            

        } 
        private void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            if (this.Frame.SourcePageType == typeof(MainPage))
            {
                this.Frame.BackStack.Clear();
            }
        }

        void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            if(gridScrollViewer != null)
            {
                e.PageState["ScrollPosition"] = gridScrollViewer.VerticalOffset;
            }            
        }

        void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {            
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
            navigationHelper.OnNavigatedTo(e);          
        }

        // From http://msdn.microsoft.com/en-us/library/bb613579.aspx
        private childItem FindVisualChild<childItem>(DependencyObject obj)
            where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                    return (childItem)child;
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }
        
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {            
            this.navigationHelper.OnNavigatedFrom(e);
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
