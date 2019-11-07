/****************************** Module Header ******************************\
* Module Name:  FTPClientStatus.cs
* Project:	    CSFTPUpload
* Copyright (c) Microsoft Corporation.
* 
* The enum FTPClientStatus contains all status of FTPClient.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

namespace CSFTPUpload
{
    public enum FTPClientManagerStatus
    {
        Idle,
        Uploading
    }
}
