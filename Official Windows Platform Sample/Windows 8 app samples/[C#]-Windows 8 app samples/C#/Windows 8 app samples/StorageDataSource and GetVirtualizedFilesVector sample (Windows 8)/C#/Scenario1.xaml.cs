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
    public sealed partial class Scenario1 : SDKTemplate.Common.LayoutAwarePage
    {
        public Scenario1()
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
            ((Frame)(Window.Current.Content)).Navigate(typeof(PictureFilesView), this);
        }
    }
}
