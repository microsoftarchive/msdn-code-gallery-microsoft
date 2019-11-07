//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;
using System.Collections.Generic;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Storage.Search;
using Windows.UI.Xaml.Media;

namespace MediaServerClient
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario1 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;
        IReadOnlyList<StorageFolder> mediaServers = null;
        IReadOnlyList<StorageFile> mediaFiles = null;
        StorageFile mediaFile = null;
        public Scenario1()
        {
            this.InitializeComponent();
            this.InitilizeMediaServers();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void dmsRefreshButton_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (b != null)
            {
                rootPage.NotifyUser("Media Servers being refreshed... ", NotifyType.StatusMessage);
                InitilizeMediaServers();
            }
        }

        async private void InitilizeMediaServers()
        {
            try
            {
                dmsSelect.Items.Clear();
                mediaServers = await KnownFolders.MediaServerDevices.GetFoldersAsync();

                if (mediaServers.Count == 0)
                {
                    rootPage.NotifyUser("No MediaServers found", NotifyType.StatusMessage);
                }
                else
                {
                    foreach (StorageFolder server in mediaServers)
                    {
                        dmsSelect.Items.Add(server.DisplayName);
                    }
                    rootPage.NotifyUser("Media Servers refreshed", NotifyType.StatusMessage);
                }
            }
            catch (Exception ecp)
            {
                rootPage.NotifyUser("Error querying Media Servers :" + ecp.Message, NotifyType.ErrorMessage);
            }
        }

        private void dmsSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            localVideo.Stop();
            mediaSelect.Items.Clear();

            if (dmsSelect.SelectedIndex != -1)
            {
                rootPage.NotifyUser("Retrieving media files ...", NotifyType.StatusMessage);
                LoadMediaFiles();
            }
        }

        async private void mediaSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                localVideo.Stop();
                if (mediaSelect.SelectedIndex != -1 && mediaFiles.Count != 0)
                {
                    mediaFile = mediaFiles[mediaSelect.SelectedIndex];
                    var stream = await mediaFile.OpenAsync(FileAccessMode.Read);
                    localVideo.SetSource(stream, mediaFile.ContentType);
                    localVideo.Play();
                }
            }
            catch (Exception ecp)
            {
                rootPage.NotifyUser("Error during file selection :" + ecp.Message, NotifyType.ErrorMessage);
            }
        }

        async private System.Threading.Tasks.Task<uint> BrowseForVideoFiles(StorageFolder folder, List<StorageFile> collectedMediaItems, uint maxFilesToRetrieve)
        {
            uint cVideoFiles = 0;
            if (maxFilesToRetrieve > 0)
            {
                var files = await folder.GetFilesAsync(CommonFileQuery.DefaultQuery, 0, maxFilesToRetrieve);
                foreach (var file in files)
                {
                    if (file.ContentType.Length > 5 && file.ContentType.Substring(0, 5).ToLower() == "video")
                    {
                        ++cVideoFiles;
                        mediaSelect.Items.Add(file.DisplayName);
                        collectedMediaItems.Add(file);
                    }
                }

                var folders = await folder.GetFoldersAsync();
                foreach (var nextFolder in folders)
                {
                    cVideoFiles += await BrowseForVideoFiles(nextFolder, collectedMediaItems, maxFilesToRetrieve - cVideoFiles);
                }
            }
            return cVideoFiles;
        }

        async private void LoadMediaFiles()
        {
            try
            {
                StorageFolder mediaServerFolder = mediaServers[dmsSelect.SelectedIndex];

                var queryOptions = new QueryOptions();
                queryOptions.FolderDepth = FolderDepth.Deep;
                queryOptions.UserSearchFilter = "System.Kind:=video";

                if (mediaServerFolder.AreQueryOptionsSupported(queryOptions))
                {
                    var queryFolder = mediaServerFolder.CreateFileQueryWithOptions(queryOptions);
                    mediaFiles = await queryFolder.GetFilesAsync(0, 25);
                    mediaSelect.Items.Clear();
                    if (mediaFiles.Count == 0)
                    {
                        rootPage.NotifyUser("No Media Files found ", NotifyType.StatusMessage);
                    }
                    else
                    {
                        foreach (StorageFile file in mediaFiles)
                        {
                            mediaSelect.Items.Add(file.DisplayName);
                        }
                        rootPage.NotifyUser("Media files retrieved", NotifyType.StatusMessage);
                    }
                }
                else
                {
                    List<StorageFile> lstMediaItems = new List<StorageFile>();
                    var countOfVideoFilesFound = await BrowseForVideoFiles(mediaServerFolder, lstMediaItems, 25);
                    mediaFiles = lstMediaItems;

                    if (countOfVideoFilesFound > 0)
                    {
                        rootPage.NotifyUser("Media files retrieved", NotifyType.StatusMessage);
                    }
                    else
                    {
                        rootPage.NotifyUser("No Media Files found ", NotifyType.StatusMessage);
                    }
                }
            }
            catch (Exception e)
            {
                rootPage.NotifyUser("Error locating media files " + e.Message, NotifyType.ErrorMessage);
            }
        }

        private void localVideo_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            rootPage.NotifyUser("File :" + mediaFile.DisplayName + " - Playback error  :" + e.ErrorMessage, NotifyType.ErrorMessage);
        }

        private void localVideo_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            if (mediaFile != null)
            {
                switch (localVideo.CurrentState)
                {
                    case MediaElementState.Playing:
                        rootPage.NotifyUser("File :" + mediaFile.DisplayName + " - Playing", NotifyType.StatusMessage);
                        break;
                    case MediaElementState.Paused:
                        rootPage.NotifyUser("File :" + mediaFile.DisplayName + " - Paused", NotifyType.StatusMessage);
                        break;
                    case MediaElementState.Stopped:
                        rootPage.NotifyUser("File :" + mediaFile.DisplayName + " - Stopped", NotifyType.StatusMessage);
                        break;
                    case MediaElementState.Opening:
                        rootPage.NotifyUser("File :" + mediaFile.DisplayName + " - Opening", NotifyType.StatusMessage);
                        break;
                    case MediaElementState.Closed:
                        break;
                    case MediaElementState.Buffering:
                        rootPage.NotifyUser("File :" + mediaFile.DisplayName + " - Buffering", NotifyType.StatusMessage);
                        break;
                }
            }
        }
    }
}
