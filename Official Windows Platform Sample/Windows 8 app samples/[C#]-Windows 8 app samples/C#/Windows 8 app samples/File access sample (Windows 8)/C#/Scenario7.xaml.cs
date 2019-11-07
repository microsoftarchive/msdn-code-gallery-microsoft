//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

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
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario7 : SDKTemplate.Common.LayoutAwarePage
    {
        MainPage rootPage = MainPage.Current;

        public Scenario7()
        {
            this.InitializeComponent();
            CopyFileButton.Click += new RoutedEventHandler(CopyFileButton_Click);
        }

        private async void CopyFileButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                rootPage.ResetScenarioOutput(OutputTextBlock);
                StorageFile file = rootPage.sampleFile;
                if (file != null)
                {
                    StorageFile fileCopy = await file.CopyAsync(KnownFolders.DocumentsLibrary, "sample - Copy.dat", NameCollisionOption.ReplaceExisting);
                    OutputTextBlock.Text = "The file '" + file.Name + "' was copied and the new file was named '" + fileCopy.Name + "'.";
                }
            }
            catch (FileNotFoundException)
            {
                rootPage.NotifyUserFileNotExist();
            }
        }
    }
}
