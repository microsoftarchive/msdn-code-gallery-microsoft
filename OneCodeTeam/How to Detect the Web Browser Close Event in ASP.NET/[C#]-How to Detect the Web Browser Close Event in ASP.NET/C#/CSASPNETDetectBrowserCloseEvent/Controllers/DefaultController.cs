/**************************** Module Header ********************************\
* Module Name:    DefaultController.cs
* Project:        CSASPNETDetectBrowserCloseEvent
* Copyright (c) Microsoft Corporation
*
* As we know, HTTP is a stateless protocol, so the browser doesn't keep connecting 
* to the server. When users try to close the browser using alt-f4, browser close(X) 
* and right click on browser and close, all these methods are working fine, 
* but it's not possible to tell the server that the browser is closed.
*
* Default Controller
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/
using CSASPNETDetectBrowserCloseEvent.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace CSASPNETDetectBrowserCloseEvent.Controllers
{
    public class DefaultController : Controller
    {
        // GET: Default 
        public ActionResult Index()
        {
            return View();
        }

        // GET: Default/GetRefreshTime
        public string GetRefreshTime(string clientId)
        {
            ClientInfoDataSource dataSource = new ClientInfoDataSource();
            var clientInfo = dataSource.GetClientInfoByClientId(clientId);
            if (clientInfo != null)
            {
                clientInfo.RefreshTime = DateTime.Now;
                dataSource.UpdateClientInfo(clientInfo);
                dataSource.Save();
                return JsonConvert.SerializeObject(clientInfo);
            }
            else
            {
                ClientInfo newClientInfo = new ClientInfo()
                {
                    ClientID = clientId,
                    ActiveTime = DateTime.Now,
                    RefreshTime = DateTime.Now
                };
                dataSource.InsertClientInfo(newClientInfo);
                dataSource.Save();
                return JsonConvert.SerializeObject(newClientInfo);
            }
        }

        // Head: Default/RecordActiveTime
        [HttpHead]
        public void RecordActiveTime(string clientId)
        {
            ClientInfoDataSource dataSource = new ClientInfoDataSource();
            var clientInfo = dataSource.GetClientInfoByClientId(clientId);
            if (clientInfo != null)
            {
                clientInfo.ActiveTime = DateTime.Now;
                dataSource.UpdateClientInfo(clientInfo);
            }
            else
            {
                ClientInfo newClientInfo = new ClientInfo()
                {
                    ClientID = clientId,
                    ActiveTime = DateTime.Now,
                    RefreshTime = DateTime.Now
                };
                dataSource.InsertClientInfo(newClientInfo);
               
            }
            dataSource.Save();
        }

        // Head: Default/RecordCloseTime
        [HttpHead]
        public void RecordCloseTime(string clientId)
        {
            ClientInfoDataSource dataSource = new ClientInfoDataSource();
            var clientInfo = dataSource.GetClientInfoByClientId(clientId);
            if (clientInfo != null)
            {
                clientInfo.RefreshTime = DateTime.Now;
                dataSource.UpdateClientInfo(clientInfo);
            }
            dataSource.Save();
        }
    }
}