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
using System.Collections.Generic;

namespace AppBarControl
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario6 : Windows.UI.Xaml.Controls.Page
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        private StackPanel leftPanel = null;
        private StackPanel rightPanel = null;
        private List<UIElement> commands = new List<UIElement>();

        public Scenario6()
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
            rootPage.BottomAppBar.Opened += BottomAppBar_Opened;
            rootPage.BottomAppBar.Closed += BottomAppBar_Closed;

            leftPanel = rootPage.FindName("LeftPanel") as StackPanel;
            rightPanel = rootPage.FindName("RightPanel") as StackPanel;

            ShowAppBar.IsEnabled = true;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            rootPage.BottomAppBar.IsOpen = false;
            rootPage.BottomAppBar.IsSticky = false;
            ShowAppBarButtons();
        }

        void BottomAppBar_Closed(object sender, object e)
        {
            ShowAppBar.IsEnabled = true;
            HideCommands.IsEnabled = false;
        }

        void BottomAppBar_Opened(object sender, object e)
        {
            ShowAppBar.IsEnabled = false;

            AppBar ab = sender as AppBar;
            if (ab != null)
            {
                ab.IsSticky = true;
                if (leftPanel.Children.Count > 0 && rightPanel.Children.Count > 0)
                {
                    HideCommands.IsEnabled = true;
                }
            }
        }


        /// <summary>
        /// This is the click handler for the 'Show AppBar' button.  
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowAppBarClick(object sender, RoutedEventArgs e)
        {
            rootPage.BottomAppBar.IsOpen = true;
            HideCommands.IsEnabled = true;
        }

        /// <summary>
        /// This is the click handler for the 'Show Commands' button.  
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowCommands_Click(object sender, RoutedEventArgs e)
        {
            HideCommands.IsEnabled = true;
            ShowCommands.IsEnabled = false;
            ShowAppBarButtons();
        }

        /// <summary>
        /// This is the click handler for the 'Hide Commands' button.  
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HideCommands_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (b != null)
            {
                b.IsEnabled = false;
                ShowCommands.IsEnabled = true;
                commands.Clear();
                HideAppBarButtons(rightPanel);
            }

        }

        private void HideAppBarButtons(StackPanel panel)
        {
            foreach (var item in panel.Children)
            {
                commands.Add(item);
            }
            panel.Children.Clear();
        }

        private void ShowAppBarButtons()
        {
            foreach (var item in commands)
            {
                rightPanel.Children.Add(item);
            }
            commands.Clear();
        }
    }
}
