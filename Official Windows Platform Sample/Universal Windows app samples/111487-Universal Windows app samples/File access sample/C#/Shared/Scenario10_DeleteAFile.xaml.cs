// Copyright (c) Microsoft. All rights reserved.

using SDKTemplate;
using System;
using System.IO;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace FileAccess
{
    /// <summary>
    /// Deleting a file.
    /// </summary>
    public sealed partial class Scenario10 : Page
    {
        MainPage rootPage;

        public Scenario10()
        {
            this.InitializeComponent();
            DeleteFileButton.Click += new RoutedEventHandler(DeleteFileButton_Click);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            rootPage = MainPage.Current;
            rootPage.ValidateFile();
        }

        private async void DeleteFileButton_Click(object sender, RoutedEventArgs e)
        {
            StorageFile file = rootPage.sampleFile;
            if (file != null)
            {
                try
                {
                    string filename = file.Name;
                    await file.DeleteAsync();
                    rootPage.sampleFile = null;
                    rootPage.NotifyUser(String.Format("The file '{0}' was deleted", filename), NotifyType.StatusMessage);
                }
                catch (FileNotFoundException)
                {
                    rootPage.NotifyUserFileNotExist();
                }
            }
            else
            {
                rootPage.NotifyUserFileNotExist();
            }
        }
    }
}
