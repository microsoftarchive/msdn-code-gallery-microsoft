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
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace FileAccess
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario1 : SDKTemplate.Common.LayoutAwarePage
    {
        MainPage rootPage = MainPage.Current;

        public Scenario1()
        {
            this.InitializeComponent();
            CreateFileButton.Click += new RoutedEventHandler(CreateFileButton_Click);
        }

        /// <summary>
        /// Creates a new file
        /// </summary>
        private async void CreateFileButton_Click(object sender, RoutedEventArgs e)
        {
            StorageFolder storageFolder = KnownFolders.DocumentsLibrary;
            rootPage.sampleFile = await storageFolder.CreateFileAsync(MainPage.filename, CreationCollisionOption.ReplaceExisting);
            OutputTextBlock.Text = "The file '" + rootPage.sampleFile.Name + "' was created.";
        }
    }
}
