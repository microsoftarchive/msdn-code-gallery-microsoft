/****************************** Module Header ******************************\
 * Module Name:  IDownloader.cs
 * Project:      CSMultiThreadedWebDownloader
 * Copyright (c) Microsoft Corporation.
 * 
 * This interface defines the basic properties and methods of a WebDownloader. 
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
using System.Net;
namespace CSMultiThreadedWebDownloader
{
    public interface IDownloader
    {

        #region Basic settings of a WebDownloader.
        
        Uri Url { get;}
        string DownloadPath { get; set; }
        long TotalSize { get; set; }

        ICredentials Credentials { get; set; }
        IWebProxy Proxy { get; set; }

        #endregion


        #region Support the "Pause", "Resume" and Multi-Threads feature.

        bool IsRangeSupported { get; set; }
        long StartPoint { get; set; }
        long EndPoint { get; set; }

        #endregion

        #region The downloaded data and status.
        
        long DownloadedSize { get; }
        int CachedSize { get; }

        bool HasChecked { get; set; }
        DownloadStatus Status { get; }
        TimeSpan TotalUsedTime { get; }

        #endregion

        #region Advanced settings of a WebDownloader

        int BufferSize { get; set; }
        int BufferCountPerNotification { get; set; }
        int MaxCacheSize { get; set; }

        #endregion


        event EventHandler<DownloadCompletedEventArgs> DownloadCompleted;
        event EventHandler<DownloadProgressChangedEventArgs> DownloadProgressChanged;
        event EventHandler StatusChanged;

        void CheckUrl(out string fileName);

        void BeginDownload();
        void Download();

        void Pause();

        void Resume();
        void BeginResume();

        void Cancel();
    }
}
