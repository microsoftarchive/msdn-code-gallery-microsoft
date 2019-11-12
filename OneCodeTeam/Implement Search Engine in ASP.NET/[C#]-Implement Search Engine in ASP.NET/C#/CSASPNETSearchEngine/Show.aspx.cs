/****************************** Module Header ******************************\
* Module Name:    Show.aspx.cs
* Project:        CSASPNETSearchEngine
* Copyright (c) Microsoft Corporation
*
* This page receives a parameter from Query String named "id", and then calls
* DataAccess class to retrieve an record from database and then show its in 
* the page. 
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

namespace CSASPNETSearchEngine
{
    public partial class Show : System.Web.UI.Page
    {
        /// <summary>
        /// The record which is displaying.
        /// </summary>
        protected Article Data;

        protected void Page_Load(object sender, EventArgs e)
        {
            long id = 0;

            // Only query database in the first load and ensure the input parameter is valid.
            if (!IsPostBack && 
                !string.IsNullOrEmpty(Request.QueryString["id"]) && 
                long.TryParse(Request.QueryString["id"], out id))
            {
                DataAccess dataAccess = new DataAccess();
                Data = dataAccess.GetArticle(id);
            }

            // Ensure the result is not null.
            if (Data == null)
            {
                Data = new Article();
            }
        }
    }
}