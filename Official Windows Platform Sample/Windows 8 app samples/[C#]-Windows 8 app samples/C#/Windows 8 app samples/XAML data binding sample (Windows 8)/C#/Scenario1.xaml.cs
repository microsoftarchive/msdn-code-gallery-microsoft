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

namespace DataBinding
{

    public sealed partial class Scenario1 : SDKTemplate.Common.LayoutAwarePage
    {
        public Scenario1()
        {
            this.InitializeComponent();
        }

        private void Scenario1Reset(object sender, RoutedEventArgs e)
        {
            sliderOneWayDataSource.Value = 10;
            sliderTwoWayDataSource.Value = 50;
            sliderOneTimeDataSource.Value = 100;
        }

    }
}
