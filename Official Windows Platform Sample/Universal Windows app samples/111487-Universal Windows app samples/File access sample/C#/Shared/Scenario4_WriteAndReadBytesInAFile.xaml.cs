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
    /// Writing and reading bytes in a file.
    /// </summary>
    public sealed partial class Scenario4 : Page
    {
        MainPage rootPage;

        public Scenario4()
        {
            this.InitializeComponent();
            WriteBytesButton.Click += new RoutedEventHandler(WriteBytesButton_Click);
            ReadBytesButton.Click += new RoutedEventHandler(ReadBytesButton_Click);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            rootPage = MainPage.Current;
            rootPage.ValidateFile();
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
            StorageFile file = rootPage.sampleFile;
            if (file != null)
            {
                try
                {
                    string userContent = InputTextBox.Text;
                    if (!String.IsNullOrEmpty(userContent))
                    {
                        IBuffer buffer = GetBufferFromString(userContent);
                        await FileIO.WriteBufferAsync(file, buffer);
                        rootPage.NotifyUser(String.Format("The following {0} bytes of text were written to '{1}':{2}{3}", buffer.Length, file.Name, Environment.NewLine, userContent), NotifyType.StatusMessage);
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

        private async void ReadBytesButton_Click(object sender, RoutedEventArgs e)
        {
            StorageFile file = rootPage.sampleFile;
            if (file != null)
            {
                try
                {
                    IBuffer buffer = await FileIO.ReadBufferAsync(file);
                    using (DataReader dataReader = DataReader.FromBuffer(buffer))
                    {
                        string fileContent = dataReader.ReadString(buffer.Length);
                        rootPage.NotifyUser(String.Format("The following {0} bytes of text were read from '{1}':{2}{3}", buffer.Length, file.Name, Environment.NewLine, fileContent), NotifyType.StatusMessage);
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
