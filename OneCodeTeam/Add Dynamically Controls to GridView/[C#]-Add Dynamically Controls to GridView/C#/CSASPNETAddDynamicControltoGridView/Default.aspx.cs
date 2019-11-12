/****************************** Module Header ******************************\
* Module Name:    Default.aspx.cs
* Project:        CSASPNETAddDynamicControltoGridView
* Copyright (c) Microsoft Corporation
*
* The project illustrates how to add dynamic LinkButton in Gridview.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************/

using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CSASPNETAddDynamicControltoGridView
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            gdvCustomer.DataBind();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                AddLinkButton();
            }
        }

        /// <summary>
        /// To initialize the page.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        /// <summary>
        /// Add a LinkButton To GridView Row.
        /// </summary>
        private void AddLinkButton()
        {
            foreach (GridViewRow row in gdvCustomer.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    LinkButton lb = new LinkButton();
                    lb.Text = "Approve";
                    lb.CommandName = "ApproveVacation";
                    lb.Command += LinkButton_Command;
                    row.Cells[0].Controls.Add(lb);
                }
            }
        }

        protected void LinkButton_Command(object sender, CommandEventArgs e)
        {
            if (e.CommandName == "ApproveVacation")
            {
                LinkButton lb = (LinkButton)sender;
                lb.Text = "OK";
            }
        }

        protected void gdvCustomer_DataBound(object sender, EventArgs e)
        {
            AddLinkButton();
        }
    }
}
