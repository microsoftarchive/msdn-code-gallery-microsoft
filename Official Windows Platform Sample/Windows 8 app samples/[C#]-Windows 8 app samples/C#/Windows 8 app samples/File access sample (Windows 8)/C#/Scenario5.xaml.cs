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
using Windows.Storage.FileProperties;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace FileAccess
{
    public sealed partial class Scenario5 : SDKTemplate.Common.LayoutAwarePage
    {
        MainPage rootPage = MainPage.Current;

        static readonly string dateAccessedProperty = "System.DateAccessed";
        static readonly string fileOwnerProperty    = "System.FileOwner";

        public Scenario5()
        {
            this.InitializeComponent();
            ShowPropertiesButton.Click += new RoutedEventHandler(ShowPropertiesButton_Click);
        }

        private async void ShowPropertiesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                rootPage.ResetScenarioOutput(OutputTextBlock);
                StorageFile file = rootPage.sampleFile;
                if (file != null)
                {
                    // Get top level file properties
                    StringBuilder outputText = new StringBuilder();
                    outputText.AppendLine("File name: " + file.Name);
                    outputText.AppendLine("File type: " + file.FileType);

                    // Get basic properties
                    BasicProperties basicProperties = await file.GetBasicPropertiesAsync();
                    outputText.AppendLine("File size: " + basicProperties.Size + " bytes");
                    outputText.AppendLine("Date modified: " + basicProperties.DateModified);

                    // Get extra properties
                    List<string> propertiesName = new List<string>();
                    propertiesName.Add(dateAccessedProperty);
                    propertiesName.Add(fileOwnerProperty);
                    IDictionary<string, object> extraProperties = await file.Properties.RetrievePropertiesAsync(propertiesName);
                    var propValue = extraProperties[dateAccessedProperty];
                    if (propValue != null)
                    {
                        outputText.AppendLine("Date accessed: " + propValue);
                    }
                    propValue = extraProperties[fileOwnerProperty];
                    if (propValue != null)
                    {
                        outputText.AppendLine("File onwer: " + propValue);
                    }

                    OutputTextBlock.Text = outputText.ToString();
                }
            }
            catch (FileNotFoundException)
            {
                rootPage.NotifyUserFileNotExist();
            }
        }
    }
}
