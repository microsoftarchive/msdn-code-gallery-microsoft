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
            this.BottomAppBar.Opened += BottomAppBar_Opened;
            this.BottomAppBar.Closed += BottomAppBar_Closed;
            HideCommands.IsEnabled = true;

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.BottomAppBar.IsOpen = false;
            this.BottomAppBar.IsSticky = false;
            ShowAppBarButtons();
        }

        void BottomAppBar_Closed(object sender, object e)
        {
            HideCommands.IsEnabled = false;
        }

        void BottomAppBar_Opened(object sender, object e)
        {
            CommandBar cb = sender as CommandBar;
            if (cb != null)
            {
                cb.IsSticky = true;
                if (cb.PrimaryCommands.Count > 0)
                {
                    HideCommands.IsEnabled = true;
                }
            }
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
                HideAppBarButtons();
            }

        }

        private void HideAppBarButtons()
        {
            foreach (var item in ((CommandBar)BottomAppBar).PrimaryCommands)
            {
                commands.Add(item as UIElement);
            }
            ((CommandBar)BottomAppBar).PrimaryCommands.Clear();
        }

        private void ShowAppBarButtons()
        {
            foreach (var item in commands)
            {
                ((CommandBar)BottomAppBar).PrimaryCommands.Add(item as ICommandBarElement);
            }
            commands.Clear();
        }
    }
}
