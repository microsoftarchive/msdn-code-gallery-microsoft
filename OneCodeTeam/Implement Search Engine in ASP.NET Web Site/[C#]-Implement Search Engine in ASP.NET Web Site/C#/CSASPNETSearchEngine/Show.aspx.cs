/****************************** Module Header ******************************\
* Module Name:    Show.aspx.cs
* Project:        CSASPNETSearchEngine
* Copyright (c) Microsoft Corporation
*
* This page shows an individual record from database according to Query String
* parameter "id".
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
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