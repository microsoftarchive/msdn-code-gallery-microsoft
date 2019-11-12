/****************************** Module Header ******************************\
* Module Name:  FTPClientManager.cs
* Project:	    CSFTPUpload
* Copyright (c) Microsoft Corporation.
* 
* The class FTPClientManager supplies following features:
* 1. Verify whether a file or a directory exists on the FTP server.
* 2. List subdirectories and files of a folder on the FTP server.
* 3. Delete files or directories on the FTP server.
* 4. Create a directory on the FTP server.
* 5. Manage the FTPUploadClient to upload files to the FTP server. 
* 
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
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
using System.Linq;

namespace CSFTPUpload
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

        public event EventHandler<FileUploadCompletedEventArgs> FileUploadCompleted;

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
            
            if(this.Credentials!=null)
            {
            request.Credentials = this.Credentials;
            }
            else
            {
                request.UseDefaultCredentials = true; ;
            }

            
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

            if (this.Credentials != null)
            {
                request.Credentials = this.Credentials;
            }
            else
            {
                request.UseDefaultCredentials = true; ;
            }

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
        /// Get the sub directories and files of the current Url by default.
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
        /// Create a sub directory of a folder on the remote FTP server.
        /// </summary>
        public void CreateDirectoryOnFTPServer(Uri serverPath, string subDirectoryName)
        {

            // Create the Url for the new sub directory.
            Uri subDirUrl = new Uri(serverPath, subDirectoryName);

            // Check whether sub directory exist.
            bool urlExist = VerifyFTPUrlExist(subDirUrl);

            if (urlExist)
            {
                return;
            }

            try
            {
                // Create an FtpWebRequest to create the sub directory.
                FtpWebRequest request = WebRequest.Create(subDirUrl) as FtpWebRequest;
                request.Credentials = this.Credentials;
                request.Method = WebRequestMethods.Ftp.MakeDirectory;

                using (FtpWebResponse response = request.GetResponse() as FtpWebResponse)
                {
                    this.OnNewMessageArrived(new NewMessageEventArg
                    {
                        NewMessage = response.StatusDescription
                    });
                }
            }

            // If the folder does not exist, create the folder.
            catch (System.Net.WebException webEx)
            {

                FtpWebResponse ftpResponse = webEx.Response as FtpWebResponse;

                string msg = string.Format(
                                   "There is an error while creating folder {0}. "
                                   + " StatusCode: {1}  StatusDescription: {2} ",
                                   subDirUrl.ToString(),
                                   ftpResponse.StatusCode.ToString(),
                                   ftpResponse.StatusDescription);
                ApplicationException errorException
                    = new ApplicationException(msg, webEx);

                // Fire the ErrorOccurred event with the error.
                ErrorEventArgs e = new ErrorEventArgs
                {
                    ErrorException = errorException
                };

                this.OnErrorOccurred(e);
            }
        }

        /// <summary>
        /// Delete items on FTP server.
        /// </summary>
        public void DeleteItemsOnFTPServer(IEnumerable<FTPFileSystem> fileSystems)
        {
            if (fileSystems == null)
            {
                throw new ArgumentException("The item to delete is null!");
            }

            foreach (var fileSystem in fileSystems)
            {
                DeleteItemOnFTPServer(fileSystem);
            }

        }

        /// <summary>
        /// Delete an item on FTP server.
        /// </summary>
        public void DeleteItemOnFTPServer(FTPFileSystem fileSystem)
        {
            // Check whether sub directory exist.
            bool urlExist = VerifyFTPUrlExist(fileSystem.Url);

            if (!urlExist)
            {
                return;
            }

            try
            {

                // Non-Empty folder cannot be deleted.
                if (fileSystem.IsDirectory)
                {
                    var subFTPFiles = GetSubDirectoriesAndFiles(fileSystem.Url);

                    DeleteItemsOnFTPServer(subFTPFiles);
                }

                // Create an FtpWebRequest to create the sub directory.
                FtpWebRequest request = WebRequest.Create(fileSystem.Url) as FtpWebRequest;
                request.Credentials = this.Credentials;

                request.Method = fileSystem.IsDirectory
                    ? WebRequestMethods.Ftp.RemoveDirectory : WebRequestMethods.Ftp.DeleteFile;

                using (FtpWebResponse response = request.GetResponse() as FtpWebResponse)
                {
                    this.OnNewMessageArrived(new NewMessageEventArg
                    {
                        NewMessage = response.StatusDescription
                    });
                }
            }
            catch (System.Net.WebException webEx)
            {
                FtpWebResponse ftpResponse = webEx.Response as FtpWebResponse;

                string msg = string.Format(
                                   "There is an error while deleting {0}. "
                                   + " StatusCode: {1}  StatusDescription: {2} ",
                                   fileSystem.Url.ToString(),
                                   ftpResponse.StatusCode.ToString(),
                                   ftpResponse.StatusDescription);
                ApplicationException errorException
                    = new ApplicationException(msg, webEx);

                // Fire the ErrorOccurred event with the error.
                ErrorEventArgs e = new ErrorEventArgs
                {
                    ErrorException = errorException
                };

                this.OnErrorOccurred(e);
            }
        }

        /// <summary>
        /// Upload a whole local folder to FTP server.
        /// </summary>
        public void UploadFolder(DirectoryInfo localFolder, Uri serverPath,
            bool createFolderOnServer)
        {
            // The method UploadFoldersAndFiles will create or override a folder by default.
            if (createFolderOnServer)
            {
                UploadFoldersAndFiles(new FileSystemInfo[] { localFolder }, serverPath);
            }

            // Upload the files and sub directories of the local folder.
            else
            {
                UploadFoldersAndFiles(localFolder.GetFileSystemInfos(), serverPath);
            }
        }

        /// <summary>
        /// Upload local folders and files to FTP server.
        /// </summary>
        public void UploadFoldersAndFiles(IEnumerable<FileSystemInfo> fileSystemInfos,
            Uri serverPath)
        {
            if (this.status != FTPClientManagerStatus.Idle)
            {
                throw new ApplicationException("This client is busy now.");
            }

            this.Status = FTPClientManagerStatus.Uploading;

            FTPUploadClient uploadClient = new FTPUploadClient(this);

            // Register the events.
            uploadClient.AllFilesUploadCompleted +=
                new EventHandler(uploadClient_AllFilesUploadCompleted);
            uploadClient.FileUploadCompleted +=
                new EventHandler<FileUploadCompletedEventArgs>(uploadClient_FileUploadCompleted);

            uploadClient.UploadDirectoriesAndFiles(fileSystemInfos, serverPath);
        }


        void uploadClient_FileUploadCompleted(object sender, FileUploadCompletedEventArgs e)
        {
            this.OnFileUploadCompleted(e);
        }

        void uploadClient_AllFilesUploadCompleted(object sender, EventArgs e)
        {
            this.Status = FTPClientManagerStatus.Idle;
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

        protected virtual void OnFileUploadCompleted(FileUploadCompletedEventArgs e)
        {
            if (FileUploadCompleted != null)
            {
                FileUploadCompleted(this, e);
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
