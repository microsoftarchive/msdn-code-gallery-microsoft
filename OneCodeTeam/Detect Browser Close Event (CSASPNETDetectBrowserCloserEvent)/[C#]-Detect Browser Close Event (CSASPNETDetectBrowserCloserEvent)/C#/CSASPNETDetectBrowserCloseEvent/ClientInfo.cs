/**************************** Module Header ********************************\
* Module Name:    ClientInfo.cs
* Project:        CSASPNETDetectBrowserCloseEvent
* Copyright (c) Microsoft Corporation
*
* As we know, HTTP is a stateless protocol, the browser doesn't keep connecting
* to the server. When user try to close the browser using alt-f4, browser close(X) 
* and right click on browser and close -> this all is done and is working fine, 
* it's not possible to tell the server that the browser is closed. The sample 
* demonstrates how to detect the browser close event with iframe.

* This class is used as client's entity.
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
using System.Web;

namespace CSASPNETDetectBrowserCloseEvent
{
    public class ClientInfo
    {
        public ClientInfo()
        { }

        // ClientID
        private string _clientID;

        // Last ActiveTime of the client
        private DateTime _activeTime;

        // Last RefreshTime of the iframe
        private DateTime _refreshTime;
        
        public string ClientID
        {
            get
            {
                return _clientID;
            }
            set
            {
                _clientID = value;
            }
        }

        public DateTime ActiveTime
        {
            get
            {
                return _activeTime;
            }
            set
            {
                _activeTime = value;
            }
        }

        public DateTime RefreshTime
        {
            get
            {
                return _refreshTime;
            }
            set
            {
                _refreshTime = value;
            }
        }

        /// <summary>
        /// Search the client by clientID
        /// </summary>
        /// <param name="clientList">ClientList</param>
        /// <param name="strClientID">ClientID</param>
        public static ClientInfo GetClinetInfoByClientID(List<ClientInfo> clientList, string strClientID)
        {
            for (int i = 0; i < clientList.Count; i++)
            {
                if (clientList[i].ClientID == strClientID)
                {
                    return clientList[i];
                }
            }
            return new ClientInfo();
        }

    }
}