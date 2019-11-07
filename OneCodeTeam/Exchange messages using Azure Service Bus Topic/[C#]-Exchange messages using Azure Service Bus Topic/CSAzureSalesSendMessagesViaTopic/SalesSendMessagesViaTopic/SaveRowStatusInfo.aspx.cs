/****************************** Module Header ******************************\
*Module Name:  SaveRowStatusInfo.aspx.cs
*Project:      SalesSendMessagesViaTopic
*Copyright (c) Microsoft Corporation.
* 
*In contrast to queues, in which each message is processed by a single consumer,
*topics and subscriptions provide a one-to-many form of communication, in a publish/subscribe pattern.
*
*This sample show that the Sales department is responsible for verifying orders placed by customers, 
*keeping record of the orders and then sending the order information to Production department to make sure that the goods be delivered in time.  

*This source is subject to the Microsoft Public License.
*See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
*All other rights reserved.
* 
*THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
*EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
*WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SalesSendMessagesViaTopic
{
    public partial class SaveRowStatusInfo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string strRowStatus = Request["RowStatus"];
            if(strRowStatus.Length>0)
            {
               if(Session["RowStatus"]!=null)
               {
                   Session["RowStatus"] = strRowStatus;
               }else
               {
                   Session.Add("RowStatus", strRowStatus);
               }
              string jsonResult = "true";
              Response.Write(jsonResult);
              Response.End();
            }
        }
    }
}