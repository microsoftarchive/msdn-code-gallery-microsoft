/****************************** Module Header ******************************\
* Module Name:  FileDownloadCompletedEventArgs.cs
* Project:	    CSFTPDownload
* Copyright (c) Microsoft Corporation.
* 
* The class FileDownloadCompletedEventArgs defines the arguments used by the 
* FileDownloadCompleted event of FTPClient.
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
using System.IO;

namespace CSFTPDownload
{
    public class FileDownloadCompletedEventArgs : EventArgs
    {
        public Uri ServerPath { get; set; }
        public FileInfo LocalFile { get; set; }
    }
}
