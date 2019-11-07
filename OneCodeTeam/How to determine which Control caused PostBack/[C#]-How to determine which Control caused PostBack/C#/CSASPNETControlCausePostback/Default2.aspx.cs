/****************************** Module Header ******************************\
Module Name:  Default2.aspx.cs
Project:      CSASPNETControlCausePostback
Copyright (c) Microsoft Corporation.
 
The sample code demonstrates how to create a web application that can determine 
which control causes the postback event on an Asp.net page. Sometimes, we need 
to perform some specific actions based on the specific control which causes the 
postback. For example, we can get controls’ id property that and do some operations, 
such as set TextBox’s text with ViewState variable. In this sample, we can also 
transfer some data through postbacks.

This page shows how to determine which server control causes the postback event.
 
This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
All other rights reserved.
 
THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/



using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

namespace CSASPNETControlCausePostback
{
    public partial class Default2 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack)
            {
                StringBuilder builder = new StringBuilder();
                if (!String.IsNullOrEmpty(Request["__EVENTTARGET"]) && !String.IsNullOrEmpty(Request["__EVENTARGUMENT"]))
                {
                    string target = Request["__EVENTTARGET"] as string;
                    string argument = Request["__EVENTARGUMENT"] as string;
                    builder.Append("Cause postback control:");
                    builder.Append("<br />");
                    builder.Append(target);
                    builder.Append("<br />");
                    builder.Append("<br />");
                    builder.Append("Postback data:");
                    builder.Append("<br />");
                    builder.Append(argument);
                    lbMessage.Text = builder.ToString();
                }

            }
        }
    }
}
