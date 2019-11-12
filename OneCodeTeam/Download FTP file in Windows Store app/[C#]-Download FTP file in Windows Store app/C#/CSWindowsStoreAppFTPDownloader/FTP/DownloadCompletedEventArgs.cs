/****************************** Module Header ******************************\
 * Module Name:    DownloadCompletedEventArgs.cs
 * Project:        CSWindowsStoreAppFTPDownloader
 * Copyright (c) Microsoft Corporation.
 * 
 * The event args when a file is downloaded.
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
using Windows.Storage;

namespace CSWindowsStoreAppFTPDownloader.FTP
{
    public class DownloadCompletedEventArgs:EventArgs
    {
        public Uri RequestFile { get; set; }
        public IStorageFile LocalFile { get; set; }
        public Exception Error { get; set; }
    }
}
