//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;
using System.Text;
using System.IO;
using Windows.Storage;
using Windows.Security.EnterpriseData;

namespace FileRevocation
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class S3_GetStatus : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage RootPage = MainPage.Current;

        public S3_GetStatus()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        /// <summary>
        /// Get status of the file and folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void GetStatus_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((null == RootPage.SampleFile)
                    || (null == RootPage.TargetFile)
                    || (null == RootPage.SampleFolder)
                    || (null == RootPage.TargetFolder))
                {
                    RootPage.NotifyUser("You need to click the Setup button in the Protect a file or folder with an Enterprise Identity scenario.", NotifyType.ErrorMessage);
                    return;
                }

                FileProtectionStatus SampleFileProtectionStatus = await FileRevocationManager.GetStatusAsync(RootPage.SampleFile);
                FileProtectionStatus TargetFileProtectionStatus = await FileRevocationManager.GetStatusAsync(RootPage.TargetFile);
                FileProtectionStatus SampleFolderProtectionStatus = await FileRevocationManager.GetStatusAsync(RootPage.SampleFolder);
                FileProtectionStatus TargetFolderProtectionStatus = await FileRevocationManager.GetStatusAsync(RootPage.TargetFolder);

                RootPage.NotifyUser("The protection status of the file " + RootPage.SampleFile.Name + " is " + SampleFileProtectionStatus + ".\n" +
                                    "The protection status of the file " + RootPage.TargetFile.Name + " is " + TargetFileProtectionStatus + ".\n" +
                                    "The protection status of the folder " + RootPage.SampleFolder.Name + " is " + SampleFolderProtectionStatus + ".\n" +
                                    "The protection status of the folder " + RootPage.TargetFolder.Name + " is " + TargetFolderProtectionStatus + ".",
                                    NotifyType.StatusMessage);
            }
            catch (FileNotFoundException)
            {
                RootPage.NotifyUserFileNotExist();
            }
        }
    }
}
