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
using System.Collections.Generic;
using System.IO;
using System.Text;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace FileAccess
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario6 : SDKTemplate.Common.LayoutAwarePage
    {
        MainPage rootPage = MainPage.Current;

        public Scenario6()
        {
            this.InitializeComponent();
            AddToListButton.Click += new RoutedEventHandler(AddToListButton_Click);
            ShowListButton.Click += new RoutedEventHandler(ShowListButton_Click);
            OpenFromListButton.Click += new RoutedEventHandler(OpenFromListButton_Click);
        }

        private void AddToListButton_Click(object sender, RoutedEventArgs e)
        {
            rootPage.ResetScenarioOutput(OutputTextBlock);
            StorageFile file = rootPage.sampleFile;
            if (file != null)
            {
                if (MRURadioButton.IsChecked.Value)
                {
                    rootPage.mruToken = StorageApplicationPermissions.MostRecentlyUsedList.Add(file, file.Name);
                    OutputTextBlock.Text = "The file '" + file.Name + "' was added to the MRU list and a token was stored.";
                }
                else if (FALRadioButton.IsChecked.Value)
                {
                    rootPage.falToken = StorageApplicationPermissions.FutureAccessList.Add(file, file.Name);
                    OutputTextBlock.Text = "The file '" + file.Name + "' was added to the FAL list and a token was stored.";
                }
            }
        }

        private void ShowListButton_Click(object sender, RoutedEventArgs e)
        {
            rootPage.ResetScenarioOutput(OutputTextBlock);
            StorageFile file = rootPage.sampleFile;
            if (file != null)
            {
                if (MRURadioButton.IsChecked.Value)
                {
                    AccessListEntryView entries = StorageApplicationPermissions.MostRecentlyUsedList.Entries;
                    if (entries.Count > 0)
                    {
                        StringBuilder outputText = new StringBuilder("The MRU list contains the following item(s):" + Environment.NewLine + Environment.NewLine);
                        foreach (AccessListEntry entry in entries)
                        {
                            outputText.AppendLine(entry.Metadata); // Application previously chose to store file.Name in this field
                        }

                        OutputTextBlock.Text = outputText.ToString();
                    }
                    else
                    {
                        OutputTextBlock.Text = "The MRU list is empty, please select 'Most Recently Used' list and click 'Add to List' to add a file to the MRU list.";
                    }
                }
                else if (FALRadioButton.IsChecked.Value)
                {
                    AccessListEntryView entries = StorageApplicationPermissions.FutureAccessList.Entries;
                    if (entries.Count > 0)
                    {
                        StringBuilder outputText = new StringBuilder("The FAL list contains the following item(s):" + Environment.NewLine + Environment.NewLine);
                        foreach (AccessListEntry entry in entries)
                        {
                            outputText.AppendLine(entry.Metadata); // Application previously chose to store file.Name in this field
                        }

                        OutputTextBlock.Text = outputText.ToString();
                    }
                    else
                    {
                        OutputTextBlock.Text = "The FAL list is empty, please select 'Future Access List' list and click 'Add to List' to add a file to the FAL list.";
                    }
                }
            }
        }

        private async void OpenFromListButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                rootPage.ResetScenarioOutput(OutputTextBlock);
                if (rootPage.sampleFile != null)
                {
                    if (MRURadioButton.IsChecked.Value)
                    {
                        if (rootPage.mruToken != null)
                        {
                            // Open the file via the token that was stored when adding this file into the MRU list
                            StorageFile file = await StorageApplicationPermissions.MostRecentlyUsedList.GetFileAsync(rootPage.mruToken);

                            // Read the file
                            string fileContent = await FileIO.ReadTextAsync(file);
                            OutputTextBlock.Text = "The file '" + file.Name + "' was opened by a stored token from the MRU list, it contains the following text:" + Environment.NewLine + Environment.NewLine + fileContent;
                        }
                        else
                        {
                            OutputTextBlock.Text = "The MRU list is empty, please select 'Most Recently Used' list and click 'Add to List' to add a file to the MRU list.";
                        }
                    }
                    else if (FALRadioButton.IsChecked.Value)
                    {
                        if (rootPage.falToken != null)
                        {
                            // Open the file via the token that was stored when adding this file into the FAL list
                            StorageFile file = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(rootPage.falToken);

                            // Read the file
                            string fileContent = await FileIO.ReadTextAsync(file);
                            OutputTextBlock.Text = "The file '" + file.Name + "' was opened by a stored token from the FAL list, it contains the following text:" + Environment.NewLine + Environment.NewLine + fileContent;
                        }
                        else
                        {
                            OutputTextBlock.Text = "The FAL list is empty, please select 'Future Access List' list and click 'Add to List' to add a file to the FAL list.";
                        }
                    }
                }
            }
            catch (FileNotFoundException)
            {
                rootPage.NotifyUserFileNotExist();
            }
        }
    }
}
