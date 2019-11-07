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
    public sealed partial class Scenario2 : SDKTemplate.Common.LayoutAwarePage
    {
        MainPage rootPage = MainPage.Current;

        public Scenario2()
        {
            this.InitializeComponent();
            WriteTextButton.Click += new RoutedEventHandler(WriteTextButton_Click);
            ReadTextButton.Click += new RoutedEventHandler(ReadTextButton_Click);
        }

        private async void WriteTextButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                rootPage.ResetScenarioOutput(OutputTextBlock);
                StorageFile file = rootPage.sampleFile;
                if (file != null)
                {
                    string userContent = InputTextBox.Text;
                    if (!String.IsNullOrEmpty(userContent))
                    {
                        await FileIO.WriteTextAsync(file, userContent);
                        OutputTextBlock.Text = "The following text was written to '" + file.Name + "':" + Environment.NewLine + Environment.NewLine + userContent;
                    }
                    else
                    {
                        OutputTextBlock.Text = "The text box is empty, please write something and then click 'Write' again.";
                    }
                }
            }
            catch (FileNotFoundException)
            {
                rootPage.NotifyUserFileNotExist();
            }
        }

        private async void ReadTextButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                rootPage.ResetScenarioOutput(OutputTextBlock);
                StorageFile file = rootPage.sampleFile;
                if (file != null)
                {
                    string fileContent = await FileIO.ReadTextAsync(file);
                    OutputTextBlock.Text = "The following text was read from '" + file.Name + "':" + Environment.NewLine + Environment.NewLine + fileContent;
                }
            }
            catch (FileNotFoundException)
            {
                rootPage.NotifyUserFileNotExist();
            }
        }
    }
}
