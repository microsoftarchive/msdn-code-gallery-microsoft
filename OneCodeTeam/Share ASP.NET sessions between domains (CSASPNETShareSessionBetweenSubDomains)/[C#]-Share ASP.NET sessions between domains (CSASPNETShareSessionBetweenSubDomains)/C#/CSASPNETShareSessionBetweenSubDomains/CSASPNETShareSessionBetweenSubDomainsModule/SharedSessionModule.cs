/****************************** Module Header ******************************\
* Module Name:    SharedSessionModule.cs
* Project:        CSASPNETShareSessionBetweenSubDomains
* Copyright (c) Microsoft Corporation
*
* This project demonstrates how to configure a SQL Server as SessionState and 
* make a module to share Session between two Web Sites with the same root domain.
* 
* SharedSessionModule is used to make Web Sites use the same Application Id and 
* Session Id to achieve sharing Session.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
\*****************************************************************************/

using System;
using System.Web;
using System.Reflection;
using System.Configuration;

namespace CSASPNETShareSessionBetweenSubDomainsModule
{
    /// <summary>
    /// A HttpModule used for sharing the session between Applications in 
    /// sub domains.
    /// </summary>
    public class SharedSessionModule : IHttpModule
    {
        // Cache settings on memory.
        protected static string applicationName = ConfigurationManager.AppSettings["ApplicationName"];
        protected static string rootDomain = ConfigurationManager.AppSettings["RootDomain"];

        #region IHttpModule Members
        /// <summary>
        /// Initializes a module and prepares it to handle requests.
        /// </summary>
        /// <param name="context">
        /// An System.Web.HttpApplication
        /// that provides access to the methods,
        /// properties, and events common to all application objects within 
        /// an ASP.NET application.
        /// </param>
        public void Init(HttpApplication context)
        {
            // This module requires both Application Name and Root Domain to work.
            if (string.IsNullOrEmpty(applicationName) || 
                string.IsNullOrEmpty(rootDomain))
            {
                return;
            }

            // Change the Application Name in runtime.
            FieldInfo runtimeInfo = typeof(HttpRuntime).GetField("_theRuntime", 
                BindingFlags.Static | BindingFlags.NonPublic);
            HttpRuntime theRuntime = (HttpRuntime)runtimeInfo.GetValue(null);
            FieldInfo appNameInfo = typeof(HttpRuntime).GetField("_appDomainAppId", 
                BindingFlags.Instance | BindingFlags.NonPublic);

            appNameInfo.SetValue(theRuntime, applicationName);

            // Subscribe Events.
            context.PostRequestHandlerExecute += new EventHandler(context_PostRequestHandlerExecute);
        }

        /// <summary>
        /// Disposes of the resources (other than memory) used by the module
        /// that implements.
        /// </summary>
        public void Dispose()
        {
        }
        #endregion

        /// <summary>
        /// Before sending response content to client, change the Cookie to Root Domain
        /// and store current Session Id.
        /// </summary>
        /// <param name="sender">
        /// An instance of System.Web.HttpApplication that provides access to
        /// the methods, properties, and events common to all application
        /// objects within an ASP.NET application.
        /// </param>
        void context_PostRequestHandlerExecute(object sender, EventArgs e)
        {
            HttpApplication context = (HttpApplication)sender;

            // ASP.NET store a Session Id in cookie to specify current Session.
            HttpCookie cookie = context.Response.Cookies["ASP.NET_SessionId"];

            if (context.Session != null &&
                !string.IsNullOrEmpty(context.Session.SessionID))
            {
                // Need to store current Session Id during every request.
                cookie.Value = context.Session.SessionID;

                // All Applications use one root domain to store this Cookie
                // So that it can be shared.
                if (rootDomain != "localhost")
                {
                    cookie.Domain = rootDomain;
                }

                // All Virtual Applications and Folders share this Cookie too.
                cookie.Path = "/";
            }
        }
    }
}
