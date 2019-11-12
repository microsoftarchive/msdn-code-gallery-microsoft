//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System;
using System.Collections.Generic;
using System.IO;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace SDKTemplate
{
    public partial class MainPage : SDKTemplate.Common.LayoutAwarePage
    {
        public const string FEATURE_NAME = "File access C# sample";

        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "Creating a file",                                    ClassType = typeof(FileAccess.Scenario1) },
            new Scenario() { Title = "Writing and reading text in a file",                 ClassType = typeof(FileAccess.Scenario2) },
            new Scenario() { Title = "Writing and reading bytes in a file",                ClassType = typeof(FileAccess.Scenario3) },
            new Scenario() { Title = "Writing and reading using a stream",                 ClassType = typeof(FileAccess.Scenario4) },
            new Scenario() { Title = "Displaying file properties",                         ClassType = typeof(FileAccess.Scenario5) },
            new Scenario() { Title = "Persisting access to a storage item for future use", ClassType = typeof(FileAccess.Scenario6) },
            new Scenario() { Title = "Copying a file",                                     ClassType = typeof(FileAccess.Scenario7) },
            new Scenario() { Title = "Deleting a file",                                    ClassType = typeof(FileAccess.Scenario8) },
        };

        public const string filename = "sample.dat";
        public StorageFile sampleFile = null;
        public string mruToken = null;
        public string falToken = null;

        /// <summary>
        /// Checks if sample file already exists, if it does assign it to sampleFile
        /// </summary>
        private async void Initialize()
        {
            try
            {
                sampleFile = await Windows.Storage.KnownFolders.DocumentsLibrary.GetFileAsync(filename);
            }
            catch (FileNotFoundException)
            {
                // sample file doesn't exist so scenario one must be run
            }
        }

        private void ValidateFile(Type scenarioClass)
        {
            if (scenarioClass != typeof(FileAccess.Scenario1) && sampleFile == null)
            {
                NotifyUserFileNotExist();
            }
        }

        internal void ResetScenarioOutput(TextBlock output)
        {
            // clear Error/Status
            NotifyUser("", NotifyType.ErrorMessage);
            NotifyUser("", NotifyType.StatusMessage);
            // clear scenario output
            output.Text = "";
        }

        internal void NotifyUserFileNotExist()
        {
            NotifyUser(String.Format("The file '{0}' does not exist. Use scenario one to create this file.", filename), NotifyType.ErrorMessage);
        }
    }

    public class Scenario
    {
        public string Title { get; set; }

        public Type ClassType { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }
}
