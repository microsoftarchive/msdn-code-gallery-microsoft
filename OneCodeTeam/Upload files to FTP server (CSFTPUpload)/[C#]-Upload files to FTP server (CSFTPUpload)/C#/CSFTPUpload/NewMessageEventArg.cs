/****************************** Module Header ******************************\
* Module Name:  NewMessageEventArg.cs
* Project:	    CSFTPUpload
* Copyright (c) Microsoft Corporation.
* 
* The class NewMessageEventArg defines the arguments used by the NewMessageArrived
* event of FTPClient.
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

namespace CSFTPUpload
{
    public class NewMessageEventArg:EventArgs
    {
        public string NewMessage { get; set; }
    }
}
