/****************************** Module Header ******************************\
* Module Name:    Category.aspx.cs
* Project:        CSASPNETBreadcrumbWithQueryString
* Copyright (c) Microsoft Corporation
*
* This page shows a item list and displays the category name in the breadcrumb.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
\*****************************************************************************/
using System;
using System.Web;

namespace CSASPNETBreadcrumbWithQueryString
{
    public partial class Category : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && !string.IsNullOrEmpty(Request.QueryString["name"]))
            {
                // Show a item list.
                gvItems.DataSource = Database.GetItemsByCategory(Request.QueryString["name"]);
                gvItems.DataBind();

                // Handle SiteMapResolve event to dynamically change current SiteMapNode.
                SiteMap.SiteMapResolve += new SiteMapResolveEventHandler(SiteMap_SiteMapResolve);
            }
        }

        /// <summary>
        /// Occurs when the CurrentNode property is accessed.
        /// </summary>
        /// <param name="sender">
        /// The source of the event, an instance of the SiteMapProvider class.
        /// </param>
        /// <param name="e">
        /// A SiteMapResolveEventArgs that contains the event data.
        /// </param>
        /// <returns>
        /// The SiteMapNode that represents the result of the SiteMapResolveEventHandler operation.
        /// </returns>
        SiteMapNode SiteMap_SiteMapResolve(object sender, SiteMapResolveEventArgs e)
        {
            // Only need one execution in one request.
            SiteMap.SiteMapResolve -= new SiteMapResolveEventHandler(SiteMap_SiteMapResolve);

            if (SiteMap.CurrentNode != null)
            {
                // SiteMap.CurrentNode is readonly, so we need to clone one to operate.
                SiteMapNode currentNode = SiteMap.CurrentNode.Clone(true);

                currentNode.Title = Request.QueryString["name"];

                // Use the changed one in the breadcrumb.
                return currentNode;
            }
            return null;
        }
    }
}