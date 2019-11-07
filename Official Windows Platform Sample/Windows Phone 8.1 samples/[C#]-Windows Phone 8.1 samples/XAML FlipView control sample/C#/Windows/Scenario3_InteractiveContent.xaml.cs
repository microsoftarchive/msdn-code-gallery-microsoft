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
using Controls_FlipView.Data;
using Windows.UI;

namespace Controls_FlipView
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario3 : Page
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public static Scenario3 Current;

        public Scenario3()
        {
            this.InitializeComponent();
            Current = this;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Get our sample data
            var sampleData = new Controls_FlipView.Data.SampleDataSource();

            // Construct the table of contents used for navigating the FlipView
            // Create a StackPanel to host the TOC
            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Vertical;
            sp.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left;
            sp.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Top;

            // Add the TOC title
            TextBlock tb = new TextBlock();
            tb.Text = "Table of Contents";
            tb.Style = this.Resources["TOCTitle"] as Style;
            sp.Children.Add(tb);

            // Create the TOC from the data
            // Use buttons for each TOC entry using the Tag property
            // to contain the index of the target
            int i = 0;
            foreach (SampleDataItem item in sampleData.Items)
            {
                Button b = new Button();
                b.Style = this.Resources["ButtonStyle1"] as Style;
                b.Content = item.Title;
                b.Click += TOCButton_Click;
                b.Tag = (++i).ToString();
                sp.Children.Add(b);
            }

            // Add the TOC to our data set
            sampleData.Items.Insert(0, sp);

            // Use a template selector to style the TOC entry differently from the other data entries
            FlipView3.ItemTemplateSelector = new ItemSelector();
            FlipView3.ItemsSource = sampleData.Items;
        }

        void TOCButton_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (b != null)
            {
                FlipView3.SelectedIndex = Convert.ToInt32(b.Tag);
            }
        }
    }

    public sealed class ItemSelector : DataTemplateSelector
    {
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            SampleDataItem dataItem = item as SampleDataItem;
            DataTemplate itemTemplate = Scenario3.Current.Resources["ImageTemplate"] as DataTemplate;
            DataTemplate tocTemplate = Scenario3.Current.Resources["TOCTemplate"] as DataTemplate;

            if (dataItem != null)
            {
                return itemTemplate;
            }
            else
            {
                return tocTemplate;
            }
        }
    }
}
