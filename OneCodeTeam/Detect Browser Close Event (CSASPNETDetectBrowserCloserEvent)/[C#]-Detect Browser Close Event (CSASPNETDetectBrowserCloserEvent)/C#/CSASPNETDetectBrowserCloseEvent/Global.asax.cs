/**************************** Module Header ********************************\
* Module Name:    Global.asax
* Project:        CSASPNETDetectBrowserCloseEvent
* Copyright (c) Microsoft Corporation
*
* As we know, HTTP is a stateless protocol, the browser doesn't keep connecting
* to the server. When user try to close the browser using alt-f4, browser close(X) 
* and right click on browser and close -> this all is done and is working fine, 
* it's not possible to tell the server that the browser is closed. The sample 
* demonstrates how to detect the browser close event with iframe.

* This class is used to detect the browser whether closed every three seconds.
* And it will auto delete off-line client.
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
using System.Web.Security;
using System.Web.SessionState;
using System.Timers;

namespace CSASPNETDetectBrowserCloseEvent
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            Timer timer = new Timer(3000);
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.Start();
        }

        /// <summary>
        /// Search the off-line client and delete it.
        /// </summary>
        protected void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ClientInfo client = new ClientInfo();
            List<ClientInfo> clientList = (List<ClientInfo>)Application["ClientInfo"];

            // If the iframe is no refresh beyond 5 seconds 
            // or the page has no active beyond 20 minutes,
            // delete this client.
            for (int i = 0; i < clientList.Count; i++)
            {
                if (clientList[i].RefreshTime < DateTime.Now.AddSeconds(0 - 5)
                    || clientList[i].ActiveTime < DateTime.Now.AddMinutes(0 - 20))
                {
                    client = ClientInfo.GetClinetInfoByClientID(clientList, clientList[i].ClientID);
                    clientList.Remove(client);
                }
            }

            Application["ClientInfo"] = clientList;
        }


        /// <summary>
        /// Add new client into Application
        /// </summary>
        protected void Session_Start(object sender, EventArgs e)
        {
            Application.Lock();
            ClientInfo client = new ClientInfo();

            // If there are clients in Application
            if (Application["ClientInfo"] != null)
            {
                List<ClientInfo> clientList = (List<ClientInfo>)Application["ClientInfo"];

                // New client information
                client.ClientID = this.Session.SessionID;
                client.ActiveTime = DateTime.Now;
                client.RefreshTime = DateTime.Now;

                bool flag = false;
                foreach (ClientInfo clientinfo in clientList)
                {
                    // If the client exist in clientList
                    if (clientinfo.ClientID == client.ClientID)
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    clientList.Add(client);
                    Application["ClientInfo"] = clientList;
                }
            }
            else
            {
                List<ClientInfo> clientList = new List<ClientInfo>();
                client.ClientID = this.Session.SessionID;
                client.RefreshTime = DateTime.Now;
                clientList.Add(client);
                Application["ClientInfo"] = clientList;
            }
            Application.UnLock();
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}