// Copyright (c) Microsoft. All rights reserved.

using SDKTemplate;
using System;
using Windows.Storage;
using Windows.UI.Xaml;
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
            GetFileButton.Click += new RoutedEventHandler(GetFileButton_Click);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            rootPage = MainPage.Current;
        }

        private async void GetFileButton_Click(object sender, RoutedEventArgs e)
        {
            StorageFolder storageFolder = KnownFolders.PicturesLibrary;
            StorageFile file = await storageFolder.TryGetItemAsync("sample.dat") as StorageFile;
            if (file != null)
            {
                rootPage.NotifyUser(String.Format("Operation result: {0}", file.Name), NotifyType.StatusMessage);
            }
            else
            {
                rootPage.NotifyUser("Operation result: null", NotifyType.StatusMessage);
            }
        }
    }
}
