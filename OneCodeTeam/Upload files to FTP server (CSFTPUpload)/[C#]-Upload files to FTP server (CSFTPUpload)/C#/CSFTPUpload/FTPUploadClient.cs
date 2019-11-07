/****************************** Module Header ******************************\
* Module Name:  FTPUploadClient.cs
* Project:	    CSFTPUpload
* Copyright (c) Microsoft Corporation.
* 
* This class is used to upload a file to a FTP server. When the upload 
* starts, it will upload the file in a background thread. 
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
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Net;

namespace CSFTPUpload
{
    public partial class FTPClientManager
    {

        public class FTPUploadClient
        {
            // 2M bytes.
            public const int MaxCacheSize = 2097152;

            // 2K bytes.
            public const int BufferSize = 2048;

            FTPClientManager manager;

            public event EventHandler<FileUploadCompletedEventArgs>
                FileUploadCompleted;

            public event EventHandler AllFilesUploadCompleted;

            public FTPUploadClient(FTPClientManager manager)
            {
                if (manager == null)
                {
                    throw new ArgumentNullException("FTPClientManager cannot be null.");
                }

                this.manager = manager;
            }

            /// <summary>
            /// Upload files, directories and their subdirectories.
            /// </summary>
            public void UploadDirectoriesAndFiles(IEnumerable<FileSystemInfo> fileSysInfos,
                Uri serverPath)
            {
                if (fileSysInfos == null)
                {
                    throw new ArgumentNullException(
                        "The files to upload cannot be null.");
                }

                // Create a thread to upload data.
                ParameterizedThreadStart threadStart =
                    new ParameterizedThreadStart(StartUploadDirectoriesAndFiles);
                Thread uploadThread = new Thread(threadStart);
                uploadThread.IsBackground = true;
                uploadThread.Start(new object[] { fileSysInfos, serverPath });
            }

            /// <summary>
            /// Upload files, directories and their subdirectories.
            /// </summary>
            void StartUploadDirectoriesAndFiles(object state)
            {
                var paras = state as object[];

                IEnumerable<FileSystemInfo> fileSysInfos =
                    paras[0] as IEnumerable<FileSystemInfo>;
                Uri serverPath = paras[1] as Uri;

                foreach (var fileSys in fileSysInfos)
                {
                    UploadDirectoryOrFile(fileSys, serverPath);
                }

                this.OnAllFilesUploadCompleted(EventArgs.Empty);
            }

            /// <summary>
            /// Upload a single file or directory.
            /// </summary>
            void UploadDirectoryOrFile(FileSystemInfo fileSystem, Uri serverPath)
            {

                // Upload the file directly.
                if (fileSystem is FileInfo)
                {
                    UploadFile(fileSystem as FileInfo, serverPath);
                }

                // Upload a directory.
                else
                {

                    // Construct the sub directory Uri.
                    Uri subDirectoryPath = new Uri(serverPath, fileSystem.Name + "/");

                    this.manager.CreateDirectoryOnFTPServer(serverPath, fileSystem.Name + "/");

                    // Get the sub directories and files.
                    var subDirectoriesAndFiles = (fileSystem as DirectoryInfo)
                        .GetFileSystemInfos();

                    // Upload the files in the folder and sub directories.
                    foreach (var subFile in subDirectoriesAndFiles)
                    {
                        UploadDirectoryOrFile(subFile, subDirectoryPath);
                    }
                }
            }

            /// <summary>
            /// Upload a single file directly.
            /// </summary>
            void UploadFile(FileInfo file, Uri serverPath)
            {
                if (file == null)
                {
                    throw new ArgumentNullException(" The file to upload is null. ");
                }

                Uri destPath = new Uri(serverPath, file.Name);

                // Create a request to upload the file.
                FtpWebRequest request = WebRequest.Create(destPath) as FtpWebRequest;

                request.Credentials = this.manager.Credentials;

                // Upload file.
                request.Method = WebRequestMethods.Ftp.UploadFile;

                FtpWebResponse response = null;
                Stream requestStream = null;
                FileStream localFileStream = null;

                try
                {

                    // Retrieve the response from the server.
                    response = request.GetResponse() as FtpWebResponse;

                    // Open the local file to read.
                    localFileStream = file.OpenRead();

                    // Retrieve the request stream.
                    requestStream = request.GetRequestStream();

                    int bytesSize = 0;
                    byte[] uploadBuffer = new byte[FTPUploadClient.BufferSize];

                    while (true)
                    {

                        // Read a buffer of local file.
                        bytesSize = localFileStream.Read(uploadBuffer, 0,
                          uploadBuffer.Length);

                        if (bytesSize == 0)
                        {
                            break;
                        }
                        else
                        {

                            // Write the buffer to the request stream.
                            requestStream.Write(uploadBuffer, 0, bytesSize);

                        }
                    }

                    var fileUploadCompletedEventArgs = new FileUploadCompletedEventArgs
                    {

                        LocalFile = file,
                        ServerPath = destPath
                    };

                    this.OnFileUploadCompleted(fileUploadCompletedEventArgs);

                }
                catch (System.Net.WebException webEx)
                {
                    FtpWebResponse ftpResponse = webEx.Response as FtpWebResponse;

                    string msg = string.Format(
                                       "There is an error while upload {0}. "
                                       + " StatusCode: {1}  StatusDescription: {2} ",
                                       file.FullName,
                                       ftpResponse.StatusCode.ToString(),
                                       ftpResponse.StatusDescription);
                    ApplicationException errorException
                        = new ApplicationException(msg, webEx);

                    // Fire the ErrorOccurred event with the error.
                    ErrorEventArgs e = new ErrorEventArgs
                    {
                        ErrorException = errorException
                    };

                    this.manager.OnErrorOccurred(e);
                }
                catch (Exception ex)
                {
                    string msg = string.Format(
                                       "There is an error while upload {0}. "
                                       + " See InnerException for detailed error. ",
                                       file.FullName);
                    ApplicationException errorException
                        = new ApplicationException(msg, ex);

                    // Fire the ErrorOccurred event with the error.
                    ErrorEventArgs e = new ErrorEventArgs
                    {
                        ErrorException = errorException
                    };

                    this.manager.OnErrorOccurred(e);
                }
                finally
                {
                    if (response != null)
                    {
                        response.Close();
                    }

                    if (requestStream != null)
                    {
                        requestStream.Close();
                    }

                    if (localFileStream != null)
                    {
                        localFileStream.Close();
                    }
                }
            }

            protected virtual void OnFileUploadCompleted(FileUploadCompletedEventArgs e)
            {

                if (FileUploadCompleted != null)
                {
                    FileUploadCompleted(this, e);
                }
            }

            protected virtual void OnAllFilesUploadCompleted(EventArgs e)
            {
                if (AllFilesUploadCompleted != null)
                {
                    AllFilesUploadCompleted(this, e);
                }
            }
        }
    }
}
