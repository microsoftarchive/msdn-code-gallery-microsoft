// Copyright (c) Microsoft. All rights reserved.

using SDKTemplate;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace FileAccess
{
    /// <summary>
    /// Getting a file's parent folder.
    /// </summary>
    public sealed partial class Scenario2 : Page
    {
        MainPage rootPage;

        public Scenario2()
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
