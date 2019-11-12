//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace AppUIBasics.Navigation
{
    public sealed partial class HierarchicalNavControl : NavUserControl
    {
        private ToggleButton currentCheckedToggleButton = null;

        public HierarchicalNavControl()
        {
            this.InitializeComponent();
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            if (this.currentCheckedToggleButton != null)
            {
                this.currentCheckedToggleButton.IsChecked = false;
            }

            this.currentCheckedToggleButton = (ToggleButton)sender;

            SecondLevelNavRow.DataContext = ((PageInfo)this.currentCheckedToggleButton.DataContext).ChildData;
            SecondLevelNavRow.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            this.HideSecondLevelNav();
        }

        // Call this from the Closed handler of the parent app bar.
        internal void HideSecondLevelNav()
        {
            if (this.SecondLevelNavRow.Visibility == Windows.UI.Xaml.Visibility.Visible)
            {
                this.SecondLevelNavRow.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                if (this.currentCheckedToggleButton != null)
                {
                    this.currentCheckedToggleButton.IsChecked = false;
                    this.currentCheckedToggleButton = null;
                }
            }
        }

        private void NavFlipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.currentCheckedToggleButton != null && NavFlipView.Items.Count > 1)
            {
                this.HideSecondLevelNav();
            }
        }


        // Overrides

        protected override void NavButton_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (b != null)
            {
                // If the button links to a page, navigate to the page.
                PageInfo pageInfo = (PageInfo)b.DataContext;
                if (pageInfo == null) return;

                // If the button links to a page, navigate to the page.
                if (pageInfo.PageType != typeof(Type))
                {
                    if (pageInfo.Data == null && NavigationRootPage.RootFrame.CurrentSourcePageType != pageInfo.PageType)
                    {
                        NavigationRootPage.RootFrame.Navigate(pageInfo.PageType);
                        NavigationRootPage.Current.TopAppBar.IsOpen = false;
                    }
                    else if (pageInfo.Data != null)
                    {
                        NavigationRootPage.RootFrame.Navigate(pageInfo.PageType, pageInfo.Data.ToString());
                        NavigationRootPage.Current.TopAppBar.IsOpen = false;
                    }
                }
                else
                {
                    NavToggleButton toggleButton = b as NavToggleButton;
                    if (toggleButton != null)
                    {
                        if (toggleButton.Toggle == this.currentCheckedToggleButton)
                        {
                            this.HideSecondLevelNav();
                        }
                        else
                        {
                            if (SecondLevelNavRow.Visibility == Windows.UI.Xaml.Visibility.Visible)
                            {
                                this.HideSecondLevelNav();
                            }

                            toggleButton.Toggle.IsChecked = true;
                            this.currentCheckedToggleButton = toggleButton.Toggle;
                            return;
                        }
                    }
                }
            }
        }

        protected override void GetPageInfo()
        {
            pageInfoList.Clear();

            if (this.DataContext != null)
            {
                pageInfoList = this.ConvertDataContextToList(this.DataContext);

                // Insert 'Home' button.
                pageInfoList.Insert(0, new PageInfo("Home", typeof(MainPage)));
            }

            // To populate the navigation control manually, instead of from the DataContext,
            // delete the previous code and add your page info as shown here. 
            // Use the fully qualified class name for each page.

            // pageInfoList.Add(new PageInfo("Home", typeof(HubPage)));
            // pageInfoList.Add(new PageInfo("Page 2", typeof(Page2)));
            // pageInfoList.Add(new PageInfo("Page 3", typeof(Page3), "Navigation Parameter"));
            // pageInfoList.Add(new PageInfo("Button", typeof(ItemPage), "Button"));

            // This one has a sub-menu, but doesn't navigate to a page.
            // pageInfoList.Add(new PageInfo("Button Menu", typeof(Type), null, await AppUIBasics.Data.ControlInfoDataSource.GetGroupAsync("Buttons")));
        }

        protected override Button GetButton(PageInfo pageInfo)
        {
            if (pageInfo.ChildData == null)
            {
                return base.GetButton(pageInfo);
            }
            else
            {
                NavToggleButton newButton = new NavToggleButton();
                newButton.DataContext = pageInfo;
                newButton.Content = pageInfo.Title;
                newButton.Click += NavButton_Click;
                newButton.Style = (Style)this.Resources["NavToggleButtonStyle"];

                return newButton;
            }
        }
    }
}
