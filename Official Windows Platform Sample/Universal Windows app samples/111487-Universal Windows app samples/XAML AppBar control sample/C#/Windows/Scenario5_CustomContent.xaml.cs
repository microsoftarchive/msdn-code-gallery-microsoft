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
    public sealed partial class Scenario5 : Windows.UI.Xaml.Controls.Page
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;
        StackPanel leftPanel;
        List<UIElement> leftItems;

        public Scenario5()
        {
            this.InitializeComponent();
            leftItems = new List<UIElement>();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Add some custom (non-AppBarButton) content
            leftPanel = rootPage.FindName("LeftPanel") as StackPanel;

            foreach (UIElement element in leftPanel.Children)
            {
                leftItems.Add(element);
            }
            leftPanel.Children.Clear();

            // Create a combo box
            ComboBox cb = new ComboBox();
            cb.Height = 32.0;
            cb.Width = 100.0;
            cb.Items.Add("Baked");
            cb.Items.Add("Fried");
            cb.Items.Add("Frozen");
            cb.Items.Add("Chilled");

            cb.SelectedIndex = 0;

            leftPanel.Children.Add(cb);

            // Create a text box
            TextBox tb = new TextBox();
            tb.Text = "Search for desserts.";
            tb.Width = 300.0;
            tb.Height = 30.0;
            tb.Margin = new Thickness(10.0, 0.0, 0.0, 0.0);
            tb.GotFocus += tb_GotFocus;

            leftPanel.Children.Add(tb);

            // Add a button
            Button b = new Button();
            b.Content = "Search";
            b.Click += b_Click;

            leftPanel.Children.Add(b);
        }

        private void b_Click(object sender, RoutedEventArgs e)
        {
            rootPage.NotifyUser("Search button pressed", NotifyType.StatusMessage);
        }

        private void tb_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            tb.Text = String.Empty;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            leftPanel.Children.Clear();
            foreach (UIElement element in leftItems)
            {
                leftPanel.Children.Add(element);
            }

        }
    }
}
