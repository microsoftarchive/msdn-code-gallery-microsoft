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

namespace ControlAndAppStyle
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario1 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Scenario1()
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
        }

        private void styleKeydInResource_Click(object sender, RoutedEventArgs e)
        {
            styleKeydInResource_Output.Visibility = Visibility.Visible;
            implicitStyle_Output.Visibility = Visibility.Collapsed;
            implicitStyleScoped_Output.Visibility = Visibility.Collapsed;
            implicitStyleInApp_Output.Visibility = Visibility.Collapsed;

        }

        private void implicitStyle_Click(object sender, RoutedEventArgs e)
        {
            styleKeydInResource_Output.Visibility = Visibility.Collapsed;
            implicitStyle_Output.Visibility = Visibility.Visible;
            implicitStyleScoped_Output.Visibility = Visibility.Collapsed;
            implicitStyleInApp_Output.Visibility = Visibility.Collapsed;

        }

        private void implicitStyleScoped_Click(object sender, RoutedEventArgs e)
        {
            styleKeydInResource_Output.Visibility = Visibility.Collapsed;
            implicitStyle_Output.Visibility = Visibility.Collapsed;
            implicitStyleScoped_Output.Visibility = Visibility.Visible;
            implicitStyleInApp_Output.Visibility = Visibility.Collapsed;

        }

        private void implicitStyleInApp_Click(object sender, RoutedEventArgs e)
        {
            styleKeydInResource_Output.Visibility = Visibility.Collapsed;
            implicitStyle_Output.Visibility = Visibility.Collapsed;
            implicitStyleScoped_Output.Visibility = Visibility.Collapsed;
            implicitStyleInApp_Output.Visibility = Visibility.Visible;

        }

    }
}
