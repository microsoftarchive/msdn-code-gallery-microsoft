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
using System.IO;
using Windows.Security.EnterpriseData;
using Windows.Storage;

namespace FileRevocation
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class S5_Cleanup : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage RootPage = MainPage.Current;

        public S5_Cleanup()
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
        /// Delete the 'sample' file and folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Cleanup_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (null != RootPage.SampleFolder)
                {
                    Windows.Storage.Search.StorageItemQueryResult FolderStorageQuery = RootPage.SampleFolder.CreateItemQuery();
                    uint FolderItems = await FolderStorageQuery.GetItemCountAsync();
                    if (FolderItems > 0)
                    {
                        RootPage.NotifyUser("You need to delete the items inside the " + RootPage.SampleFolder.Name + " folder in order to delete the folder.", NotifyType.ErrorMessage);
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
                        RootPage.NotifyUser("You need to delete the items inside the " + RootPage.TargetFolder.Name + " folder in order to delete the folder.", NotifyType.ErrorMessage);
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


                RootPage.NotifyUser("The files " + MainPage.SampleFilename + " and " + MainPage.TargetFilename + " were deleted.\n" +
                                    "The folders " + MainPage.SampleFoldername + " and " + MainPage.TargetFoldername + " were deleted.",
                                    NotifyType.StatusMessage);
            }
            catch (FileNotFoundException)
            {
                RootPage.NotifyUserFileNotExist();
            }
        }
    }
}
