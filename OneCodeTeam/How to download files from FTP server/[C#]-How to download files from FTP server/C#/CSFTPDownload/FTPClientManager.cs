/****************************** Module Header ******************************\
* Module Name:  FTPClientManager.cs
* Project:	    CSFTPDownload
* Copyright (c) Microsoft Corporation.
* 
* The class FTPClientManager supplies following features:
* 1. Verify whether a file or a directory exists on the FTP server.
* 2. List subdirectories and files of a folder on the FTP server.
* 3. Delete files or directories on the FTP server.
* 4. Create a directory on the FTP server.
* 5. Manage the FTPDownloadClient to download files from the FTP server. 
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

namespace CSFTPDownload
{
    public partial class FTPClientManager
    {
       

        /// <summary>
        /// The Credentials to connect to the FTP server.
        /// </summary>
        public ICredentials Credentials { get; set; }

        /// <summary>
        /// The current URL of this FTPClient.
        /// </summary>
        public Uri Url { get; private set; }

        FTPClientManagerStatus status;

        /// <summary>
        /// Get or Set the status of this FTPClient.
        /// </summary>
        public FTPClientManagerStatus Status
        {
            get
            {
                return status;
            }

            private set
            {
                if (status != value)
                {
                    status = value;

                    // Raise a OnStatusChanged event.
                    this.OnStatusChanged(EventArgs.Empty);

                }
            }
        }

        public event EventHandler UrlChanged;

        public event EventHandler<ErrorEventArgs> ErrorOccurred;

        public event EventHandler StatusChanged;

        public event EventHandler<FileDownloadCompletedEventArgs> FileDownloadCompleted;

        public event EventHandler<NewMessageEventArg> NewMessageArrived;

        /// <summary>
        ///  Initialize a FTPClient instance.
        /// </summary>
        public FTPClientManager(Uri url, ICredentials credentials)
        {
            this.Credentials = credentials;

            // Check whether the Url exists and the credentials is correct.
            // If there is an error, an exception will be thrown.
            CheckFTPUrlExist(url);

            this.Url = url;

            // Set the Status.
            this.Status = FTPClientManagerStatus.Idle;

        }

        /// <summary>
        /// Navigate to the parent folder.
        /// </summary>
        public void NavigateParent()
        {
            if (Url.AbsolutePath != "/")
            {

                // Get the parent url.
                Uri newUrl = new Uri(this.Url, "..");

                // Check whether the Url exists.
                CheckFTPUrlExist(newUrl);

                this.Url = newUrl;
                this.OnUrlChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Navigate a url.
        /// </summary>
        public void Naviagte(Uri newUrl)
        {
            // Check whether the Url exists.
            bool urlExist = VerifyFTPUrlExist(newUrl);

            this.Url = newUrl;
            this.OnUrlChanged(EventArgs.Empty);
        }

        /// <summary>
        /// If the Url does not exist, an exception will be thrown.
        /// </summary>
        void CheckFTPUrlExist(Uri url)
        {
            bool urlExist = VerifyFTPUrlExist(url);

            if (!urlExist)
            {
                throw new ApplicationException("The url does not exist");
            }
        }

        /// <summary>
        /// Verify whether the url exists.
        /// </summary>
        bool VerifyFTPUrlExist(Uri url)
        {
            bool urlExist = false;

            if (url.IsFile)
            {
                urlExist = VerifyFileExist(url);
            }
            else
            {
                urlExist = VerifyDirectoryExist(url);
            }

            return urlExist;
        }

        /// <summary>
        /// Verify whether the directory exists.
        /// </summary>
        bool VerifyDirectoryExist(Uri url)
        {
            FtpWebRequest request = WebRequest.Create(url) as FtpWebRequest;
            request.Credentials = this.Credentials;
            request.Method = WebRequestMethods.Ftp.ListDirectory;

            FtpWebResponse response = null;

            try
            {
                response = request.GetResponse() as FtpWebResponse;

                return response.StatusCode == FtpStatusCode.DataAlreadyOpen;
            }
            catch (System.Net.WebException webEx)
            {
                FtpWebResponse ftpResponse = webEx.Response as FtpWebResponse;

                if (ftpResponse.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    return false;
                }

                throw;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }
        }

        /// <summary>
        /// Verify whether the file exists.
        /// </summary>
        bool VerifyFileExist(Uri url)
        {
            FtpWebRequest request = WebRequest.Create(url) as FtpWebRequest;
            request.Credentials = this.Credentials;
            request.Method = WebRequestMethods.Ftp.GetFileSize;

            FtpWebResponse response = null;

            try
            {
                response = request.GetResponse() as FtpWebResponse;

                return response.StatusCode == FtpStatusCode.FileStatus;
            }
            catch (System.Net.WebException webEx)
            {
                FtpWebResponse ftpResponse = webEx.Response as FtpWebResponse;

                if (ftpResponse.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    return false;
                }

                throw;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }
        }

        /// <summary>
        /// Get the sub directories and files of the current Url by dafault.
        /// </summary>
        public IEnumerable<FTPFileSystem> GetSubDirectoriesAndFiles()
        {
            return GetSubDirectoriesAndFiles(this.Url);
        }

        /// <summary>
        /// Get the sub directories and files of the Url. It will be used in enumerate 
        /// all the folders.
        /// When run the FTP LIST protocol method to get a detailed listing of the files  
        /// on an FTP server, the server will response many records of information. Each 
        /// record represents a file. 
        /// </summary>
        public IEnumerable<FTPFileSystem> GetSubDirectoriesAndFiles(Uri url)
        {
            FtpWebRequest request = WebRequest.Create(url) as FtpWebRequest;

            request.Credentials = this.Credentials;

            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;


            FtpWebResponse response = null;
            Stream responseStream = null;
            StreamReader reader = null;
            try
            {
                response = request.GetResponse() as FtpWebResponse;

                this.OnNewMessageArrived(new NewMessageEventArg
                {
                    NewMessage = response.StatusDescription
                });

                responseStream = response.GetResponseStream();
                reader = new StreamReader(responseStream);

                List<FTPFileSystem> subDirs = new List<FTPFileSystem>();

                string subDir = reader.ReadLine();

                // Find out the FTP Directory Listing Style from the recordString.
                FTPDirectoryListingStyle style = FTPDirectoryListingStyle.MSDOS;
                if (!string.IsNullOrEmpty(subDir))
                {
                    style = FTPFileSystem.GetDirectoryListingStyle(subDir);
                }
                while (!string.IsNullOrEmpty(subDir))
                {
                    subDirs.Add(FTPFileSystem.ParseRecordString(url, subDir, style));

                    subDir = reader.ReadLine();
                }
                return subDirs;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }

                // Close the StreamReader object and the underlying stream, and release
                // any system resources associated with the reader.
                if (reader != null)
                {
                    reader.Close();
                }
            }
        }

        /// <summary>
        /// Download files, directories and their subdirectories.
        /// </summary>
        public void DownloadDirectoriesAndFiles(IEnumerable<FTPFileSystem> files, string localPath)
        {
            if (this.status != FTPClientManagerStatus.Idle)
            {
                throw new ApplicationException("This client is busy now.");
            }

            this.Status = FTPClientManagerStatus.Downloading;

            FTPDownloadClient downloadClient = new FTPDownloadClient(this);
            downloadClient.FileDownloadCompleted += new EventHandler<FileDownloadCompletedEventArgs>(downloadClient_FileDownloadCompleted);
            downloadClient.AllFilesDownloadCompleted += new EventHandler(downloadClient_AllFilesDownloadCompleted);
            downloadClient.DownloadDirectoriesAndFiles(files,localPath);
            
        }

        void downloadClient_AllFilesDownloadCompleted(object sender, EventArgs e)
        {
            this.Status = FTPClientManagerStatus.Idle;
        }

        void downloadClient_FileDownloadCompleted(object sender, FileDownloadCompletedEventArgs e)
        {
            this.OnFileDownloadCompleted(e);
        }
      
        protected virtual void OnUrlChanged(EventArgs e)
        {
            if (UrlChanged != null)
            {
                UrlChanged(this, e);
            }
        }

        protected virtual void OnStatusChanged(EventArgs e)
        {
            if (StatusChanged != null)
            {
                StatusChanged(this, e);
            }
        }

        protected virtual void OnFileDownloadCompleted(FileDownloadCompletedEventArgs e)
        {
            if (FileDownloadCompleted != null)
            {
                FileDownloadCompleted(this, e);
            }
        }

        protected virtual void OnErrorOccurred(ErrorEventArgs e)
        {
            this.Status = FTPClientManagerStatus.Idle;

            if (ErrorOccurred != null)
            {
                ErrorOccurred(this, e);
            }
        }

        protected virtual void OnNewMessageArrived(NewMessageEventArg e)
        {
            if (NewMessageArrived != null)
            {
                NewMessageArrived(this, e);
            }
        }
    }
}
