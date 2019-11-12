/****************************** Module Header ******************************\
* Module Name:    Default.aspx.cs
* Project:        CSASPNETSearchEngine
* Copyright (c) Microsoft Corporation
*
* This is the Search Page. User input one or more keywords in the text box, this
* page show the result according the input.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
\*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;

namespace CSASPNETSearchEngine
{
    public partial class Default : System.Web.UI.Page
    {
        /// <summary>
        /// The keywords input by user.
        /// </summary>
        protected List<string> keywords = new List<string>(); 

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            // Turn user input to a list of keywords.
            string[] keywords = tbKeyWords.Text.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            // The basic validation.
            if (keywords.Length <= 0)
            {
                lbAlert.Text = "Please input keyword.";
                return;
            }
            this.keywords = keywords.ToList();

            // Do search operation.
            DataAccess dataAccess = new DataAccess();
            List<Article> list = dataAccess.Search(this.keywords);

            ShowResult(list);
        }

        protected void btnListAll_Click(object sender, EventArgs e)
        {
            DataAccess dataAccess = new DataAccess();
            List<Article> list = dataAccess.GetAll();

            ShowResult(list);
        }

        #region Helpers

        /// <summary>
        /// Display a list of records in the page.
        /// </summary>
        /// <param name="list"></param>
        protected void ShowResult(List<Article> list)
        {
            RepeaterSearchResult.DataSource = list;
            RepeaterSearchResult.DataBind();
        }

        #endregion
    }
}