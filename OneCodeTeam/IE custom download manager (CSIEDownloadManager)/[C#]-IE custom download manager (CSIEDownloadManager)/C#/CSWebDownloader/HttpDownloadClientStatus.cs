/****************************** Module Header ******************************\
 * Module Name:  HttpDownloadClientStatus.cs
 * Project:      CSWebDownloader
 * Copyright (c) Microsoft Corporation.
 * 
 * The enum HttpDownloadClientStatus contains all status of HttpDownloadClient.
 * 
 * This source is subject to the Microsoft Public License.
 * See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
 * All other rights reserved.
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

namespace CSWebDownloader
{
    public enum HttpDownloadClientStatus
    {
        Idle = 0,
        Checked = 1,
        Downloading = 2,
        Pausing = 3,
        Paused = 4,
        Canceling = 5,
        Canceled = 6,
        Completed = 7
    }
}
