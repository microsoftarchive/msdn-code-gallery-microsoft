// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Interop;
using Windows.UI.Xaml.Navigation;
using SDKTemplateCS;

namespace KeyboardEventsSampleCS
{
    public sealed partial class ScenarioInput1 : Page
    {
        MainPage rootPage;

        public ScenarioInput1()
        {
            InitializeComponent();
        }

        void Scenario1Open_Click(Object sender, RoutedEventArgs e)
        {
            rootPage.Frame.Navigate(typeof(KeyboardPage));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Get a pointer to our main page
            rootPage = e.Parameter as MainPage;
        }
    }
}
