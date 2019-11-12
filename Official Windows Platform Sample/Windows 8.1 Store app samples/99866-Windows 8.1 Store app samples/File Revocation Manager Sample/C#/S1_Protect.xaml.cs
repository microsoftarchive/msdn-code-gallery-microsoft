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
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.Security.EnterpriseData;
using System.Collections.Generic;

namespace FileRevocation
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class S1_Protect : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage RootPage = MainPage.Current;

        public S1_Protect()
        {
            this.InitializeComponent();
            Initialize();
        }

        public async void Initialize()
        {
            try
            {
                if (Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.ContainsItem(MainPage.PickedFolderToken))
                {
                    RootPage.PickedFolder = null;

                    RootPage.PickedFolder = await Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.GetFolderAsync(MainPage.PickedFolderToken);

                    RootPage.SampleFile = null;
                    RootPage.TargetFile = null;

                    RootPage.SampleFolder = null;
                    RootPage.TargetFolder = null;

                    RootPage.SampleFile = await RootPage.PickedFolder.CreateFileAsync(MainPage.SampleFilename, CreationCollisionOption.OpenIfExists);
                    RootPage.TargetFile = await RootPage.PickedFolder.CreateFileAsync(MainPage.TargetFilename, CreationCollisionOption.OpenIfExists);

                    RootPage.SampleFolder = await RootPage.PickedFolder.CreateFolderAsync(MainPage.SampleFoldername, CreationCollisionOption.OpenIfExists);
                    RootPage.TargetFolder = await RootPage.PickedFolder.CreateFolderAsync(MainPage.TargetFoldername, CreationCollisionOption.OpenIfExists);
                }
            }
            catch (FileNotFoundException)
            {
                RootPage.NotifyUserFileNotExist();
            }
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
        /// Create files and folders
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Setup_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (null != RootPage.SampleFolder)
                {
                    Windows.Storage.Search.StorageItemQueryResult FolderStorageQuery = RootPage.SampleFolder.CreateItemQuery();
                    uint FolderItems = await FolderStorageQuery.GetItemCountAsync();
                    if (FolderItems > 0)
                    {
                        RootPage.NotifyUser("You need to delete the items inside the " + RootPage.SampleFolder.Name + " folder in order to regenerate the folder.", NotifyType.ErrorMessage);
                        return;
                    }

                    await RootPage.SampleFolder.DeleteAsync();
                    RootPage.SampleFolder = null;
                }

                if (null != RootPage.TargetFolder)
                {
                    Windows.Storage.Search.StorageItemQueryResult FolderStorageQuery = RootPage.TargetFolder.CreateItemQuery();
                    uint FolderItems = await FolderStorageQuery.GetItemCountAsync();
                    if (FolderItems > 0)
                    {
                        RootPage.NotifyUser("You need to delete the items inside the " + RootPage.TargetFolder.Name + " folder in order to regenerate the folder.", NotifyType.ErrorMessage);
                        return;
                    }

                    await RootPage.TargetFolder.DeleteAsync();
                    RootPage.TargetFolder = null;
                }

                if (null != RootPage.SampleFile)
                {
                    await RootPage.SampleFile.DeleteAsync();
                    RootPage.SampleFile = null;
                }
                
                if (null != RootPage.TargetFile)
                {
                    await RootPage.TargetFile.DeleteAsync();
                    RootPage.TargetFile = null;
                }

                if (null != RootPage.PickedFolder)
                {
                    RootPage.PickedFolder = null;
                }

                Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Clear();

                FolderPicker folderPicker = new FolderPicker();
                folderPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                folderPicker.FileTypeFilter.Add(".docx");
                folderPicker.FileTypeFilter.Add(".xlsx");
                folderPicker.FileTypeFilter.Add(".pptx");
                folderPicker.FileTypeFilter.Add(".txt");
                RootPage.PickedFolder = await folderPicker.PickSingleFolderAsync();
                if (null == RootPage.PickedFolder)
                {
                    RootPage.NotifyUser("Please choose a base folder in which to create the SDK Sample related files and folders by clicking the Setup button.", NotifyType.ErrorMessage);
                    return;
                }

                StorageApplicationPermissions.FutureAccessList.AddOrReplace(MainPage.PickedFolderToken, RootPage.PickedFolder);

                RootPage.SampleFolder = await RootPage.PickedFolder.CreateFolderAsync(MainPage.SampleFoldername, CreationCollisionOption.ReplaceExisting);

                RootPage.TargetFolder = await RootPage.PickedFolder.CreateFolderAsync(MainPage.TargetFoldername, CreationCollisionOption.ReplaceExisting);

                RootPage.SampleFile = await RootPage.PickedFolder.CreateFileAsync(MainPage.SampleFilename, CreationCollisionOption.ReplaceExisting);

                RootPage.TargetFile = await RootPage.PickedFolder.CreateFileAsync(MainPage.TargetFilename, CreationCollisionOption.ReplaceExisting);

                RootPage.NotifyUser("The files " + RootPage.SampleFile.Name + " and " + RootPage.TargetFile.Name + " were created.\n" +
                                    "The folders " + RootPage.SampleFolder.Name + " and " + RootPage.TargetFolder.Name + " were created.",
                                    NotifyType.StatusMessage);
            }
            catch (FileNotFoundException)
            {
                RootPage.NotifyUserFileNotExist();
            }
        }

        /// <summary>
        /// Protect the file with enterprise id that user entered
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ProtectFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (null == RootPage.SampleFile)
                {
                    RootPage.NotifyUser("You need to click the Setup button first.", NotifyType.ErrorMessage);
                    return;
                }
            
                if ("" == InputTextBox.Text)
                {
                    RootPage.NotifyUser("Please enter an Enterpise ID that you want to use.", NotifyType.ErrorMessage);
                    return;
                }

                FileProtectionStatus ProtectionStatus = await FileRevocationManager.ProtectAsync(RootPage.SampleFile, InputTextBox.Text);

                RootPage.NotifyUser("The protection status of the file " + RootPage.SampleFile.Name + " is " + ProtectionStatus + ".\n", NotifyType.StatusMessage);
            }
            catch (FileNotFoundException)
            {
                RootPage.NotifyUserFileNotExist();
            }

            //
            // NOTE: Generally you should not rely on exception handling
            // to validate an Enterprise ID string. In real-world
            // applications, the domain name of the enterprise might be
            // parsed out of an email address or a URL, and may even be
            // entered by a user. Your app-specific code to extract the
            // Enterprise ID should validate the Enterprise ID string is an
            // internationalized domain name before passing it to 
            // ProtectAsync.
            //

            catch (ArgumentException)
            {
                RootPage.NotifyUser("Given Enterprise ID string is invalid.\n" +
                                    "Please try again using a properly formatted Internationalized Domain Name as the Enterprise ID string.",
                                    NotifyType.ErrorMessage);
            }
        }

        /// <summary>
        /// Protect the folder with enterprise id that user entered
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ProtectFolder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (null == RootPage.SampleFolder)
                {
                    RootPage.NotifyUser("You need to click the Setup button first.", NotifyType.ErrorMessage);
                    return;
                }
            
                if ("" == InputTextBox.Text)
                {
                    RootPage.NotifyUser("Please enter an Enterpise ID that you want to use.", NotifyType.ErrorMessage);
                    return;
                }

                // Make sure the folder is empty before you protect it
                Windows.Storage.Search.StorageItemQueryResult StorageQuery = RootPage.SampleFolder.CreateItemQuery();
                uint Items = await StorageQuery.GetItemCountAsync();
                if (Items > 0)
                {
                    RootPage.NotifyUser("You need to empty the " + RootPage.SampleFolder.Name + " before you can protect it.", NotifyType.ErrorMessage);
                    return;
                }

                FileProtectionStatus ProtectionStatus = await FileRevocationManager.ProtectAsync(RootPage.SampleFolder, InputTextBox.Text);

                RootPage.NotifyUser("The protection status of the folder " + RootPage.SampleFolder.Name + " is " + ProtectionStatus + ".\n", NotifyType.StatusMessage);
            }
            catch (FileNotFoundException)
            {
                RootPage.NotifyUserFileNotExist();
            }

            //
            // NOTE: Generally you should not rely on exception handling
            // to validate an Enterprise ID string. In real-world
            // applications, the domain name of the enterprise might be
            // parsed out of an email address or a URL, and may even be
            // entered by a user. Your app-specific code to extract the
            // Enterprise ID should validate the Enterprise ID string is an
            // internationalized domain name before passing it to 
            // ProtectAsync.
            //

            catch (ArgumentException)
            {
                RootPage.NotifyUser("Given Enterprise ID string is invalid.\n" +
                                    "Please try again using a properly formatted Internationalized Domain Name as the Enterprise ID string.",
                                    NotifyType.ErrorMessage);
            }
        }
    }
}
