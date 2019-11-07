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
    public sealed partial class Scenario2 : Page
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Scenario2()
        {
            this.InitializeComponent();
            Scenario2Reset();
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

        private void ComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
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
                    case 0: // None
                        scrollViewer.HorizontalSnapPointsType = SnapPointsType.None;
                        break;
                    case 1: //Optional
                        scrollViewer.HorizontalSnapPointsType = SnapPointsType.Optional;
                        break;
                    case 2: // Optional Single
                        scrollViewer.HorizontalSnapPointsType = SnapPointsType.OptionalSingle;
                        break;
                    case 3: // Mandatory
                        scrollViewer.HorizontalSnapPointsType = SnapPointsType.Mandatory;
                        break;
                    case 4: // Mandatory Single
                        scrollViewer.HorizontalSnapPointsType = SnapPointsType.MandatorySingle;
                        break;
                    default:
                        scrollViewer.HorizontalSnapPointsType = SnapPointsType.None;
                        break;
                }
            }
        }

        private void Scenario2Reset(object sender, RoutedEventArgs e)
        {
            Scenario2Reset();
        }

        void Scenario2Reset()
        {
            //Restore to defaults
            scrollViewer.HorizontalSnapPointsType = SnapPointsType.None;
            snapPointsCombo.SelectedIndex = 0;
        }

    }
}
