//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using System.Collections.Generic;
using System;
using Windows.Security.EnterpriseData;
using Windows.Storage;
using FileRevocation;

namespace SDKTemplate
{
    public partial class MainPage : SDKTemplate.Common.LayoutAwarePage
    {
        // Change the string below to reflect the name of your sample.
        // This is used on the main page as the title of the sample.
        public const string FEATURE_NAME = "File Revocation Manager";

        // Change the array below to reflect the name of your scenarios.
        // This will be used to populate the list of scenarios on the main page with
        // which the user will choose the specific scenario that they are interested in.
        // These should be in the form: "Navigating to a web page".
        // The code in MainPage will take care of turning this into: "1) Navigating to a web page"
        List<Scenario> scenarios = new List<Scenario>

        {
            new Scenario() { Title = "Protect a file or folder with an enterprise identity", ClassType = typeof(S1_Protect) },
            new Scenario() { Title = "Copy enterprise protection", ClassType = typeof(S2_CopyProtection) },
            new Scenario() { Title = "Get the protection status of the files and folders", ClassType = typeof(S3_GetStatus) },
            new Scenario() { Title = "Revoke an enterprise identity", ClassType = typeof(S4_Revoke) },
            new Scenario() { Title = "Cleanup the files and folders", ClassType = typeof(S5_Cleanup) }
        };

        public const string PickedFolderToken = "PickedFolderToken";
        public StorageFolder PickedFolder = null;
        public const string SampleFilename = "RevokeSample.txt";
        public const string SampleFoldername = "RevokeSample";
        public StorageFile SampleFile = null;
        public StorageFolder SampleFolder = null;
        public const string TargetFilename = "RevokeTarget.txt";
        public const string TargetFoldername = "RevokeTarget";
        public StorageFile TargetFile = null;
        public StorageFolder TargetFolder = null;

        internal void NotifyUserFileNotExist()
        {
            NotifyUser("A file or folder used by the application does not exist.\n" +
                        "Please try again after clicking the Setup Button in the Protect a file or folder with an Enterprise Identity scenario.",
                        NotifyType.ErrorMessage);
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
