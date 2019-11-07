/****************************** Module Header ******************************\
 * Module Name:  Default.aspx.cs
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

using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace CSASPNETMVCFileDownload
{
    public partial class _Default : Page
    {
        public void Page_Load(object sender, System.EventArgs e)
        {
            // Change the current path so that the Routing handler can correctly interpret
            // the request, then restore the original path so that the OutputCache module
            // can correctly process the response (if caching is enabled).

            string originalPath = Request.Path;
            HttpContext.Current.RewritePath(Request.ApplicationPath, false);
            IHttpHandler httpHandler = new MvcHttpHandler();
            httpHandler.ProcessRequest(HttpContext.Current);
            HttpContext.Current.RewritePath(originalPath, false);
        }
    }
}
