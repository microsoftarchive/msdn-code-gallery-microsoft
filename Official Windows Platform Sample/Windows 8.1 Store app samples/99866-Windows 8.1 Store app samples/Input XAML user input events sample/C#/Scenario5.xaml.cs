//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;
using Windows.UI.Xaml.Media;
using Windows.UI;

namespace Input
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario5 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Scenario5()
        {
            this.InitializeComponent();
            scenario5Reset.Loaded += scenario5Reset_Loaded;
        }

        void scenario5Reset_Loaded(object sender, RoutedEventArgs e)
        {
            scenario5Reset.Focus(Windows.UI.Xaml.FocusState.Programmatic);
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        void Scenario5Reset(object sender, RoutedEventArgs e)
        {
            Scenario5Reset();
        }

        void Scenario5Reset()
        {
            Scenario5UpdateVisuals(bChange, "");
            keyState.Text = "";
        }

        private void Output_KeyDown_1(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            keyState.Text = "\""+ e.Key.ToString() + "\"" + " KeyDown";
        }

        private void Output_KeyUp_1(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            keyState.Text = "\"" + e.Key.ToString() + "\"" + " KeyUp";
            Scenario5UpdateVisuals(bChange, e.Key.ToString());
        }

        void Scenario5UpdateVisuals(Border border, String color)
        {
            switch (color.ToLower())
            {
                case "v":
                    border.Background = new SolidColorBrush(Colors.Violet);
                    ((TextBlock)border.Child).Text = "Violet";
                    break;
                case "i":
                    border.Background = new SolidColorBrush(Colors.Indigo);
                    ((TextBlock)border.Child).Text = "Indigo";
                    break;
                case "b":
                    border.Background = new SolidColorBrush(Colors.Blue);
                    ((TextBlock)border.Child).Text = "Blue";
                    break;
                case "g":
                    border.Background = new SolidColorBrush(Colors.Green);
                    ((TextBlock)border.Child).Text = "Green";
                    break;
                case "y":
                    border.Background = new SolidColorBrush(Colors.Yellow);
                    ((TextBlock)border.Child).Text = "Yellow";
                    break;
                case "o":
                    border.Background = new SolidColorBrush(Colors.Orange);
                    ((TextBlock)border.Child).Text = "Orange";
                    break;
                case "r":
                    border.Background = new SolidColorBrush(Colors.Red);
                    ((TextBlock)border.Child).Text = "Red";
                    break;
                default:
                    border.Background = new SolidColorBrush(Colors.White);
                    ((TextBlock)border.Child).Text = "White";
                    break;
            }            
        }        
    }
}
