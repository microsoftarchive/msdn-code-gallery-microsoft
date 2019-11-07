/****************************** Module Header ******************************\
* Module Name:  FTPDownloadClient.cs
* Project:	    CSFTPDownload
* Copyright (c) Microsoft Corporation.
* 
* This class is used to download files from a FTP server. When the download 
* starts, it will download the file in a background thread. The downloaded 
* data is stored in a MemoryStream first, and then written to local file.
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
using System.Net;
using System.Threading;

namespace CSFTPDownload
{
    public partial class FTPClientManager
    {

        public class FTPDownloadClient
        {
            // 2M bytes.
            public const int MaxCacheSize = 2097152;

            // 2K bytes.
            public const int BufferSize = 2048;

            FTPClientManager manager;

            public event EventHandler<FileDownloadCompletedEventArgs> 
                FileDownloadCompleted;

            public event EventHandler AllFilesDownloadCompleted;

            public FTPDownloadClient(FTPClientManager manager)
            {
                if (manager == null)
                {
                    throw new ArgumentNullException("FTPClientManager cannot be null.");
                }

                this.manager = manager;
            }

            /// <summary>
            /// Download files, directories and their subdirectories.
            /// </summary>
            public void DownloadDirectoriesAndFiles(IEnumerable<FTPFileSystem> files,
                string localPath)
            {
                if (files == null)
                {
                    throw new ArgumentNullException(
                        "The files to download cannot be null.");
                }

                // Create a thread to download data.
                ParameterizedThreadStart threadStart =
                    new ParameterizedThreadStart(StartDownloadDirectoriesAndFiles);
                Thread downloadThread = new Thread(threadStart);
                downloadThread.IsBackground = true;
                downloadThread.Start(new object[] { files, localPath });
            }

            /// <summary>
            /// Download files, directories and their subdirectories.
            /// </summary>
            void StartDownloadDirectoriesAndFiles(object state)
            {
                var paras = state as object[];

                IEnumerable<FTPFileSystem> files = paras[0] as IEnumerable<FTPFileSystem>;
                string localPath = paras[1] as string;

                foreach (var file in files)
                {
                    DownloadDirectoryOrFile(file, localPath);
                }

                this.OnAllFilesDownloadCompleted(EventArgs.Empty);
            }

            /// <summary>
            /// Download a single file or directory.
            /// </summary>
            void DownloadDirectoryOrFile(FTPFileSystem fileSystem, string localPath)
            {

                // Download the file directly.
                if (!fileSystem.IsDirectory)
                {
                    DownloadFile(fileSystem, localPath);
                }

                // Download a directory.
                else
                {

                    // Construct the directory Path.
                    string directoryPath = localPath + "\\" + fileSystem.Name;

                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    // Get the sub directories and files.
                    var subDirectoriesAndFiles =
                        this.manager.GetSubDirectoriesAndFiles(fileSystem.Url);

                    // Download the files in the folder and sub directories.
                    foreach (var subFile in subDirectoriesAndFiles)
                    {
                        DownloadDirectoryOrFile(subFile, directoryPath);
                    }
                }
            }

            /// <summary>
            /// Download a single file directly.
            /// </summary>
            void DownloadFile(FTPFileSystem file, string localPath)
            {
                if (file.IsDirectory)
                {
                    throw new ArgumentException(
                        "The FTPFileSystem to download is a directory in fact");
                }

                string destPath = localPath + "\\" + file.Name;

                // Create a request to the file to be  downloaded.
                FtpWebRequest request = WebRequest.Create(file.Url) as FtpWebRequest;

                request.Credentials = this.manager.Credentials;

                // Download file.
                request.Method = WebRequestMethods.Ftp.DownloadFile;

                FtpWebResponse response = null;
                Stream responseStream = null;
                MemoryStream downloadCache = null;


                try
                {

                    // Retrieve the response from the server and get the response stream.
                    response = request.GetResponse() as FtpWebResponse;

                    this.manager.OnNewMessageArrived(new NewMessageEventArg
                    {
                        NewMessage = response.StatusDescription
                    });

                    responseStream = response.GetResponseStream();

                    // Cache data in memory.
                    downloadCache = new MemoryStream(FTPDownloadClient.MaxCacheSize);
                    byte[] downloadBuffer = new byte[FTPDownloadClient.BufferSize];

                    int bytesSize = 0;
                    int cachedSize = 0;

                    // Download the file until the download is completed.
                    while (true)
                    {

                        // Read a buffer of data from the stream.
                        bytesSize = responseStream.Read(downloadBuffer, 0, 
                            downloadBuffer.Length);

                        // If the cache is full, or the download is completed, write 
                        // the data in cache to local file.
                        if (bytesSize == 0
                            || MaxCacheSize < cachedSize + bytesSize)
                        {
                            try
                            {
                                // Write the data in cache to local file.
                                WriteCacheToFile(downloadCache, destPath, cachedSize);

                                // Stop downloading the file if the download is paused, 
                                // canceled or completed. 
                                if (bytesSize == 0)
                                {
                                    break;
                                }

                                // Reset cache.
                                downloadCache.Seek(0, SeekOrigin.Begin);
                                cachedSize = 0;
                            }
                            catch (Exception ex)
                            {
                                string msg = string.Format(
                                    "There is an error while downloading {0}. "
                                    + " See InnerException for detailed error. ", 
                                    file.Url);
                                ApplicationException errorException 
                                    = new ApplicationException(msg, ex);

                                // Fire the DownloadCompleted event with the error.
                                ErrorEventArgs e = new ErrorEventArgs
                                {
                                    ErrorException = errorException
                                };

                                this.manager.OnErrorOccurred(e);

                                return;
                            }

                        }

                        // Write the data from the buffer to the cache in memory.
                        downloadCache.Write(downloadBuffer, 0, bytesSize);
                        cachedSize += bytesSize;
                    }

                    var fileDownloadCompletedEventArgs = new FileDownloadCompletedEventArgs
                    {

                        LocalFile = new FileInfo(destPath),
                        ServerPath = file.Url
                    };

                    this.OnFileDownloadCompleted(fileDownloadCompletedEventArgs);
                }
                finally
                {
                    if (response != null)
                    {
                        response.Close();
                    }

                    if (responseStream != null)
                    {
                        responseStream.Close();
                    }

                    if (downloadCache != null)
                    {
                        downloadCache.Close();
                    }
                }
            }

            /// <summary>
            /// Write the data in cache to local file.
            /// </summary>
            void WriteCacheToFile(MemoryStream downloadCache, string downloadPath,
                int cachedSize)
            {
                using (FileStream fileStream = new FileStream(downloadPath, 
                    FileMode.Append))
                {
                    byte[] cacheContent = new byte[cachedSize];
                    downloadCache.Seek(0, SeekOrigin.Begin);
                    downloadCache.Read(cacheContent, 0, cachedSize);
                    fileStream.Write(cacheContent, 0, cachedSize);
                }
            }

            protected virtual void OnFileDownloadCompleted(FileDownloadCompletedEventArgs e)
            {

                if (FileDownloadCompleted != null)
                {
                    FileDownloadCompleted(this, e);
                }
            }

            protected virtual void OnAllFilesDownloadCompleted(EventArgs e)
            {
                if (AllFilesDownloadCompleted != null)
                {
                    AllFilesDownloadCompleted(this, e);
                }
            }
        }
    }
}
