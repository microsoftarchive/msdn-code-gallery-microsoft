/**************************** Module Header ********************************\
* Module Name:    ClientInfo.cs
* Project:        CSASPNETDetectBrowserCloseEvent
* Copyright (c) Microsoft Corporation
*
* As we know, HTTP is a stateless protocol, so the browser doesn't keep connecting 
* to the server. When users try to close the browser using alt-f4, browser close(X) 
* and right click on browser and close, all these methods are working fine, 
* but it's not possible to tell the server that the browser is closed.
*
* This class is the client entity.
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
using System.Collections.Generic;
using System.Web;
using System.Xml.Linq;
using System.Linq;

namespace CSASPNETDetectBrowserCloseEvent
{
    public class ClientInfo
    {
        private static string filePath = HttpContext.Current.Server.MapPath("~/App_Data/ClientInfos.xml");
        public static List<ClientInfo> ClientInfoList=new List<ClientInfo>();
    
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
            ClientInfo clientInfo=new ClientInfo();
            ClientInfoList.Add(clientInfo);
            return clientInfo;
        }

        public void InsertClientInfo()
        {   
            XDocument xDoc = XDocument.Load(filePath);
            xDoc.Root.Add(new XElement("ClientInfo",
                new XElement("ClientID", this.ClientID),
                new XElement("ActiveTime", this.ActiveTime),
                new XElement("RefreshTime", this.RefreshTime)));
            xDoc.Save(filePath);
        }

        public static ClientInfo GetClientInfoByID(string ClientID)
        {
            XDocument xDoc = XDocument.Load(filePath);
            var query = from clientInfo in xDoc.Root.Elements()
                        where clientInfo.Element("ClientID").Value == ClientID
                        select clientInfo;
            if (query.Count()>0)
            {
                ClientInfo clientInfo = new ClientInfo();
                clientInfo.ClientID = query.First().Element("ClientID").Value;
                clientInfo.ActiveTime = DateTime.Parse(query.First().Element("ActiveTime").Value);
                clientInfo.RefreshTime = DateTime.Parse(query.First().Element("RefreshTime").Value);
                return clientInfo;
            }
            return null;
        }
    }
}