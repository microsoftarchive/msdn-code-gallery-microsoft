/****************************** Module Header ******************************\
* Module Name:    Global.asax.cs
* Project:        CSASPNETShareSessionBetweenSubDomainsSite1
* Copyright (c) Microsoft Corporation
*
* This project demonstrates how to configure a SQL Server as SessionState and 
* make a module to share Session between two Web Sites with the same root domain.
* 
* The code in Global.asax is just to ensure you have set up Session State Sql Server
* and can run this sample without any configuration or command line
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
\*****************************************************************************/

using System;

namespace CSASPNETShareSessionBetweenSubDomainsSite1
{
    public class Global : System.Web.HttpApplication
    {
        // Need to configure Sql Server before runing this sample.
        protected void Application_Error(object sender, EventArgs e)
        {
            Response.Clear();
            Response.Write(@"Before runing this sample, please run this command:<br />");
            Response.Write(@"""<b>C:\Windows\Microsoft.NET\Framework\v4.0.30319\aspnet_regsql.exe -S localhost\sqlexpress -E -ssadd</b>""<br />");
            Response.Write("to configure localhost Sql Server Experssion to support Session State.<br /><br />");
            Response.Write("To know how to rollback this configuration, please check the ReadMe.txt file.");
            Response.End();
        }
    }
}