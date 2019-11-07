// Copyright (c) Microsoft. All rights reserved.

using SDKTemplate;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace FileAccess
{
    /// <summary>
    /// Attempting to get a file with no error on failure.
    /// </summary>
    public sealed partial class Scenario11 : Page
    {
        MainPage rootPage;

        public Scenario11()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            rootPage = MainPage.Current;
            rootPage.NotifyUser("Windows Phone doesn’t currently support this function.", NotifyType.ErrorMessage);
        }
    }
}
