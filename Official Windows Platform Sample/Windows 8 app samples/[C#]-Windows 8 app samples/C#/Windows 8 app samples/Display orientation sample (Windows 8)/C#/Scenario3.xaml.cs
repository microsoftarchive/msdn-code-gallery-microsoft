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
using SDKTemplate;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace DisplayOrientation
{
    public sealed partial class Scenario3 : SDKTemplate.Common.LayoutAwarePage
    {
        public Scenario3()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            screenOrientation.Text = DisplayProperties.CurrentOrientation.ToString();
            DisplayProperties.OrientationChanged += OnOrientationChanged;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            DisplayProperties.OrientationChanged -= OnOrientationChanged;
        }

        void OnOrientationChanged(object sender)
        {
            screenOrientation.Text = DisplayProperties.CurrentOrientation.ToString();
        }
    }
}
