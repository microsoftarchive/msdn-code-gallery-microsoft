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

namespace ScrollViewerSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario1 : Page
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Scenario1()
        {
            this.InitializeComponent();
            Scenario1Reset();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
#if WINDOWS_PHONE_APP
            MainScrollViewer.VerticalScrollMode = ScrollMode.Enabled;
            MainScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
#else
            MainScrollViewer.VerticalScrollMode = ScrollMode.Enabled;
            MainScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
#endif
        }


        private void CheckBox_Checked_HorizontalRailed(object sender, RoutedEventArgs e)
        {
            scrollViewer.IsHorizontalRailEnabled = true;
        }

        private void CheckBox_Unchecked_HorizontalRailed(object sender, RoutedEventArgs e)
        {
            scrollViewer.IsHorizontalRailEnabled = false;
        }

        private void CheckBox_Checked_VerticalRailed(object sender, RoutedEventArgs e)
        {
            scrollViewer.IsVerticalRailEnabled = true;
        }

        private void CheckBox_Unchecked_VerticalRailed(object sender, RoutedEventArgs e)
        {
            scrollViewer.IsVerticalRailEnabled = false;
        }

        private void hsmCombo_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (scrollViewer == null)
            {
                return;
            }

            ComboBox cb = sender as ComboBox;

            if (cb != null)
            {
                switch (cb.SelectedIndex)
                {
                    case 0: // Auto
                        scrollViewer.HorizontalScrollMode = ScrollMode.Auto;
                        break;
                    case 1: //Enabled
                        scrollViewer.HorizontalScrollMode = ScrollMode.Enabled;
                        break;
                    case 2: // Disabled
                        scrollViewer.HorizontalScrollMode = ScrollMode.Disabled;
                        break;
                    default:
                        scrollViewer.HorizontalScrollMode = ScrollMode.Enabled;
                        break;
                }
            }
        }

        private void hsbvCombo_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (scrollViewer == null)
            {
                return;
            }

            ComboBox cb = sender as ComboBox;

            if (cb != null)
            {
                switch (cb.SelectedIndex)
                {
                    case 0: // Auto
                        scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                        break;
                    case 1: //Visible
                        scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
                        break;
                    case 2: // Hidden
                        scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                        break;
                    case 3: // Disabled
                        scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                        break;
                    default:
                        scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                        break;
                }
            }
        }

        private void vsmCombo_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (scrollViewer == null)
            {
                return;
            }

            ComboBox cb = sender as ComboBox;

            if (cb != null)
            {
                switch (cb.SelectedIndex)
                {
                    case 0: // Auto
                        scrollViewer.VerticalScrollMode = ScrollMode.Auto;
                        break;
                    case 1: //Enabled
                        scrollViewer.VerticalScrollMode = ScrollMode.Enabled;
                        break;
                    case 2: // Disabled
                        scrollViewer.VerticalScrollMode = ScrollMode.Disabled;
                        break;
                    default:
                        scrollViewer.VerticalScrollMode = ScrollMode.Enabled;
                        break;
                }
            }
        }

        private void vsbvCombo_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (scrollViewer == null)
            {
                return;
            }

            ComboBox cb = sender as ComboBox;

            if (cb != null)
            {
                switch (cb.SelectedIndex)
                {
                    case 0: // Auto
                        scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                        break;
                    case 1: //Visible
                        scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                        break;
                    case 2: // Hidden
                        scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                        break;
                    case 3: // Disabled
                        scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                        break;
                    default:
                        scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                        break;
                }
            }
        }

        private void Scenario1Reset(object sender, RoutedEventArgs e)
        {
            Scenario1Reset();
        }

        void Scenario1Reset()
        {
            //Restore to defaults
            hsbvCombo.SelectedIndex = 3;            
            hsmCombo.SelectedIndex = 1;            
            vsbvCombo.SelectedIndex = 1;            
            vsmCombo.SelectedIndex = 1;            
            hrCheckbox.IsChecked = true;           
            vrCheckbox.IsChecked = true;
        }

    }
}
