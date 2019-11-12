/**************************** Module Header ********************************\
* Module Name:    ClientInfoDataSource.cs
* Project:        CSASPNETDetectBrowserCloseEvent
* Copyright (c) Microsoft Corporation
*
* As we know, HTTP is a stateless protocol, so the browser doesn't keep connecting 
* to the server. When users try to close the browser using alt-f4, browser close(X) 
* and right click on browser and close, all these methods are working fine, 
* but it's not possible to tell the server that the browser is closed.
*
* This class is used as simple data source class.
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
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace CSASPNETDetectBrowserCloseEvent.Models
{
    public class ClientInfoDataSource
    {
        private static string filePath = HttpContext.Current.Server.MapPath("~/App_Data/ClientInfos.xml");

        private static XDocument clientInfosXDoc;
       
        public ClientInfoDataSource()
        {
            clientInfosXDoc = XDocument.Load(filePath);
        }

        /// <summary>
        /// Get ClientInfo by ClientId
        /// </summary>
        /// <param name="clientID"></param>
        /// <returns></returns>
        public ClientInfo GetClientInfoByClientId(string clientID)
        {
            var query = from clientInfoXml in clientInfosXDoc.Descendants("ClientID")
                        where clientInfoXml.Value == clientID
                        select clientInfoXml.Parent;

            return convertToClientInfo(query.FirstOrDefault());
        }



        /// <summary>
        /// Insert ClientInfo message to XML file
        /// </summary>
        /// <param name="clientInfo"></param>
        /// <returns></returns>
        public void InsertClientInfo(ClientInfo clientInfo)
        {  
		    clientInfosXDoc.Root.Add(convertToClientInfoXElement(clientInfo));
        }

        /// <summary>
        /// Update ActiveTime and RefreshTime
        /// </summary>
        /// <param name="clientInfo"></param>
        public void UpdateClientInfo(ClientInfo clientInfo)
        {
            var query = from x in clientInfosXDoc.Root.Elements()
                        where x.Element("ClientID").Value == clientInfo.ClientID
                        select x;
            if (query.Count()>0)
	        {
                query.FirstOrDefault().Element("ActiveTime").Value = clientInfo.ActiveTime.ToString("MM/dd/yyyy HH:mm:ss");
                query.FirstOrDefault().Element("RefreshTime").Value = clientInfo.RefreshTime.ToString("MM/dd/yyyy HH:mm:ss");
	        }
        }

        /// <summary>
        /// Save data source changes
        /// </summary>
        public void Save()
        {
            clientInfosXDoc.Save(filePath);
        }

        /// <summary>
        /// Convert XML message to Class
        /// </summary>
        /// <param name="clientInfoXml"></param>
        /// <returns></returns>
        private ClientInfo convertToClientInfo(XElement clientInfoXml)
        {
            if (clientInfoXml!=null)
            {
                ClientInfo clientInfo = new ClientInfo();
                clientInfo.ClientID = clientInfoXml.Element("ClientID").Value;
                clientInfo.ActiveTime = DateTime.Parse(clientInfoXml.Element("ActiveTime").Value);
                clientInfo.RefreshTime = DateTime.Parse(clientInfoXml.Element("RefreshTime").Value);
                return clientInfo;
            }
            return null;
        }
        /// <summary>
        /// Convert Class to XML message
        /// </summary>
        /// <param name="clientInfo"></param>
        /// <returns></returns>
        private XElement convertToClientInfoXElement(ClientInfo clientInfo)
        {
            if (clientInfo!=null)
            {
                XElement xDoc = new XElement("ClientInfo",
                    new XElement("ClientID", clientInfo.ClientID),
                    new XElement("ActiveTime", clientInfo.ActiveTime.ToString("MM/dd/yyyy HH:mm:ss")),
                    new XElement("RefreshTime", clientInfo.RefreshTime.ToString("MM/dd/yyyy HH:mm:ss")));
                return xDoc;
            }
            return null;
        }
    }
}