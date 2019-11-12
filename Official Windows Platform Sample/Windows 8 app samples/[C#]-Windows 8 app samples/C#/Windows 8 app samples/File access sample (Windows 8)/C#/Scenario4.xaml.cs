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
    public sealed partial class Scenario4 : SDKTemplate.Common.LayoutAwarePage
    {
        MainPage rootPage = MainPage.Current;

        public Scenario4()
        {
            this.InitializeComponent();
            WriteToStreamButton.Click += new RoutedEventHandler(WriteToStreamButton_Click);
            ReadFromStreamButton.Click += new RoutedEventHandler(ReadFromStreamButton_Click);
        }

        private async void WriteToStreamButton_Click(object sender, RoutedEventArgs e)
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
                        using (StorageStreamTransaction transaction = await file.OpenTransactedWriteAsync())
                        {
                            using (DataWriter dataWriter = new DataWriter(transaction.Stream))
                            {
                                dataWriter.WriteString(userContent);
                                transaction.Stream.Size = await dataWriter.StoreAsync(); // reset stream size to override the file
                                await transaction.CommitAsync();
                                OutputTextBlock.Text = "The following text was written to '" + file.Name + "' using a stream:" + Environment.NewLine + Environment.NewLine + userContent;
                            }
                        }
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

        private async void ReadFromStreamButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                rootPage.ResetScenarioOutput(OutputTextBlock);
                StorageFile file = rootPage.sampleFile;
                if (file != null)
                {
                    using (IRandomAccessStream readStream = await file.OpenAsync(FileAccessMode.Read))
                    {
                        using (DataReader dataReader = new DataReader(readStream))
                        {
                            UInt64 size = readStream.Size;
                            if (size <= UInt32.MaxValue)
                            {
                                UInt32 numBytesLoaded = await dataReader.LoadAsync((UInt32)size);
                                string fileContent = dataReader.ReadString(numBytesLoaded);
                                OutputTextBlock.Text = "The following text was read from '" + file.Name + "' using a stream:" + Environment.NewLine + Environment.NewLine + fileContent;
                            }
                            else
                            {
                                OutputTextBlock.Text = "File " + file.Name + " is too big for LoadAsync to load in a single chunk. Files larger than 4GB need to be broken into multiple chunks to be loaded by LoadAsync.";
                            }
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
