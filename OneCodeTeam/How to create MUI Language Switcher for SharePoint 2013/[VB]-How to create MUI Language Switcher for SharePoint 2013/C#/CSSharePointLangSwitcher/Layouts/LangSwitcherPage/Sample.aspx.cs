/****************************** Module Header ******************************\
* Module Name:    Sample.aspx.cs
* Project:        CSSharePointLangSwitcher
* Copyright (c) Microsoft Corporation
*
* This is the test page
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
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Web;

namespace CSSharePointLangSwitcher.Layouts.LangSwitcher
{
    public partial class Sample : LayoutsPageBase
    {
        // The key of current selected language in the cookies.
        private static string strKeyName = "LangSwitcher_Setting";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Gets the list of installed languages and bind them to DropDownList control.
                SPLanguageCollection languages = SPLanguageSettings.GetGlobalInstalledLanguages(15);
                ddlLanguages.DataSource = languages;
                ddlLanguages.DataBind();

                // Add a item at the top of the DropDownList and and set it selected by default. 
                ddlLanguages.Items.Insert(0, "NotSelected");
                ddlLanguages.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Save current selected language to cookie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (ddlLanguages.SelectedIndex > 0)
            {
                // Selected language.
                string strLanguage = ddlLanguages.SelectedValue;

                // Set the Cookies.
                HttpCookie acookie = new HttpCookie(strKeyName);
                acookie.Value = strLanguage;
                acookie.Expires = DateTime.MaxValue;
                Response.Cookies.Add(acookie);

                Response.Redirect(Request.RawUrl);
            }         
        }
    }
}
