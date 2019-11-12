// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.ApplicationSettings;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The SettingsFlyout item template is documented at http://go.microsoft.com/fwlink/?LinkId=273769

namespace ApplicationSettings
{

    public sealed partial class SettingsFlyout2 : SettingsFlyout
    {
        bool isSecondContentLayer = false;

        public SettingsFlyout2()
        {
            this.InitializeComponent();

            // Handle all key events when loaded into visual tree
            this.Loaded += (sender, e) =>
            {
                Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated += SettingsFlyout2_AcceleratorKeyActivated;
            };
            this.Unloaded += (sender, e) =>
            {
                Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated -= SettingsFlyout2_AcceleratorKeyActivated;
            };
        }

        /// <summary>
        /// This is the handler for the button Click event. Content of the second layer is dynamically 
        /// generated and shown.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Click(object sender, RoutedEventArgs e)
        {
            // Attach BackClick handler to override default back button behavior
            this.BackClick += SettingsFlyout2_BackClick;

            // Create second layer of content.
            TextBlock header = new TextBlock();
            header.Text = "Layer 2 Content Header";
            header.Style = (Style)Application.Current.Resources["TitleTextBlockStyle"];
            TextBlock tb = new TextBlock();
            tb.Text = "Layer 2 of content.  Click the back button to return to the previous content.";
            tb.Style = (Style)Application.Current.Resources["BodyTextBlockStyle"];

            StackPanel sp = new StackPanel();
            sp.Children.Add(header);
            sp.Children.Add(tb);
            this.Content = sp;

            this.isSecondContentLayer = true;
        }

        /// <summary>
        /// This is the handler for the SettingsFlyout2 BackClick event. Original content is restored 
        /// and the event args are marked as handled.
        /// This handler is only attached when the second content layer is visible.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void SettingsFlyout2_BackClick(object sender, BackClickEventArgs e)
        {
            // Return to previous content and remove BackClick handler
            e.Handled = true;
            this.isSecondContentLayer = false;
            this.Content = this.content1;
            this.BackClick -= SettingsFlyout2_BackClick;
        }

        /// <summary>
        /// Invoked on every keystroke, including system keys such as Alt key combinations, when
        /// this page is active and occupies the entire window.  Used to detect keyboard back 
        /// navigation via Alt+Left key combination.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void SettingsFlyout2_AcceleratorKeyActivated(CoreDispatcher sender, AcceleratorKeyEventArgs args)
        {
            // Only investigate further when Left is pressed
            if (args.EventType == CoreAcceleratorKeyEventType.SystemKeyDown &&
                args.VirtualKey == VirtualKey.Left)
            {
                var coreWindow = Window.Current.CoreWindow;
                var downState = CoreVirtualKeyStates.Down;

                // Check for modifier keys
                // The Menu VirtualKey signifies Alt
                bool menuKey = (coreWindow.GetKeyState(VirtualKey.Menu) & downState) == downState;
                bool controlKey = (coreWindow.GetKeyState(VirtualKey.Control) & downState) == downState;
                bool shiftKey = (coreWindow.GetKeyState(VirtualKey.Shift) & downState) == downState;

                if (menuKey && !controlKey && !shiftKey)
                {
                    args.Handled = true;

                    // If in second content layer, return to previous content
                    // Otherwise, dismiss the SettingsFlyout
                    if (this.isSecondContentLayer)
                    {
                        this.isSecondContentLayer = false;
                        this.Content = this.content1;
                        this.BackClick -= SettingsFlyout2_BackClick;
                    }
                    else
                    {
                        this.Hide();
                    }
                }
            }
        }
    }
}
