/****************************** Module Header ******************************\
 * Module Name:  DownloadStatus.cs
 * Project:      CSMultiThreadedWebDownloader
 * Copyright (c) Microsoft Corporation.
 * 
 * The enum DownloadStatus contains all status of 
 * an IDownloader instance.
 * 
 * This source is subject to the Microsoft Public License.
 * See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
 * All other rights reserved.
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

namespace CSMultiThreadedWebDownloader
{
    public enum DownloadStatus
    {
        /// <summary>
        /// The DownloadClient is initialized.
        /// </summary>
        Initialized,

        /// <summary>
        /// The client is waiting for an idle thread / resource to start downloading.
        /// </summary>
        Waiting,

        /// <summary>
        /// The client is downloading data.
        /// </summary>
        Downloading,

        /// <summary>
        /// The client is releasing the resource, and then the downloading
        /// will be paused.
        /// </summary>
        Pausing,

        /// <summary>
        /// The downloading is paused.
        /// </summary>
        Paused,

        /// <summary>
        /// The client is releasing the resource, and then the downloading
        /// will be canceled.
        /// </summary>
        Canceling,

        /// <summary>
        /// The downloading is Canceled.
        /// </summary>
        Canceled,

        /// <summary>
        /// The downloading is Completed.
        /// </summary>
        Completed
    }
}
