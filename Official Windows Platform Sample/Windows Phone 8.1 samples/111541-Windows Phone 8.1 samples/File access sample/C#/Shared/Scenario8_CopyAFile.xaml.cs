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
    /// Copying a file.
    /// </summary>
    public sealed partial class Scenario8 : Page
    {
        MainPage rootPage;

        public Scenario8()
        {
            this.InitializeComponent();
            CopyFileButton.Click += new RoutedEventHandler(CopyFileButton_Click);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            rootPage = MainPage.Current;
            rootPage.ValidateFile();
        }

        private async void CopyFileButton_Click(object sender, RoutedEventArgs e)
        {
            StorageFile file = rootPage.sampleFile;
            if (file != null)
            {
                try
                {
                    StorageFile fileCopy = await file.CopyAsync(KnownFolders.PicturesLibrary, "sample - Copy.dat", NameCollisionOption.ReplaceExisting);
                    rootPage.NotifyUser(String.Format("The file '{0}' was copied and the new file was named '{1}'.", file.Name, fileCopy.Name), NotifyType.StatusMessage);
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
