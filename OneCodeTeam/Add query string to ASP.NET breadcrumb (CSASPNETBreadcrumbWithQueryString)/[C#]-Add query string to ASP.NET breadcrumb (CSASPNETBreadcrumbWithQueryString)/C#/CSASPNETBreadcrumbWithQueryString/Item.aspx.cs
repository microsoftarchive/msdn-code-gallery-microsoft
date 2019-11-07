/****************************** Module Header ******************************\
* Module Name:    Item.aspx.cs
* Project:        CSASPNETBreadcrumbWithQueryString
* Copyright (c) Microsoft Corporation
*
* This page shows a item name. At the same time, this page show the 
* category name and the item name in the breadcrumb.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************/

using System;
using System.Web;

namespace CSASPNETBreadcrumbWithQueryString
{
    public partial class Item : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && !string.IsNullOrEmpty(Request.QueryString["name"]))
            {
                // Show the item name.
                lbMsg.Text = Request.QueryString["name"];

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
                currentNode.ParentNode.Title = Database.GetCategoryByItem(Request.QueryString["name"]);
                currentNode.ParentNode.Url = "/Category.aspx?name=" + Database.GetCategoryByItem(Request.QueryString["name"]);

                // Use the changed one in the Breadcrumb.
                return currentNode;
            }
            return null;
        }
    }
}