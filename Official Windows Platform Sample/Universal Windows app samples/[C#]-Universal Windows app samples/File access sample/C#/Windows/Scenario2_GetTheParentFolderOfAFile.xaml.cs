// Copyright (c) Microsoft. All rights reserved.

using SDKTemplate;
using System;
using System.IO;
using System.Text;
using Windows.Storage;
using Windows.UI.Xaml;
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
            GetParentButton.Click += new RoutedEventHandler(GetParent_Click);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            rootPage = MainPage.Current;
            rootPage.ValidateFile();
        }

        private async void GetParent_Click(object sender, RoutedEventArgs e)
        {
            StorageFile file = rootPage.sampleFile;
            if (file != null)
            {
                try
                {
                    StorageFolder parentFolder = await file.GetParentAsync();
                    if (parentFolder != null)
                    {
                        StringBuilder outputText = new StringBuilder();
                        outputText.AppendLine(String.Format("Item: {0} ({1})", file.Name, file.Path));
                        outputText.Append(String.Format("Parent: {0} ({1})", parentFolder.Name, parentFolder.Path));

                        rootPage.NotifyUser(outputText.ToString(), NotifyType.StatusMessage);
                    }
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
