//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;
using System.Text;
using System.IO;
using Windows.Storage;
using Windows.Security.EnterpriseData;
using System.Collections.Generic;

namespace FileRevocation
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class S2_CopyProtection : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage RootPage = MainPage.Current;

        public S2_CopyProtection()
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
        /// Copy the protection from the source file to the target file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CopyProtectionToFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((null == RootPage.SampleFile) || (null == RootPage.TargetFile))
                {
                    RootPage.NotifyUser("You need to click the Setup button in the Protect a file or folder with an Enterprise Identity scenario.", NotifyType.ErrorMessage);
                    return;
                }

                bool IsProtectionCopied = await FileRevocationManager.CopyProtectionAsync(RootPage.SampleFile, RootPage.TargetFile);

                // Get the target file protection status
                FileProtectionStatus TargetProtectionStatus = await FileRevocationManager.GetStatusAsync(RootPage.TargetFile);

                if (!IsProtectionCopied)
                {
                    // Make sure the source file is protected
                    FileProtectionStatus SourceProtectionStatus = await FileRevocationManager.GetStatusAsync(RootPage.SampleFile);
                    if (FileProtectionStatus.Protected != SourceProtectionStatus)
                    {
                        RootPage.NotifyUser("The protection cannot be copied since the status of the source file " + RootPage.SampleFile.Name + " is " + SourceProtectionStatus + ".\n" +
                                            "Please try again after clicking the Setup Button followed by the Protect File button in the Protect a file or folder with an Enterprise Identity scenario.",
                                            NotifyType.ErrorMessage);
                        return;
                    }

                    // Check the target file protection status
                    if (FileProtectionStatus.Protected == TargetProtectionStatus)
                    {
                        RootPage.NotifyUser("The protection cannot be copied since the target file " + RootPage.TargetFile.Name + " is already protected by another Enterprise Identity.\n" +
                                            "Please try again after clicking the Setup Button followed by the Protect File button in the Protect a file or folder with an Enterprise Identity scenario.",
                                            NotifyType.ErrorMessage);
                        return;
                    }
                    else
                    {
                        RootPage.NotifyUser("The protection cannot be copied since the status of the target file " + RootPage.TargetFile.Name + " is " + TargetProtectionStatus + ".\n" +
                                            "Please try again after clicking the Setup Button followed by the Protect File button in the Protect a file or folder with an Enterprise Identity scenario.",
                                            NotifyType.ErrorMessage);
                        return;
                    }
                }

                RootPage.NotifyUser("The protection was copied.\n" +
                                    "The protection status of the target file " + RootPage.TargetFile.Name + " is " + TargetProtectionStatus + ".\n",
                                    NotifyType.StatusMessage);
            }
            catch (FileNotFoundException)
            {
                RootPage.NotifyUserFileNotExist();
            }
        }

        /// <summary>
        /// Copy the protection from the source folder to the target folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CopyProtectionToFolder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((null == RootPage.SampleFolder) || (null == RootPage.TargetFolder))
                {
                    RootPage.NotifyUser("You need to click the Setup button in the Protect a file or folder with an Enterprise Identity scenario.", NotifyType.ErrorMessage);
                    return;
                }

                // Make sure the folder is empty before you protect it
                Windows.Storage.Search.StorageItemQueryResult StorageQuery = RootPage.TargetFolder.CreateItemQuery();
                uint Items = await StorageQuery.GetItemCountAsync();
                if (Items > 0)
                {
                    RootPage.NotifyUser("You need to empty the " + RootPage.TargetFolder.Name + " before you can protect it.", NotifyType.ErrorMessage);
                    return;
                }

                bool IsProtectionCopied = await FileRevocationManager.CopyProtectionAsync(RootPage.SampleFolder, RootPage.TargetFolder);

                // Get the target folder protection status
                FileProtectionStatus TargetProtectionStatus = await FileRevocationManager.GetStatusAsync(RootPage.TargetFolder);

                if (!IsProtectionCopied)
                {
                    // Make sure the source folder is protected
                    FileProtectionStatus SourceProtectionStatus = await FileRevocationManager.GetStatusAsync(RootPage.SampleFolder);
                    if (FileProtectionStatus.Protected != SourceProtectionStatus)
                    {
                        RootPage.NotifyUser("The protection cannot be copied since the status of the source folder " + RootPage.SampleFolder.Name + " is " + SourceProtectionStatus + ".\n" +
                                            "Please try again after clicking the Setup Button followed by the Protect Folder button in the Protect a file or folder with an Enterprise Identity scenario.",
                                            NotifyType.ErrorMessage);
                        return;
                    }

                    // Check the target folder protection status
                    if (FileProtectionStatus.Protected == TargetProtectionStatus)
                    {
                        RootPage.NotifyUser("The protection cannot be copied since the target folder " + RootPage.TargetFolder.Name + " is already protected by another Enterprise Identity.\n" +
                                            "Please try again after clicking the Setup Button followed by the Protect Folder button in the Protect a file or folder with an Enterprise Identity scenario.",
                                            NotifyType.ErrorMessage);
                        return;
                    }
                    else
                    {
                        RootPage.NotifyUser("The protection cannot be copied since the status of the target folder " + RootPage.TargetFolder.Name + " is " + TargetProtectionStatus + ".\n" +
                                            "Please try again after clicking the Setup Button followed by the Protect Folder button in the Protect a file or folder with an Enterprise Identity scenario.",
                                            NotifyType.ErrorMessage);
                        return;
                    }
                }

                RootPage.NotifyUser("The protection was copied.\n" +
                                    "The protection status of the target folder " + RootPage.TargetFolder.Name + " is " + TargetProtectionStatus + ".\n",
                                    NotifyType.StatusMessage);
            }
            catch (FileNotFoundException)
            {
                RootPage.NotifyUserFileNotExist();
            }
        }
    }
}
