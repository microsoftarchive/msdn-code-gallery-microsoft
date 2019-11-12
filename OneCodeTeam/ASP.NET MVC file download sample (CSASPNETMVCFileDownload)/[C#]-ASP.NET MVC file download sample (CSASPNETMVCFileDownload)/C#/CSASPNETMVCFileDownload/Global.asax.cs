/****************************** Module Header ******************************\
 * Module Name:  Program.cs
 * Project:              CSASPNETMVCFileDownload
 * Copyright (c) Microsoft Corporation.
 * 
 * The CSASPNETMVCFileDownload example demonstrates how to use C# codes to 
 * create an ASP.NET MVC FileDownload application. The applicatino supports
 * basic site navigation, explore files in a certain fileshare and allow 
 * client user to download a selected file among the file list.
 * 
 * 
 * This source is subject to the Microsoft Public License.
 * See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
 * All other rights reserved.
 * 
 * History:
 * * 8/27/2009 1:35 PM Steven Cheng Created
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CSASPNETMVCFileDownload
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        // Function for registering all the MVC url routing rules
        public static void RegisterRoutes(RouteCollection routes)
        {
            // Rule for ignore axd resource request
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Rule for our filedownload requests
            routes.MapRoute(
                "FileDownload",
                 "File/{Action}/{fn}",
                 new { controller = "File", action = "List", fn = "" }
                 );

            // Other generic rules
            routes.MapRoute(
                "Default",                                              // Route name
                "{controller}/{action}/{id}",                           // URL with parameters
                new { controller = "Home", action = "Index", id = "" }  // Parameter defaults
            );


        }

        protected void Application_Start()
        {
            // All the rules will be registered at the application's startup time
            RegisterRoutes(RouteTable.Routes);
        }
    }
}