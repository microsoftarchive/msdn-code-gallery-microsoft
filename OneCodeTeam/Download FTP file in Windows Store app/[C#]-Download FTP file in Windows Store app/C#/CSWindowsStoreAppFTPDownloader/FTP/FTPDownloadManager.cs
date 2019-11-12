/****************************** Module Header ******************************\
* Module Name:  FTPDownloadManager.cs
* Project:	    CSWindowsStoreAppFTPDownloader
* Copyright (c) Microsoft Corporation.
* 
* The class FTPClientManager supplies following features:
* 1. List subdirectories and files of a folder on the FTP server.
* 2. Download files from the FTP server.
*    a. If the file is less than 1MB, download it using WebRequest.
*    b. Download large file using BackgroundDownloader.
* 
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;

namespace CSWindowsStoreAppFTPDownloader.FTP
{
    public class FTPDownloadManager
    {
        public static readonly FTPDownloadManager Instance = new FTPDownloadManager();

        /// <summary>
        /// The backgroundDownloaders that are active
        /// </summary>
        public List<DownloadOperation> ActiveBackgroundDownloaders
        {
            get;
            set;
        }

        public event EventHandler<DownloadCompletedEventArgs> DownloadCompleted;

        private FTPDownloadManager()
        {
            ActiveBackgroundDownloaders = new List<DownloadOperation>();
        }

        /// <summary>
        /// Get the active downloads. 
        /// This method is called when app is launched.
        /// </summary>
        public async void Initialize()
        {
            var downloads = await BackgroundDownloader.GetCurrentDownloadsAsync();
            ActiveBackgroundDownloaders.AddRange(downloads);

            List<Task> tasks = new List<Task>();

            foreach (var download in this.ActiveBackgroundDownloaders)
            {
                Progress<DownloadOperation> progressCallback = new Progress<DownloadOperation>(DownloadProgress);
                tasks.Add(download.AttachAsync().AsTask(progressCallback));
            }

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Get the sub directories and files of the Url. It will be used in enumerate 
        /// all the folders.
        /// When run the FTP LIST protocol method to get a detailed listing of the files  
        /// on an FTP server, the server will response many records of information. Each 
        /// record represents a file. 
        /// </summary>
        public async Task<IEnumerable<FTPFileSystem>> ListFtpContentAsync(string url, ICredentials credential)
        {
            Uri currentUri = null;
            if (!Uri.TryCreate(url, UriKind.Absolute, out currentUri) ||
                currentUri == null)
            {
                throw new ArgumentException("URL: " + url + " is not valid.");
            }

            return await ListFtpContentAsync(currentUri, credential);
        }

        /// <summary>
        /// Get the sub directories and files of the Url. It will be used in enumerate 
        /// all the folders.
        /// When run the FTP LIST protocol method to get a detailed listing of the files  
        /// on an FTP server, the server will response many records of information. Each 
        /// record represents a file or a directory. 
        /// </summary>
        public async Task<IEnumerable<FTPFileSystem>> ListFtpContentAsync(Uri url, ICredentials credential)
        {
            Uri currentUri = url;

            // This request is FtpWebRequest in fact.
            WebRequest request = WebRequest.Create(currentUri);
            if (credential != null)
            {
                request.Credentials = credential;
            }

            request.Proxy = WebRequest.DefaultWebProxy;
            // Set the method to LIST.
            request.Method = "LIST";

            // Get response.
            using (WebResponse response = await request.GetResponseAsync())
            {

                // Get response stream.
                using (Stream responseStream = response.GetResponseStream())
                {

                    using (StreamReader reader = new StreamReader(responseStream))
                    {
                        List<FTP.FTPFileSystem> subDirs = new List<FTP.FTPFileSystem>();

                        string item = reader.ReadLine();

                        // Find out the FTP Directory Listing Style from the recordString.
                        FTP.FTPDirectoryListingStyle style = FTP.FTPDirectoryListingStyle.MSDOS;
                        if (!string.IsNullOrEmpty(item))
                        {
                            style = FTP.FTPFileSystem.GetDirectoryListingStyle(item);
                        }
                        while (!string.IsNullOrEmpty(item))
                        {
                            subDirs.Add(FTP.FTPFileSystem.ParseRecordString(currentUri, item, style));
                            item = reader.ReadLine();
                        }

                        return subDirs;
                    }
                }
            }

        }

        /// <summary>
        /// Download files, directories and their subdirectories.
        /// </summary>
        public async void DownloadFTPItemsAsync(IEnumerable<FTPFileSystem> itemsToDownload,
           StorageFolder targetFolder, NetworkCredential credential)
        {

            // Create a BackgroundDownloader
            BackgroundDownloader downloader = new BackgroundDownloader();

            // Download sub folders and files.
            foreach (var item in itemsToDownload.ToList())
            {
                if (item.IsDirectory)
                {

                    // Create a local folder.
                    var subFolder = await targetFolder.CreateFolderAsync(item.Name,
                        CreationCollisionOption.OpenIfExists);

                    // Download the whole folder.
                    DownloadFTPDirectoryAsync(downloader, credential, item, subFolder);
                }
                else
                {

                    // Create a local file.
                    var file = await targetFolder.CreateFileAsync(item.Name,
                        CreationCollisionOption.ReplaceExisting);

                    // Download the file.
                    DownloadFTPFileAsync(downloader, credential, item, file);
                }

            }
        }

        /// <summary>
        /// Download a whole directory.
        /// </summary>
        private async void DownloadFTPDirectoryAsync(
            BackgroundDownloader downloader,
            NetworkCredential credential,
            FTPFileSystem item,
            StorageFolder targetFolder)
        {

            // List the sub folders and files.
            var subItems = await this.ListFtpContentAsync(item.Url, credential);

            // Download the sub folders and files.
            foreach (var subitem in subItems)
            {
                if (subitem.IsDirectory)
                {

                    // Create a local folder.
                    var subFolder = await targetFolder.CreateFolderAsync(subitem.Name,
                        CreationCollisionOption.ReplaceExisting);

                    // Download the whole folder.
                    DownloadFTPDirectoryAsync(downloader, credential, subitem, subFolder);
                }
                else
                {

                    // Create a local file.
                    var file = await targetFolder.CreateFileAsync(subitem.Name,
                        CreationCollisionOption.GenerateUniqueName);

                    // Download the file.
                    DownloadFTPFileAsync(downloader, credential, subitem, file);
                }
            }
        }

        /// <summary>
        /// Download a single file directly.
        /// </summary>
        private async void DownloadFTPFileAsync(
            BackgroundDownloader downloader,
            NetworkCredential credential,
            FTPFileSystem item,
            StorageFile targetFile)
        {
            if (item.Size > 1048576) // 1M Byte
            {
                Progress<DownloadOperation> progressCallback = new Progress<DownloadOperation>(DownloadProgress);

                Uri urlWithCredential = item.Url;

                if (credential != null)
                {
                    urlWithCredential = new Uri(item.Url.ToString().ToLower().Replace(@"ftp://",
                        string.Format(@"ftp://{0}:{1}@", 
                        FTPFileSystem.EncodeUrl(credential.UserName), 
                        FTPFileSystem.EncodeUrl(credential.Password))));
                }

                DownloadOperation download = downloader.CreateDownload(
                    urlWithCredential,
                    targetFile);
                ActiveBackgroundDownloaders.Add(download);
                await download.StartAsync().AsTask(progressCallback);
            }
            else
            {
                await FTPClient.DownloadFTPFileAsync(item, targetFile, credential)
                    .ContinueWith(new Action<Task<DownloadCompletedEventArgs>>(DownloadProgress));
            }

        }

        /// <summary>
        /// Raise DownloadCompleted event when a file is downloaded.
        /// </summary>
        private void DownloadProgress(Task<DownloadCompletedEventArgs> task)
        {
            if ( this.DownloadCompleted != null)
            {
                this.DownloadCompleted(this, task.Result);
            }
        }


        /// <summary>
        /// Raise DownloadCompleted event when a file is downloaded.
        /// </summary>
        private void DownloadProgress(DownloadOperation download)
        {
            if (download.Progress.Status == BackgroundTransferStatus.Completed)
            {
                ActiveBackgroundDownloaders.Remove(download);

                if (this.DownloadCompleted != null)
                {
                    this.DownloadCompleted(this, new DownloadCompletedEventArgs
                    {
                        RequestFile = download.RequestedUri,
                        LocalFile = download.ResultFile,
                        Error=null 
                    });
                }
            }
            else if (download.Progress.Status == BackgroundTransferStatus.Error)
            {
                ActiveBackgroundDownloaders.Remove(download);

                if (this.DownloadCompleted != null)
                {
                    this.DownloadCompleted(this, new DownloadCompletedEventArgs
                    {
                        RequestFile = download.RequestedUri,
                        LocalFile = download.ResultFile,
                        Error = new Exception("Failed to download file")
                    });
                }
            }
        }
    }
}
