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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SDKTemplateCS;
using Expression.Blend.SampleData.SampleDataSource;

namespace ListViewInteraction
{
    public sealed partial class ScenarioInput4 : Page
    {
        // A pointer back to the main page which is used to gain access to the input and output frames and their content.
        MainPage rootPage = null;
        GridView FlavorGrid = null;
        TextBlock CustomCarton = null;
        GridView FixinsGrid = null;

        public ScenarioInput4()
        {
            InitializeComponent();
        }

        #region Template-Related Code - Do not remove
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Get a pointer to our main page
            rootPage = e.Parameter as MainPage;

            // We want to be notified with the OutputFrame is loaded so we can get to the content.
            rootPage.OutputFrameLoaded += new System.EventHandler(rootPage_OutputFrameLoaded);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            rootPage.OutputFrameLoaded -= new System.EventHandler(rootPage_OutputFrameLoaded);
        }

        #endregion

        #region Use this code if you need access to elements in the output frame - otherwise delete
        void rootPage_OutputFrameLoaded(object sender, object e)
        {
            // At this point, we know that the Output Frame has been loaded and we can go ahead
            // and reference elements in the page contained in the Output Frame.

            // Get a pointer to the content within the OutputFrame.
            Page outputFrame = (Page)rootPage.OutputFrame.Content;

            // Go find the elements that we need for this scenario.
            // ex: flipView1 = outputFrame.FindName("FlipView1") as FlipView;
            FlavorGrid = outputFrame.FindName("FlavorGrid") as GridView;
            CustomCarton = outputFrame.FindName("CustomCarton") as TextBlock;
            FixinsGrid = outputFrame.FindName("FixinsGrid") as GridView;
        }

        #endregion

        void CreateCustomCarton_Click(object sender, RoutedEventArgs e)
        {
            rootPage.NotifyUser("", NotifyType.StatusMessage);
            if (FlavorGrid.SelectedItems.Count > 0)
            {
                CustomCarton.Text = "Custom Carton: ";
                char[] charsToTrim = { ',', ' ' };
                CustomCarton.Text += ((Item)FlavorGrid.SelectedItem).Title;
                if (FixinsGrid.SelectedItems.Count > 0)
                {
                    CustomCarton.Text += " with ";
                    foreach (Item topping in FixinsGrid.SelectedItems)
                    {
                        CustomCarton.Text += topping.Title + ", ";
                    }
                    CustomCarton.Text = CustomCarton.Text.TrimEnd(charsToTrim);
                    CustomCarton.Text += " toppings";
                }
            }
            else
            {
                rootPage.NotifyUser("Please select at least a flavor...", NotifyType.ErrorMessage);
            }
        }


    }
}
