/****************************** Module Header ******************************\
* Module Name:    DynamicBreadcrumb.aspx.cs
* Project:        CSASPNETBreadcrumbWithQueryString
* Copyright (c) Microsoft Corporation
*
* This page shows that even a page is not in the site map, we still can 
* create the breadcrumb dynamically. 
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
    public partial class DynamicBreadcrumb : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
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

            // We can dynamically create many SiteMapNodes here.
            SiteMapNode childNode = new SiteMapNode(SiteMap.Provider, "2");
            childNode.Url = "/child.aspx";
            childNode.Title = "child";

            childNode.ParentNode = new SiteMapNode(SiteMap.Provider, "1");
            childNode.ParentNode.Url = "/root.aspx";
            childNode.ParentNode.Title = "root";

            // Also we can associate the dynamic nodes with the existent site map.
            SiteMapNode nodeFromSiteMap = GetSiteMapNode("item");
            if (nodeFromSiteMap != null)
            {
                childNode.ParentNode.ParentNode = nodeFromSiteMap;
            }

            // Use the new SiteMapNode in the breadcrumb.
            return childNode;
        }

        /// <summary>
        /// Get a siteMapNode from the site map.
        /// </summary>
        /// <param name="key">
        /// The resourceKey of the siteMapNode.
        /// </param>
        /// <returns></returns>
        SiteMapNode GetSiteMapNode(string key)
        {
            return GetSiteMapNode(SiteMap.RootNode, key);
        }
        SiteMapNode GetSiteMapNode(SiteMapNode rootNode, string key)
        {
            if (rootNode.ResourceKey == key)
            {
                return rootNode;
            }
            else if (rootNode.HasChildNodes)
            {
                foreach (SiteMapNode childNode in rootNode.ChildNodes)
                {
                    SiteMapNode resultNode = GetSiteMapNode(childNode, key);
                    if (resultNode != null)
                    {
                        return resultNode;
                    }
                }
            }
            return null;
        }
    }
}