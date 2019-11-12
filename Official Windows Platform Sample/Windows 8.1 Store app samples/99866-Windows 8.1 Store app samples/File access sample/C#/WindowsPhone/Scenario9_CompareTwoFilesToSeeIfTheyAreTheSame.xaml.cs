// Copyright (c) Microsoft. All rights reserved.

using SDKTemplate;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace FileAccess
{
    /// <summary>
    /// Comparing two files to see if they are the same file.
    /// </summary>
    public sealed partial class Scenario9 : Page
    {
        MainPage rootPage;

        public Scenario9()
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
