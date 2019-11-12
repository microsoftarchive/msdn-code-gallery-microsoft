//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace DataSourceAdapter
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario2 : SDKTemplate.Common.LayoutAwarePage
    {
        public Scenario2()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            RunButton.Click += new RoutedEventHandler(RunButton_Click);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            RunButton.Click -= new RoutedEventHandler(RunButton_Click);
        }

        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            ((Frame)(Window.Current.Content)).Navigate(typeof(PictureItemsView), this);
        }
    }
}
