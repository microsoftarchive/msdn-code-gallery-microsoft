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
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace FileAccess
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario3 : SDKTemplate.Common.LayoutAwarePage
    {
        MainPage rootPage = MainPage.Current;

        public Scenario3()
        {
            this.InitializeComponent();
            WriteBytesButton.Click += new RoutedEventHandler(WriteBytesButton_Click);
            ReadBytesButton.Click += new RoutedEventHandler(ReadBytesButton_Click);
        }

        private IBuffer GetBufferFromString(String str)
        {
            using (InMemoryRandomAccessStream memoryStream = new InMemoryRandomAccessStream())
            {
                using (DataWriter dataWriter = new DataWriter(memoryStream))
                {
                    dataWriter.WriteString(str);
                    return dataWriter.DetachBuffer();
                }
            }
        }

        private async void WriteBytesButton_Click(object sender, RoutedEventArgs e)
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
                        IBuffer buffer = GetBufferFromString(userContent);
                        await FileIO.WriteBufferAsync(file, buffer);
                        OutputTextBlock.Text = "The following " + buffer.Length + " bytes of text were written to '" + file.Name + "':" + Environment.NewLine + Environment.NewLine + userContent;
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

        private async void ReadBytesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                rootPage.ResetScenarioOutput(OutputTextBlock);
                StorageFile file = rootPage.sampleFile;
                if (file != null)
                {
                    IBuffer buffer = await FileIO.ReadBufferAsync(file);
                    using (DataReader dataReader = DataReader.FromBuffer(buffer))
                    {
                        string fileContent = dataReader.ReadString(buffer.Length);
                        OutputTextBlock.Text = "The following " + buffer.Length + " bytes of text were read from '" + file.Name + "':" + Environment.NewLine + Environment.NewLine + fileContent;
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
