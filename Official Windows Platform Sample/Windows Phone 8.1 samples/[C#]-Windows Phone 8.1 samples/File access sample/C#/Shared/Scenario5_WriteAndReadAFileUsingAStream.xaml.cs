// Copyright (c) Microsoft. All rights reserved.

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
    /// Writing and reading using a stream.
    /// </summary>
    public sealed partial class Scenario5 : Page
    {
        MainPage rootPage;

        public Scenario5()
        {
            this.InitializeComponent();
            WriteToStreamButton.Click += new RoutedEventHandler(WriteToStreamButton_Click);
            ReadFromStreamButton.Click += new RoutedEventHandler(ReadFromStreamButton_Click);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            rootPage = MainPage.Current;
            rootPage.ValidateFile();
        }

        private async void WriteToStreamButton_Click(object sender, RoutedEventArgs e)
        {
            StorageFile file = rootPage.sampleFile;
            if (file != null)
            {
                try
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
                                rootPage.NotifyUser(String.Format("The following text was written to '{0}' using a stream:{1}{2}", file.Name, Environment.NewLine, userContent), NotifyType.StatusMessage);
                            }
                        }
                    }
                    else
                    {
                        rootPage.NotifyUser("The text box is empty, please write something and then click 'Write' again.", NotifyType.ErrorMessage);
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

        private async void ReadFromStreamButton_Click(object sender, RoutedEventArgs e)
        {
            StorageFile file = rootPage.sampleFile;
            if (file != null)
            {
                try
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
                                rootPage.NotifyUser(String.Format("The following text was read from '{0}' using a stream:{1}{2}", file.Name, Environment.NewLine, fileContent), NotifyType.StatusMessage);
                            }
                            else
                            {
                                rootPage.NotifyUser(String.Format("File {0} is too big for LoadAsync to load in a single chunk. Files larger than 4GB need to be broken into multiple chunks to be loaded by LoadAsync.", file.Name), NotifyType.ErrorMessage);
                            }
                        }
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
